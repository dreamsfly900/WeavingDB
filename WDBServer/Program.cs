using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDBServer
{
    class Program
    {
        static void Main(string[] args)
        {
            WeavingDBLogical.DBcontrol dbcon = new WeavingDBLogical.DBcontrol();
            while (true)
            {
               string str= Console.ReadLine();
                switch (str)
                {
                    case "linknum":
                        Console.WriteLine(dbcon.count);
                        break;
                }
            }
        }
    }
}
