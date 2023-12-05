using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace MCProtocol
{

    public class ProtocolRunner
    {
        public IEnumerable<bool> Start(PLCDevice plc, IUIUpdatable ui)
        {
            var stopwatch = new Stopwatch();
            while (true)
            {
                //設定情報
                var kin1_z1 = new ushort[] { 15001, 15002, 15003, 15004, 15005, 15006, 15007 };

                //プロトコル開始待ち
                while (!plc.ReadMR(4310))
                    yield return true;
                plc.WriteMR(4311, true);
                ui.AddCommLog("", "◆プロトコル開始");
                stopwatch.Restart();
                plc.WriteMR(4310, false);

                //プロトコル実行
                var cur = DateTime.Now;

                for (var adr = 30000; adr < 90000; adr += 20)
                {
                    stopwatch.Restart();
                    var old = cur;
                    var cmd = plc.ReadDM(adr);
                    var ret = new ushort[20];
                    var no = (ushort)((adr - 30000) / 20);
                    
                    switch (cmd)
                    {
                        case 1: //"PVER";"「１」：プロトコルバージョン",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 2: //"MVIP";"「２」：原点復帰",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 3: //"LOPT";"「３」：ピペットチップ装着",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 4: //"DSPT";"「４」：ピペットチップ廃棄",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 5: //"SNPT";"「５」：ノズル先端検知",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 6: //"NOWA";"「６」：ノズル洗浄",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 7: //"NODR";"「７」：ノズル乾燥",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 8: //"LDRG";"「８」：試薬液面検知",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 9: //"STRG";"「９」：試薬撹拌",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 10: //"SCRG"; "「10」：試薬吸引",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 11: //"DSRG"; "「11」：試薬吐出",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 12: //"SDRG"; "「12」：試薬吐出後攪拌",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 13: //"DCRG"; "「13」：試薬廃液",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 14: //"WACP"; "「14」：液溜め洗浄",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 15: //"CRL1"; "「15」：液溜め残液制御１",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 16: //"CRL2"; "「16」：液溜め残液制御２",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 17: //"STMP"; "「17」：タイムスタンプ",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 18: //"DSPM"; "「18」：ポンプ廃液",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 19: //"WAIT"; "「19」：タイマ",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 20: //"SPAR"; "「20」：エア吹付",
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        case 21: //"PCNT"; "「21」：プロトコルファイル継続",
                            ui.AddCommLog("", "◆PCNT");
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);

                            //PCNT完了待ち
                            while (true)
                            {
                                yield return true;

                                var stage2_406_mr = plc.ReadMR(12606);  //0になるのを待つ
                                if (stage2_406_mr) continue;

                                var tray3_1 = plc.ReadMR(2011);
                                if (!tray3_1) continue;

                                var tray4_1 = plc.ReadMR(2012);
                                if (!tray4_1) continue;

                                //var tray3_1 = plc.ReadMR(2002);
                                //if (!tray3_1) continue;

                                //var tray4_1 = plc.ReadMR(2003);
                                //if (!tray4_1) continue;

                                //var tray3_2 = plc.ReadMR(2006);
                                //if (!tray3_2) continue;

                                //var tray4_2 = plc.ReadMR(2007);
                                //if (!tray4_2) continue;


                                break;
                            }
                            break;

                        case 22: //"PEND"; "「22」：プロトコルファイル終了",
                            ui.AddCommLog("", "◆PEND：ステージ２完了待ち");
                            cur = cur.AddSeconds(5);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);

                            //プロトコル終了通知
                            ui.AddCommLog("", "◆PEND完了");
                            adr = 9999999;
                            break;

                        case 23: //"S1FN"; "「23」：ステージ1プロトコル終了",
                            ui.AddCommLog("", "◆S1FN");
                            plc.WriteMR(4309, true);
                            cur = cur.AddSeconds(0);
                            InnerLogWrite(plc, no, cmd, old, cur, ret);
                            break;

                        default:
                            //不明
                            break;
                    }

                    //ウェイト
                    while (stopwatch.ElapsedMilliseconds < 700)
                        yield return true;

                    //バッファ空き待ち                    
                    foreach (var isFull in IsFull(plc))
                        yield return true;
                }

                ui.AddCommLog("", "◆プロトコル完了");
                plc.WriteMR(4311, false);
            }
        }

        static IEnumerable<bool> IsFull(PLCDevice plc)
        {
            //現在の書き込みポインタを読み込み
            while (true)
            {
                var rp = plc.ReadDM(29400);
                var wp = (plc.ReadDM(29401) + 1) % 10;
                if (rp == wp)
                {
                    //満杯
                    yield return true;
                }
                else
                {
                    //空きあり
                    yield break;
                }
            }
        }

        static void InnerLogWrite(PLCDevice plc, ushort no, ushort cmd, DateTime st, DateTime ed, ushort[] data)
        {
            //現在の書き込みポインタを読み込み
            var wp = plc.ReadDM(29401);

            //開始時刻
            var st_hh = st.Hour * 10000;
            var st_mm = st.Minute * 100;
            var st_ss = st.Second * 1;
            var st_dt = (uint)(st_hh + st_mm + st_ss);

            //完了時刻
            var ed_hh = ed.Hour * 10000;
            var ed_mm = ed.Minute * 100;
            var ed_ss = ed.Second * 1;
            var ed_dt = (uint)(ed_hh + ed_mm + ed_ss);

            var address = 29000 + 40 * wp;
            plc.WriteDM(address + 0, no);            //行番号
            plc.WriteDM(address + 1, cmd);           //PID
            plc.WriteDM2(address + 2, st_dt);        //開始時間
            plc.WriteDM2(address + 4, ed_dt);        //終了時間
            plc.WriteDM2(address + 6, data[0]);      //コマンド1
            plc.WriteDM2(address + 8, data[1]);      //コマンド2
            plc.WriteDM2(address + 10, data[2]);     //コマンド3
            plc.WriteDM2(address + 12, data[3]);     //コマンド4
            plc.WriteDM2(address + 14, data[4]);     //コマンド5
            plc.WriteDM2(address + 16, data[5]);     //コマンド6
            plc.WriteDM2(address + 18, data[6]);     //コマンド7
            plc.WriteDM2(address + 20, data[7]);     //コマンド8
            plc.WriteDM2(address + 22, data[8]);     //コマンド9
            plc.WriteDM2(address + 24, data[9]);     //コマンド10
            plc.WriteDM2(address + 26, data[10]);    //コマンド11
            plc.WriteDM2(address + 28, data[11]);    //コマンド12
            plc.WriteDM2(address + 30, data[12]);    //コマンド13
            plc.WriteDM2(address + 32, data[13]);    //コマンド14
            plc.WriteDM2(address + 34, data[14]);    //コマンド15
            plc.WriteDM2(address + 36, data[15]);    //コマンド16
            plc.WriteDM2(address + 38, data[16]);    //コマンド17

            //次の書き込みポインタへ移動
            var np = (ushort)((wp + 1) % 10);
            plc.WriteDM(29401, np);
        }
    }

    /// <summary>
    /// PLCデバイス
    /// </summary>
    public class PLCDevice
    {
        public readonly ConcurrentDictionary<int, bool> R = new();
        public readonly ConcurrentDictionary<int, bool> MR = new();
        public readonly ConcurrentDictionary<int, ushort> DM = new();
        readonly ConcurrentDictionary<int, bool> _R = new();
        readonly ConcurrentDictionary<int, bool> _MR = new();
        readonly ConcurrentDictionary<int, ushort> _DM = new();

        readonly Task DoTask;
        readonly Task DoPluginsTask;
        public bool IsFinished => DoTask.IsCompleted;

        void Test()
        {
#if true
            //ビット操作の確認
            R.SetBit(1000, 1, new byte[] { 0x10 });
            var ret0 = R.GetWord(1000, 1);
            R.SetBit(1001, 2, new byte[] { 0x01 });
            R.SetBit(1003, 2, new byte[] { 0x11 });
            var ret1 = R.GetBit(1000, 1);
            var ret2 = R.GetBit(1001, 1);
            var ret3 = R.GetBit(1000, 5);

            R.SetWord(1500, 1, new byte[] { 0x01, 0x20 });
            var retA = R.GetWord(1500, 1);
            var retB = R.GetBit(1500, 1);
            var retC = R.GetBit(1500 + 12, 2);
            var retD = R.GetBit(1500 + 13, 1);

            //ワード操作の確認
            R.SetWord(2000, 2, new[] { (byte)0xAB, (byte)0x12, (byte)0x34, (byte)0xCD });
            var ret4 = R.GetWord(2000, 2);
            var keys = R.Keys.ToList();
            keys.Sort();
            foreach (var key in keys)
            {
                var flg = R[key] ? "●" : "－";
                Debug.WriteLine($"{key}:{flg}");
            }
            var ret5 = R.GetBit(2000, 32);

            //DM操作の確認
            DM.SetWord(2000, 2, new[] { (byte)0xAB, (byte)0x12, (byte)0x34, (byte)0xCD });
            var ret6 = DM.GetWord(2000, 2);

            //Thread.Sleep(1000);
            //var ret7 = MR[100];
#endif
        }

        readonly IUIUpdatable Updatable;
        public PLCDevice(int port, IUIUpdatable updatable)
        {
            Updatable = updatable;
            Processor.Execute(this);
            Test();
            DoTask = Start(port);
            DoPluginsTask = PluginsStart();
        }

        Task Start(int port)
        {
            return Task.Run(() =>
            {
                var listener = new TcpListener(IPAddress.Any, port);
                listener.Start();

                while (true)
                {
                    Updatable.UpdateConnect(ListenPacket.Count);
                    new ListenPacket(listener.AcceptSocket(), RecieveAndResponse, Updatable).Start();
                }
            });
        }

        Task PluginsStart()
        {
            var plugins = new List<IEnumerator<bool>>()
            {
                new ProtocolRunner().Start(this, Updatable).GetEnumerator(),   //プロトコル実行
            };

            return Task.Run(() =>
            {
                while (true)
                {
                    foreach (var plugin in plugins)
                        plugin.MoveNext();
                    Thread.Sleep(10);
                }
            });
        }

        /// <summary>
        /// 受信処理
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        byte[] RecieveAndResponse(Socket socket, byte[] bytes)
        {
            var res = new List<byte>
            {
                (byte)(0x80 | bytes[0]),                            //フレーム
                bytes[1],
                bytes[2],                                           //ネットワーク番号
                bytes[3],                                           //PC番号
                bytes[4],                                           //IO要求先ユニット
                bytes[5],
                bytes[6]                                            //要求先ユニット局番号
            };

            var dat = Parse(bytes);                                 //データ解析
            var len = dat.Length + 2;
            res.Add((byte)(len & 0xff));                            //L:応答データ長    7
            res.Add((byte)(len >> 8));                              //H                 8
            res.Add(0x00);                                          //終了コード        9
            res.Add(0x00);
            res.AddRange(dat);                                      //データ追記        10

            return res.ToArray();
        }

        static int AddressDecode(int address)
        {
            return ((address >> 4) * 100) + (address & 0xf);
        }

        static int AddressEncode(int address)
        {
            return ((address / 100) << 4) + (address % 100);
        }


        byte[] Parse(byte[] bytes)
        {
            //var length = bytes[7] + (bytes[8] << 8);              //受信データ長
            //var timer = bytes[9] + (bytes[10] << 8);              //CPU監視タイマ
            var cmd = bytes[11] + (bytes[12] << 8);                 //コマンド
            var sub = bytes[13] + (bytes[14] << 8);                 //サブコマンド
            //データ部
            var adr = bytes[15] | bytes[16] << 8 | bytes[17] << 16; //アドレス
            var dev = bytes[18];                                    //デバイスコード
            var len = bytes[19] | bytes[20] << 8;                   //デバイス数
            var siz = sub == 1 ? 1 : 2;
            var dat = bytes[21..(21 + len * siz)];                  //受信データ

            var _adr = adr;
            adr = AddressDecode(adr);
            switch (cmd)
            {
                case 0x0401:
                    //一括読み込み
                    if (dev == 0x9c && sub == 0)
                    {
                        var resp = R.GetWord(adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Updatable.AddCommLog($"R{adr}", $"Read: len={len}, dat={body}");
                        return resp;
                    }
                    if (dev == 0x9c && sub == 1)
                    {
                        var resp = R.GetBit(adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Updatable.AddCommLog($"R{adr}", $"Read: len={len}, dat={body}");
                        return resp;
                    }
                    if (dev == 0x90 && sub == 0)
                    {
                        var resp = MR.GetWord(adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Updatable.AddCommLog($"MR{adr}", $"Read: len={len}, dat={body}");
                        return resp;
                    }
                    if (dev == 0x90 && sub == 1)
                    {
                        var resp = MR.GetBit(adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Updatable.AddCommLog($"MR{adr}", $"Read: len={len}, dat={body}");
                        return resp;
                    }
                    if (dev == 0xA8 && sub == 0)
                    {
                        adr = _adr;
                        var resp = DM.GetWord(adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Updatable.AddCommLog($"DM{adr}", $"Read: len={len}, dat={body}");
                        return resp;
                    }
                    break;

                case 0x1401:
                    //一括書き込み
                    try
                    {
                        var body = string.Join(" ", dat.Select(_ => _.ToString("X2")));
                        if (dev == 0x9c && sub == 0)
                        {
                            Updatable.AddCommLog($"R{adr}", $"Write:len={len}, dat={body}");
                            return R.SetWord(adr, len, dat);
                        }
                        if (dev == 0x9c && sub == 1)
                        {
                            Updatable.AddCommLog($"R{adr}", $"Write: R  len={len}, dat={body}");
                            return R.SetBit(adr, len, dat);
                        }
                        if (dev == 0x90 && sub == 0)
                        {
                            Updatable.AddCommLog($"MR{adr}", $"Write:len={len}, dat={body}");
                            return MR.SetWord(adr, len, dat);
                        }
                        if (dev == 0x90 && sub == 1)
                        {
                            Updatable.AddCommLog($"MR{adr}", $"Write:len={len}, dat={body}");
                            return MR.SetBit(adr, len, dat);
                        }
                        if (dev == 0xA8 && sub == 0)
                        {
                            adr = _adr;
                            Updatable.AddCommLog($"DM{adr}", $"Write:len={len}, dat={body}");
                            return DM.SetWord(adr, len, dat);
                        }
                    }
                    finally
                    {
                        Processor.Execute(this);
                    }
                    break;

                default:
                    break;
            }
            return Array.Empty<byte>();
        }

        public bool ReadR(int address) => MR.TryGetValue(address, out bool value) && value;
        public bool ReadMR(int address) => MR.TryGetValue(address, out bool value) && value;
        public ushort ReadDM(int address) => DM.TryGetValue(address, out ushort value) ? value : (ushort)0;

        public void WriteR(int address, bool flg)
        {
            R[address] = flg;
            Updatable.AddCommLog($" R{address}", $"Write {flg}");
        }
        public void WriteMR(int address, bool flg)
        {
            MR[address] = flg;
            Updatable.AddCommLog($"MR{address}", $"Write {flg}");
        }
        public void WriteDM(int address, ushort value)
        {
            DM[address] = value;
            Updatable.AddCommLog($"DM{address}", $"Write {value}");
        }
        public void WriteDM2(int address, uint value)
        {
            var l = (ushort)(value & 0xffff);
            var h = (ushort)((value >> 16) & 0xffff);
            WriteDM(address + 0, l);
            WriteDM(address + 1, h);
        }

    }
}