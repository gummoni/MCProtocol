using System.Net;
using System.Net.Sockets;

namespace MCProtocol
{
    public class PLC
    {
        public readonly Dictionary<int, bool> R = new();
        public readonly Dictionary<int, bool> MR = new();
        public readonly Dictionary<int, ushort> DM = new();
        readonly Task DoTask;
        public bool IsFinished => DoTask.IsCompleted;

        public PLC(int port)
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
                    var socket = listener.AcceptSocket();
                    Task.Run(() =>
                    {
                        using (socket)
                        {
                            try
                            {
                                while (socket.Connected)
                                {
                                    var len = socket.Available;
                                    if (0 == socket.Available || len != socket.Available)
                                    {
                                        len = socket.Available;
                                        Thread.Sleep(1);
                                    }
                                    var request = new byte[len];
                                    var response = RecieveAndResponse(request);
                                    socket.Send(response, response.Length, SocketFlags.None);
                                }
                            }
                            finally
                            {
                            }
                        }
                    });
                }
            });
        }

        /// <summary>
        /// 受信処理
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        byte[] RecieveAndResponse(byte[] bytes)
        {
            var res = new List<byte>();
            res.Add((byte)(0x80 | bytes[0]));                       //フレーム
            res.Add(bytes[1]);
            res.Add(bytes[2]);                                      //ネットワーク番号
            res.Add(bytes[3]);                                      //PC番号
            res.Add(bytes[4]);                                      //IO要求先ユニット
            res.Add(bytes[5]);
            res.Add(bytes[6]);                                      //要求先ユニット局番号

            var dat = Parse(bytes);                                 //データ解析
            var len = dat.Length;
            res.Add((byte)(len & 0xff));                            //L:応答データ長
            res.Add((byte)(len >> 8));                              //H
            res.Add(0x00);                                          //終了コード
            res.Add(0x00);
            res.AddRange(dat);                                      //データ追記

            return res.ToArray();
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

            switch (cmd)
            {
                case 0x0401:
                    //一括読み込み
                    if (dev == 0xa0 && sub == 0) return R.GetWord(adr, len);
                    if (dev == 0xa0 && sub == 1) return R.GetBit(adr, len);
                    if (dev == 0x90 && sub == 0) return MR.GetWord(adr, len);
                    if (dev == 0x90 && sub == 1) return MR.GetBit(adr, len);
                    if (dev == 0xA8 && sub == 0) return DM.GetWord(adr, len);
                    break;

                case 0x1401:
                    //一括書き込み
                    if (dev == 0xa0 && sub == 0) return R.SetWord(adr, len, dat);
                    if (dev == 0xa0 && sub == 1) return R.SetBit(adr, len, dat);
                    if (dev == 0x90 && sub == 0) return MR.SetWord(adr, len, dat);
                    if (dev == 0x90 && sub == 1) return MR.SetBit(adr, len, dat);
                    if (dev == 0xA8 && sub == 0) return DM.SetWord(adr, len, dat);
                    break;

                default:
                    break;
            }
            return Array.Empty<byte>();
        }

    }
}