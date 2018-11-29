
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

        private void button1_Click(object sender, EventArgs e)
        {
            DBClient dbc = new DBClient("127.0.0.1", 18989, "admin", "123123");
            dbc.Set<String>("asdasd", "等相关资料以及软件在后续使用过程中因为开发原因或数据来源改动造成的功能无法正常使用等问题及时进行修改，包括提供相关开发接口。");
            int i = 0;
            while (i < 50)
            {
                DateTime dt = DateTime.Now;
                String str = dbc.Get<String>("asdasd");
                DateTime dt2 = DateTime.Now;
                listBox1.Items.Add("第"+(i++)+"次查询耗时：" +(dt2 - dt).TotalMilliseconds + "毫秒");
            }
        }
    }
}
