
using System;
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
namespace WeavingDB
{
  
    public partial class WeavingDB : Form
    {
        public WeavingDB()
        {
            InitializeComponent();
        }
       
        private void WeavingDB_Load(object sender, EventArgs e)
        {
            //object obj= Newtonsoft.Json.JsonConvert.PopulateObject(builder.ToString());122.114.53.233
            //127.0.0.1
        }


        private void button1_Click(object sender, EventArgs e)
        {
            DBClient dbc = new DBClient("127.0.0.1", 18989, "admin", "123123");
            double gggg = 0;
            dbc.open();
         //   String str2 = dbc.Get<String>("asdasd");
            dbc.Set<String>("asdasd", "1");
            int i = 0;
         
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
            //int i = 0;
            //while (i < 1)
            //{
            //    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(go), null);
            //    i++;
            //}
            go(null);
        }

        void go(object ob)
        {
            DBClient dbc = new DBClient("127.0.0.1", 18989, "admin", "123");
            dbc.open();
            user u = new user();
            bool bbc = dbc.inserttable<user>("ddd", u);
            dbc.Createtable("ddd");

            bbc = dbc.inserttable<user>("ddd", u);


            //每次插入一组数据
            List<user> list = new List<user>();

            int i = 0;
            while (i < 10000)
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
            listBox1.Items.Add("数据SQL查询" + (dt2 - dt).TotalMilliseconds + "毫秒。" + "查询数量:" + rrs.Count);

            dbc.deletetable("ddd", "id<100");

            dbc.deletetable("ddd", "id<100");


            dbc.Removetable("ddd");
            dbc.close();
        }
    }
    public class user
    {
        public int id = 0;
        public string name = "";
    }
}
