using System;
using System.Collections.Generic;
using Swifter.Json;

namespace WeavingDB
{
    public class DataEncoding
    {
        public static string userid;
        public static string pwd;

        static int ConvertToInt(byte[] list)
        {
            int ret = 0;
            int i = 0;
            foreach (byte item in list)
            {
                ret += (item << i);
                i += 8;
            }
            return ret;
        }

        static byte[] ConvertToByteList(int v)
        {
            List<byte> ret = new List<byte>();
            int value = v;
            while (value != 0)
            {
                ret.Add((byte)value);
                value >>= 8;
            }
            byte[] bb = new byte[ret.Count];
            ret.CopyTo(bb);
            return bb;
        }

        public static T BytesToT<T>(byte[] bytes)
        {
            var str = System.Text.Encoding.UTF8.GetString(bytes);
            // return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
            return JsonFormatter.DeserializeObject<T>(str);
        }

        public static byte[] TToBytes<T>(T obj)
        {
            //string str = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            var str = JsonFormatter.SerializeObject(obj);
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static byte[] EncodingsetKV(string key, byte[] data)
        {
            byte[] users = Userpwdencoding();
            byte[] keys = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] rowdata = new byte[users.Length + 1 + keys.Length + data.Length];
            Array.Copy(users, 0, rowdata, 0, users.Length);
            rowdata[users.Length] = (byte)keys.Length;
            Array.Copy(keys, 0, rowdata, users.Length + 1, keys.Length);
            Array.Copy(data, 0, rowdata, users.Length + 1 + keys.Length, data.Length);
            return rowdata;
        }

        /// <summary>
        /// 将多个String 编码成可分离的byte[]
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static byte[] Encodingdata(params string[] datas)
        {
            List<byte> list = new List<byte>();
            foreach (string str in datas)
            {
                byte[] tem = System.Text.Encoding.UTF8.GetBytes(str);
                byte[] lens = ConvertToByteList(tem.Length);
                list.Add((byte)lens.Length);
                list.AddRange(lens);
                list.AddRange(tem);
            }
            return list.ToArray();
        }

        /// <summary>
        /// 将可分离的byte[]，转换为多个string
        /// </summary>
        /// <param name="datas"></param>
        /// <returns></returns>
        public static string[] Dencdingdata(byte[] datas)
        {
            List<string> list = new List<string>();
            while (true)
            {
                int len = datas[0];
                byte[] lenb = new byte[len];
                Array.Copy(datas, 1, lenb, 0, len);
                int lens = ConvertToInt(lenb);
                lenb = new byte[lens];
                Array.Copy(datas, 1 + len, lenb, 0, lens);
                list.Add(System.Text.Encoding.UTF8.GetString(lenb));

                if (datas.Length == 1 + len + lens)
                    break;

                byte[] temp = new byte[datas.Length - (1 + len + lens)];
                Array.Copy(datas, 1 + len + lens, temp, 0, temp.Length);
                datas = temp;
            }
            return list.ToArray();
        }

        public static byte[] EncodinggetKV(string key)
        {
            byte[] users = Userpwdencoding();
            byte[] keys = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] rowdata = new byte[users.Length + 1 + keys.Length];
            Array.Copy(users, 0, rowdata, 0, users.Length);
            rowdata[users.Length] = (byte)keys.Length;
            Array.Copy(keys, 0, rowdata, users.Length + 1, keys.Length);
            return rowdata;
        }

        static byte[] Userpwdencoding()
        {
            byte[] uu = System.Text.Encoding.UTF8.GetBytes(userid);
            byte[] pp = System.Text.Encoding.UTF8.GetBytes(pwd);
            byte[] uupp = new byte[uu.Length + pp.Length + 2];
            uupp[0] = (byte)uu.Length;
            Array.Copy(uu, 0, uupp, 1, uu.Length);
            uupp[uu.Length + 1] = (byte)pp.Length;
            Array.Copy(pp, 0, uupp, 2 + uu.Length, pp.Length);
            return uupp;
        }
        public static bool SetKVdecode(byte[] rowsdata, out string key, out byte[] data,out int timeout)
        {
            key = "";
            data = new byte[0];
            timeout = 0;
            try
            {
                bool bb = Userpwddecode(rowsdata, out data);
                byte len = data[0];
                byte[] keyb = new byte[len];
                Array.Copy(data, 1, keyb, 0, keyb.Length);
                key = System.Text.Encoding.UTF8.GetString(keyb);
                byte[] data2 = new byte[data.Length - (len + 1+ 4)];
                Array.Copy(data, len + 1, data2, 0, data2.Length);
                
                byte[] time = new byte[4];
                Array.Copy(data, data.Length-4, time, 0, 4);
                data = data2;
                timeout = byteArrayToInt(time);
                return bb;
            }
            catch
            { return false; }
        }
        public static bool SetKVdecode(byte[] rowsdata, out string key, out byte[] data)
        {
            key = "";
            data = new byte[0];
            try
            {
                bool bb = Userpwddecode(rowsdata, out data);
                byte len = data[0];
                byte[] keyb = new byte[len];
                Array.Copy(data, 1, keyb, 0, keyb.Length);
                key = System.Text.Encoding.UTF8.GetString(keyb);
                byte[] data2 = new byte[data.Length - (len + 1)];
                Array.Copy(data, len + 1, data2, 0, data2.Length);
                data = data2;
                return bb;
            }
            catch
            { return false; }
        }

        public static byte[] EncodingsetKVs(string[] key, byte[] data)
        {
            byte[] users = Userpwdencoding();
            byte[] keys = Encodingdata(key);
            byte[] lens = ConvertToByteList(keys.Length);
            byte[] rowdata = new byte[users.Length + 1 + keys.Length + data.Length + lens.Length];
            Array.Copy(users, 0, rowdata, 0, users.Length);
            rowdata[users.Length] = (byte)lens.Length;
            Array.Copy(lens, 0, rowdata, users.Length + 1, lens.Length);
            Array.Copy(keys, 0, rowdata, users.Length + 1 + lens.Length, keys.Length);
            Array.Copy(data, 0, rowdata, users.Length + 1 + keys.Length + lens.Length, data.Length);
            return rowdata;
        }

        public static bool SetKVsdecode(byte[] rowsdata, out string[] key, out List<byte[]> datas)
        {
            key = new string[0];
            datas = new List<byte[]>();

            try
            {
                bool bb = Userpwddecode(rowsdata, out byte[] data);
                int len = data[0];
                byte[] lenb = new byte[len];
                Array.Copy(data, 1, lenb, 0, len);
                int lens = ConvertToInt(lenb);
                lenb = new byte[lens];
                Array.Copy(data, 1 + len, lenb, 0, lens);
                key = Dencdingdata(lenb);
                byte[] temp = new byte[data.Length - (1 + len + lens)];
                Array.Copy(data, 1 + len + lens, temp, 0, temp.Length);
                datas = Dencdingdatalist(temp);
                return bb;
            }
            catch
            { return false; }
        }

        public static byte[] EncodingsetKV(string key, byte[] data, int timeoutminute)
        {
            byte[] users = Userpwdencoding();
            byte[] keys = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] rowdata = new byte[users.Length + 1 + keys.Length + data.Length+4];
            Array.Copy(users, 0, rowdata, 0, users.Length);
            rowdata[users.Length] = (byte)keys.Length;
            Array.Copy(keys, 0, rowdata, users.Length + 1, keys.Length);
            Array.Copy(data, 0, rowdata, users.Length + 1 + keys.Length, data.Length);
            byte[] time = intToByteArray(timeoutminute);
            Array.Copy(time, 0, rowdata, users.Length + 1 + keys.Length+ data.Length, 4);
            return rowdata;
        }
         
        public static int byteArrayToInt(byte[] b)
        {
            return b[3] & 0xFF |
                        (b[2] & 0xFF) << 8 |
                        (b[1] & 0xFF) << 16 |
                        (b[0] & 0xFF) << 24;
        }
        public static byte[] intToByteArray(int a)
        {
                    return new byte[] {
                (byte) ((a >> 24) & 0xFF),
                (byte) ((a >> 16) & 0xFF),
                (byte) ((a >> 8) & 0xFF),
                (byte) (a & 0xFF)
            };
        }
        public static List<byte[]> Dencdingdatalist(byte[] datas)
        {
            List<byte[]> list = new List<byte[]>();
            while (true)
            {
                int len = datas[0];
                byte[] lenb = new byte[len];
                Array.Copy(datas, 1, lenb, 0, len);
                int lens = ConvertToInt(lenb);
                lenb = new byte[lens];
                Array.Copy(datas, 1 + len, lenb, 0, lens);
                list.Add(lenb);

                if (datas.Length == 1 + len + lens)
                    break;

                byte[] temp = new byte[datas.Length - (1 + len + lens)];
                Array.Copy(datas, (1 + len + lens), temp, 0, temp.Length);
                datas = temp;
            }
            return list;
        }

        public static byte[] Encodingdatalist(List<byte[]> datas)
        {
            List<byte> list = new List<byte>();
            foreach (byte[] str in datas)
            {
                byte[] tem = str;
                byte[] lens = ConvertToByteList(tem.Length);
                list.Add((byte)lens.Length);
                list.AddRange(lens);
                list.AddRange(tem);
            }
            return list.ToArray();
        }

        public static bool GetKVdecode(byte[] rowsdata, out string key)
        {
            key = "";
            try
            {
                bool bb = Userpwddecode(rowsdata, out byte[] data);
                byte len = data[0];
                byte[] keyb = new byte[len];
                Array.Copy(data, 1, keyb, 0, keyb.Length);
                key = System.Text.Encoding.UTF8.GetString(keyb);
                return bb;
            }
            catch
            {
                return false;
            }
        }

        static bool Userpwddecode(byte[] rowsdata, out byte[] data)
        {
            byte len = rowsdata[0];
            byte[] id = new byte[len];
            Array.Copy(rowsdata, 1, id, 0, id.Length);
            string user = System.Text.Encoding.UTF8.GetString(id);
            byte len2 = rowsdata[len + 1];
            byte[] pwdd = new byte[len2];
            Array.Copy(rowsdata, len + 2, pwdd, 0, pwdd.Length);
            string userpwd = System.Text.Encoding.UTF8.GetString(pwdd);
            data = new byte[rowsdata.Length - (len + 2 + len2)];
            Array.Copy(rowsdata, len + 2 + len2, data, 0, data.Length);
            if (user == userid && userpwd == pwd)
                return true;
            return false;
        }
    }
}
