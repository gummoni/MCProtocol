namespace MCProtocol
{
    partial class FlagForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            RRegCheckedListBox = new CheckedListBox();
            MRRegCheckedListBox = new CheckedListBox();
            comboBox1 = new ComboBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            DMValueTextBox = new TextBox();
            label4 = new Label();
            DMReadButton = new Button();
            DMWriteButton = new Button();
            DMAddressTextBox = new TextBox();
            label5 = new Label();
            ReloadButton = new Button();
            SuspendLayout();
            // 
            // RRegCheckedListBox
            // 
            RRegCheckedListBox.FormattingEnabled = true;
            RRegCheckedListBox.Location = new Point(12, 31);
            RRegCheckedListBox.Name = "RRegCheckedListBox";
            RRegCheckedListBox.Size = new Size(263, 418);
            RRegCheckedListBox.TabIndex = 0;
            RRegCheckedListBox.ItemCheck += RRegCheckedListBox_ItemCheck;
            // 
            // MRRegCheckedListBox
            // 
            MRRegCheckedListBox.FormattingEnabled = true;
            MRRegCheckedListBox.Location = new Point(281, 31);
            MRRegCheckedListBox.Name = "MRRegCheckedListBox";
            MRRegCheckedListBox.Size = new Size(263, 418);
            MRRegCheckedListBox.TabIndex = 0;
            MRRegCheckedListBox.ItemCheck += MRRegCheckedListBox_ItemCheck;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Location = new Point(552, 31);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(164, 23);
            comboBox1.TabIndex = 1;
            comboBox1.Visible = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 9);
            label1.Name = "label1";
            label1.Size = new Size(51, 15);
            label1.TabIndex = 2;
            label1.Text = "Rレジスタ";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(281, 9);
            label2.Name = "label2";
            label2.Size = new Size(62, 15);
            label2.TabIndex = 2;
            label2.Text = "MRレジスタ";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(552, 9);
            label3.Name = "label3";
            label3.Size = new Size(63, 15);
            label3.TabIndex = 2;
            label3.Text = "DMレジスタ";
            // 
            // DMValueTextBox
            // 
            DMValueTextBox.Location = new Point(722, 76);
            DMValueTextBox.Name = "DMValueTextBox";
            DMValueTextBox.Size = new Size(100, 23);
            DMValueTextBox.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(722, 54);
            label4.Name = "label4";
            label4.Size = new Size(19, 15);
            label4.TabIndex = 2;
            label4.Text = "値";
            // 
            // DMReadButton
            // 
            DMReadButton.Location = new Point(666, 145);
            DMReadButton.Name = "DMReadButton";
            DMReadButton.Size = new Size(75, 51);
            DMReadButton.TabIndex = 4;
            DMReadButton.Text = "読み込み";
            DMReadButton.UseVisualStyleBackColor = true;
            DMReadButton.Click += DMReadButton_Click;
            // 
            // DMWriteButton
            // 
            DMWriteButton.Location = new Point(747, 145);
            DMWriteButton.Name = "DMWriteButton";
            DMWriteButton.Size = new Size(75, 51);
            DMWriteButton.TabIndex = 4;
            DMWriteButton.Text = "書き込み";
            DMWriteButton.UseVisualStyleBackColor = true;
            DMWriteButton.Click += DMWriteButton_Click;
            // 
            // DMAddressTextBox
            // 
            DMAddressTextBox.Location = new Point(616, 76);
            DMAddressTextBox.Name = "DMAddressTextBox";
            DMAddressTextBox.Size = new Size(100, 23);
            DMAddressTextBox.TabIndex = 3;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(616, 54);
            label5.Name = "label5";
            label5.Size = new Size(26, 15);
            label5.TabIndex = 2;
            label5.Text = "DM";
            // 
            // ReloadButton
            // 
            ReloadButton.Location = new Point(550, 396);
            ReloadButton.Name = "ReloadButton";
            ReloadButton.Size = new Size(75, 51);
            ReloadButton.TabIndex = 4;
            ReloadButton.Text = "更新";
            ReloadButton.UseVisualStyleBackColor = true;
            ReloadButton.Click += ReloadButton_Click;
            // 
            // FlagForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(838, 459);
            Controls.Add(DMWriteButton);
            Controls.Add(ReloadButton);
            Controls.Add(DMReadButton);
            Controls.Add(DMAddressTextBox);
            Controls.Add(DMValueTextBox);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(comboBox1);
            Controls.Add(MRRegCheckedListBox);
            Controls.Add(RRegCheckedListBox);
            Name = "FlagForm";
            Text = "FlagForm";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private CheckedListBox RRegCheckedListBox;
        private CheckedListBox MRRegCheckedListBox;
        private ComboBox comboBox1;
        private Label label1;
        private Label label2;
        private Label label3;
        private TextBox DMValueTextBox;
        private Label label4;
        private Button DMReadButton;
        private Button DMWriteButton;
        private TextBox DMAddressTextBox;
        private Label label5;
        private Button ReloadButton;
    }
}