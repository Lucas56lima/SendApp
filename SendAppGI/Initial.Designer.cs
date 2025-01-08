using System.Windows.Forms;

namespace SendAppGI
{
    partial class Initial
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            splitContainer1 = new SplitContainer();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            button4 = new Button();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            textBox3 = new TextBox();
            textBox2 = new TextBox();
            textBox1 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 0);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(button3);
            splitContainer1.Panel1.Controls.Add(button2);
            splitContainer1.Panel1.Controls.Add(button1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(button4);
            splitContainer1.Panel2.Controls.Add(label3);
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Panel2.Controls.Add(textBox3);
            splitContainer1.Panel2.Controls.Add(textBox2);
            splitContainer1.Panel2.Controls.Add(textBox1);
            splitContainer1.Size = new Size(402, 169);
            splitContainer1.SplitterDistance = 134;
            splitContainer1.TabIndex = 0;
            // 
            // button3
            // 
            button3.Location = new Point(12, 74);
            button3.Name = "button3";
            button3.Size = new Size(105, 25);
            button3.TabIndex = 3;
            button3.Text = "Logs";
            button3.UseVisualStyleBackColor = true;
            button3.Click += Button3_Click;
            // 
            // button2
            // 
            button2.Location = new Point(12, 43);
            button2.Name = "button2";
            button2.Size = new Size(105, 25);
            button2.TabIndex = 2;
            button2.Text = "Dados";
            button2.UseVisualStyleBackColor = true;
            button2.Click += Button2_Click;
            // 
            // button1
            // 
            button1.Location = new Point(12, 12);
            button1.Name = "button1";
            button1.Size = new Size(105, 25);
            button1.TabIndex = 1;
            button1.Text = "Início";
            button1.UseVisualStyleBackColor = true;
            button1.Click += Button1_Click;
            // 
            // button4
            // 
            button4.Font = new Font("Microsoft Sans Serif", 8.25F);
            button4.ForeColor = SystemColors.ControlText;
            button4.Location = new Point(69, 117);
            button4.Name = "button4";
            button4.Size = new Size(183, 24);
            button4.TabIndex = 6;
            button4.Text = "Editar";
            button4.UseVisualStyleBackColor = true;
            button4.Click += Button4_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 9.5F);
            label3.Location = new Point(25, 79);
            label3.Name = "label3";
            label3.Size = new Size(43, 17);
            label3.TabIndex = 5;
            label3.Text = "Senha";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 9.5F);
            label2.Location = new Point(25, 48);
            label2.Name = "label2";
            label2.Size = new Size(44, 17);
            label2.TabIndex = 4;
            label2.Text = "E-mail";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9.5F);
            label1.Location = new Point(25, 17);
            label1.Name = "label1";
            label1.Size = new Size(44, 17);
            label1.TabIndex = 3;
            label1.Text = "Nome";
            // 
            // textBox3
            // 
            textBox3.Font = new Font("Segoe UI", 9.5F);
            textBox3.Location = new Point(69, 76);
            textBox3.Name = "textBox3";
            textBox3.PasswordChar = '*';
            textBox3.ReadOnly = true;
            textBox3.Size = new Size(183, 24);
            textBox3.TabIndex = 2;
            // 
            // textBox2
            // 
            textBox2.Font = new Font("Segoe UI", 9.5F);
            textBox2.Location = new Point(69, 45);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(183, 24);
            textBox2.TabIndex = 1;
            // 
            // textBox1
            // 
            textBox1.Font = new Font("Segoe UI", 9.5F);
            textBox1.Location = new Point(69, 14);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(183, 24);
            textBox1.TabIndex = 0;
            // 
            // Initial
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(402, 169);
            Controls.Add(splitContainer1);
            HelpButton = true;
            Name = "Initial";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Configurações";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Label labelServer = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(25, 48),
                Name = "labelServer",
                Size = new Size(44, 17),
                TabIndex = 4,
                Text = "SMTP"
            };

            TextBox textBoxServer = new TextBox
            {
                Font = new Font("Segoe UI", 9.5F),
                Location = new Point(69, 45),
                Name = "textBoxServer",
                ReadOnly = true,
                Size = new Size(150, 24),
                TabIndex = 1
            };
            var controls = new Control[] { textBox1, textBox2, textBox3, label1, label2, label3, button4 };
            AlterVisibilityControls(controls, false);
            splitContainer1.Panel2.Controls.Add(labelServer);
            splitContainer1.Panel2.Controls.Add(textBoxServer);
            splitContainer1.Panel2.Refresh();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            var controls = new Control[] { textBox1, textBox2, textBox3, label1, label2, label3, button4};
            foreach (var control in controls)
            {
                control.Visible = false;
            }
            var labelServer = splitContainer1.Panel2.Controls["labelServer"];
            var textBoxServer = splitContainer1.Panel2.Controls["textBoxServer"];
            if (textBoxServer != null)
                splitContainer1.Panel2.Controls.Remove(textBoxServer);
            if (labelServer != null)
                splitContainer1.Panel2.Controls.Remove(labelServer);

            DataGridView dataGridView = new DataGridView
            {
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Name = "dataGridView1",
                Size = new Size(264, 169),
                TabIndex = 7
            };

            splitContainer1.Panel2.Controls.Add(dataGridView);
            splitContainer1.Panel2.Refresh();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            // Torna os TextBoxes editáveis
            textBox1.ReadOnly = false;
            textBox2.ReadOnly = false;
            textBox3.ReadOnly = false;

            // Cria o botão "Salvar"
            Button buttonSave = new Button
            {
                Font = new Font("Microsoft Sans Serif", 8.25F),
                ForeColor = SystemColors.ControlText,
                Location = new Point(50, 117), // Define a posição
                Name = "buttonSaveDynamic",
                Size = new Size(90, 24), // Tamanho do botão
                Text = "Salvar",
                UseVisualStyleBackColor = true
            };
            // Adiciona evento de clique para o botão "Salvar"
            buttonSave.Click += ButtonSave_Click;

            // Cria o botão "Cancelar"
            Button buttonCancel = new Button
            {
                Font = new Font("Microsoft Sans Serif", 8.25F),
                ForeColor = SystemColors.ControlText,
                Location = new Point(150, 117), // Define a posição (ao lado do botão "Salvar")
                Name = "buttonCancelDynamic",
                Size = new Size(90, 24), // Tamanho do botão
                Text = "Cancelar",
                UseVisualStyleBackColor = true
            };
            // Adiciona evento de clique para o botão "Cancelar"
            buttonCancel.Click += ButtonCancel_Click;
            button4.Visible = false;
            // Adiciona os botões ao painel do SplitContainer
            splitContainer1.Panel2.Controls.Add(buttonSave);
            splitContainer1.Panel2.Controls.Add(buttonCancel);

            // Atualiza o painel para exibir os botões
            splitContainer1.Panel2.Refresh();
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {                      
            RemoveDynamicButtons();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var controls = new Control[] { textBox1, textBox2, textBox3, label1, label2, label3, button4 };
            
            AlterVisibilityControls(controls,true);
            
            var dataGridView = splitContainer1.Panel2.Controls["dataGridView1"];
            if (dataGridView != null)
                splitContainer1.Panel2.Controls.Remove(dataGridView);
            splitContainer1.Panel2.Refresh();
        }

        private void RemoveDynamicButtons()
        {
            var buttonSave = splitContainer1.Panel2.Controls["buttonSaveDynamic"];
            var buttonCancel = splitContainer1.Panel2.Controls["buttonCancelDynamic"];

            if (buttonSave != null)
                splitContainer1.Panel2.Controls.Remove(buttonSave);

            if (buttonCancel != null)
                splitContainer1.Panel2.Controls.Remove(buttonCancel);

            button4.Visible = true;
            textBox1.ReadOnly = true;
            textBox2.ReadOnly = true;
            textBox3.ReadOnly = true;
            splitContainer1.Panel2.Refresh();
        }

        private void AlterVisibilityControls(Control[] controls, bool state)
        {
            foreach (var control in controls)
            {
                control.Visible = state;
            }
        }    

        #endregion

        private SplitContainer splitContainer1;
        private Button button3;
        private Button button2;
        private Button button1;
        private Label label3;
        private Label label2;
        private Label label1;
        private TextBox textBox3;
        private TextBox textBox2;
        private TextBox textBox1;
        private Button button4;
    }
}
