using System.Collections.Concurrent;
using System.Diagnostics;

namespace MCProtocol
{
    public static class MCProtocolExtensions
    {
        static int Conv(int adr)
        {
            var l = adr / 100;
            var k = adr % 100;
            var c = k / 16;
            var h = k % 16;
            return (l + c) * 100 + h;
        }

        public static byte GetByte(this ConcurrentDictionary<int, bool> dic, int adr)
        {
            var b0 = dic.TryGetValue(Conv(adr + 0), out bool _b0) && _b0 ? 0x01 : 0x00;
            var b1 = dic.TryGetValue(Conv(adr + 1), out bool _b1) && _b1 ? 0x02 : 0x00;
            var b2 = dic.TryGetValue(Conv(adr + 2), out bool _b2) && _b2 ? 0x04 : 0x00;
            var b3 = dic.TryGetValue(Conv(adr + 3), out bool _b3) && _b3 ? 0x08 : 0x00;
            var b4 = dic.TryGetValue(Conv(adr + 4), out bool _b4) && _b4 ? 0x10 : 0x00;
            var b5 = dic.TryGetValue(Conv(adr + 5), out bool _b5) && _b5 ? 0x20 : 0x00;
            var b6 = dic.TryGetValue(Conv(adr + 6), out bool _b6) && _b6 ? 0x40 : 0x00;
            var b7 = dic.TryGetValue(Conv(adr + 7), out bool _b7) && _b7 ? 0x80 : 0x00;
            return (byte)(b7 | b6 | b5 | b4 | b3 | b2 | b1 | b0);
        }

        public static byte[] GetWord(this ConcurrentDictionary<int, bool> dic, int adr)
        {
            var l = GetByte(dic, adr + 8);
            var h = GetByte(dic, adr + 0);
            return new byte[] { l, h };
        }

        public static byte[] GetWord(this ConcurrentDictionary<int, bool> dic, int adr, int len)
        {
            var res = new List<byte>();
            for (var idx = 0; idx < len; idx++)
            {
                res.AddRange(GetWord(dic, adr + idx * 16));
            }
            return res.ToArray();
        }

        public static void SetByte(this ConcurrentDictionary<int, bool> dic, int adr, byte val)
        {
            dic[Conv(adr + 0)] = (val & 0x01) != 0;
            dic[Conv(adr + 1)] = (val & 0x02) != 0;
            dic[Conv(adr + 2)] = (val & 0x04) != 0;
            dic[Conv(adr + 3)] = (val & 0x08) != 0;
            dic[Conv(adr + 4)] = (val & 0x10) != 0;
            dic[Conv(adr + 5)] = (val & 0x20) != 0;
            dic[Conv(adr + 6)] = (val & 0x40) != 0;
            dic[Conv(adr + 7)] = (val & 0x80) != 0;
        }

        public static byte[] SetBit(this ConcurrentDictionary<int, bool> dic, int adr, int len, byte[] dat)
        {
            for (var idx = 0; idx < len; idx += 2)
            {
                dic[Conv(adr + idx + 0)] = (dat[idx] & 0xf0) != 0;
                if ((idx + 1) < len)
                    dic[Conv(adr + idx + 1)] = (dat[idx] & 0x0f) != 0;
            }
            return Array.Empty<byte>();
        }

        public static byte[] SetWord(this ConcurrentDictionary<int, bool> dic, int adr, int len, byte[] val)
        {
            for (var idx = 0; idx < len; idx++)
            {
                SetByte(dic, adr + idx * 16 + 0, val[idx * 2 + 1]);
                SetByte(dic, adr + idx * 16 + 8, val[idx * 2 + 0]);
            }
            return Array.Empty<byte>();
        }

        public static byte GetBit(this ConcurrentDictionary<int, bool> dic, int adr, bool isEven)
        {
            if (isEven)
            {
                var val0 = (byte)(dic.TryGetValue(Conv(adr + 0), out bool _val0) && _val0 ? 0x10 : 0);
                var val1 = (byte)(dic.TryGetValue(Conv(adr + 1), out bool _val1) && _val1 ? 0x01 : 0);
                return (byte)(val0 | val1);
            }
            else
            {
                var val0 = (byte)(dic.TryGetValue(Conv(adr + 0), out bool _val0) && _val0 ? 0x10 : 0);
                return (byte)(val0);
            }
        }

        public static byte[] GetBit(this ConcurrentDictionary<int, bool> dic, int adr, int len)
        {
            var res = new List<byte>();
            for (var idx = 0; idx < len; idx += 2)
            {
                res.Add(GetBit(dic, adr + idx, (idx + 1) < len));
            }
            return res.ToArray();
        }

        public static byte[] GetWord(this ConcurrentDictionary<int, ushort> dic, int adr, int len)
        {
            var res = new List<byte>();
            for (var idx = 0; idx < len; idx++)
            {
                var val = dic.TryGetValue(adr + idx, out ushort _val) ? _val : 0;
                res.Add((byte)(val & 0xff));
                res.Add((byte)(val >> 8));
            }

            return res.ToArray();
        }

        public static byte[] SetWord(this ConcurrentDictionary<int, ushort> dic, int adr, int len, byte[] dat)
        {
            var cnt = 0;
            try
            {
                for (var idx = 0; idx < len / 2; idx++, cnt++)
                {
                    dic[adr + cnt] = (ushort)((dat[idx * 2 + 1] << 8) | (dat[idx * 2 + 0]));
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return Array.Empty<byte>();
        }

        //ビット単位で読み書きする場合、ニブルで表現
        //byte[] = | B0 | B1 | B2 | B3 
        //           10   01   10   01
        //           ||   ||   ||   ||
        //           ||   ||   ||   |+---R07
        //           ||   ||   ||   +----R06
        //           ||   ||   |+--------R05
        //           ||   ||   +---------R04
        //           ||   |+-------------R03
        //           ||   +--------------R02
        //           |+------------------R01
        //           +-------------------R00
        //
        //

        //ワード単位で読み書きする場合、1ワード内ビットフラグで表現
        //ushort[] = | B1(LH) | B1(LH) | B2(LH) | B3(LH) |
        //     ------------------------
        //       |   L   | |   H   |
        //     ------------------------
        //B0 = 0b1000 0000 1000 0000　
        //       |||| |||| |||| |||+b0
        //       |||| |||| |||| ||+-b1
        //       |||| |||| |||| |+--b2
        //       |||| |||| |||| +---b3
        //       |||| |||| |||+-----b4
        //       |||| |||| ||+------b5
        //       |||| |||| |+-------b6
        //       |||| |||| +--------b7
        //       |||| ||||
        //       |||| |||+----------b8
        //       |||| ||+-----------b9
        //       |||| |+------------b10
        //       |||| +-------------b11
        //       |||+---------------b12
        //       ||+----------------b13
        //       |+-----------------b14
        //       +------------------b15
        //


    }
}