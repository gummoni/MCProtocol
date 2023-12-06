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

        string _old = "";
        public void AddCommLog(string adr, string message)
        {
            if (UpdateTask == null)
            {
                UpdateTask = Task.Run(LogTextUpdateTask);
            }
            var _adr = MemISDic.GetComment(adr);
            var time1 = DateTime.Now.ToString("HH:mm:ss.fff");                                         // 現在時刻の文字フォーマット

            var chk = $"{_adr}:{message}";
            if (chk == _old) return;
            _old = chk;

            var msg = $"{time1},{chk}";
            LogTextBoxQueues.Enqueue(msg);
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

        static readonly string[] StatusFilterComm = new[]
        {
            "MR4309",   //S1FN
            "MR4312",   //PEND

            "DM29400",  //RP
            "DM29401",  //WP

            //インナーログ
            "DM29000",
            "DM29001",
            "DM29002",
            "DM29003",
            "DM29004",
            "DM29005",
            "DM29006",
            "DM29007",
            "DM29008",
            "DM29009",
            "DM29010",
            "DM29011",
            "DM29012",
            "DM29013",
            "DM29014",
            "DM29015",
            "DM29016",
            "DM29017",
            "DM29018",
            "DM29019",
            "DM29020",
            "DM29021",
            "DM29022",
            "DM29023",
            "DM29024",
            "DM29025",
            "DM29026",
            "DM29027",
            "DM29028",
            "DM29029",
            "DM29030",
            "DM29031",
            "DM29032",
            "DM29033",
            "DM29034",
            "DM29035",
            "DM29036",
            "DM29037",
            "DM29038",
            "DM29039",
            "DM29040",
            "DM29041",
            "DM29042",
            "DM29043",
            "DM29044",
            "DM29045",
            "DM29046",
            "DM29047",
            "DM29048",
            "DM29049",
            "DM29050",
            "DM29051",
            "DM29052",
            "DM29053",
            "DM29054",
            "DM29055",
            "DM29056",
            "DM29057",
            "DM29058",
            "DM29059",
            "DM29060",
            "DM29061",
            "DM29062",
            "DM29063",
            "DM29064",
            "DM29065",
            "DM29066",
            "DM29067",
            "DM29068",
            "DM29069",
            "DM29070",
            "DM29071",
            "DM29072",
            "DM29073",
            "DM29074",
            "DM29075",
            "DM29076",
            "DM29077",
            "DM29078",
            "DM29079",
            "DM29080",
            "DM29081",
            "DM29082",
            "DM29083",
            "DM29084",
            "DM29085",
            "DM29086",
            "DM29087",
            "DM29088",
            "DM29089",
            "DM29090",
            "DM29091",
            "DM29092",
            "DM29093",
            "DM29094",
            "DM29095",
            "DM29096",
            "DM29097",
            "DM29098",
            "DM29099",
            "DM29100",
            "DM29101",
            "DM29102",
            "DM29103",
            "DM29104",
            "DM29105",
            "DM29106",
            "DM29107",
            "DM29108",
            "DM29109",
            "DM29110",
            "DM29111",
            "DM29112",
            "DM29113",
            "DM29114",
            "DM29115",
            "DM29116",
            "DM29117",
            "DM29118",
            "DM29119",
            "DM29120",
            "DM29121",
            "DM29122",
            "DM29123",
            "DM29124",
            "DM29125",
            "DM29126",
            "DM29127",
            "DM29128",
            "DM29129",
            "DM29130",
            "DM29131",
            "DM29132",
            "DM29133",
            "DM29134",
            "DM29135",
            "DM29136",
            "DM29137",
            "DM29138",
            "DM29139",
            "DM29140",
            "DM29141",
            "DM29142",
            "DM29143",
            "DM29144",
            "DM29145",
            "DM29146",
            "DM29147",
            "DM29148",
            "DM29149",
            "DM29150",
            "DM29151",
            "DM29152",
            "DM29153",
            "DM29154",
            "DM29155",
            "DM29156",
            "DM29157",
            "DM29158",
            "DM29159",
            "DM29160",
            "DM29161",
            "DM29162",
            "DM29163",
            "DM29164",
            "DM29165",
            "DM29166",
            "DM29167",
            "DM29168",
            "DM29169",
            "DM29170",
            "DM29171",
            "DM29172",
            "DM29173",
            "DM29174",
            "DM29175",
            "DM29176",
            "DM29177",
            "DM29178",
            "DM29179",
            "DM29180",
            "DM29181",
            "DM29182",
            "DM29183",
            "DM29184",
            "DM29185",
            "DM29186",
            "DM29187",
            "DM29188",
            "DM29189",
            "DM29190",
            "DM29191",
            "DM29192",
            "DM29193",
            "DM29194",
            "DM29195",
            "DM29196",
            "DM29197",
            "DM29198",
            "DM29199",
            "DM29200",
            "DM29201",
            "DM29202",
            "DM29203",
            "DM29204",
            "DM29205",
            "DM29206",
            "DM29207",
            "DM29208",
            "DM29209",
            "DM29210",
            "DM29211",
            "DM29212",
            "DM29213",
            "DM29214",
            "DM29215",
            "DM29216",
            "DM29217",
            "DM29218",
            "DM29219",
            "DM29220",
            "DM29221",
            "DM29222",
            "DM29223",
            "DM29224",
            "DM29225",
            "DM29226",
            "DM29227",
            "DM29228",
            "DM29229",
            "DM29230",
            "DM29231",
            "DM29232",
            "DM29233",
            "DM29234",
            "DM29235",
            "DM29236",
            "DM29237",
            "DM29238",
            "DM29239",
            "DM29240",
            "DM29241",
            "DM29242",
            "DM29243",
            "DM29244",
            "DM29245",
            "DM29246",
            "DM29247",
            "DM29248",
            "DM29249",
            "DM29250",
            "DM29251",
            "DM29252",
            "DM29253",
            "DM29254",
            "DM29255",
            "DM29256",
            "DM29257",
            "DM29258",
            "DM29259",
            "DM29260",
            "DM29261",
            "DM29262",
            "DM29263",
            "DM29264",
            "DM29265",
            "DM29266",
            "DM29267",
            "DM29268",
            "DM29269",
            "DM29270",
            "DM29271",
            "DM29272",
            "DM29273",
            "DM29274",
            "DM29275",
            "DM29276",
            "DM29277",
            "DM29278",
            "DM29279",
            "DM29280",
            "DM29281",
            "DM29282",
            "DM29283",
            "DM29284",
            "DM29285",
            "DM29286",
            "DM29287",
            "DM29288",
            "DM29289",
            "DM29290",
            "DM29291",
            "DM29292",
            "DM29293",
            "DM29294",
            "DM29295",
            "DM29296",
            "DM29297",
            "DM29298",
            "DM29299",
            "DM29300",
            "DM29301",
            "DM29302",
            "DM29303",
            "DM29304",
            "DM29305",
            "DM29306",
            "DM29307",
            "DM29308",
            "DM29309",
            "DM29310",
            "DM29311",
            "DM29312",
            "DM29313",
            "DM29314",
            "DM29315",
            "DM29316",
            "DM29317",
            "DM29318",
            "DM29319",
            "DM29320",
            "DM29321",
            "DM29322",
            "DM29323",
            "DM29324",
            "DM29325",
            "DM29326",
            "DM29327",
            "DM29328",
            "DM29329",
            "DM29330",
            "DM29331",
            "DM29332",
            "DM29333",
            "DM29334",
            "DM29335",
            "DM29336",
            "DM29337",
            "DM29338",
            "DM29339",
            "DM29340",
            "DM29341",
            "DM29342",
            "DM29343",
            "DM29344",
            "DM29345",
            "DM29346",
            "DM29347",
            "DM29348",
            "DM29349",
            "DM29350",
            "DM29351",
            "DM29352",
            "DM29353",
            "DM29354",
            "DM29355",
            "DM29356",
            "DM29357",
            "DM29358",
            "DM29359",
            "DM29360",
            "DM29361",
            "DM29362",
            "DM29363",
            "DM29364",
            "DM29365",
            "DM29366",
            "DM29367",
            "DM29368",
            "DM29369",
            "DM29370",
            "DM29371",
            "DM29372",
            "DM29373",
            "DM29374",
            "DM29375",
            "DM29376",
            "DM29377",
            "DM29378",
            "DM29379",
            "DM29380",
            "DM29381",
            "DM29382",
            "DM29383",
            "DM29384",
            "DM29385",
            "DM29386",
            "DM29387",
            "DM29388",
            "DM29389",
            "DM29390",
            "DM29391",
            "DM29392",
            "DM29393",
            "DM29394",
            "DM29395",
            "DM29396",
            "DM29397",
            "DM29398",
            "DM29399",
        };

        static readonly string[] StatusFilter1A = new[] 
        {
            "MR2100",   //試薬リザーバ１在荷
            "MR2101",   //試薬リザーバ２在荷
            "MR2102",   //試薬リザーバ３在荷
            "MR2103",   //試薬リザーバ４在荷
            "MR2104",   //試薬リザーバ５在荷
            "MR2105",   //試薬リザーバ６在荷
            "MR2106",   //試薬リザーバ７在荷

            "R35000",
            "R35009",
            "R35010",
            "R35109",
            "R35110",
            "R35111",
            "R35112",
            "R35113",
            "R35114",
            "R35115",

            "R36000",
            "R36001",
            "R36002",
            "R36003",
            "R36004",
            "R36005",
            "R36006",
            "R36007",
            "R36008",
            "R36009",

            "R36012",
            "R36013",
            "R36014",
            "R36015",
            "R36103",
            "R36114",

            "DM600",
            "DM608",
            "DM610",
            "DM612",
            "DM614",
            "DM616",
            "DM618",
            "DM620",

            "DM1050",

            "DM4310",
            "MR4311",

            "R36104",    //空圧異常1A
        };

        static readonly string[] StatusFilter2A = new[]
        {
            "DM600",
            "DM608",
            "DM610",
            "DM1050",

            "R1400",

            "R34000",
            "R35000",
            "R35001",
            "R35002",
            "R35003",
            "R35004",
            "R35006",
            "R35007",    //空圧異常2A
            "R35011",
            "R35014",

            "MR4311",
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
                    var filter = UnitTypeCheckBox.Checked ? StatusFilter2A : StatusFilter1A;
                    var isStatus = filter.FirstOrDefault(data.Contains) != null || StatusFilterComm.FirstOrDefault(data.Contains) != null;
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
            Processor.IsRandomMode = HeightRandomCheckBox.Checked;
        }
    }
}