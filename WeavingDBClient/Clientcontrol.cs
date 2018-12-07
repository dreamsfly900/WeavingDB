using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeavingDB;
using Weave.TCPClient;
namespace WeavingDBClient
{
   public class Clientcontrol
    {
        Weave.TCPClient.P2Pclient p2Pclient;
        string IP = ""; int port = 0;
        byte[] rowsdata;
        public bool finsh = false;
        public string error = "";
       
        public Clientcontrol(string ip, int _port)
        {
          
            IP = ip;
            port = _port;
             
         
        }

        internal bool open()
        {
            if (p2Pclient != null)
            {
                p2Pclient.receiveServerEventbit -= P2Pclient_receiveServerEventbit;
                p2Pclient.ErrorMge -= P2Pclient_ErrorMge;
            }
            p2Pclient = new Weave.TCPClient.P2Pclient(Weave.TCPClient.DataType.bytes);
            p2Pclient.receiveServerEventbit += P2Pclient_receiveServerEventbit;
            p2Pclient.ErrorMge += P2Pclient_ErrorMge;
            if (!p2Pclient.start(IP, port, false))
            {
                return false;
                //throw new Exception("连接失败！");
            
            }
            return true;
        }

        private void P2Pclient_ErrorMge(int type, string error)
        {
           
        }

        private void P2Pclient_receiveServerEventbit(byte command, byte[] data)
        {
            try
            {
                switch (command)
                {
                    case 0x01://set
                        rowsdata = data;

                        break;
                    case 0x02://get
                        if (data.Length == 1)
                        {
                            rowsdata = null;
                            break;
                        }
                        rowsdata = GZIP.Decompress(data);
                        break;
                    case 0x03://RemoveKV
                        rowsdata = data;
                        break;
                    case 0x04://Createtable
                        rowsdata = data;
                        break;
                    case 0x05://Removetable
                        rowsdata = data;
                        break;
                    case 0x06://inserttable
                        rowsdata = data;
                        break;
                    case 0x07://inserttableARRAY
                        rowsdata = data;
                        break;
                    case 0x08://select
                        rowsdata = GZIP.Decompress(data);
                        break;
                    case 0x09:
                        rowsdata = data;
                        break;
                    case 0x10:
                        rowsdata = data;
                        break;
                    case 0x11:
                        if (data.Length == 1)
                        {
                            rowsdata = null;
                            break;
                        }
                        rowsdata = GZIP.Decompress(data);
                        break;
                    case 0x12:
                        rowsdata = data;
                        break;
                    case 0xfe:
                        error = System.Text.Encoding.UTF8.GetString(data);
                        break;

                }
            }
            catch
            {

            }
         
            finsh = true;
            
        }

        internal void close()
        {
            p2Pclient.stop();
        }

        public byte[] Send(byte command, byte[] data)
        {

        
            error = "";
            finsh = false;
          
            if (p2Pclient.send(command, GZIP.Compress(data)))
            {
                DateTime dt = DateTime.Now;
                while (!finsh)
                {
                    //  System.Threading.Thread.Sleep(10);
                    if ((DateTime.Now - dt).TotalSeconds > 60)
                    {
                        p2Pclient.stop();
                        throw new Exception("获取数据超时！");
                    }
                }

            }
            else
                error = "发送失败";
            if (error != "")
                throw new Exception(error);
            return rowsdata;
        }
    }
}
