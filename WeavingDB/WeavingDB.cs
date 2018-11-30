
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
        double gggg = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            gggg = 0;
            dbc.open();
            dbc.Set<String>("asdasd", "1");
            int i = 0;
            String str2 = dbc.Get<String>("asdasd");
            while (i < 10000)
            {
                i++;
                DateTime dt = DateTime.Now;
                String str = dbc.Get<String>("asdasd");
                DateTime dt2 = DateTime.Now;
                double gg= (dt2 - dt).TotalMilliseconds;
                gggg += gg;
                listBox1.Items.Add("第" + (i++) + "次查询耗时：" + gg + "毫秒");
            }
        
            label1.Text = "1W次总耗时："+ gggg;
            dbc.close();
        }
    }
}
