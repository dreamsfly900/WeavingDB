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
        public bool Set<T>(string key, T t)
        {
            byte[] rowdata = DataEncoding.encodingsetKV(key, TToBytes<T>(t));
             //DataEncoding.encodinggetKV("2018092100000");
          return  Convert.ToBoolean( ccon.Send(0x01, rowdata)[0]);
        }
        public T  Get<T>(string key)
        {
            byte[] rowdata = DataEncoding.encodinggetKV(key);
            //DataEncoding.encodinggetKV("2018092100000");
            return BytesToT<T>(ccon.Send(0x02, rowdata));
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
