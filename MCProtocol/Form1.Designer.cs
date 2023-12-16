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
            HeightRandomCheckBox = new CheckBox();
            UnitTypeCheckBox = new CheckBox();
            RButton = new Button();
            MRButton = new Button();
            DMButton = new Button();
            ScriptRunButton = new Button();
            ScriptFolderButton = new Button();
            ConnectTextBox = new TextBox();
            label1 = new Label();
            openFileDialog1 = new OpenFileDialog();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            LogTextBox = new TextBox();
            tabPage2 = new TabPage();
            StatusTextBox = new TextBox();
            panel1.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(HeightRandomCheckBox);
            panel1.Controls.Add(UnitTypeCheckBox);
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
            panel1.Size = new Size(819, 66);
            panel1.TabIndex = 0;
            // 
            // HeightRandomCheckBox
            // 
            HeightRandomCheckBox.AutoSize = true;
            HeightRandomCheckBox.Location = new Point(314, 41);
            HeightRandomCheckBox.Name = "HeightRandomCheckBox";
            HeightRandomCheckBox.Size = new Size(103, 19);
            HeightRandomCheckBox.TabIndex = 4;
            HeightRandomCheckBox.Text = "高さランダム有効";
            HeightRandomCheckBox.UseVisualStyleBackColor = true;
            HeightRandomCheckBox.CheckedChanged += UnitTypeCheckBox_CheckedChanged;
            // 
            // UnitTypeCheckBox
            // 
            UnitTypeCheckBox.AutoSize = true;
            UnitTypeCheckBox.Location = new Point(167, 41);
            UnitTypeCheckBox.Name = "UnitTypeCheckBox";
            UnitTypeCheckBox.Size = new Size(81, 19);
            UnitTypeCheckBox.TabIndex = 4;
            UnitTypeCheckBox.Text = "OK2Aモード";
            UnitTypeCheckBox.UseVisualStyleBackColor = true;
            UnitTypeCheckBox.CheckedChanged += UnitTypeCheckBox_CheckedChanged;
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
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 66);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(819, 384);
            tabControl1.TabIndex = 1;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(LogTextBox);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(811, 356);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "ログ";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // LogTextBox
            // 
            LogTextBox.Dock = DockStyle.Fill;
            LogTextBox.Font = new Font("ＭＳ ゴシック", 9F, FontStyle.Regular, GraphicsUnit.Point);
            LogTextBox.Location = new Point(3, 3);
            LogTextBox.Multiline = true;
            LogTextBox.Name = "LogTextBox";
            LogTextBox.ScrollBars = ScrollBars.Both;
            LogTextBox.Size = new Size(805, 350);
            LogTextBox.TabIndex = 2;
            LogTextBox.WordWrap = false;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(StatusTextBox);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(811, 356);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "ステータス";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // StatusTextBox
            // 
            StatusTextBox.Dock = DockStyle.Fill;
            StatusTextBox.Font = new Font("ＭＳ ゴシック", 9F, FontStyle.Regular, GraphicsUnit.Point);
            StatusTextBox.Location = new Point(3, 3);
            StatusTextBox.Multiline = true;
            StatusTextBox.Name = "StatusTextBox";
            StatusTextBox.ScrollBars = ScrollBars.Both;
            StatusTextBox.Size = new Size(805, 350);
            StatusTextBox.TabIndex = 3;
            StatusTextBox.WordWrap = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(819, 450);
            Controls.Add(tabControl1);
            Controls.Add(panel1);
            Name = "Form1";
            Text = "PLCエミュレータv0.7 (MCプロトコル ip=127.0.0.1, port=5000)";
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private TextBox ConnectTextBox;
        private Label label1;
        private Button ScriptFolderButton;
        private Button ScriptRunButton;
        private OpenFileDialog openFileDialog1;
        private Button DMButton;
        private Button RButton;
        private Button MRButton;
        private CheckBox UnitTypeCheckBox;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TextBox LogTextBox;
        private TabPage tabPage2;
        private TextBox StatusTextBox;
        private CheckBox HeightRandomCheckBox;
    }
}