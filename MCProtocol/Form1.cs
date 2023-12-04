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

        static readonly string[] StatusFilter = new[] 
        {
            "DM1050",
            "R35000",
            "R36000",
            "R35009",
            "R35010",
            "MR2100",
            "MR2101",
            "MR2102",
            "MR2103",
            "MR2104",
            "MR2105",
            "R35114",
            "R36004",
            "R35113",
            "R36003",
            "R36009",
            "R35109",
            "R35115",
            "R36005",
            "R35110",
            "R36000",
            "R36006",
            "R35111",
            "R36001",
            "R36007",
            "R35112",
            "R36002",
            "R36008",
            "R36103",
            "R36114",
            "R36012",
            "R36014",
            "R36013",
            "R36015",
            "DM608",
            "DM610",
            "DM612",
            "DM614",
            "DM616",
            "DM618",
            "DM620",
            "DM4310",
            "MR4311",
            "DM600",
        };


        void LogTextUpdateTask()
        {
            while (IsPower)
            {
                LogTextBoxEvent.Wait();
                LogTextBoxEvent.Reset();
                if (LogTextBoxQueues.IsEmpty)
                    continue;

                var messageLog = DoInvoke(() => LogTextBox.Text);
                var messageStatus = DoInvoke(() => StatusTextBox.Text);
                while (LogTextBoxQueues.TryDequeue(out string? data))
                {
                    var isStatus = StatusFilter.FirstOrDefault(data.Contains) != null;
                    if (isStatus)
                    {
                        messageStatus += $"{data}\r\n";
                    }
                    else
                    {
                        messageLog += $"{data}\r\n";
                    }
                }

                DoInvoke(() =>
                {
                    StatusTextBox.SuspendLayout();
                    StatusTextBox.Text = messageStatus;
                    StatusTextBox.SelectionStart = messageStatus.Length;
                    StatusTextBox.ScrollToCaret();
                    StatusTextBox.ResumeLayout();

                    LogTextBox.SuspendLayout();
                    LogTextBox.Text = messageLog;
                    LogTextBox.SelectionStart = messageLog.Length;
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

    public interface IUIUpdatable
    {
        void UpdateConnect(int count);
        void AddCommLog(string message);
    }
}