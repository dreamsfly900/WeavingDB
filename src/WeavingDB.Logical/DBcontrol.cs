using System;
using System.Collections.Generic;
using Weave.Base;
using Weave.Server;

namespace WeavingDB.Logical
{
    public class DBcontrol
    {
        readonly WeaveP2Server wserver = new WeaveP2Server(WeaveDataTypeEnum.Bytes);
        public int count = 0;
        public DBmanage dbm = new DBmanage();
        private readonly string userid = "";
        private readonly string pwd = "";
        readonly int port = 0;
        public DBcontrol()
        {
            try
            {
                port = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["port"]);
                userid = (System.Configuration.ConfigurationManager.AppSettings["userid"]);
                pwd = (System.Configuration.ConfigurationManager.AppSettings["pwd"]);
                DataEncoding.userid = userid;
                DataEncoding.pwd = pwd;

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
                        if (DataEncoding.SetKVdecode(rowsdata, out key, out datas))
                        {
                            bool bb = (dbm.Set(key, datas));
                            wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));

                        break;
                    case 0x02:
                        if (DataEncoding.GetKVdecode(rowsdata, out key))
                        {
                            try
                            {

                                byte[] bbb = dbm.Get(key);
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
                            catch
                            {

                            }
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x03:
                        if (DataEncoding.GetKVdecode(rowsdata, out key))
                        {
                            bool bb = dbm.Remove(key);
                            wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));

                        break;
                    case 0x04:
                        if (DataEncoding.GetKVdecode(rowsdata, out key))
                        {
                            bool bb = dbm.Createtable(key);
                            wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));

                        break;
                    case 0x05:
                        if (DataEncoding.GetKVdecode(rowsdata, out key))
                        {
                            bool bb = false;
                            try
                            {
                                bb = dbm.Deletetable(key);
                            }
                            catch
                            { }
                            wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x06:
                        if (DataEncoding.SetKVdecode(rowsdata, out key, out datas))
                        {
                            bool bb = (dbm.Insettabledata(key, System.Text.Encoding.UTF8.GetString(datas)));
                            wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x07:
                        if (DataEncoding.SetKVdecode(rowsdata, out key, out datas))
                        {
                            bool bb = (dbm.InsettabledataArray(key, System.Text.Encoding.UTF8.GetString(datas)));
                            wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x08:
                        if (DataEncoding.SetKVdecode(rowsdata, out key, out datas))
                        {
                            string[] ss = DataEncoding.Dencdingdata(datas);
                            if (ss.Length == 5)
                            {
                                string str = dbm.Selecttabledata(key, ss[0], Convert.ToByte(ss[1]), Convert.ToInt32(ss[3]), Convert.ToInt32(ss[4]), out int count, ss[2]);
                                byte[] senddata = GZIP.Compress(DataEncoding.Encodingdata(count.ToString(), str));
                                wserver.Send(soc, command, senddata);
                            }
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x09:
                        if (DataEncoding.SetKVdecode(rowsdata, out key, out datas))
                        {
                            string[] ss = DataEncoding.Dencdingdata(datas);
                            if (ss.Length == 1)
                            {
                                bool bb = dbm.Deletetabledata(key, ss[0]);

                                wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                            }
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x10:
                        if (DataEncoding.SetKVdecode(rowsdata, out key, out datas))
                        {
                            string[] ss = DataEncoding.Dencdingdata(datas);
                            if (ss.Length == 2)
                            {
                                string where = ss[0];
                                string uodatedata = ss[1];
                                if (uodatedata != "")
                                {
                                    bool bb = dbm.Updatetabledata(key, where, uodatedata);

                                    wserver.Send(soc, command, new byte[] { Convert.ToByte(bb) });
                                }
                            }
                        }
                        else
                            wserver.Send(soc, 0xfe, System.Text.Encoding.UTF8.GetBytes("账号或密码不正确"));
                        break;
                    case 0x11://返回通配符KEY
                        if (DataEncoding.GetKVdecode(rowsdata, out key))
                        {
                            try
                            {
                                string[] keys = dbm.Selctekey(key);
                                if (keys.Length == 0)
                                {
                                    wserver.Send(soc, command, new byte[1]);
                                }
                                else
                                {
                                    byte[] temp = DataEncoding.Encodingdata(keys);


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
                        if (DataEncoding.SetKVsdecode(rowsdata, out kets, out list))
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
                                    bool bb = (dbm.Set(kets[i], list[i]));
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
        }


    }
}
