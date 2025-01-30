using Domain.Entities;
using SendAppGI.Services;


namespace SendAppGI
{
    public partial class Register : Form
    {
        private SplitContainer splitContainer;
        private readonly DataStoreService _dataStoreService;
        private readonly FileService _fileService;
        private Button btnSalvar;
        private TextBox txtPath;
        private Label lblPath;
        public Register(DataStoreService dataStoreService,FileService fileService)
        {
            _dataStoreService = dataStoreService;
            _fileService = fileService; 
            InitializeComponent();
        }

        private void InitializeComponent()
        {            
            NotifyIcon notifyIcon = new NotifyIcon();
            notifyIcon.Icon = SystemIcons.WinLogo ; // ícone de exemplo
            notifyIcon.Visible = true;
            notifyIcon.Text = "SendApp";

            Text = "Registro";
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

            FillControls();            
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

        private async void BtnSave_Click(object sender, EventArgs e)
        {
           
            
            if(!string.IsNullOrEmpty(txtPath.Text))
            {
                Store store = new()
                {
                    Name = "1234",
                    Cnpj = "00000",
                    Email = "000000",
                    Password = "000000",
                    Path = txtPath.Text
                };
                await _dataStoreService.PostStoreAsync(store);                
                await _fileService.GetFileForPathAsync(store.Path);
                MarkAsCompleted();
                this.Close();
            }                
            else
            {                
                MessageBox.Show("Erro: Não foi possível salvar os dados.");
            }
        }


        private void FillControls()
        {
            btnSalvar = CreateButton("Salvar", 80, 100, BtnSave_Click);
            lblPath = CreateLabel("Local:", 20, 60);
            txtPath = CreateTextBox(80, 60, false);            
            var btnBrowse = new Button
            {
                Text = "...",
                Width = 30,
                Height = txtPath.Height,
                Left = txtPath.Right + 5,
                Top = txtPath.Top,
                Enabled = true
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
            splitContainer.Panel2.Controls.AddRange([lblPath,txtPath,btnBrowse,btnSalvar]);
        }

        private void MarkAsCompleted()
        {
            // Cria um arquivo indicando que o formulário já foi exibido
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FirstRun.lock");
            File.WriteAllText(filePath, "completed");
        }
       
    }

}
