using System.Text;

namespace Sofar.Common.Helper
{
    public static class HexDataHelper
    {
        public static string ByteToHexString(byte[] data)
        {
            if (data == null) return "";
            string hexStr = "";
            foreach (byte b in data)
            {
                hexStr += (b.ToString("X2") + " ");//一个字节一个字节的处理，
            }
            
            return hexStr.Remove(hexStr.Length - 1);
        }

        public static string ByteToHexString(byte[] data, bool add0x = false)
        {
            if (data == null) return "";
            StringBuilder sb = new StringBuilder();//清除字符串构造器的内容
            foreach (byte b in data)
            {
                sb.Append(b.ToString("X2"));//字节间不加空格
            }
            return add0x ? "0x" + sb.ToString() : sb.ToString();
        }

        /// <summary>
        /// 字符串转byte数组
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] HexStringToByte(string str)
        {
            var arr = str.Split(' ');
            byte[] result = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                if (string.IsNullOrEmpty(arr[i])) continue;
                int n = Convert.ToInt32(arr[i], 16);
                result[i] = (byte)n;
            }
            return result;
        }

        /// <summary>
        /// 字符串转byte数组
        /// 如 1234 =》{0x12, 0x34 }
        /// 小端
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] HexStringToByte2(string str)
        {
            List<byte> list = new List<byte>();
            for (int i = str.Length - 2; i >= 0; i -= 2)
            {
                string value = str.Substring(i, 2);
                byte b = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
                list.Add(b);
            }
            return list.ToArray();
        }

        public static byte[] ShortToByte(short value)
        {
            byte[] data = new byte[2];
            int k = 0;
            data[k++] = (byte)(value >> 8);
            data[k++] = (byte)(value & 0xff);
            return data;
        }

        public static byte[] ShortToByte(ushort value)
        {
            byte[] data = new byte[2];
            int k = 0;
            data[k++] = (byte)(value >> 8);
            data[k++] = (byte)(value & 0xff);
            return data;
        }

        public static byte[] ShortToByte(short value, bool IsLittleEndian = false)
        {
            byte[] data = new byte[2];
            int k = 0;
            if (IsLittleEndian)
            {
                data[k++] = (byte)(value & 0xff);
                data[k++] = (byte)(value >> 8);
            }
            else
            {
                data[k++] = (byte)(value >> 8);
                data[k++] = (byte)(value & 0xff);
            }
            return data;
        }

        public static byte[] UShortToByte(ushort value, bool IsLittleEndian = false)
        {
            byte[] data = new byte[2];
            int k = 0;
            if (IsLittleEndian)
            {
                data[k++] = (byte)(value & 0xff);
                data[k++] = (byte)(value >> 8);
            }
            else
            {
                data[k++] = (byte)(value >> 8);
                data[k++] = (byte)(value & 0xff);
            }
            return data;
        }

        public static byte[] IntToByte(int value, bool IsLittleEndian)
        {
            byte[] data = new byte[4];
            int k = 0;
            if (IsLittleEndian)
            {
                data[k++] = (byte)(value >> 24);
                data[k++] = (byte)(value >> 16);
                data[k++] = (byte)(value >> 8);
                data[k++] = (byte)(value & 0xff);
            }
            else//big-Endian
            {
                data[k++] = (byte)(value >> 8);
                data[k++] = (byte)(value & 0xff);
                data[k++] = (byte)(value >> 24);
                data[k++] = (byte)(value >> 16);
            }

            return data;
        }

        public static byte[] IntToByteInverse(int value, bool IsLittleEndian)
        {
            byte[] data = new byte[4];
            int k = 0;
            if (IsLittleEndian)
            {
                data[k++] = (byte)(value & 0xff);
                data[k++] = (byte)(value >> 8);
                data[k++] = (byte)(value >> 16);
                data[k++] = (byte)(value >> 24);
            }
            else//big-Endian
            {
                data[k++] = (byte)(value >> 24);
                data[k++] = (byte)(value >> 16);
                data[k++] = (byte)(value >> 8);
                data[k++] = (byte)(value & 0xff);
            }

            return data;
        }

        public static byte[] UIntToByte(uint value, bool IsLittleEndian)
        {
            byte[] data = new byte[4];
            int k = 0;
            if (IsLittleEndian)
            {
                data[k++] = (byte)(value >> 24);
                data[k++] = (byte)(value >> 16);
                data[k++] = (byte)(value >> 8);
                data[k++] = (byte)(value & 0xff);
            }
            else //big-Endian
            {
                data[k++] = (byte)(value >> 8);
                data[k++] = (byte)(value & 0xff);
                data[k++] = (byte)(value >> 24);
                data[k++] = (byte)(value >> 16);
            }
            return data;
        }

        public static int ByteToShort(byte[] buffer)
        {
            var byte1 = buffer[0] << 8;
            var byte2 = buffer[1] & 0xff;
            int u32 = Convert.ToInt32(byte1 + byte2);
            return u32;
        }

        public static int ByteToInt(byte[] buffer, bool IsLittleEndian)
        {
            int byte1, byte2, byte3, byte4;
            if (IsLittleEndian)
            {
                byte1 = buffer[0] << 24;
                byte2 = buffer[1] << 16;
                byte3 = buffer[2] << 8;
                byte4 = buffer[3] & 0xff;
                int u32 = Convert.ToInt32(byte1 + byte2 + byte3 + byte4);
                return u32;
            }
            else
            {
                byte1 = buffer[0] << 8;
                byte2 = buffer[1] & 0xff;
                byte3 = buffer[2] << 24;
                byte4 = buffer[3] << 16;
                int u32 = Convert.ToInt32(byte1 + byte2 + byte3 + byte4);
                return u32;
            }
        }

        public static uint ByteToUInt(byte[] buffer, bool IsLittleEndian)
        {
            uint byte1, byte2, byte3, byte4;
            if (IsLittleEndian)
            {
                byte1 = (uint)buffer[0] << 24;
                byte2 = (uint)buffer[1] << 16;
                byte3 = (uint)buffer[2] << 8;
                byte4 = (uint)buffer[3] & 0xff;
                uint u32 = Convert.ToUInt32(byte1 + byte2 + byte3 + byte4);
                return u32;
            }
            else
            {
                byte1 = (uint)buffer[0] << 8;
                byte2 = (uint)buffer[1] & 0xff;
                byte3 = (uint)buffer[2] << 24;
                byte4 = (uint)buffer[3] << 16;
                uint u32 = Convert.ToUInt32(byte1 + byte2 + byte3 + byte4);
                return u32;
            }
        }

        public static byte[] LongToByte(long value, bool IsLittleEndian)
        {
            byte[] data = new byte[8];
            int k = 0;

            if (IsLittleEndian)
            {
                data[k++] = (byte)(value >> 56);
                data[k++] = (byte)(value >> 48);
                data[k++] = (byte)(value >> 40);
                data[k++] = (byte)(value >> 32);
                data[k++] = (byte)(value >> 24);
                data[k++] = (byte)(value >> 16);
                data[k++] = (byte)(value >> 8);
                data[k++] = (byte)(value & 0xff);
            }
            else//big-Endian
            {
                data[k++] = (byte)(value & 0xff);
                data[k++] = (byte)(value >> 8);
                data[k++] = (byte)(value >> 16);
                data[k++] = (byte)(value >> 24);
                data[k++] = (byte)(value >> 32);
                data[k++] = (byte)(value >> 40);
                data[k++] = (byte)(value >> 48);
                data[k++] = (byte)(value >> 56);
            }
            return data;
        }

        public static string GetDebugByteString(byte[] data, string title)
        {
            if (data == null) return "";
            StringBuilder sb = new StringBuilder();//清除字符串构造器的内容
            sb.AppendFormat("{0} {1} ", DateTime.Now, title);

            foreach (byte b in data)
            {
                sb.Append(b.ToString("X2") + " ");//一个字节一个字节的处理，
            }
            return sb.ToString();
        }
    }
}