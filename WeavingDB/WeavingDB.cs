
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
            //object obj= Newtonsoft.Json.JsonConvert.PopulateObject(builder.ToString());
        }
        DBClient dbc = new DBClient("127.0.0.1", 18989, "admin", "123123");
        private void button1_Click(object sender, EventArgs e)
        {
           
            dbc.open();
            dbc.Set<String>("asdasd", "等相关资料以及软件在后续使用过程中因为");
            int i = 0;
            while (i < 50)
            {
                DateTime dt = DateTime.Now;
                String str = dbc.Get<String>("asdasd");
                DateTime dt2 = DateTime.Now;
                listBox1.Items.Add("第"+(i++)+"次查询耗时：" +(dt2 - dt).TotalMilliseconds + "毫秒");
            }
            dbc.close();
        }
    }
}
