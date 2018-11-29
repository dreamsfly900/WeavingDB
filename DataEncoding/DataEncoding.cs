using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeavingDB
{
    public class DataEncoding
    {
        public static string userid;
        public static string pwd;
        public static byte[] encodingsetKV(string key, byte[] data)
        {

            byte[] users = userpwdencoding();
            byte[] keys = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] rowdata = new byte[users.Length + 1 + keys.Length + data.Length];
            Array.Copy(users, 0, rowdata, 0, users.Length);
            rowdata[users.Length] = (byte)keys.Length;
            Array.Copy(keys, 0, rowdata, users.Length + 1, keys.Length);
            Array.Copy(data, 0, rowdata, users.Length + 1 + keys.Length, data.Length);
            return rowdata;
        }
        public static byte[] encodinggetKV(string key)
        {

            byte[] users = userpwdencoding();
            byte[] keys = System.Text.Encoding.UTF8.GetBytes(key);
            byte[] rowdata = new byte[users.Length + 1 + keys.Length];
            Array.Copy(users, 0, rowdata, 0, users.Length);
            rowdata[users.Length] = (byte)keys.Length;
            Array.Copy(keys, 0, rowdata, users.Length + 1, keys.Length);
            // Array.Copy(data, 0, rowdata, users.Length + 1 + keys.Length, data.Length);
            return rowdata;
        }
        static byte[] userpwdencoding()
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
        public static bool setKVdecode(byte[] rowsdata, out string key, out byte[] data)
        {
            key = "";
            data = new byte[0];
            try
            {
                bool bb = userpwddecode(rowsdata, out data);
                byte len = data[0];
                byte[] keyb = new byte[len];
                Array.Copy(data, 1, keyb, 0, keyb.Length);
                key = System.Text.Encoding.UTF8.GetString(keyb);
                byte[] data2 = new byte[data.Length - (len + 1)];
                Array.Copy(data, len + 1, data2, 0, data2.Length);
                data = data2;
                return bb;
            }
            catch { return false; }
        }
        public static bool getKVdecode(byte[] rowsdata, out string key)
        {
            key = "";
            byte[] data = new byte[0];
            try
            {
                bool bb = userpwddecode(rowsdata, out data);
                byte len = data[0];
                byte[] keyb = new byte[len];
                Array.Copy(data, 1, keyb, 0, keyb.Length);
                key = System.Text.Encoding.UTF8.GetString(keyb);
                //byte[] data2 = new byte[data.Length - (len + 1)];
                //Array.Copy(data, len + 1, data2, 0, data2.Length);
                //data = data2;
                return bb;
            }
            catch { return false; }
        }

        static bool userpwddecode(byte[] rowsdata, out byte[] data)
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
