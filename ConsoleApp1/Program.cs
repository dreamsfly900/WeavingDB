using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeavingDB;
using WeavingDBLogical;

namespace ConsoleApp1
{

    class Program
    {

      
    
        
        public class user
        {
            public int id { get; set; }
            public string name { get; set; }
            public byte aa { get; set; }
            public byte [] aas { get; set; }
           // public byte[,] aass;
            public DateTime dt { get; set; }
            public bool bb  { get; set; }
            
        }
       
        static T BytesToT<T>(byte[] bytes)
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

        static byte[] TToBytes<T>(T obj)
        {
            var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            using (var ms = new System.IO.MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        static unsafe IntPtr tobytes(byte[] write)
        {
            IntPtr write_data = Marshal.AllocHGlobal(write.Length);
            Marshal.Copy(write, 0, write_data, write.Length);
            // Marshal.FreeHGlobal(write_data);
            return write_data;
        }
        static unsafe byte[] tobyte(byte* write_data,int len)
        {
            IntPtr ip = (IntPtr)write_data;
            byte[] write = new byte[len];
            Marshal.Copy((IntPtr)write_data, write, 0, write.Length);
            return write;
        }
        static  DBmanage dbm = new DBmanage();
        
        static unsafe void Main(string[] args)
        {
            //char* p2 = (char*)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi("asdfasdf").ToPointer();
          
            byte [] bbtemp=  DataEncoding.encodingdata("","2312312","");
          string  [] strbb=  DataEncoding.dencdingdata(bbtemp);
            String strsss = Newtonsoft.Json.JsonConvert.SerializeObject("asdfsadf");
       
            byte[] p = GZIP.Compress(TToBytes<String>("2141234"));

          
            binaryvoid byv = new binaryvoid();
            byv.data =( tobytes(p));
            byv.len = p.Length;
            IntPtr sp = Marshal.AllocHGlobal(Marshal.SizeOf(byv));
            Marshal.StructureToPtr(byv, sp, false);
            binaryvoid byv2 = new binaryvoid();
            Marshal.PtrToStructure(sp, byv2);
            byte[] abc = tobyte((byte*)byv2.data, byv2.len);
           String sstr= BytesToT<String>( GZIP.Decompress(abc));
                 

                 DBLogical dblo = new DBLogical();
            int i = 0;
            List<listDmode> listu = new List<listDmode>();
            liattable ltable = new liattable();
            ltable.datas = listu;
            
            string str = "";
            JObject objbb;
             
            
             
            //objbb = JObject.Parse(str);
            int count = 10000;
            user u = new user() { id = i++, name = "111" + i };
            str = Newtonsoft.Json.JsonConvert.SerializeObject(u);

            objbb = JObject.Parse(str);
            listu.Add(dblo.insertintoJson(objbb, ref ltable.datahead));
            while (i < count)
            {
                 u = new user() { id = i++, name = ""+ i, dt = DateTime.Now.AddSeconds(new Random().Next(0,100)), aa=123};
                 str = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                
                objbb = JObject.Parse(str);
                //dbm.set(i.ToString(), new byte[0]);
                
                lock (listu)
                {
                    listu.Add(dblo.insertintoJson(objbb, ref ltable.datahead));

                }
                str = null;
                objbb = null;

            }
           
          //object obj = dbm.get("111");
             
             
            Console.WriteLine("全部数据：");
            Console.WriteLine(count+"条");
       
            while (true)
            {
                try
                {
                    dblo = new DBLogical();
                    Console.Clear();
                    string ss = "name=='aa' ";
                    Console.WriteLine("请输入查询条件");
                    ss = Console.ReadLine();
                    // var fields = obj.GetType().GetProperties();
                    var news = new { name = "特大喜讯" };
                    
                    dblo.updatedata(listu, ss, ltable.datahead, JObject.FromObject(news));
                         DateTime dt=DateTime.Now,dt2=DateTime.Now;
                    if (ss != "")
                    {


                        // listu = null;
                      
                        dt = DateTime.Now;
                        void*[][] objsall = dblo.selecttiem(listu, ss, ltable.datahead); 
                            dt2 = DateTime.Now;
                        // List<long> objsall = new List<long>();
                        if (objsall != null || objsall.Length>0)
                        {
                            Console.WriteLine("请输入排序：正序0，倒叙1");
                            byte order = Convert.ToByte(Console.ReadLine());
                            Console.WriteLine("请输入页数：");
                            int page = Convert.ToInt32( Console.ReadLine());
                            Console.WriteLine("请输入每页数量：");
                            int viewlen =Convert.ToInt32( Console.ReadLine());
                            Console.WriteLine("请输入排序列：");
                            string coll = (Console.ReadLine());
                            Hashtable[] objbb2 = dblo.viewdata(objsall, order, coll, page, viewlen, ltable.datahead);
                            Console.WriteLine("耗时：" + (dt2 - dt).TotalMilliseconds + "毫秒--查询后的数据：");
                             string str2 = Newtonsoft.Json.JsonConvert.SerializeObject(objbb2);

                            //  List<user> liss= Newtonsoft.Json.JsonConvert.DeserializeObject<List<user>>(str2);
                            Console.WriteLine("索引:" + str2);
                            //str2 = "";
                            objbb2 = null;
                        }
                        objsall = null;
                       
                        GC.Collect();
                    }
                   // dblo.deletedata(listu, ss, ltable.datahead);

                   
                     Console.ReadLine();
                }
                catch (Exception e){
                    Console.WriteLine(e.Message);
                }
            }

        }


 
    }
}
