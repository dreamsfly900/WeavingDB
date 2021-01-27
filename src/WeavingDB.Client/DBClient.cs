using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using WeavingDB.Logical;

namespace WeavingDB.Client
{
    public class DBClient
    {
        readonly string IP = "";
        readonly int port = 0;
        readonly Clientcontrol ccon;
        public int timeout = 60;
        public DBClient(string ip, int _port, string userid, string pwd)
        {
            IP = ip;
            port = _port;
            DataEncoding.userid = userid;
            DataEncoding.pwd = pwd;
            ccon = new Clientcontrol(IP, port);
            ccon.timeout = timeout;
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
        /// <param name="type"></param>
        /// <returns></returns>
        public T Selecttable<T>(string tablename, int type = 0)
        {
            return Selecttable<T>(tablename, "", 0, "", 0, 0, out _,"", type);
        }
        /// <summary>
        /// 有条件的查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="where"></param>
        ///  <param name="type">0为json传输，1为二进制压缩传输</param>
        /// <returns></returns>
        public T Selecttable<T>(string tablename, string where, int type = 0)
        {
            return Selecttable<T>(tablename, where, 0, "", 0, 0, out _,"", type);
        }
        /// <summary>
        /// 查询通过WHERE 条件，并且规定输出列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="where"></param>
        /// <param name="viewcol"></param>
        /// <returns></returns>
        public T Selecttable<T>(string tablename, string where,string viewcol, int type = 0)
        {
            return Selecttable<T>(tablename, where, 0, "", 0, 0, out _, viewcol, type);
        }
        /// <summary>
        /// 查询数据并排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="where"></param>
        /// <param name="viewcol"></param>
        /// <param name="order"></param>
        /// <param name="coll"></param>
        /// <returns></returns>
        public T Selecttable<T>(string tablename, string where, string viewcol, byte order, string coll, int type = 0)
        {
            return Selecttable<T>(tablename, where, order, coll, 0, 0, out _, viewcol, type);
        }
       

        /// <summary>
        /// 查询数据并排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="order"></param>
        /// <param name="coll"></param>
        /// <returns></returns>
        public T Selecttable<T>(string tablename, byte order, string coll, int type = 0)
        {
            return Selecttable<T>(tablename, "", order, coll, 0, 0, out _,"", type);
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
        public T Selecttable<T>(string tablename, string where, byte order, string coll, int pageindex, int pagesize, out int count,string viewcol="",int type=0)
        {
           
            count = 0;

            byte[] wherdata = DataEncoding.Encodingdata(where, order.ToString(), coll, pageindex.ToString(), pagesize.ToString(), viewcol, type.ToString());
            byte[] rowdata = DataEncoding.EncodingsetKV(tablename, wherdata);
            byte[] alldata = ccon.Send(0x08, rowdata);
            try
            {
                if (type == 1)
                {
                    DateTime dt = DateTime.Now;
                    if(alldata.Length<=10)
                        return default(T);
                    JArray alldatasJ = BinaryData.DecodeBinaryDataJson(alldata);
                   
                    T t=  alldatasJ.ToObject<T>();
                    DateTime dt2 = DateTime.Now;
                    
                    Console.WriteLine("BinaryData" + (dt2 - dt).TotalMilliseconds);
                    return t;
                }
                else
                {
                    DateTime dt = DateTime.Now;
                    string[] ssr = DataEncoding.Dencdingdata(alldata);
                    count = Convert.ToInt32(ssr[0]);
                    string temp = ssr[1];
                    T t= Newtonsoft.Json.JsonConvert.DeserializeObject<T>(temp);
                    DateTime dt2 = DateTime.Now;
                 //   Console.WriteLine("JSON" + (dt2 - dt).TotalMilliseconds);
                    return t;
                }
            }
            catch
            {
                return default(T);
            }
        }
        public int SelectCount(string tablename, string where)
        {
           

            byte[] wherdata = DataEncoding.Encodingdata(where);
            byte[] rowdata = DataEncoding.EncodingsetKV(tablename, wherdata);
            byte[] alldata = ccon.Send(0x14, rowdata);
            try
            {
                string[] ssr = DataEncoding.Dencdingdata(alldata);
               int  count = Convert.ToInt32(ssr[0]);
                 
                return count;
            }
            catch
            {
                return 0;
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
        public bool Set<T>(string key, T t,int timeoutminute)
        {
            byte[] rowdata = DataEncoding.EncodingsetKV(key, DataEncoding.TToBytes<T>(t), timeoutminute);
            return Convert.ToBoolean(ccon.Send(0x13, rowdata)[0]);
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
