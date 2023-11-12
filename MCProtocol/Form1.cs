using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;

namespace MCProtocol
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
    }


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

        static byte GetByte(Dictionary<int, bool> dic, int adr)
        {
            var b0 = dic.TryGetValue(adr + 0, out bool _b0) && _b0 ? 0x01 : 0x00;
            var b1 = dic.TryGetValue(adr + 1, out bool _b1) && _b1 ? 0x02 : 0x00;
            var b2 = dic.TryGetValue(adr + 2, out bool _b2) && _b2 ? 0x04 : 0x00;
            var b3 = dic.TryGetValue(adr + 3, out bool _b3) && _b3 ? 0x08 : 0x00;
            var b4 = dic.TryGetValue(adr + 4, out bool _b4) && _b4 ? 0x10 : 0x00;
            var b5 = dic.TryGetValue(adr + 5, out bool _b5) && _b5 ? 0x20 : 0x00;
            var b6 = dic.TryGetValue(adr + 6, out bool _b6) && _b6 ? 0x40 : 0x00;
            var b7 = dic.TryGetValue(adr + 7, out bool _b7) && _b7 ? 0x80 : 0x00;
            return (byte)(b7 | b6 | b5 | b4 | b3 | b2 | b1 | b0);
        }

        static byte[] GetWord(Dictionary<int, bool> dic, int adr)
        {
            var l = GetByte(dic, adr + 0);
            var h = GetByte(dic, adr + 8);
            return new byte[] { l, h };
        }

        static byte[] GetWord(Dictionary<int, bool> dic, int adr, int len)
        {
            var res = new List<byte>();
            for (var idx = 0; idx < len; idx++)
            {
                res.AddRange(GetWord(dic, adr + idx * 16));
            }
            return res.ToArray();
        }

        static void SetByte(Dictionary<int, bool> dic, int adr, byte val)
        {
            dic[adr + 0] = (val & 0x01) != 0;
            dic[adr + 1] = (val & 0x02) != 0;
            dic[adr + 2] = (val & 0x04) != 0;
            dic[adr + 3] = (val & 0x08) != 0;
            dic[adr + 4] = (val & 0x10) != 0;
            dic[adr + 5] = (val & 0x20) != 0;
            dic[adr + 6] = (val & 0x40) != 0;
            dic[adr + 7] = (val & 0x80) != 0;
        }

        static byte[] SetWord(Dictionary<int, bool> dic, int adr, int len, byte[] val)
        {
            for (var idx = 0; idx < len; idx += 2)
            {
                SetByte(dic, adr + 0, val[idx * 2 + 0]);
                if (1 < val.Length)
                    SetByte(dic, adr + 8, val[idx * 2 + 1]);
            }
            return Array.Empty<byte>();
        }

        static byte GetBit(Dictionary<int, bool> dic, int adr)
        {
            var val0 = (byte)(dic.TryGetValue(adr + 0, out bool _val0) && _val0 ? 0x10 : 0);
            var val1 = (byte)(dic.TryGetValue(adr + 1, out bool _val1) && _val1 ? 0x01 : 0);
            return (byte)(val0 | val1);
        }

        static byte[] GetBit(Dictionary<int, bool> dic, int adr, int len)
        {
            var res = new List<byte>();
            for (var idx = 0; idx < len; idx++)
            {
                res.Insert(0, GetBit(dic, adr + idx * 2));
            }
            return res.ToArray();
        }

        byte[] ReadDMWord(int adr, int len)
        {
            var res = new List<byte>();
            for (var idx = 0; idx < len; idx++)
            {
                var val = DM.TryGetValue(adr + idx, out ushort _val) ? _val : 0;
                res.Insert(0, (byte)(val >> 8));
                res.Insert(0, (byte)(val & 0xff));
            }

            return res.ToArray();
        }

        static byte[] SetBit(Dictionary<int, bool> dic, int adr, int len, byte[] dat)
        {
            for (var idx = 0; idx < len; idx += 2)
            {
                dic[adr + idx + 0] = (dat[idx] >> 4) != 0;
                dic[adr + idx + 1] = (dat[idx] & 0xf) != 0;
            }
            return Array.Empty<byte>();
        }

        byte[] WriteDMWord(int adr, int len, byte[] dat)
        {
            for (var idx = 0; idx < len; idx++)
            {
                DM[adr + idx] = (ushort)((dat[idx] << 8) | (dat[idx]));
            }

            return Array.Empty<byte>();
        }

        byte[] Parse(byte[] bytes)
        {
            //var length = bytes[7] + (bytes[8] << 8);              //��M�f�[�^��
            //var timer = bytes[9] + (bytes[10] << 8);              //CPU�Ď��^�C�}
            var cmd = bytes[11] + (bytes[12] << 8);                 //�R�}���h
            var sub = bytes[13] + (bytes[14] << 8);                 //�T�u�R�}���h
            //�f�[�^��
            var adr = bytes[15] | bytes[16] << 8 | bytes[17] << 16; //�A�h���X
            var dev = bytes[18];                                    //�f�o�C�X�R�[�h
            var len = bytes[19] | bytes[20] << 8;                   //�f�o�C�X��
            var dat = bytes[21..(21 + len)];                        //��M�f�[�^

            switch (cmd)
            {
                case 0x0401:
                    //�ꊇ�ǂݍ���
                    if (dev == 0xa0 && sub == 0) return GetWord(R, adr, len);
                    if (dev == 0xa0 && sub == 1) return GetBit(R, adr, len);
                    if (dev == 0x90 && sub == 0) return GetWord(MR, adr, len);
                    if (dev == 0x90 && sub == 1) return GetBit(MR, adr, len);
                    if (dev == 0xA8 && sub == 0) return ReadDMWord(adr, len);
                    break;

                case 0x1401:
                    //�ꊇ��������
                    if (dev == 0xa0 && sub == 0) return SetWord(R, adr, len, dat);
                    if (dev == 0xa0 && sub == 1) return SetBit(R, adr, len, dat);
                    if (dev == 0x90 && sub == 0) return SetWord(MR, adr, len, dat);
                    if (dev == 0x90 && sub == 1) return SetBit(MR, adr, len, dat);
                    if (dev == 0xA8 && sub == 0) return WriteDMWord(adr, len, dat);
                    break;

                default:
                    break;
            }
            return Array.Empty<byte>();
        }

        /// <summary>
        /// ��M����
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        byte[] RecieveAndResponse(byte[] bytes)
        {
            var res = new List<byte>();
            res.Add((byte)(0x80 | bytes[0]));                       //�t���[��
            res.Add(bytes[1]);
            res.Add(bytes[2]);                                      //�l�b�g���[�N�ԍ�
            res.Add(bytes[3]);                                      //PC�ԍ�
            res.Add(bytes[4]);                                      //IO�v���惆�j�b�g
            res.Add(bytes[5]);
            res.Add(bytes[6]);                                      //�v���惆�j�b�g�ǔԍ�

            var dat = Parse(bytes);                                 //�f�[�^���
            var len = dat.Length;
            res.Add((byte)(len & 0xff));                            //L:�����f�[�^��
            res.Add((byte)(len >> 8));                              //H
            res.Add(0x00);                                          //�I���R�[�h
            res.Add(0x00);
            res.AddRange(dat);                                      //�f�[�^�ǋL

            return res.ToArray();
        }
    }
}