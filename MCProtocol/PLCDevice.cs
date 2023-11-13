using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace MCProtocol
{

    /// <summary>
    /// PLCデバイス
    /// </summary>
    public class PLCDevice
    {
        public readonly Dictionary<int, bool> R = new();
        public readonly Dictionary<int, bool> MR = new();
        public readonly Dictionary<int, ushort> DM = new();
        readonly Task DoTask;
        public bool IsFinished => DoTask.IsCompleted;

        public PLCDevice(int port)
        {
            DoTask = Start(port);
        }

        Task Start(int port)
        {
            return Task.Run(() =>
            {
                var listener = new TcpListener(IPAddress.Any, port);
                listener.Start();

                while (true)
                {
                    new ListenPacket(listener.AcceptSocket(), RecieveAndResponse).Start();
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
            var dat = bytes[21..(21 + len)];                        //受信データ

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
                        Debug.WriteLine($"Read:  R  adr={adr}, len={len}, dat={body}");
                        return resp;
                    }
                    if (dev == 0x9c && sub == 1)
                    {
                        var resp = R.GetBit(adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Debug.WriteLine($"Read:  R  adr={adr}, len={len}, dat={body}");
                        return resp;
                    }
                    if (dev == 0x90 && sub == 0)
                    {
                        var resp = MR.GetWord(adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Debug.WriteLine($"Read: MR  adr={adr}, len={len}, dat={body}");
                        return resp;
                    }
                    if (dev == 0x90 && sub == 1)
                    {
                        var resp = MR.GetBit(adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Debug.WriteLine($"Read: MR  adr={adr}, len={len}, dat={body}");
                        return resp;
                    }
                    if (dev == 0xA8 && sub == 0)
                    {
                        var resp = DM.GetWord(_adr, len);
                        var body = string.Join(" ", resp.Select(_ => _.ToString("X2")));
                        Debug.WriteLine($"Read: DM  adr={_adr}, len={len}, dat={body}");
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
                            Debug.WriteLine($"Write: R  adr={adr}, len={len}, dat={body}");
                            return R.SetWord(adr, len, dat);
                        }
                        if (dev == 0x9c && sub == 1)
                        {
                            Debug.WriteLine($"Write: R  adr={adr}, len={len}, dat={body}");
                            return R.SetBit(adr, len, dat);
                        }
                        if (dev == 0x90 && sub == 0)
                        {
                            Debug.WriteLine($"Write: MR adr={adr}, len={len}, dat={body}");
                            return MR.SetWord(adr, len, dat);
                        }
                        if (dev == 0x90 && sub == 1)
                        {
                            Debug.WriteLine($"Write: MR adr={adr}, len={len}, dat={body}");
                            return MR.SetBit(adr, len, dat);
                        }
                        if (dev == 0xA8 && sub == 0)
                        {
                            Debug.WriteLine($"Write: DM adr={_adr}, len={len}, dat={body}");
                            return DM.SetWord(_adr, len, dat);
                        }
                    }
                    finally
                    {
                        Processor.Execute(this, Environment.CurrentDirectory);
                    }
                    break;

                default:
                    break;
            }
            return Array.Empty<byte>();
        }

    }
}