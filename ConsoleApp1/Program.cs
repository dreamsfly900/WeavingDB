﻿using Newtonsoft.Json.Linq;
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
using WeavingDBLogical;

namespace ConsoleApp1
{

    class Program
    {

      
    
        
        public class user
        {
            public int id;
            public string name;
            public byte aa;
          //  public byte [] aas;
           // public byte[,] aass;
            public DateTime dt;
            public bool bb = true;
            
        }
        public class user2
        {
            public int idaa;
            public string id;
        }
      static  DBmanage dbm = new DBmanage();
        static unsafe void Main(string[] args)
        {
          

               DBLogical dblo = new DBLogical();
            int i = 0;
            List<listDmode> listu = new List<listDmode>();
            liattable ltable = new liattable();
            ltable.datas = listu;
            List<user> liseruser = new List<user>();
            string str = "";
            JObject objbb;
            user uu = new user() { id = i++, name = "" + i, dt = DateTime.Now.AddSeconds(new Random().Next(0, 100)), aa = 123 };
            liseruser.Add(uu);
            uu = new user() { id = i++, name = "" + i, dt = DateTime.Now.AddSeconds(new Random().Next(0, 100)), aa = 123 };
            liseruser.Add(uu);
          
            str = Newtonsoft.Json.JsonConvert.SerializeObject(liseruser);
            JArray ja = JArray.Parse(str);
            //objbb = JObject.Parse(str);
            int count = 1000000;

            while (i < count)
            {
                user u = new user() { id = i++, name = ""+ i, dt = DateTime.Now.AddSeconds(new Random().Next(0,100)), aa=123};
                 str = Newtonsoft.Json.JsonConvert.SerializeObject(u);
                
                objbb = JObject.Parse(str);
                dbm.set(i.ToString(), new byte[0]);
                
                lock (listu)
                {
                    listu.Add(dblo.insertintoJson(objbb, ref ltable.datahead));

                }
                str = null;
                objbb = null;

            }
           
          //object obj = dbm.get("111");
            user2 u2 = new user2() { idaa = i + 123 ,id="adf"};
            str = Newtonsoft.Json.JsonConvert.SerializeObject(u2);
             objbb = JObject.Parse(str);

            listu.Add(dblo.insertintoJson(objbb, ref ltable.datahead));


             
            Console.WriteLine("全部数据：");
            Console.WriteLine(count+"条");
         //   var oobj = listuu.Where<user>(c => c.id >= 12 && c.name == "" || c.name == "aa");
         
           
            //foreach (user uuu in oobj)
            //{

            //}
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
                            // string str2 = Newtonsoft.Json.JsonConvert.SerializeObject(objbb2);

                            //  List<user> liss= Newtonsoft.Json.JsonConvert.DeserializeObject<List<user>>(str2);
                            //Console.WriteLine("索引:" + str2);
                            //str2 = "";
                            objbb2 = null;
                        }
                        objsall = null;
                       
                        GC.Collect();
                    }
                  //  dblo.deletedata(listu, ss, ltable.datahead);

                   
                     Console.ReadLine();
                }
                catch (Exception e){
                    Console.WriteLine(e.Message);
                }
            }

        }


 
    }
}