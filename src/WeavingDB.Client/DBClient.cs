using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace WeavingDB.Client
{
    public class DBClient
    {
        readonly string IP = "";
        readonly int port = 0;
        readonly Clientcontrol ccon;

        public DBClient(string ip, int _port, string userid, string pwd)
        {
            IP = ip;
            port = _port;
            DataEncoding.userid = userid;
            DataEncoding.pwd = pwd;
            ccon = new Clientcontrol(IP, port);
        }

        public bool Open()
        {
            return ccon.Open();
        }

        public bool Createtable(string tablename)
        {
            byte[] rowdata = DataEncoding.EncodinggetKV(tablename);
            return Convert.ToBoolean(ccon.Send(0x04, rowdata)[0]);
        }

        public bool Removetable(string tablename)
        {
            byte[] rowdata = DataEncoding.EncodinggetKV(tablename);
            return Convert.ToBoolean(ccon.Send(0x05, rowdata)[0]);
        }

        public bool Inserttable<T>(string tablename, params T[] t)
        {
            string str;
            if (t.Length == 1)
                str = Newtonsoft.Json.JsonConvert.SerializeObject(t[0]);
            else
                str = Newtonsoft.Json.JsonConvert.SerializeObject(t);
            byte[] rowdata = DataEncoding.EncodingsetKV(tablename, Encoding.UTF8.GetBytes(str));
            if (t.Length > 1)
            {
                return Convert.ToBoolean(ccon.Send(0x07, rowdata)[0]);
            }
            else
            {
                return Convert.ToBoolean(ccon.Send(0x06, rowdata)[0]);
            }
        }

        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public T Selecttable<T>(string tablename)
        {
            return Selecttable<T>(tablename, "", 0, "", 0, 0, out _);
        }

        /// <summary>
        /// 有条件的查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public T Selecttable<T>(string tablename, string where)
        {
            return Selecttable<T>(tablename, where, 0, "", 0, 0, out _);
        }

        /// <summary>
        /// 查询数据并排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="order"></param>
        /// <param name="coll"></param>
        /// <returns></returns>
        public T Selecttable<T>(string tablename, byte order, string coll)
        {
            return Selecttable<T>(tablename, "", order, coll, 0, 0, out _);
        }

        /// <summary>
        /// 查询数据并排序与分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="where"></param>
        /// <param name="order"></param>
        /// <param name="coll"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public T Selecttable<T>(string tablename, string where, byte order, string coll, int pageindex, int pagesize, out int count)
        {
            count = 0;

            byte[] wherdata = DataEncoding.Encodingdata(where, order.ToString(), coll, pageindex.ToString(), pagesize.ToString());
            byte[] rowdata = DataEncoding.EncodingsetKV(tablename, wherdata);
            byte[] alldata = ccon.Send(0x08, rowdata);
            try
            {
                string[] ssr = DataEncoding.Dencdingdata(alldata);
                count = Convert.ToInt32(ssr[0]);
                string temp = ssr[1];
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(temp);
            }
            catch
            {
                return default(T);
            }
        }

        public bool Deletetable(string tablename, string where)
        {
            try
            {
                byte[] wherdata = DataEncoding.Encodingdata(where);
                byte[] rowdata = DataEncoding.EncodingsetKV(tablename, wherdata);
                byte[] alldata = ccon.Send(0x09, rowdata);
                try
                {
                    return Convert.ToBoolean(alldata[0]);
                }
                catch
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public bool Updatetable(string tablename, string where, dynamic obj)
        {
            string data = Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            byte[] wherdata = DataEncoding.Encodingdata(where, data);
            byte[] rowdata = DataEncoding.EncodingsetKV(tablename, wherdata);

            byte[] alldata = ccon.Send(0x10, rowdata);
            try
            {
                return Convert.ToBoolean(alldata[0]);
            }
            catch
            {
                return false;
            }
        }

        public bool SetAll<T>(Hashtable ht)
        {
            List<byte[]> list = new List<byte[]>();
            List<string> keys = new List<string>();
            foreach (var key in ht.Keys)
            {
                list.Add(DataEncoding.TToBytes<T>((T)ht[key]));
                keys.Add((string)key);
            }
            byte[] data = DataEncoding.Encodingdatalist(list);
            byte[] rowdata = DataEncoding.EncodingsetKVs(keys.ToArray(), data);
            return Convert.ToBoolean(ccon.Send(0x12, rowdata)[0]);
        }

        public bool Set<T>(string key, T t)
        {
            byte[] rowdata = DataEncoding.EncodingsetKV(key, DataEncoding.TToBytes<T>(t));
            return Convert.ToBoolean(ccon.Send(0x01, rowdata)[0]);
        }

        public bool RemoveKV(string key)
        {
            byte[] rowdata = DataEncoding.EncodinggetKV(key);
            return Convert.ToBoolean(ccon.Send(0x03, rowdata)[0]);
        }

        public T Get<T>(string key)
        {
            byte[] rowdata = DataEncoding.EncodinggetKV(key);
            byte[] data = ccon.Send(0x02, rowdata);
            if (data != null)
                return DataEncoding.BytesToT<T>(data);
            return default(T);
        }

        public string[] GetKey(string key)
        {
            byte[] rowdata = DataEncoding.EncodinggetKV(key);
            byte[] data = ccon.Send(0x11, rowdata);
            if (data != null)
                return DataEncoding.Dencdingdata(data);
            return null;
        }

        public void Close()
        {
            ccon.Close();
        }
    }
}
