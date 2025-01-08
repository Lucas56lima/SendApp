using Domain.Entities;
using Service.Services;
using System.Net.Http.Json;
using System.Text.Json;

namespace SendAppGI
{
    public partial class Initial : Form
    {
        private SplitContainer splitContainer;
        private Button btnInicio, btnDados, btnLogs, btnEditar;
        private TextBox txtNome, txtEmail, txtSenha;
        private Label lblNome, lblEmail, lblSenha;
        public StoreService storeService;
        public SchedulingService schedulingService;
        public HttpClient client = new();
        public Initial()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Configuração geral da janela
            Text = "Configurações";
            StartPosition = FormStartPosition.CenterScreen;
            Size = new Size(500, 300);

            // Configuração do SplitContainer
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
            splitContainer.Panel1.Controls.AddRange(new Control[] { btnInicio, btnDados, btnLogs });

            // Painel Direito - Detalhes
            lblNome = CreateLabel("Nome:", 20, 20);
            txtNome = CreateTextBox(80, 20, true);

            lblEmail = CreateLabel("E-mail:", 20, 60);
            txtEmail = CreateTextBox(80, 60, true);

            lblSenha = CreateLabel("Senha:", 20, 100);
            txtSenha = CreateTextBox(80, 100, true, true);

            btnEditar = CreateButton("Editar", 80, 140, BtnEditar_Click);

            splitContainer.Panel2.Controls.AddRange(new Control[] { lblNome, txtNome, lblEmail, txtEmail, lblSenha, txtSenha, btnEditar });
        }

        private Button CreateButton(string text, int x, int y, EventHandler onClick)
        {
            return new Button
            {
                Text = text,
                Location = new Point(x, y),
                Size = new Size(100, 30),
                UseVisualStyleBackColor = true
            }.WithEvent(onClick);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F)
            };
        }

        private TextBox CreateTextBox(int x, int y, bool readOnly, bool isPassword = false)
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
            lblNome = CreateLabel("Nome:", 20, 20);
            txtNome = CreateTextBox(80, 20, true);

            lblEmail = CreateLabel("E-mail:", 20, 60);
            txtEmail = CreateTextBox(80, 60, true);

            lblSenha = CreateLabel("Senha:", 20, 100);
            txtSenha = CreateTextBox(80, 100, true, true);

            btnEditar = CreateButton("Editar", 80, 140, BtnEditar_Click);

            splitContainer.Panel2.Controls.AddRange(new Control[] { lblNome, txtNome, lblEmail, txtEmail, lblSenha, txtSenha, btnEditar });
        }

        private void BtnDados_Click(object sender, EventArgs e)
        {
            ClearPanel2();

            var lblServer = CreateLabel("SMTP:", 20, 20);
            var txtServer = CreateTextBox(80, 20, true);
            var lblPath = CreateLabel("Local:", 20, 60);
            var txtPath = CreateTextBox(80, 60, true);

            // Criando o botão para abrir o FolderBrowserDialog
            var btnBrowse = new Button
            {
                Text = "...",
                Width = 30,
                Height = txtPath.Height,
                Left = txtPath.Right + 5,
                Top = txtPath.Top
            };

            // Evento para abrir o FolderBrowserDialog
            btnBrowse.Click += (s, ev) =>
            {
                using (var folderDialog = new FolderBrowserDialog())
                {
                    if (folderDialog.ShowDialog() == DialogResult.OK)
                    {
                        txtPath.Text = folderDialog.SelectedPath;
                    }
                }
            };

            // Criando o botão Editar
            var btnEdit = new Button
            {
                Text = "Editar",
                Width = 60,
                Height = 30,
                Left = txtPath.Left + (txtPath.Width - 60) / 2,  // Centralizando horizontalmente
                Top = txtPath.Top + 40  // Ajuste de acordo com sua necessidade
            };

            // Variável de controle para saber se está no modo de edição
            bool isEditing = false;

            // Evento de clique para alternar entre modo leitura e edição
            btnEdit.Click += (s, ev) =>
            {
                if (isEditing)
                {
                    // Desativa a edição e o FolderBrowserDialog
                    txtPath.ReadOnly = true;
                    btnBrowse.Enabled = false;
                    btnEdit.Text = "Editar";
                }
                else
                {
                    // Ativa a edição e o FolderBrowserDialog
                    txtPath.ReadOnly = false;
                    btnBrowse.Enabled = true;
                    btnEdit.Text = "Salvar";
                }
                // Alterna o estado de edição
                isEditing = !isEditing;
            };

            splitContainer.Panel2.Controls.AddRange(new Control[] { lblServer, txtServer, lblPath, txtPath, btnBrowse, btnEdit });
        }


        private void BtnLogs_Click(object sender, EventArgs e)
        {
            ClearPanel2();

            var listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details
            };
            listView.Columns.Add("Logs", -2, HorizontalAlignment.Left);
            listView.Items.Add(new ListViewItem("Log 1"));
            listView.Items.Add(new ListViewItem("Log 2"));

            splitContainer.Panel2.Controls.Add(listView);
        }

        private void BtnEditar_Click(object sender, EventArgs e)
        {
            txtNome.ReadOnly = txtEmail.ReadOnly = txtSenha.ReadOnly = false;

            var btnSave = CreateButton("Salvar", 80, 140, BtnSave_Click);
            var btnCancel = CreateButton("Cancelar", 190, 140, BtnCancel_Click);

            btnEditar.Visible = false;
            splitContainer.Panel2.Controls.AddRange(new Control[] { btnSave, btnCancel });
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            Store store = new Store
            {
                Name = txtNome.Text,
                Cnpj = "00000000",
                Email = txtEmail.Text,
                Password = txtSenha.Text,
                Path = "C:\\Users\\Usuário\\Desktop\\Nova pasta"
            };
            await PostStoreAsync(store);
            txtNome.ReadOnly = txtEmail.ReadOnly = txtSenha.ReadOnly = true;
            MessageBox.Show("Dados salvos com sucesso.");
            BtnCancel_Click(null, null);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            txtNome.ReadOnly = txtEmail.ReadOnly = txtSenha.ReadOnly = true;
            btnEditar.Visible = true;

            ClearPanel2(exclude: new Control[] { lblNome, txtNome, lblEmail, txtEmail, lblSenha, txtSenha, btnEditar });
        }

        private void ClearPanel2(Control[] exclude = null)
        {
            var controlsToRemove = splitContainer.Panel2.Controls.Cast<Control>().Except(exclude ?? Array.Empty<Control>()).ToList();
            foreach (var control in controlsToRemove)
                splitContainer.Panel2.Controls.Remove(control);
        }

        public async Task PostStoreAsync(Store store)
        {
            using HttpResponseMessage response = await client.PostAsJsonAsync("https://localhost:7185/api/Store/PostStoreAsync", store);
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
