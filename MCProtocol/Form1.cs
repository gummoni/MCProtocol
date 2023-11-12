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

        public Task Start(int port)
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

        byte[] ReadRBit(int adr, int len)
        {
            var res = new List<byte>();
            for (var idx = 0; idx < len; idx += 2)
            {
                var val0 = (byte)(R.TryGetValue(adr + idx + 0, out bool _val0) && _val0 ? 0x10 : 0);
                var val1 = (byte)(R.TryGetValue(adr + idx + 1, out bool _val1) && _val1 ? 0x01 : 0);
                res.Insert(0, (byte)(val0 | val1));
            }

            return res.ToArray();
        }

        byte[] ReadMRBit(int adr, int len)
        {
            var res = new List<byte>();
            for (var idx = 0; idx < len; idx += 2)
            {
                var val0 = (byte)(MR.TryGetValue(adr + idx + 0, out bool _val0) && _val0 ? 0x10 : 0);
                var val1 = (byte)(MR.TryGetValue(adr + idx + 1, out bool _val1) && _val1 ? 0x01 : 0);
                res.Insert(0, (byte)(val0 | val1));
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

        byte[] WriteRBit(int adr, int len, byte[] dat)
        {
            for (var idx = 0; idx < len; idx += 2)
            {
                R[adr + idx + 0] = (dat[idx] >> 4) != 0;
                R[adr + idx + 1] = (dat[idx] & 0xf) != 0;
            }

            return Array.Empty<byte>();
        }

        byte[] WriteMRBit(int adr, int len, byte[] dat)
        {
            for (var idx = 0; idx < len; idx += 2)
            {
                MR[adr + idx + 0] = (dat[idx] >> 4) != 0;
                MR[adr + idx + 1] = (dat[idx] & 0xf) != 0;
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
                    if (dev == 0xa0 && sub == 1) return ReadRBit(adr, len);
                    if (dev == 0x90 && sub == 1) return ReadMRBit(adr, len);
                    if (dev == 0xA8 && sub == 0) return ReadDMWord(adr, len);
                    break;

                case 0x1401:
                    //�ꊇ��������
                    if (dev == 0xa0 && sub == 1) return WriteRBit(adr, len, dat);
                    if (dev == 0x90 && sub == 1) return WriteMRBit(adr, len, dat);
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