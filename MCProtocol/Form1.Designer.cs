namespace MCProtocol
{
    partial class Form1
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
            panel1 = new Panel();
            RButton = new Button();
            MRButton = new Button();
            DMButton = new Button();
            ScriptRunButton = new Button();
            ScriptFolderButton = new Button();
            ConnectTextBox = new TextBox();
            label1 = new Label();
            LogTextBox = new TextBox();
            openFileDialog1 = new OpenFileDialog();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(RButton);
            panel1.Controls.Add(MRButton);
            panel1.Controls.Add(DMButton);
            panel1.Controls.Add(ScriptRunButton);
            panel1.Controls.Add(ScriptFolderButton);
            panel1.Controls.Add(ConnectTextBox);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(819, 39);
            panel1.TabIndex = 0;
            // 
            // RButton
            // 
            RButton.Location = new Point(666, 5);
            RButton.Name = "RButton";
            RButton.Size = new Size(93, 23);
            RButton.TabIndex = 3;
            RButton.Text = "R参照";
            RButton.UseVisualStyleBackColor = true;
            RButton.Click += RButton_Click;
            // 
            // MRButton
            // 
            MRButton.Location = new Point(567, 5);
            MRButton.Name = "MRButton";
            MRButton.Size = new Size(93, 23);
            MRButton.TabIndex = 3;
            MRButton.Text = "MR参照";
            MRButton.UseVisualStyleBackColor = true;
            MRButton.Click += MRButton_Click;
            // 
            // DMButton
            // 
            DMButton.Location = new Point(468, 6);
            DMButton.Name = "DMButton";
            DMButton.Size = new Size(93, 23);
            DMButton.TabIndex = 3;
            DMButton.Text = "DM参照";
            DMButton.UseVisualStyleBackColor = true;
            DMButton.Click += DMButton_Click;
            // 
            // ScriptRunButton
            // 
            ScriptRunButton.Location = new Point(314, 6);
            ScriptRunButton.Name = "ScriptRunButton";
            ScriptRunButton.Size = new Size(116, 23);
            ScriptRunButton.TabIndex = 3;
            ScriptRunButton.Text = "スクリプト実行";
            ScriptRunButton.UseVisualStyleBackColor = true;
            ScriptRunButton.Click += ScriptRunButton_Click;
            // 
            // ScriptFolderButton
            // 
            ScriptFolderButton.Location = new Point(167, 6);
            ScriptFolderButton.Name = "ScriptFolderButton";
            ScriptFolderButton.Size = new Size(141, 23);
            ScriptFolderButton.TabIndex = 2;
            ScriptFolderButton.Text = "スクリプトフォルダを開く";
            ScriptFolderButton.UseVisualStyleBackColor = true;
            ScriptFolderButton.Click += ScriptFolderButton_Click;
            // 
            // ConnectTextBox
            // 
            ConnectTextBox.Location = new Point(61, 6);
            ConnectTextBox.Name = "ConnectTextBox";
            ConnectTextBox.ReadOnly = true;
            ConnectTextBox.Size = new Size(100, 23);
            ConnectTextBox.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(43, 15);
            label1.TabIndex = 0;
            label1.Text = "接続数";
            // 
            // LogTextBox
            // 
            LogTextBox.Dock = DockStyle.Fill;
            LogTextBox.Location = new Point(0, 39);
            LogTextBox.Multiline = true;
            LogTextBox.Name = "LogTextBox";
            LogTextBox.ScrollBars = ScrollBars.Both;
            LogTextBox.Size = new Size(819, 411);
            LogTextBox.TabIndex = 1;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(819, 450);
            Controls.Add(LogTextBox);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "PLCエミュレータv0.1 (MCプロトコル port=5000)";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private TextBox ConnectTextBox;
        private Label label1;
        private TextBox LogTextBox;
        private Button ScriptFolderButton;
        private Button ScriptRunButton;
        private OpenFileDialog openFileDialog1;
        private Button DMButton;
        private Button RButton;
        private Button MRButton;
    }
}