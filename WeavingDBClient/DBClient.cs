using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weave.TCPClient;
using WeavingDB;
namespace WeavingDBClient
{
    public class DBClient
    {
        string IP = ""; int port = 0; 
        Clientcontrol ccon;
        public DBClient(string ip, int _port, string userid, string pwd)
        {
             
            IP = ip;
            port = _port;
            WeavingDB.DataEncoding.userid = userid;
            DataEncoding.pwd = pwd;
            ccon = new Clientcontrol(IP,port);
        }
        public bool open()
        {
           return ccon.open();
        }
        public bool Createtable(string tablename)
        {
            byte[] rowdata = DataEncoding.encodinggetKV(tablename);
            return Convert.ToBoolean(ccon.Send(0x04, rowdata)[0]);
        }
        public bool Removetable(string tablename)
        {
            byte[] rowdata = DataEncoding.encodinggetKV(tablename);
            return Convert.ToBoolean(ccon.Send(0x05, rowdata)[0]);
        }
        public bool inserttable<T>(String tablename,params T [] t)
        {
            //Type tp = t.GetType();
            String str = "";
            if (t.Length == 1)
                 str=  Newtonsoft.Json.JsonConvert.SerializeObject(t[0]);
            else
                str = Newtonsoft.Json.JsonConvert.SerializeObject(t);
            byte[] rowdata = DataEncoding.encodingsetKV(tablename, System.Text.Encoding.UTF8.GetBytes(str));
            if (t.Length > 1)
            {
                return Convert.ToBoolean(ccon.Send(0x07, rowdata)[0]);
            }
            else
            {
                return Convert.ToBoolean(ccon.Send(0x06, rowdata)[0]);
            }
            //  return Convert.ToBoolean(ccon.Send(0x07, rowdata)[0]);
            // else
            //      return Convert.ToBoolean(ccon.Send(0x06, rowdata)[0]);
           
        }
        /// <summary>
        /// 查询全部数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <returns></returns>
        public T selecttable<T>(string tablename)
        {
            int count = 0;
            return selecttable<T>(tablename, "", 0, "", 0, 0, out count);
        }
        /// <summary>
        /// 有条件的查询数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="where"></param>
        /// <returns></returns>
        public T selecttable<T>(string tablename, string where)
        {
            int count = 0;
            return selecttable<T>(tablename, where, 0, "", 0, 0,out count);
        }
        /// <summary>
        /// 查询数据并排序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tablename"></param>
        /// <param name="order"></param>
        /// <param name="coll"></param>
        /// <returns></returns>
        public T selecttable<T>(string tablename,  byte order, String coll)
        {
            int count = 0;
            return selecttable<T>(tablename, "", order, coll, 0, 0, out count);
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
        public T selecttable<T>(string tablename,string where, byte order,String coll,int pageindex,int pagesize,out int count)
        {
            
            count = 0;

            byte[] wherdata = DataEncoding.encodingdata(where, order.ToString(), coll, pageindex.ToString(), pagesize.ToString());
            byte[] rowdata = DataEncoding.encodingsetKV(tablename, wherdata);
            byte[] alldata = ccon.Send(0x08, rowdata);
            try
            {

                
                string[] ssr = DataEncoding.dencdingdata(alldata);
                count = Convert.ToInt32(ssr[0]);
                string temp = ssr[1];//.Replace("\r\n", ""); 
                   // [{"list":"[{   \"name\": \"\"  }]","name":"","id":0}]
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(temp);
            }
            catch(Exception ee)
            {
                return default(T);
            }
        }

        public bool deletetable(string tablename, string where)
        {
            try
            {
                byte[] wherdata = DataEncoding.encodingdata(where);
                byte[] rowdata = DataEncoding.encodingsetKV(tablename, wherdata);
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
        public bool updatetable(string tablename, string where,dynamic obj)
        {
            string data=    Newtonsoft.Json.JsonConvert.SerializeObject(obj);
            byte[] wherdata = DataEncoding.encodingdata(where, data);
            byte[] rowdata = DataEncoding.encodingsetKV(tablename, wherdata);

            byte[] alldata = ccon.Send(0x10, rowdata);
            try
            {



                return Convert.ToBoolean(alldata[0]);
            }
            catch(Exception ee)
            {
              //  throw ee;
                return false;
            }
          
        }
        public bool SetAll<T>(Hashtable ht)
        {

            //DataEncoding.encodinggetKV("2018092100000");
            List<byte[]> list = new List<byte[]>();
            List<string> keys = new List<string>();
            foreach (var key in ht.Keys)
            {
              
                list.Add(DataEncoding.TToBytes<T>((T)ht[key]));
                keys.Add((String)key);



            }
            byte[] data = DataEncoding.encodingdatalist(list);
            byte[] rowdata = DataEncoding.encodingsetKVs(keys.ToArray() , data);
            return Convert.ToBoolean(ccon.Send(0x12, rowdata)[0]);
        }
        public bool Set<T>(string key, T t)
        {
            byte[] rowdata = DataEncoding.encodingsetKV(key, DataEncoding.TToBytes<T>(t));
             //DataEncoding.encodinggetKV("2018092100000");
          return  Convert.ToBoolean( ccon.Send(0x01, rowdata)[0]);
        }
        public bool RemoveKV(string key)
        {
            byte[] rowdata = DataEncoding.encodinggetKV(key); 
            return Convert.ToBoolean(ccon.Send(0x03, rowdata)[0]);
        }
        public T  Get<T>(string key)
        {
            
                byte[] rowdata = DataEncoding.encodinggetKV(key);
                //DataEncoding.encodinggetKV("2018092100000");
                //GZIP.Compress(rowdata);
                //return BytesToT<T>(TToBytes(key));
                byte[] data = ccon.Send(0x02, rowdata);
                if (data != null)
                    return DataEncoding.BytesToT<T>(data);
                return default(T);
            
        }
        public String[] GetKey(string key)
        {

            byte[] rowdata = DataEncoding.encodinggetKV(key);
            //DataEncoding.encodinggetKV("2018092100000");
            //GZIP.Compress(rowdata);
            //return BytesToT<T>(TToBytes(key));
            byte[] data = ccon.Send(0x11, rowdata);
            if (data != null)
                return DataEncoding.dencdingdata(data);
            return null;

        }
        public void close()
        {
           
            ccon.close();
        }
      
 
    }

}
