using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Weave.Base;
using Weave.Server;
using WeavingDB;
namespace WeavingDBLogical
{

   public class DBcontrol
    {
      WeaveP2Server wserver = new Weave.Server.WeaveP2Server(WeaveDataTypeEnum.Bytes);
        public int count = 0;
        DBmanage dbm = new DBmanage();
        string userid = "",pwd="";
        int port = 0;
        public DBcontrol()
        {
            port = Convert.ToInt32 (System.Configuration.ConfigurationManager.AppSettings["port"]);
            userid =(System.Configuration.ConfigurationManager.AppSettings["userid"]);
            pwd = (System.Configuration.ConfigurationManager.AppSettings["pwd"]);
            WeavingDB.DataEncoding.userid = userid;
            WeavingDB.DataEncoding.pwd = pwd;
         
            wserver.weaveUpdateSocketListEvent += Wserver_weaveUpdateSocketListEvent;
            wserver.weaveDeleteSocketListEvent += Wserver_weaveDeleteSocketListEvent;
            wserver.weaveReceiveBitEvent += Wserver_weaveReceiveBitEvent;
            wserver.Start(port);


        }

        private void Wserver_weaveDeleteSocketListEvent(System.Net.Sockets.Socket soc)
        {
            count--;
        }

        private void Wserver_weaveUpdateSocketListEvent(System.Net.Sockets.Socket soc)
        {
            count++;
        }

        private void Wserver_weaveReceiveBitEvent(byte command, byte[] data, System.Net.Sockets.Socket soc)
        {
            try
            {
                byte[] rowsdata = GZIP.Decompress(data);
                string key = "";
                byte[] datas;
                switch (command)
                {
                    case 0x01:
                        if (WeavingDB.DataEncoding.setKVdecode(rowsdata, out key, out datas))
                        {
                            bool bb = (dbm.set(key, datas));
                            wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }

                        break;
                    case 0x02:
                        if (WeavingDB.DataEncoding.getKVdecode(rowsdata, out key))
                        {
                            wserver.Send(soc, command, GZIP.Compress(dbm.get(key)));
                        }

                        break;

                }
            }
            catch (Exception e)
            {
                wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes(e.Message));
            }

          //  soc.Close();
        }

       
    }
}
