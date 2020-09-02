using Newtonsoft.Json.Linq;
using SQLDBlogic.logic;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WeavingDB.Logical;

namespace SQLDBlogic
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        List<ListDmode> listu = new List<ListDmode>();
        Liattable ltable = new Liattable();

        private unsafe void Form1_Load(object sender, EventArgs e)
        {
            System.IO.StreamReader sr = new System.IO.StreamReader("ab.json");
            string ssr = sr.ReadToEnd();
            sr.Close();
            JArray objbbs = JArray.Parse(ssr);
           
        
            ltable.datas = listu;
           
            DBLogic dblo = new DBLogic();
            foreach (JObject jo in objbbs)
            {
                //dbm.set(i.ToString(), new byte[0]);


                var tee = dblo.insertintoJson(jo, ref ltable.datahead);
                dblo.insertintoIndex(jo, tee, ltable.datahead, ref ltable.tree);
                listu.Add(tee);
              
              //  break;
            }
            string str = "11B06";
          
            IntPtr p1 = Marshal.StringToHGlobalAnsi(str);
            


            long p2 = Convert.ToDateTime("2020-08-13T11:20:47+08:00").ToFileTime();

            var temp2 = dblo.SelectCount(listu, "warningTime>='2020-08-13T11:20:47+08:00' && warningTime<='2020-08-30T00:22:47+08:00' ", ltable.datahead, ltable);

            int count = dblo.updatedata(listu, " eventType = '11B06'",
                  ltable.datahead, JObject.FromObject(new { eventType = "特大喜讯" }), ltable);


            int count2 = dblo.deletedata(listu, "eventType=='特大喜讯'",
                 ltable.datahead, ltable);

            //int count3 = dblo.updatedata(listu, " eventType=='特大喜讯' ",
            //    ltable.datahead, JObject.FromObject(new { eventType = "11B06" }), ltable);

            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(gogo));

            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(gogo));
            IntPtr intp = IntPtr.Zero;
            int counts = ltable.tree["eventType"].freeintPtrs.Count;
            int i = 0;
            while (i<counts)
            {
                ltable.tree["eventType"].freeintPtrs.TryDequeue(out intp);
              bool gb= ltable.tree["eventType"].Contains(ltable.tree["eventType"].root,intp);
                if (!gb)
                    Marshal.FreeHGlobal(intp);
                else
                    ltable.tree["eventType"].freeintPtrs.Enqueue(intp);
                i++;
            }
            DateTime dt = DateTime.Now;

            var temp=   dblo.selecttiem(listu, "warningTime>='2020-08-13T11:20:47+08:00' ", ltable.datahead, ltable);
            DateTime dt2 = DateTime.Now;
            Console.WriteLine("耗时：" + (dt2 - dt).TotalMilliseconds + "毫秒--查询后的数据：");


            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(gogo));

            dt = DateTime.Now;
            var job=    dblo.viewdata(temp, ltable.datahead);
            dt2 = DateTime.Now;
            Console.WriteLine("耗时：" + (dt2 - dt).TotalMilliseconds + "毫秒--查询后的数据：");
            dt = DateTime.Now;
           
              //long p2 = Convert.ToDateTime("2020-08-14T11:20:47+08:00").ToFileTime();
            List<ListDmode> list= ltable.tree["warningTime"].searcheQualto(ltable.tree["warningTime"].root,&p2, ltable.datahead[0].type,0);
            List<ListDmode> list2 = ltable.tree["warningTime"].searcheQualto(ltable.tree["warningTime"].root, &p2, ltable.datahead[0].type, 1);
            List<ListDmode> list3 = ltable.tree["warningTime"].searcheQualto(ltable.tree["warningTime"].root, &p2, ltable.datahead[0].type, 3);
            List<ListDmode> list4 = ltable.tree["warningTime"].searcheQualto(ltable.tree["warningTime"].root, &p2, ltable.datahead[0].type, 4);
            List<ListDmode> list5 = ltable.tree["warningTime"].searcheQualto(ltable.tree["warningTime"].root, &p2, ltable.datahead[0].type, 2);
            List<ListDmode> list6 = ltable.tree["warningTime"].searcheQualto(ltable.tree["warningTime"].root, &p2, ltable.datahead[0].type, 6);
             dt2 = DateTime.Now;
            Console.WriteLine("耗时：" + (dt2 - dt).TotalMilliseconds + "毫秒--查询后的数据：");
        }

        private void gogo(object state)
        {
            DBLogic dblo = new DBLogic();
            int count2 = dblo.deletedata(listu, "eventType=='特大喜讯'",
           ltable.datahead, ltable);
        }
    }
}
