using System.Net.Sockets;
using System.Text;

namespace Mult_Reagent1
{
    public class plcio
	{
		public int completion_flg;

		public int initlen = 0;

		private Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		private static Socket _socket_status = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

		private int ConvRelayAddress2Offset(int address)
		{
			return address / 100 * 16 + address % 100;
		}

		public byte[] ReadR(int type, int _address, int readWordLen)
		{
			int address = ConvRelayAddress2Offset(_address);
			List<byte> bList = new List<byte>();
			bList.Add(80);
			bList.Add(0);
			bList.Add(0);
			bList.Add(byte.MaxValue);
			bList.Add(byte.MaxValue);
			bList.Add(3);
			bList.Add(0);
			bList.Add(12);
			bList.Add(0);
			bList.Add(16);
			bList.Add(0);
			bList.Add(1);
			bList.Add(4);
			bList.Add(0);
			bList.Add(0);
			int a1 = address >> 16;
			int a2 = (address >> 8) & 0xFF;
			int a3 = address & 0xFF;
			bList.Add((byte)a3);
			bList.Add((byte)a2);
			bList.Add((byte)a1);
			bList.Add(156);
			int b1 = readWordLen >> 8;
			int b2 = readWordLen & 0xFF;
			bList.Add((byte)b2);
			bList.Add((byte)b1);
			if (type == 0)
			{
				return SendAndRecieve(bList.ToArray());
			}
			return SendAndRecieve_Status(bList.ToArray());
		}

		public byte[] ReadMr(int type, int _address, int readWordLen)
		{
			int address = ConvRelayAddress2Offset(_address);
			List<byte> bList = new List<byte>();
			bList.Add(80);
			bList.Add(0);
			bList.Add(0);
			bList.Add(byte.MaxValue);
			bList.Add(byte.MaxValue);
			bList.Add(3);
			bList.Add(0);
			bList.Add(12);
			bList.Add(0);
			bList.Add(16);
			bList.Add(0);
			bList.Add(1);
			bList.Add(4);
			bList.Add(0);
			bList.Add(0);
			int a1 = address >> 16;
			int a2 = (address >> 8) & 0xFF;
			int a3 = address & 0xFF;
			bList.Add((byte)a3);
			bList.Add((byte)a2);
			bList.Add((byte)a1);
			bList.Add(144);
			int b1 = readWordLen >> 8;
			int b2 = readWordLen & 0xFF;
			bList.Add((byte)b2);
			bList.Add((byte)b1);
			if (type == 0)
			{
				return SendAndRecieve(bList.ToArray());
			}
			return SendAndRecieve_Status(bList.ToArray());
		}

		private byte[] SendAndRecieve_Status(byte[] data)
		{
			List<byte> bList = new List<byte>();
			if (Setting.Value.plsdata.debug_1 == 1)
			{
				for (int i = 0; i < 50; i++)
				{
					bList.Add(0);
				}
				return bList.ToArray();
			}
			_socket_status.Send(data, data.Length, SocketFlags.None);
			byte[] byteReciveMessage = new byte[1];
			int reciveSize = 0;
			while (_socket_status.Available == 0)
			{
				Thread.Sleep(10);
			}
			while (_socket_status.Available != 0)
			{
				try
				{
					byteReciveMessage = new byte[_socket_status.Available];
					reciveSize = _socket_status.Receive(byteReciveMessage, byteReciveMessage.GetLength(0), SocketFlags.None);
					byte[] array = byteReciveMessage;
					foreach (byte b in array)
					{
						bList.Add(b);
					}
				}
				catch (Exception e)
				{
					Mult_Reagent1.Log.Log.Error("セッションが切断されました。" + e.ToString(), "SendAndRecieve_Status", "C:\\job\\スタック\\開発\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\common\\Plcio.cs", 792);
					Console.WriteLine("セッションが切断されました。" + e.ToString());
				}
			}
			return bList.ToArray();
		}

		private byte[] SendAndRecieve(byte[] data)
		{
			List<byte> bList = new List<byte>();
			if (Setting.Value.plsdata.debug_1 == 1)
			{
				for (int i = 0; i < 50; i++)
				{
					bList.Add(0);
				}
				return bList.ToArray();
			}
			_socket.Send(data, data.Length, SocketFlags.None);
			byte[] byteReciveMessage = new byte[1];
			int reciveSize = 0;
			while (_socket.Available == 0)
			{
				Thread.Sleep(10);
			}
			while (_socket.Available != 0)
			{
				try
				{
					byteReciveMessage = new byte[_socket.Available];
					reciveSize = _socket.Receive(byteReciveMessage, byteReciveMessage.GetLength(0), SocketFlags.None);
					byte[] array = byteReciveMessage;
					foreach (byte b in array)
					{
						bList.Add(b);
					}
				}
				catch (Exception e)
				{
					Mult_Reagent1.Log.Log.Error("セッションが切断されました。" + e.ToString(), "SendAndRecieve", "C:\\job\\スタック\\開発\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\Mult_Reagent1\\common\\Plcio.cs", 843);
					Console.WriteLine("セッションが切断されました。" + e.ToString());
					S_Close();
				}
			}
			return bList.ToArray();
		}

		public byte[] WriteR(int type, int _address, bool on)
		{
			int address = ConvRelayAddress2Offset(_address);
			List<byte> bList = new List<byte>();
			bList.Add(80);
			bList.Add(0);
			bList.Add(0);
			bList.Add(byte.MaxValue);
			bList.Add(byte.MaxValue);
			bList.Add(3);
			bList.Add(0);
			bList.Add(13);
			bList.Add(0);
			bList.Add(16);
			bList.Add(0);
			bList.Add(1);
			bList.Add(20);
			bList.Add(1);
			bList.Add(0);
			int a1 = address >> 16;
			int a2 = (address >> 8) & 0xFF;
			int a3 = address & 0xFF;
			bList.Add((byte)a3);
			bList.Add((byte)a2);
			bList.Add((byte)a1);
			bList.Add(156);		//R
			int length = 1;
			int b1 = length >> 8;
			int b2 = length & 0xFF;
			bList.Add((byte)b2);
			bList.Add((byte)b1);
			if (on)
			{
				bList.Add(16);
			}
			else
			{
				bList.Add(0);
			}
			if (type == 0)
			{
				return SendAndRecieve(bList.ToArray());
			}
			return SendAndRecieve_Status(bList.ToArray());
		}

		public byte[] WriteMr(int type, int _address, bool on)
		{
			int address = ConvRelayAddress2Offset(_address);
			List<byte> bList = new List<byte>();
			bList.Add(80);
			bList.Add(0);
			bList.Add(0);
			bList.Add(byte.MaxValue);
			bList.Add(byte.MaxValue);
			bList.Add(3);
			bList.Add(0);
			bList.Add(13);
			bList.Add(0);
			bList.Add(16);
			bList.Add(0);
			bList.Add(1);
			bList.Add(20);
			bList.Add(1);
			bList.Add(0);
			int a1 = address >> 16;
			int a2 = (address >> 8) & 0xFF;
			int a3 = address & 0xFF;
			bList.Add((byte)a3);
			bList.Add((byte)a2);
			bList.Add((byte)a1);
			bList.Add(144);		//MR
			int length = 1;
			int b1 = length >> 8;
			int b2 = length & 0xFF;
			bList.Add((byte)b2);
			bList.Add((byte)b1);
			if (on)
			{
				bList.Add(16);
			}
			else
			{
				bList.Add(0);
			}
			if (type == 0)
			{
				return SendAndRecieve(bList.ToArray());
			}
			return SendAndRecieve_Status(bList.ToArray());
		}

		public string ReadDm_Qrproc()
		{
			int address = 300;
			int readWordLen = 50;
			byte[] byteReciveMessage = ReadDm(0, address, readWordLen);
			int wordLen = (byteReciveMessage[11] + 1) / 2;
			int offset = 13;
			List<byte> bList = new List<byte>();
			for (int i = 0; i < wordLen; i++)
			{
				bList.Add(byteReciveMessage[offset + 1]);
				bList.Add(byteReciveMessage[offset]);
				offset += 2;
			}
			return Encoding.UTF8.GetString(bList.ToArray());
		}

		public byte[] ReadDm_Qr(int address, int readWordLen)
		{
			List<byte> bList = new List<byte>();
			bList.Add(80);
			bList.Add(0);
			bList.Add(0);
			bList.Add(byte.MaxValue);
			bList.Add(byte.MaxValue);
			bList.Add(3);
			bList.Add(0);
			bList.Add(12);
			bList.Add(0);
			bList.Add(16);
			bList.Add(0);
			bList.Add(1);
			bList.Add(4);
			bList.Add(0);
			bList.Add(0);
			int a1 = address >> 16;
			int a2 = (address >> 8) & 0xFF;
			int a3 = address & 0xFF;
			bList.Add((byte)a3);
			bList.Add((byte)a2);
			bList.Add((byte)a1);
			bList.Add(168);
			int b1 = readWordLen >> 8;
			int b2 = readWordLen & 0xFF;
			bList.Add((byte)b2);
			bList.Add((byte)b1);
			return SendAndRecieve(bList.ToArray());
		}


        //一括読み出し(bit)			0401	0001●MR
        //一括読み出し(word)		0401	0000●DM
        //一括書き込み(bit)			1401	0001●MR
        //一括書き込み(word)		1401	0000●DM
        //ランダム読み出し(bit)		0403	0001
        //ランダム読み出し(word)	1401	0000

        const byte R = 0xa0;
        const byte MR = 0x90;
        const byte DM = 0xA8;
		//先頭アドレス：３
		//デバイスコード：１
		//デバイス点数：２
		//データ：左から右へ

        public byte[] Response()
		{
            List<byte> bList = new List<byte>();
            bList.Add(0xD0);            //3Eフレーム
            bList.Add(0x00);
            bList.Add(0x00);            //ネットワーク番号（同じものを返す）
            bList.Add(0xff);			//PC番号（同じものを返す）
            bList.Add(0xff);			//L:IO番号要求先ユニット（同じものを返す）
            bList.Add(0x03);            //H:
            bList.Add(0x00);            //要求先ユニット局番号（同じものを返す）		6
            bList.Add(0x06);            //L:応答データ長（応答データ部のデータ長）
            bList.Add(0x00);            //H
            bList.Add(0x00);            //L:終了コード（正常であれば００００）
            bList.Add(0x00);            //H
                                        //以降、応答データ部

			return bList.ToArray();
        }

        public byte[] ReadDm(int type, int address, int readWordLen)
		{
			List<byte> bList = new List<byte>();
			bList.Add(0x50);			//3Eフレーム	(レスポンス0xD000)	0
			bList.Add(0x00);			//
			bList.Add(0);				//ネットワーク番号（固定）			2
			bList.Add(0xff);			//PC番号（固定）					3
            bList.Add(0xff);			//L:IO番号要求先ユニット（固定）	4
            bList.Add(3);				//H:
			bList.Add(0);               //要求先ユニット局番号（固定）		6
            bList.Add(12);				//L:要求データ長					7
			bList.Add(0);				//H
			bList.Add(16);				//L:CPU監視タイマ					9
			bList.Add(0);				//H
			bList.Add(1);				//L:コマンド	一括読み出し		11
			bList.Add(4);				//H:
			bList.Add(0);				//L:サブコマンド					13
			bList.Add(0);				//H:
										//以降、要求データ部				15
			int a1 = address >> 16;
			int a2 = (address >> 8) & 0xFF;
			int a3 = address & 0xFF;
			bList.Add((byte)a3);
			bList.Add((byte)a2);
			bList.Add((byte)a1);
			bList.Add(168);
			int b1 = readWordLen >> 8;
			int b2 = readWordLen & 0xFF;
			bList.Add((byte)b2);
			bList.Add((byte)b1);
			if (type == 0)
			{
				return SendAndRecieve(bList.ToArray());
			}
			return SendAndRecieve_Status(bList.ToArray());
		}

		public byte[] WriteDm(int type, int address, byte[] data)
		{
			List<byte> bList = new List<byte>();
			bList.Add(80);
			bList.Add(0);
			bList.Add(0);
			bList.Add(byte.MaxValue);
			bList.Add(byte.MaxValue);
			bList.Add(3);
			bList.Add(0);
			ushort reqDataLen = (ushort)(12 + data.Length);
			bList.Add((byte)(reqDataLen & 0xFFu));
			bList.Add((byte)(reqDataLen >> 8));
			bList.Add(16);
			bList.Add(0);
			bList.Add(1);			//一括書き込み
			bList.Add(20);
			bList.Add(0);
			bList.Add(0);
			int a1 = address >> 16;
			int a2 = (address >> 8) & 0xFF;
			int a3 = address & 0xFF;
			bList.Add((byte)a3);
			bList.Add((byte)a2);
			bList.Add((byte)a1);
			bList.Add(168);
			int length = data.Length / 2;
			int b1 = length >> 8;
			int b2 = length & 0xFF;
			bList.Add((byte)b2);
			bList.Add((byte)b1);
			bList.AddRange(data);
			if (type == 0)
			{
				return SendAndRecieve(bList.ToArray());
			}
			return SendAndRecieve_Status(bList.ToArray());
		}
	}
}
