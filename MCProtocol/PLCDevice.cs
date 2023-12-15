using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace MCProtocol
{

    /// <summary>
    /// PLCデバイス
    /// </summary>
    public class PLCDevice
    {
        public readonly ConcurrentDictionary<int, bool> R = new();
        public readonly ConcurrentDictionary<int, bool> MR = new();
        public readonly ConcurrentDictionary<int, ushort> DM = new();

        void Test()
        {
#if false
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


            //DM.SetWord(2000, 20, new byte[] { 01, 00, 00, 00, 00, 00, 00, 00, 01, 00, 00, 00, 01, 00, 00, 00, 01, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 });
            DM.SetWord(2000, 20, new byte[] { 0x06, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0xE8, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            var _cmd = ReadDM(2000);
            var _ref = ReadDM(2001);
            var _wai = ReadDM(2002);
            var _arg1 = ReadDM(2004);
            var _arg2= ReadDM(2006);
            var _arg3 = ReadDM(2008);
            var _arg4 = ReadDM(2010);

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
            Start(port);
            PluginsStart();
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

            var dmp = string.Join(" ", res.Select(_ => $"{_:X2}"));
            Debug.WriteLine($"RESP:{dmp}");

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
                        if (resp.Length != 4)
                            ;
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
        public uint ReadDM2(int address)
        {
            var l = DM.TryGetValue(address + 0, out ushort valueL) ? valueL : (ushort)0;
            var h = DM.TryGetValue(address + 1, out ushort valueH) ? valueH : (ushort)0;
            var ret = (uint)((h << 16) + l);
            return ret;
        }

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