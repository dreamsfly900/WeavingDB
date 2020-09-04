using Newtonsoft.Json.Linq;
using SQLDBlogic.logic;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using WeavingDB.Logical;

namespace ConsoleApp1
{

    class Program
    {

      
    
        
      
      //  static  DBmanage dbm = new DBmanage();
      static  IntPtr p3 = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi("asdfasdfasdfasdfasdfassd所谓发射点发射点发啊撒士大夫阿瑟东dfasdfasdfewqrqwer");

      static  void go(object obj)
        {
            if (p3 != IntPtr.Zero)
            {
                System.Threading.Thread.Sleep(100);
                Marshal.FreeHGlobal(p3);
                p3 = IntPtr.Zero;
            }
        }
        static bool Stringtonosymbol(String _sqlsst, string rstr)
        {
            if (_sqlsst == "''")
                return false;
            Regex r = new Regex(rstr); // 定义一个Regex对象实例
            var m = r.Match(_sqlsst);


            if (m.Success)
            {
                return true;
            }
            return false;
        }
        static unsafe void Main(string[] args)
        {
            string stssr = System.Threading.Thread.GetDomain().BaseDirectory;
            byte[] shi = System.BitConverter.GetBytes((long)1);
            long sh = (long)System.BitConverter.ToUInt64(shi, 0);

            //byte[] data = DataEncoding.encodingdatalist(list);
            //DataEncoding.userid = "admin";
            //DataEncoding.pwd = "123";
            //data = DataEncoding.encodingsetKVs(new string[2] {"111", "1222"}, data);
            //string[] kets;
            //list = new List<byte[]>();
            //DataEncoding.setKVsdecode(data, out kets, out list);

            Stringtonosymbol("123", "^123(.*)$");
            ////Queue st = new Queue();
            ////st.Enqueue(11);
            ////while (true)
            ////{
            ////    if (st.Count > 0)
            ////    {
            ////        Object objo = st.Dequeue();
            ////    }
            ////}
            //char* p2 = (char*)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi("asdfasdf").ToPointer();

            //try
            //{
            //    Marshal.FreeHGlobal(p3);
            //    //Marshal.FreeHGlobal(p3);
            //}
            //catch { }
            //int iii = 0;
            //while (iii < 100)
            //{
            //    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(go), null);
            //    iii++;
            //}
            //int pbb = 2345;
            //int nSizeOfPerson = Marshal.SizeOf(pbb);
            //IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
            //Marshal.StructureToPtr(pbb, intPtr, true);
            //void* hhg = intPtr.ToPointer();
            //intPtr = IntPtr.Zero;
            //Marshal.FreeHGlobal((IntPtr)hhg);

            //Marshal.FreeHGlobal((IntPtr)hhg);
            ////   Marshal.FreeHGlobal(p3);
            //System.GC.Collect();
            // Marshal.DestroyStructure(p3, typeof(String));
            // string ss= Marshal.PtrToStringAnsi(p3);
            //  byte [] bbtemp=  DataEncoding.encodingdata("","2312312","");
            //string  [] strbb=  DataEncoding.dencdingdata(bbtemp);
            //  String strsss = Newtonsoft.Json.JsonConvert.SerializeObject("asdfsadf");

            //  byte[] p = GZIP.Compress(TToBytes<String>("2141234"));


            //  binaryvoid byv = new binaryvoid();
            //byv.data =( tobytes(p));
            //  byv.len = p.Length;
            //  IntPtr sp = Marshal.AllocHGlobal(Marshal.SizeOf(byv));
            //  Marshal.StructureToPtr(byv, sp, false);
            //  binaryvoid byv2 = new binaryvoid();
            //  Marshal.PtrToStructure(sp, byv2);
            //  byte[] abc = tobyte((byte*)byv2.data, byv2.len);
            // String sstr= BytesToT<String>( GZIP.Decompress(abc));
           

            String s = "410041114";
            String p = "%00411%";
        
          int a=  Sunday.strSunday(s, p,0);


            DBLogic dblo = new DBLogic();
            int i = 0;
            List<ListDmode> listu = new List<ListDmode>();
            Liattable ltable = new Liattable();
            ltable.datas = listu;
            
            string str = "";
            JObject objbb;
             
            
             
            //objbb = JObject.Parse(str);
            int count = 100000;
            //user u = new user() { id = i++, name = "2345ds" + i ,aas =new byte[10,10]};
            //str = Newtonsoft.Json.JsonConvert.SerializeObject(u);

            //objbb = JObject.Parse(str);
            //listu.Add(dblo.insertintoJson(objbb, ref ltable.datahead));

            System.IO.StreamReader sr = new System.IO.StreamReader("ab.json");
             string ssr=  sr.ReadToEnd();
            sr.Close();
            BPTree tree = new BPTree(3);
           // BTree<String, IntPtr> bTree = new BTree<string, IntPtr>(2);
           
            JArray objbbs = JArray.Parse(ssr);
            int iss = 0;
            long p2 = Convert.ToDateTime("2020-08-13T11:20:47+08:00").ToFileTime();
            DateTime dt = DateTime.Now, dt2 = DateTime.Now;
            foreach (JObject jo in objbbs)
            {
                //dbm.set(i.ToString(), new byte[0]);

                
                    var tee = dblo.insertintoJson(jo, ref ltable.datahead);
                    listu.Add(tee);
               //// long p1 = Convert.ToDateTime(jo["warningTime"].ToString()).ToFileTime();
               // if ((*(long*)tee.dtable2[0]) == p2)
               // { 
                
               // }
               ////  tree.insert(tree.root, tee.dtable2[0], tee, ltable.datahead[0].type);
               // dblo.insertintoIndex(jo, tee, ltable.datahead, ref ltable.tree);
                  //  bTree.Insert(jo["eventType"].ToString(), tee.dtable[0]);
                str = null;
                objbb = null;

            }
            byte[] datass = dblo.viewdatabyte(listu.ToArray(), ltable.datahead, "warningTime,eventType", 0, "", 1, 100);
            ListDmode[] alldatas = BinaryData.DecodeBinaryData(datass);
            JArray alldatasJ = BinaryData.DecodeBinaryDataJson(datass);
            dt2 = DateTime.Now;
            double haomiao = (dt2 - dt).TotalMilliseconds;

            dt = DateTime.Now;
            byte[] datas = BinaryData.EncodeBinaryData(ltable.datahead, listu.ToArray());
            dt2 = DateTime.Now;

            double haomiao3 = (dt2 - dt).TotalMilliseconds;


           

            dt = DateTime.Now;
            ListDmode[]  alldata= BinaryData.DecodeBinaryData(datas);
            dt2 = DateTime.Now;
            double haomiao2 = (dt2 - dt).TotalMilliseconds;

            dt = DateTime.Now;
            JObject[] joss = new JObject[alldata.Length];
            int uu = 0;
            foreach (ListDmode ld in alldata)
            {
                JObject jos = new JObject();
                for (int ss = 0; ss < ltable.datahead.Length; ss++)
                {
                    JProperty obj = utli.GetHashtable(ltable.datahead[ss].key, ltable.datahead[ss].type, ld.dtable2[ss], ld.LenInts[ss]);
                    jos.Add(obj);
                }
                joss[uu] = jos;
                uu++;
            }
            string str22 =JArray.FromObject(joss).ToString();
            dt2 = DateTime.Now;
            double haomiao4 = (dt2 - dt).TotalMilliseconds;

            dblo.createIndex(listu, ltable.datahead, "warningTime", ref ltable.tree);
           string path = Thread.GetDomain().BaseDirectory;
            BinaryFileData.WriteTableHead(path,"T_warning", ltable);
            Liattable templiattable= BinaryFileData.ReadTableHead(path,"T_warning");
             
            //object obj = dbm.get("111");
         //   tree.deleteKey(tree, "11B09");
            dt = DateTime.Now;
            //var data=  bTree.Search("11B06");
            // var bn= tree.search(tree.root, p2);
            IntPtr p3 = Marshal.StringToHGlobalAnsi("11B06");
            Node nd= tree.search(tree.root, &p2);
              dt2 = DateTime.Now;
            Console.WriteLine("耗时：" + (dt2 - dt).TotalMilliseconds + "毫秒--查询后的数据：" );//+ (bn!=null? bn.Count:0)
            //dt = DateTime.Now;
            //var bTree2 = bTree.Search("11B06");
            //dt2 = DateTime.Now;
            //Console.WriteLine("耗时：" + (dt2 - dt).TotalMilliseconds + "毫秒--查询后的数据：" + bTree2.Key);


           // if (bn!=null)
          //  tree.print(bn);
            Console.WriteLine("全部数据：");
            Console.WriteLine(objbbs .Count+ "条");
       
            while (true)
            {
                try
                {
                    dblo = new DBLogic();
                   // Console.Clear();
                   // string ss = "name=='aa' ";
                    Console.WriteLine("请输入查询条件");
                    string ss = Console.ReadLine();
                    // var fields = obj.GetType().GetProperties();
                    var news = new { name = "特大喜讯" };

                    // dblo.updatedata(listu, ss, ltable.datahead, JObject.FromObject(news));

                    // if (ss != "")
                    {

                        //  ss = "warningTime<='2020-08-14T11:20:47+08:00'";
                        // listu = null;
                        dt = DateTime.Now;

                        ListDmode[] objsall = dblo.selecttiem(listu, ss, ltable.datahead,ltable);
                        dt2 = DateTime.Now;
                        Console.WriteLine("耗时：" + (dt2 - dt).TotalMilliseconds + "毫秒--查询后的数据：" + objsall.Length);
                        // List<long> objsall = new List<long>();
                        if (objsall != null || objsall.Length > 0)
                        {
                            Console.WriteLine("请输入排序：正序0，倒叙1");
                            byte order = Convert.ToByte(Console.ReadLine());
                            Console.WriteLine("请输入页数：");
                            int page = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("请输入每页数量：");
                            int viewlen = Convert.ToInt32(Console.ReadLine());
                            Console.WriteLine("请输入排序列：");
                            string coll = (Console.ReadLine());
                            dt = DateTime.Now;
                            JObject[] objbb2 = dblo.viewdata(objsall,ltable.datahead, "warningTime,eventType", order, coll, page, viewlen);
                            dt2 = DateTime.Now;
                            Console.WriteLine("耗时：" + (dt2 - dt).TotalMilliseconds + "毫秒--查询后的数据：");
                            //string str2 = "[";
                            //foreach (JObject j in objbb2)
                            //{
                            //      str2 += j.ToString()+",";// Newtonsoft.Json.JsonConvert.SerializeObject(objbb2.ToString());
                            //}
                            //str2 += "]";
                            dt = DateTime.Now;
                            string str2 = Newtonsoft.Json.JsonConvert.SerializeObject(objbb2);
                            // string str2 = JArray.FromObject(objbb2).ToString();
                            dt2 = DateTime.Now;
                            Console.WriteLine("耗时：" + (dt2 - dt).TotalMilliseconds + "毫秒--对象转STRING时间：");
                            //  List<user> liss= Newtonsoft.Json.JsonConvert.DeserializeObject<List<user>>(str2);
                            // Console.WriteLine("索引:" + str2);
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
