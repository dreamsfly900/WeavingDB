using System;
using System.Linq;
using WeavingDB.Logical;

namespace wdbserverform
{
    class Program
    {

        static void Main(string[] args) {

            DBcontrol  dbcon=   new WeavingDB.Logical.DBcontrol();
           
            while (true)
            {
                Console.WriteLine("欢迎使用wdb数据库！常用命令如下：");
                Console.WriteLine("createIndex table.field创建索引 ,selectKV 查看所有kv的K值");
                Console.WriteLine("selecttable 查看所有表名,select field,field from table where 查询表 ");
                Console.WriteLine("selectcount  field,field from table where 查询表行数 ");
                try
                {
                    string str = Console.ReadLine();
                    if (str.IndexOf("createIndex") == 0)
                    {
                        string[] ss = str.Split(' ');
                        if (ss.Length > 1)
                        {
                           bool b=  dbcon.dbm.Createindex(ss[1].Split('.')[0], ss[1].Split('.')[1]);
                            Console.WriteLine(ss[1] + "索引创建成功");
                         }
                    }
                    else if (str.IndexOf("selecttable") == 0)
                    {
                        string[] keys = dbcon.dbm.CDtable.Keys.ToArray(); 
                        foreach (string key in keys)
                            Console.WriteLine(key);
                    }
                    else if (str.IndexOf("selectKV") == 0)
                    {
                       string[] keys=  dbcon.dbm.Selctekey("*");
                        foreach(string key in keys)
                        Console.WriteLine(key);
                    }
                    else if (str.IndexOf("selectcount") == 0)
                    {
                        string viewcol = str.Substring(str.IndexOf("selectcount") + ("selectcount").Length, str.IndexOf("from") - (str.IndexOf("selectcount") + ("selectcount").Length));
                        string table="";
                        string where = "";
                        if (str.IndexOf("where") >= 0)
                        {
                            table = str.Substring(str.IndexOf("from") + ("from").Length, str.IndexOf("where") - (str.IndexOf("from") + ("from").Length));
                            where = str.Substring(str.IndexOf("where") + ("where").Length, str.Length - (str.IndexOf("where") + ("where").Length));
                        }
                        else
                        {
                            table = str.Substring(str.IndexOf("from")+("from").Length);
                        }
                        int count = 0;
                        string data = dbcon.dbm.Selectcount(table, where, out count);
                      
                        Console.WriteLine(count+"行");

                    }
                    else if (str.IndexOf("select") == 0)
                    {
                       string viewcol= str.Substring(str.IndexOf("select") + ("select").Length, str.IndexOf("from")-(str.IndexOf("select") + ("select").Length));
                        string table = "";
                        string where = "";
                        if (str.IndexOf("where") >= 0) {
                            where = str.Substring(str.IndexOf("where") + ("where").Length, str.Length - (str.IndexOf("where") + ("where").Length));
                            table = str.Substring(str.IndexOf("from") + ("from").Length, str.IndexOf("where") - (str.IndexOf("from") + ("from").Length));
                        }
                        else
                        {
                            table = str.Substring(str.IndexOf("from")+("from").Length);
                        }
                        int count = 0;
                         string data=  dbcon.dbm.Selecttabledata(table, where, 0, 0, 100, out count, "", viewcol);
                        Console.WriteLine(data);
                        Console.WriteLine("仅显示前100行");

                    }
                }
                catch (Exception e) { Console.WriteLine(e.Message); }
            }

        }
        

      

    }
   
}
