using Domain.Entities;
using SendAppGI.Services;
using SendAppGI.Viewmodels;
using System.ComponentModel;


namespace SendAppGI
{
    public partial class Initial : Form
    {
        private SplitContainer splitContainer;
        private Button btnInicio, btnDados, btnLogs, btnEditar;
        private TextBox txtNome, txtEmail, txtSenha;
        private Label lblNome, lblEmail, lblSenha;        
        private readonly InitialViewModel viewModel;
       
        public Initial(DataStoreService dataStoreService,FileService fileService, MailService mailService )
        {
            viewModel = new InitialViewModel(dataStoreService, fileService, mailService)
            {
                Store = new()
            };
            viewModel.LoadStoreCommand.Execute(null);            
            viewModel.PropertyChanged += ViewModel_PropertyChanged;            
            viewModel.LoadLogsCommand.Execute(null);
            viewModel.LoadSchedulingCommand.Execute(null);
            InitializeComponent();            
            viewModel.StartWatchingCommand.Execute(null);
        }

        private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(viewModel.Store))
            {
                txtNome.Text = viewModel.Store?.Name;
                txtEmail.Text = viewModel.Store?.Email;
                txtSenha.Text = viewModel.Store?.Password;                
            }
        }

        private void InitializeComponent()
        {
            
           
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.WinLogo ; // ícone de exemplo
            notifyIcon.Visible = true;
            notifyIcon.Text = "SendApp";

            // Lógica para o ícone de bandeja
            notifyIcon.DoubleClick += (sender, e) =>
            {
                // Código para restaurar ou interagir com a aplicação
                ShowMainWindow();
            };

            // Configuração geral da janela
            Text = "Configurações";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(500, 300);
            
            splitContainer = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 40,
                IsSplitterFixed = true,
                BorderStyle = BorderStyle.Fixed3D
            };
            Controls.Add(splitContainer);

            // Painel Esquerdo - Menu
            btnInicio = CreateButton("Início", 10, 10, BtnInicio_Click);
            btnDados = CreateButton("Dados", 10, 50, BtnDados_Click);
            btnLogs = CreateButton("Logs", 10, 90, BtnLogs_Click);
            splitContainer.Panel1.Controls.AddRange([btnInicio, btnDados, btnLogs]);
            FillControls(viewModel.Store);            
        }

        private void ShowMainWindow()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
        }

        private static Button CreateButton(string text, int x, int y, EventHandler onClick) => new Button
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(100, 30),
            UseVisualStyleBackColor = true
        }.WithEvent(onClick);

        private static Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F)
            };
        }

        private static TextBox CreateTextBox(int x, int y, bool readOnly, bool isPassword = false)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Size = new Size(200, 24),
                ReadOnly = readOnly,
                PasswordChar = isPassword ? '*' : '\0'
            };
        }

        private void BtnInicio_Click(object sender, EventArgs e)
        {            
            ClearPanel2();

            viewModel.LoadStoreFromCacheCommand.Execute(null);
            var cachedStore = viewModel.Store;

            if (cachedStore != null)
            {
                FillControls(cachedStore);
            }
            else
            {
                // Caso os dados não estejam no cache, faz a requisição ao serviço
                viewModel.LoadStoreCommand.Execute(null);

                // Assina o evento PropertyChanged para capturar alterações no ViewModel
                viewModel.PropertyChanged += ViewModel_PropertyChanged;
            }            
            
        }

        private void BtnDados_Click(object sender, EventArgs e)
        {
            ClearPanel2();
            
            var lblPath = CreateLabel("Local:", 20, 60);
            var txtPath = CreateTextBox(80, 60, true);
            txtPath.Text = viewModel.Store.Path;
            var btnBrowse = new Button
            {
                Text = "...",
                Width = 30,
                Height = txtPath.Height,
                Left = txtPath.Right + 5,
                Top = txtPath.Top,
                Enabled = false
            };

            // Evento para abrir o FolderBrowserDialog
            btnBrowse.Click += (s, ev) =>
            {
                using var folderDialog = new FolderBrowserDialog();
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    txtPath.Text = folderDialog.SelectedPath;
                }
            };

            // Criando o botão Editar
            var btnEdit = new Button
            {
                Text = "Editar",
                Width = 60,
                Height = 30,
                Left = txtPath.Left + (txtPath.Width - 60) / 2,  // Centralizando horizontalmente
                Top = txtPath.Top + 40 // Ajuste de acordo com sua necessidade
            };

            // Variáveis de controle
            Button btnSave = null;
            Button btnCancel = null;
            bool isEditing = false;

            // Evento de clique para alternar entre modo leitura e edição
            btnEdit.Click += (s, ev) =>
            {
                if (isEditing)
                {
                    // Alternar para modo visualização
                    txtPath.ReadOnly = true;
                    btnBrowse.Enabled = true;
                    btnEdit.Text = "Editar";

                    // Remove os botões Salvar e Cancelar
                    splitContainer.Panel2.Controls.Remove(btnSave);
                    splitContainer.Panel2.Controls.Remove(btnCancel);
                }
                else
                {
                    // Alternar para modo edição
                    txtPath.ReadOnly = false;
                    btnBrowse.Enabled = true;
                    btnEdit.Text = "Editar";
                    btnEdit.Visible = false;

                    // Criar botão Salvar
                    btnSave = new Button
                    {
                        Text = "Salvar",
                        Width = 60,
                        Height = 30,
                        Left = btnEdit.Left - 70,
                        Top = btnEdit.Top
                    };

                    // Criar botão Cancelar
                    btnCancel = new Button
                    {
                        Text = "Cancelar",
                        Width = 60,
                        Height = 30,
                        Left = btnEdit.Right + 10,
                        Top = btnEdit.Top
                    };

                    // Eventos dos botões Salvar e Cancelar
                    btnSave.Click += (saveSender, saveEv) =>
                    {   
                        if(viewModel.PutStoreCommand.CanExecute(null))
                        {
                            if(!String.IsNullOrWhiteSpace(txtPath.Text))
                                viewModel.Store.Path = txtPath.Text;
                            viewModel.PutStoreCommand.Execute(null);
                            // Volta para o modo de visualização
                            btnEdit.PerformClick();
                        }
                         
                    };

                    btnCancel.Click += (cancelSender, cancelEv) =>
                    {                        
                        txtPath.ReadOnly = true;
                        btnBrowse.Enabled = false;
                        btnEdit.Text = "Editar";
                        btnEdit.Visible = true;
                        splitContainer.Panel2.Controls.Remove(btnSave);
                        splitContainer.Panel2.Controls.Remove(btnCancel);
                        isEditing = false; // Volta para o modo de visualização
                    };

                    // Adicionar os botões Salvar e Cancelar ao painel
                    splitContainer.Panel2.Controls.Add(btnSave);
                    splitContainer.Panel2.Controls.Add(btnCancel);
                }

                // Alternar estado de edição
                isEditing = !isEditing;
            };

            splitContainer.Panel2.Controls.AddRange([lblPath, txtPath, btnBrowse, btnEdit]);
        }



        private void BtnLogs_Click(object sender, EventArgs e)
        {
            ClearPanel2();

            var listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details
            };
            // Adicionar colunas ao ListView
            listView.Columns.Add("Log", -2, HorizontalAlignment.Left);
            listView.Columns.Add("Mensagem", -2, HorizontalAlignment.Left);
            listView.Columns.Add("Data", -2, HorizontalAlignment.Left);

            // Executar o comando para carregar logs
            viewModel.LoadLogsCommand.Execute(null);

            // Verificar se os logs foram carregados
            if (viewModel.Logs != null)
            {
                foreach (Log item in viewModel.Logs)
                {
                    // Criar um novo ListViewItem para cada linha
                    ListViewItem listViewItem = new (item.StoreName);
                    listViewItem.SubItems.Add(item.Message);
                    listViewItem.SubItems.Add(item.Created.ToString());

                    // Adicionar o ListViewItem ao ListView
                    listView.Items.Add(listViewItem);
                }
            }
            else
            {
                Log log = new ()
                {
                    StoreName = viewModel.Store.Name,
                    Message = "Sem logs para mostrar",
                    Created = DateTime.Now,
                };
                ListViewItem listViewItem = new (log.StoreName);

                // Adicionar subitens vazios ou com valores padrões
                listViewItem.SubItems.Add(log.Message); // Coluna Mensagem
                listViewItem.SubItems.Add("-"); // Coluna Data

                // Adicionar a linha padrão ao ListView
                listView.Items.Add(listViewItem);
            }               
                
           
            splitContainer.Panel2.Controls.Add(listView);
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            txtNome.ReadOnly = txtEmail.ReadOnly = txtSenha.ReadOnly = false;
            
            var btnSave = CreateButton("Salvar", 80, 140, BtnSave_Click);
            var btnCancel = CreateButton("Cancelar", 190, 140, BtnCancel_Click);

            btnEditar.Visible = false;
            splitContainer.Panel2.Controls.AddRange([btnSave, btnCancel]);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            // Certifique-se de que o viewModel não está nulo
            if (viewModel == null)
            {
                MessageBox.Show("Erro: ViewModel não foi inicializado.");
                return;
            }

            // Atualize o Store no ViewModel com os valores inseridos nas TextBox
            viewModel.Store.Name = txtNome.Text;
            viewModel.Store.Cnpj = "00000000";  // Se você deseja deixar um valor fixo
            viewModel.Store.Email = txtEmail.Text;
            viewModel.Store.Password = txtSenha.Text;
            viewModel.Store.Path = "C:\\Users\\Usuário\\Desktop\\Nova pasta";  // O caminho fixo também pode ser definido aqui

            // Defina os campos como somente leitura após salvar
            txtNome.ReadOnly = txtEmail.ReadOnly = txtSenha.ReadOnly = true;
            var textBoxes = new (TextBox TextBox, Action<string> UpdateAction)[]
            {
                (txtNome, val => viewModel.Store.Name = val),
                (txtEmail, val => viewModel.Store.Email = val),
                (txtSenha, val => viewModel.Store.Password = val)
            };

            foreach (var (textBox, updateAction) in textBoxes)
            {
                if (!string.IsNullOrWhiteSpace(textBox.Text))
                {
                    updateAction(textBox.Text);
                }
            }

            if (viewModel.PutStoreCommand.CanExecute(null))
            {                
                viewModel.PutStoreCommand.Execute(null);                
            }
            else
            {
                // Caso o comando não possa ser executado, exiba uma mensagem de erro
                MessageBox.Show("Erro: Não foi possível salvar os dados.");
            }
            BtnCancel_Click(null, null);
        }


        private void BtnCancel_Click(object sender, EventArgs e)
        {
            txtNome.ReadOnly = txtEmail.ReadOnly = txtSenha.ReadOnly = true;
            btnEditar.Visible = true;

            ClearPanel2(exclude: [lblNome, txtNome, lblEmail, txtEmail, lblSenha, txtSenha, btnEditar]);
        }

        private void ClearPanel2(Control[] exclude = null)
        {
            List<Control> controlsToRemove = splitContainer.Panel2.Controls.Cast<Control>().Except(exclude ?? []).ToList();
            foreach (var control in controlsToRemove)
                splitContainer.Panel2.Controls.Remove(control);
        }

        private void FillControls(Store store)
        {           
            lblNome = CreateLabel("Nome:", 20, 20);
            txtNome = CreateTextBox(80, 20, true);
            txtNome.Text = store.Name;

            lblEmail = CreateLabel("E-mail:", 20, 60);
            txtEmail = CreateTextBox(80, 60, true);
            txtEmail.Text = store.Email;

            lblSenha = CreateLabel("Senha:", 20, 100);
            txtSenha = CreateTextBox(80, 100, true, true);
            txtSenha.Text = store.Password;

            btnEditar = CreateButton("Editar", 80, 140, BtnEditar_Click);

            splitContainer.Panel2.Controls.AddRange([lblNome, txtNome, lblEmail, txtEmail, lblSenha, txtSenha, btnEditar]);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            }
        }
    }



    public static class Extensions
    {
        public static T WithEvent<T>(this T control, EventHandler eventHandler) where T : Control
        {
            control.Click += eventHandler;
            return control;
        }
    }
}
