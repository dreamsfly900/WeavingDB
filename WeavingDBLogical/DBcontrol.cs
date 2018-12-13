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
      public  DBmanage dbm = new DBmanage();
        string userid = "",pwd="";
        int port = 0;
        public DBcontrol()
        {
            try
            {
                port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
                userid = (System.Configuration.ConfigurationManager.AppSettings["userid"]);
                pwd = (System.Configuration.ConfigurationManager.AppSettings["pwd"]);
                WeavingDB.DataEncoding.userid = userid;
                WeavingDB.DataEncoding.pwd = pwd;

                wserver.weaveUpdateSocketListEvent += Wserver_weaveUpdateSocketListEvent;
                wserver.weaveDeleteSocketListEvent += Wserver_weaveDeleteSocketListEvent;
                wserver.weaveReceiveBitEvent += Wserver_weaveReceiveBitEvent;
                wserver.Start(port);
            }
            catch
            { }

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
                        }else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));

                        break;
                    case 0x02:
                        //byte[] hhh = new byte[] { 31, 139, 8, 0, 0, 0, 0, 0, 4, 0, 99, 96, 100, 96, 96, 248, 15, 4, 32, 26, 4, 216, 64, 12, 70, 67, 110, 0, 117, 40, 253, 6, 25, 0, 0, 0 };
                        //wserver.Send(soc, command, hhh);
                        //return;
                        if (WeavingDB.DataEncoding.getKVdecode(rowsdata, out key))
                        {
                            try
                            {

                                byte[] bbb = dbm.get(key);
                                if (bbb != null)
                                {
                                    bbb = GZIP.Compress(bbb);

                                    wserver.Send(soc, command, bbb);
                                }
                                else
                                {
                                    wserver.Send(soc, command, new byte[1]);

                                }
                            }
                            catch {

                            }
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x03:
                        if (WeavingDB.DataEncoding.getKVdecode(rowsdata, out key))
                        {
                            bool bb = dbm.Remove(key);
                            wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));

                        break;
                    case 0x04:
                        if (WeavingDB.DataEncoding.getKVdecode(rowsdata, out key))
                        {
                            bool bb = dbm.Createtable(key);
                            wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));

                        break;
                    case 0x05:
                        if (WeavingDB.DataEncoding.getKVdecode(rowsdata, out key))
                        {
                            bool bb = false;
                            try
                            {
                                 bb = dbm.deletetable(key);
                            }
                            catch
                            { }
                            wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x06:
                        if (WeavingDB.DataEncoding.setKVdecode(rowsdata, out key, out datas))
                        {
                            bool bb = (dbm.insettabledata(key, System.Text.Encoding.UTF8.GetString( datas)));
                            wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x07:
                        if (WeavingDB.DataEncoding.setKVdecode(rowsdata, out key, out datas))
                        {
                            bool bb = (dbm.insettabledataArray(key, System.Text.Encoding.UTF8.GetString(datas)));
                            wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x08:
                        if (WeavingDB.DataEncoding.setKVdecode(rowsdata, out key, out datas))
                        {
                            string [] ss= DataEncoding.dencdingdata(datas);
                            if (ss.Length == 5)
                            {
                                // where, order.ToString(), coll, pageindex.ToString(), pagesize.ToString()
                                int count = 0;
                               string str=  dbm.selecttabledata(key, ss[0], Convert.ToByte(ss[1]), Convert.ToInt32(ss[3]), Convert.ToInt32(ss[4]),out count, ss[2]);
                               byte[] senddata= GZIP.Compress( DataEncoding.encodingdata(count.ToString(), str));
                               wserver.Send(soc, command, senddata);
                            }
                         
                            //   bool bb = (dbm.insettabledataArray(key, System.Text.Encoding.UTF8.GetString(datas)));
                            // wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x09:
                        if (WeavingDB.DataEncoding.setKVdecode(rowsdata, out key, out datas))
                        {
                            string[] ss = DataEncoding.dencdingdata(datas);
                            if (ss.Length == 1)
                            {
                                // where, order.ToString(), coll, pageindex.ToString(), pagesize.ToString()
                                
                                bool bb = dbm.deletetabledata(key, ss[0]);
                               
                                wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                            }

                            //   bool bb = (dbm.insettabledataArray(key, System.Text.Encoding.UTF8.GetString(datas)));
                            // wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x10:
                        if (WeavingDB.DataEncoding.setKVdecode(rowsdata, out key, out datas))
                        {
                            string[] ss = DataEncoding.dencdingdata(datas);
                            if (ss.Length == 2)
                            {
                                // where, order.ToString(), coll, pageindex.ToString(), pagesize.ToString()
                                string where = ss[0];
                                string uodatedata = ss[1];
                                if (uodatedata != "")
                                {
                                    bool bb = dbm.updatetabledata(key, where, uodatedata);

                                    wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                                }
                            }

                            //   bool bb = (dbm.insettabledataArray(key, System.Text.Encoding.UTF8.GetString(datas)));
                            // wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x11://返回通配符KEY
                        if (WeavingDB.DataEncoding.getKVdecode(rowsdata, out key))
                        {



                            try
                            {
                                string[] keys = dbm.selctekey(key);
                                if (keys.Length == 0)
                                {
                                    wserver.Send(soc, command, new byte[1]);
                                }
                                else
                                {
                                    byte[] temp = DataEncoding.encodingdata(keys);


                                    wserver.Send(soc, command, GZIP.Compress(temp));
                                }
                            }
                            catch
                            { }
                             
                           
                        }
                        break;
                    case 0x12:
                        string[] kets;
                        List<byte[]> list = new List<byte[]>();
                        if (WeavingDB.DataEncoding.setKVsdecode(rowsdata, out kets, out list))
                        {
                            // datas
                            if (kets.Length != list.Count)
                            {
                                wserver.Send(soc, command, new byte[] { Convert.ToByte(false) });
                            }
                            else
                            {
                                for (int i = 0; i < kets.Length; i++)
                                {
                                    bool bb = (dbm.set(kets[i], list[i]));
                                    if (!bb)
                                    {
                                        wserver.Send(soc, command, new byte[] { Convert.ToByte(false) });
                                        return;
                                        
                                    }
                                }
                               
                                wserver.Send(soc, command, new byte[] { Convert.ToByte(true) });
                            }

                          
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    default:
                        wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("又在乱搞吧"));
                        break;
                }
            }
            catch (Exception e)
            {
                wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes(e.Message));
            }
        // wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("又在乱搞吧"));
            //  soc.Close();
        }

       
    }
}
