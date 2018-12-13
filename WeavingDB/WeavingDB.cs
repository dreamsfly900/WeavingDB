
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeavingDBLogical;
using WeavingDBClient;
using System.Runtime.InteropServices;
using System.IO;
using WindowsFormsApplication1;
using Newtonsoft.Json;

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

 
    private unsafe void WeavingDB_Load(object sender, EventArgs e)
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

        private void button1_Click(object sender, EventArgs e)
        {
            DBClient dbc = new DBClient("127.0.0.1", 18989, "admin", "123123");
            double gggg = 0;
            dbc.open();

          //  Hashtable ht = new Hashtable();
          //  ht.Add("123123","afasdfasdf");
          //  ht.Add("12312311", "afasdfasdf");
          //  ht.Add("1231231221", "afasaasdfasdf");
          //  ht.Add("123123122199", "afasaasdfasdf");
          // bool bb= dbc.SetAll<string>(ht);

          //string sss=  dbc.Get<String>("12312311");
            String str2 = dbc.Get<String>("asdasd");
            dbc.GetKey("?d");
            dbc.Set("asdasd", 111);
           // dbc.Set<String>("asdasd", "1");
            int i = 0;
           string [] keys=   dbc.GetKey("as?asd");//通配符?一个匹配字符
            keys = dbc.GetKey("as*");//通配符* 表示，多个模糊匹配
            dbc.GetKey("as?a?d");
            dbc.GetKey("?d");
            while (i < 10000)
            {
                i++;
                DateTime dt = DateTime.Now;
                String str = dbc.Get<String>("asdasd");
                DateTime dt2 = DateTime.Now;
                double gg = (dt2 - dt).TotalMilliseconds;
                gggg += gg;
                listBox1.Items.Add("第" + (i++) + "次查询耗时：" + gg + "毫秒");
            }



            label1.Text = "1W次总耗时：" + gggg;
            dbc.close();
            //每次插入一条数据
           
        }
       
        private void button2_Click(object sender, EventArgs e)
        {
            DBClient dbc = new DBClient("127.0.0.1", 18989, "admin", "123123");
            dbc.open();
            user u = new user();
            var uu = new aabb();
            uu.list.Add(new bbcc());
            u.list.Add(uu);
        
            if (dbc.Createtable("ddd"))
            {

            }
            bool bbc = dbc.inserttable<user>("ddd", u);
            user[] u2 = dbc.selecttable<user[]>("ddd");
            bbc = dbc.inserttable<user>("ddd", u);


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
            DateTime dt = DateTime.Now;
            bbc = dbc.inserttable<user>("ddd", list.ToArray());

            DateTime dt2 = DateTime.Now;
            listBox1.Items.Add("万条数据插入" + (dt2 - dt).TotalMilliseconds + "毫秒");

            dbc.updatetable("ddd", "id<10", new { name = "特大喜讯" });
            int count = 0;

            dt = DateTime.Now;

            var rrs = dbc.selecttable<List<user>>("ddd", "id<100", 0, "", 0, 0, out count);

            dt2 = DateTime.Now;
            if(rrs!=null)
            listBox1.Items.Add("数据SQL查询" + (dt2 - dt).TotalMilliseconds + "毫秒。" + "查询数量:" + rrs.Count);

            dbc.deletetable("ddd", "id<2000");

            //int iii = 0;
            //while (iii < 1)
            //{
            //    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(go), null);
            //    iii++;
            //}
             rrs = dbc.selecttable<List<user>>("ddd", "id<200", 0, "", 0, 0, out count);
            if(rrs!=null)
            listBox1.Items.Add("查询数量:" + rrs.Count);
            dbc.close();
            // go(null);
        }

        void go(object ob)
        {
            DBClient dbc = new DBClient("127.0.0.1", 18989, "admin", "123123");
            dbc.open();
            
            dbc.deletetable("ddd", "id<100");

            dbc.deletetable("ddd", "id<2000");


          //  dbc.Removetable("ddd");
            dbc.close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var dbClient = new WeavingDBClient.DBClient("127.0.0.1", 18989, "admin", "123123");
            dbClient.open();

            var json = File.ReadAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "1.json"));
            var model = JsonConvert.DeserializeObject(json);
            dbClient.Set("撒大受打击-41", model);
            object obj=  dbClient.Get<object>("撒大受打击-41");

            dbClient.close();
        }
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
