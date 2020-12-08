using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WeavingDB.Client;

namespace WeavingDB
{

    public partial class WeavingDB : Form
    {
        public WeavingDB()
        {
            InitializeComponent();
        }
        //[DllImport("kernel32",SetLastError = true)]
        //static extern IntPtr LocalFree(IntPtr mem);

 
    private  void WeavingDB_Load(object sender, EventArgs e)
        {
            //object obj= Newtonsoft.Json.JsonConvert.PopulateObject(builder.ToString());122.114.53.233
            //127.0.0.1
            //IntPtr p3 = System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi("asdfasdfasdfasdfasdfassd所asdfasdfasdfasdfasdfassd所谓发射点发射点发啊撒士大夫阿瑟东dfasdfasdfewqrqwerasdfasdfasdfasdfasdfassd所谓发射点发射点发啊撒士大夫阿瑟东dfasdfasdfewqrqwerasdfasdfasdfasdfasdfassd所谓发射点发射点发啊撒士大夫阿瑟东dfasdfasdfewqrqwerasdfasdfasdfasdfasdfassd所谓发射点发射点发啊撒士大夫阿瑟东dfasdfasdfewqrqwerasdfasdfasdfasdfasdfassd所谓发射点发射点发啊撒士大夫阿瑟东dfasdfasdfewqrqwerasdfasdfasdfasdfasdfassd所谓发射点发射点发啊撒士大夫阿瑟东dfasdfasdfewqrqwerasdfasdfasdfasdfasdfassd所谓发射点发射点发啊撒士大夫阿瑟东dfasdfasdfewqrqwerasdfasdfasdfasdfasdfassd所谓发射点发射点发啊撒士大夫阿瑟东dfasdfasdfewqrqwer谓发射点发射点发啊撒士大夫阿瑟东dfasdfasdfewqrqwer");
            //void* gg = p3.ToPointer();
             
            //LocalFree((IntPtr)p3.ToPointer());
       
           // LocalFree(p3);
        }
        void hhf(object obj)
        {
           
        }
       public class aaa {
            public string aaas = "";
            public byte[,] gg = new byte[10, 10];
        }
        public class network
        {
            public string name;
            public string ip;
            public bool status;
        }
        private void button1_Click(object sender, EventArgs e)
        { 
            DBClient dbc = new DBClient("127.0.0.1", 18989, "admin", "123123");
            double gggg = 0;

            //while (gggg < 10)
            //{
            //    dbc.Open();
            //    var OBJ = dbc.Get<CAllAreaInfo[]>("radar-Z9379-windstorm-06270248");
            //    dbc.Close();
            //    dbc.Open();
            //    var OBJ2 = dbc.Get<radardatamode>("radar-Z9379-VIL-06270248");
            //    dbc.Close();
            //    //dbc.Open();
            //    //var OBJ3 = dbc.Get<radardatamode>("radar-Z9379-WT-06270348");
            //    //dbc.Close();
            //    dbc.Open();
            //    var OBJ4 = dbc.Get<radardatamode>("radar-Z9379-SCR");
            //    dbc.Close();
            //    dbc.Open();
            //    var OBJ5 = dbc.Get<radardatamode>("radar-Z9379-QPF-06270248");
            //    dbc.Close();
            //    gggg++;
            //}
            //radar-Z9379-QPF-06270248
            dbc.Open();
           // var str22 = dbc.Get<network[]>("4104_equipmentmode-network");
           //  Hashtable ht = new Hashtable();
           // ht.Add("123123", "afasdfasdf");
           // ht.Add("12312311", "afasdfasdf");
           // ht.Add("1231231221", "afasaasdfasdf");
           // ht.Add("123123122199", "afasaasdfasdf");
           // bool bb = dbc.SetAll<string>(ht);

           // //string sss=  dbc.Get<String>("12312311");
           //var  str2 = dbc.Get<String>("afasdfasdf");
           // dbc.GetKey("?d");
            dbc.Set("asdasd", 111);
           // dbc.Set("aabbcc", 111222, 5);
           //// dbc.Set<String>("asdasd", "1");
            int i = 0;
           //string [] keys=   dbc.GetKey("as?asd");//通配符?一个匹配字符
           // keys = dbc.GetKey("as*");//通配符* 表示，多个模糊匹配
           // dbc.GetKey("as?a?d");
           // dbc.GetKey("?d");
            while (i < 10000)
            {
               // i++;
                DateTime dt = DateTime.Now;
                //dbc.Set("asdasd", 111);
                var str = dbc.Get<int>("asdasd");
                DateTime dt2 = DateTime.Now;
                double gg = (dt2 - dt).TotalMilliseconds;
                gggg += gg;
                listBox1.Items.Add("第" + (i++) + "次查询耗时：" + gg + "毫秒");
               // System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(ff));
            }



            label1.Text = "1W次总耗时：" + gggg;
            dbc.Close();
            //每次插入一条数据
           
        }
        
        void ff(object obj)
        {
            DBClient dbc2 = new DBClient("127.0.0.1", 18989, "admin", "123123");
            dbc2.Open();
            dbc2.RemoveKV("asdasd");
            dbc2.Close();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //*************
            DBClient dbc = new DBClient("127.0.0.1", 18989, "admin", "99sw");
            dbc.Open();
              var t_warlist2 = dbc.Selecttable<object[]>("T_warning");

            object[] t_warlist = dbc.Selecttable<object[]>("T_warning", " Areacodelist like '41%'");
            
           string sqlstr = "warningTime>'2019-03-15 14:00:00' && Areacodelist like '41%' ";

            DateTime dt = DateTime.Now;
            DateTime dt2 = DateTime.Now;
         //   listBox1.Items.Add("万条数据插入" + (dt2 - dt).TotalMilliseconds + "毫秒");
            user u = new user();
            u.name= "14,1401,1402"; 
            var uu = new aabb();
            uu.list.Add(new bbcc());
            u.list.Add(uu);
        
            if (dbc.Createtable("ddd2"))
            {

            }
         
            bool bbc = dbc.Inserttable<user>("ddd2", u);
            user[] u2 = dbc.Selecttable<user[]>("ddd2");
            bbc = dbc.Inserttable<user>("ddd", u);


            //每次插入一组数据
            List<user> list = new List<user>();

            int i = 0;
            while (i < 200)
            {
                u = new user();
                u.id = i;
                list.Add(u);
                i++;
            }
             dt = DateTime.Now;
            bbc = dbc.Inserttable<user>("ddd", list.ToArray());

             dt2 = DateTime.Now;
            listBox1.Items.Add("万条数据插入" + (dt2 - dt).TotalMilliseconds + "毫秒");

            dbc.Updatetable("ddd", "id<10", new { name = "特大喜讯" });
            int count = 0;

            dt = DateTime.Now;

            var rrs = dbc.Selecttable<List<user>>("ddd", "id<100", 0, "", 0, 0, out count);

            dt2 = DateTime.Now;
            if(rrs!=null)
            listBox1.Items.Add("数据SQL查询" + (dt2 - dt).TotalMilliseconds + "毫秒。" + "查询数量:" + rrs.Count);

            dbc.Deletetable("ddd", "id<2000");

            //int iii = 0;
            //while (iii < 1)
            //{
            //    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(go), null);
            //    iii++;
            //}
             rrs = dbc.Selecttable<List<user>>("ddd", "id<200", 0, "", 0, 0, out count);
            if(rrs!=null)
            listBox1.Items.Add("查询数量:" + rrs.Count);
            dbc.Close();
            // go(null);
        }

        void go(object ob)
        {
            DBClient dbc = new DBClient("127.0.0.1", 18989, "admin", "123123");
            dbc.Open();
            
            dbc.Deletetable("ddd", "id<100");

            dbc.Deletetable("ddd", "id<2000");


          //  dbc.Removetable("ddd");
            dbc.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //description

            //127.0.0.1
            var dbClient = new DBClient("127.0.0.1", 18989, "admin", "123123");
            dbClient.Open();
            //var count3 = dbClient.Deletetable("T_warning", "");
            ////    bool bb = dbClient.Createtable("T_warning");
            //var count2 = dbClient.SelectCount("T_warning", "");
            //System.IO.StreamReader sr = new System.IO.StreamReader("ab.json");
            //string ssr = sr.ReadToEnd();
            //sr.Close();
            //T_warning[] data = Newtonsoft.Json.JsonConvert.DeserializeObject<T_warning[]>(ssr);
            //bool ff = dbClient.Inserttable<T_warning>("T_warning", data);

            // 
            DateTime dt = DateTime.Now;
                var count = dbClient.SelectCount("T_warning", "");
            DateTime dt2 = DateTime.Now;

            listBox1.Items.Add("数据COUNT查询" + (dt2 - dt).TotalMilliseconds + "毫秒。" + count);
            //System.IO.StreamReader sr = new System.IO.StreamReader("ab.json");
            //string ssr = sr.ReadToEnd();
            //sr.Close();
            //T_warning[] data = Newtonsoft.Json.JsonConvert.DeserializeObject<T_warning[]>(ssr);
            //bool ff = dbClient.Inserttable<T_warning>("T_warning", data);
            //JArray objbbs = JArray.Parse(ssr);
            //foreach (JObject jo in objbbs)
            //{
            //    bool fff = dbClient.Inserttable<JObject>("T_warning", jo);
            //}
            //var count = 0;se
            // warningTime>'2020-08-12 10:00:00' && Areacodelist like '41%'
            //warningTime,Areacodelist,lat,lng,Typename
            //// dbClient.Deletetable("T_warning", "warningTime<'2020-09-02 00:00:00'");
             dt = DateTime.Now;
            var objjson = dbClient.Selecttable<object[]>("T_warning", "warningTime>'2020-12-08 10:00:00'");

            string str = Newtonsoft.Json.JsonConvert.SerializeObject(objjson);
            System.IO.StreamWriter sw = new System.IO.StreamWriter("ab.json");
            sw.Write(str);
            sw.Close();
            dt2 = DateTime.Now;

            listBox1.Items.Add("数据JSON查询" + (dt2 - dt).TotalMilliseconds + "毫秒。"+ objjson.Length);



            dt = DateTime.Now;


            var objjsona = dbClient.Selecttable<object[]>("T_warning", "warningTime<'2020-09-02 00:00:00'", 1);

            //string str = Newtonsoft.Json.JsonConvert.SerializeObject(objjson);
            //System.IO.StreamWriter sw = new System.IO.StreamWriter("ab.json");
            //sw.Write(str);
            //sw.Close();
            dt2 = DateTime.Now;
            if (objjsona == null)
                objjsona = new object[0];
            listBox1.Items.Add("数据Binary查询" + (dt2 - dt).TotalMilliseconds + "毫秒。" + objjsona.Length);
            //var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "1.json"));
            //var model = JsonConvert.DeserializeObject(json);
            //dbClient.Set("撒大受打击-41", model);
            //object obj=  dbClient.Get<object>("撒大受打击-41");

            dbClient.Close();
        }
    }
    public class CAllAreaInfo
    {
        public int x;
        public int y;

        public int AreaNum { get; set; }
        public int PixelNumber { get; set; }
        public Pointxy CenterPoint { get; set; }
        public List<Pointxy> Points = new List<Pointxy>();
    }
    public class Pointxy
    {
        public int x;
        public int y;

    }
    public class radardatamode
    {
        public double minlon;
        public double maxlon;
        public double minlat;
        public double maxlat;
        public double[,] data;
        public DateTime dt;
        public storm[] storms; //风暴属性
    }
    public class storm
    {
        public string ID;
        public int x;
        public int y;
        public int dbz = 0;
        public Double speed;
        public Double direction;
        public DateTime dt;

    }
    public class T_warning
    {
        public DateTime warningTime;
        public string description;
        public string eventType;
        public string Typename;
        public string headline;
        public string severity;
        public string lng;
        public string lat;
        public string Areacodelist;
        public DateTime insertime;
    }
    public class user
    {
        public int id = 0;
        public string name = "";
       public List<aabb> list=new List<aabb>();
    }

    public class aabb
    {
        public string name = "";
        public List<bbcc> list = new List<bbcc>();
    }
    public class bbcc
    {
        public string name = "";
    }
}
