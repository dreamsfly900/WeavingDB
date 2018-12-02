using System;
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
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(ssr[1]);
            }
            catch
            {
                return default(T);
            }
        }
        public bool Set<T>(string key, T t)
        {
            byte[] rowdata = DataEncoding.encodingsetKV(key, TToBytes<T>(t));
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
                    return BytesToT<T>(data);
                return default(T);
            
        }
        public void close()
        {
           
            ccon.close();
        }
        private T BytesToT<T>(byte[] bytes)
        {
            using (var ms = new System.IO.MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length);
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                ms.Position = 0;
                var x = bf.Deserialize(ms);
                return (T)x;
            }
        }

        private byte[] TToBytes<T>(T obj)
        {
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var ms = new System.IO.MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
 
    }

}
