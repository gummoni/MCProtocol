using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace MCProtocol
{
    public partial class Form1 : Form, IUIUpdatable
    {
        readonly PLCDevice PLC;

        public Form1()
        {
            InitializeComponent();
            PLC = new(5000, this);
            openFileDialog1.InitialDirectory = Path.Combine(Environment.CurrentDirectory, "Scripts");
        }

        void DoInvoke(Action action)
        {
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        public void UpdateConnect(int count)
        {
            DoInvoke(() =>
            {
                ConnectTextBox.Text = $"{count}";
            });
        }

        public void AddCommLog(string message)
        {
            DoInvoke(() =>
            {
                LogTextBox.Text += $"{message}\r\n";
            });
        }

        private void ScriptFolderButton_Click(object sender, EventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", Path.Combine(Environment.CurrentDirectory, "Scripts"));
            }
            catch
            {
            }
        }

        private async void ScriptRunButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    AddCommLog($"[{openFileDialog1.FileName}]ファイル開始");
                    await Processor.Dispatch(PLC, openFileDialog1.FileName);
                    AddCommLog($"[{openFileDialog1.FileName}]ファイル完了");
                }
                catch
                {
                    AddCommLog($"[{openFileDialog1.FileName}]ファイル中断");
                }
            }
        }

        private void DMButton_Click(object sender, EventArgs e)
        {
            new DicEditForm("DM", PLC.DM).ShowDialog(this);
        }

        private void MRButton_Click(object sender, EventArgs e)
        {
            new DicEditForm("MR", PLC.MR).ShowDialog(this);
        }

        private void RButton_Click(object sender, EventArgs e)
        {
            new DicEditForm(" R", PLC.R).ShowDialog(this);
        }
    }

    public interface IUIUpdatable
    {
        void UpdateConnect(int count);
        void AddCommLog(string message);
    }
}