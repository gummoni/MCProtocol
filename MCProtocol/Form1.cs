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
            Processor.Start(PLC, this);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            StopCommLog();
            Processor.Stop();
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

        public void AddCommLog(string adr, string message)
        {
            if (UpdateTask == null)
            {
                UpdateTask = Task.Run(LogTextUpdateTask);
            }
            var _adr = MemISDic.GetComment(adr);
            var time1 = DateTime.Now.ToString("HH:mm:ss.fff");                                         // 現在時刻の文字フォーマット

            LogTextBoxQueues.Enqueue($"{time1},{_adr}:{message}");
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

        private void ScriptRunButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    AddCommLog("", $"[{openFileDialog1.FileName}]ファイル開始");
                    foreach (var _ in Processor.Dispatch(PLC, openFileDialog1.FileName))
                        ;
                    AddCommLog("", $"[{openFileDialog1.FileName}]ファイル完了");
                }
                catch
                {
                    AddCommLog("", $"[{openFileDialog1.FileName}]ファイル中断");
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

        private void UnitTypeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            MemISDic.IsOK2AMode = UnitTypeCheckBox.Checked;
        }
    }

    public interface IUIUpdatable
    {
        void UpdateConnect(int count);
        void AddCommLog(string adr, string message);
    }
}