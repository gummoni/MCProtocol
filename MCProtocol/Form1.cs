using System.Collections.Concurrent;
using System.Diagnostics;

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

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            StopCommLog();
        }

        void DoInvoke(Action action)
        {
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        T DoInvoke<T>(Func<T> action) => InvokeRequired ? Invoke(action) : action();

        public void UpdateConnect(int count)
        {
            DoInvoke(() =>
            {
                ConnectTextBox.Text = $"{count}";
            });
        }

        public void AddCommLog(string message)
        {
            if (UpdateTask == null)
            {
                UpdateTask = Task.Run(LogTextUpdateTask);
            }
            LogTextBoxQueues.Enqueue(message);
            LogTextBoxEvent.Set();
        }

        public void StopCommLog()
        {
            IsPower = false;
            LogTextBoxQueues.Clear();
            LogTextBoxEvent.Set();
        }

        Task? UpdateTask;
        bool IsPower = true;
        readonly ManualResetEventSlim LogTextBoxEvent = new();
        readonly ConcurrentQueue<string> LogTextBoxQueues = new();
        
        void LogTextUpdateTask()
        {
            while (IsPower)
            {
                LogTextBoxEvent.Wait();
                LogTextBoxEvent.Reset();
                if (LogTextBoxQueues.IsEmpty)
                    continue;

                var message = DoInvoke(() => LogTextBox.Text);
                while (LogTextBoxQueues.TryDequeue(out string? data))
                    message += $"{data}\r\n";
                DoInvoke(() =>
                {
                    LogTextBox.SuspendLayout();
                    LogTextBox.Text = message;
                    LogTextBox.SelectionStart = message.Length;
                    LogTextBox.ScrollToCaret();
                    LogTextBox.ResumeLayout();
                });
            }
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