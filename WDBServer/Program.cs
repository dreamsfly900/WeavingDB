using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace WDBServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TownCrier tcc=   new TownCrier();
            HostFactory.Run(x =>                                 //1
            {
                x.Service<TownCrier>(s =>                        //2
                {
                    s.ConstructUsing(name => tcc);     //3
                    s.WhenStarted(tc => tc.Start());              //4
                    s.WhenStopped(tc => tc.Stop());               //5
                });
                x.RunAsLocalSystem();                            //6
                x.StartAutomaticallyDelayed();
                x.SetDescription("雷达-遥感-等值面图数据库");        //7
                x.SetDisplayName("WeavingDB");                       //8
                x.SetServiceName("WeavingDB");                       //9
            });
           
          
            while (true)
            {
               string str= Console.ReadLine();
                switch (str)
                {
                    case "linknum":
                        Console.WriteLine(tcc.dbcon.count);
                        break;
                }
            }
        }

        public class TownCrier
        {
            public WeavingDB.Logical.DBcontrol dbcon;
            public TownCrier()
            {
                
            }

            public void Start()
            {
                dbcon = new WeavingDB.Logical.DBcontrol();

            }

            public void Stop()
            {


            }
        }

    }
}
