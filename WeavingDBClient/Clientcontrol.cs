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
        bool type = false;
        public Clientcontrol(string ip, int _port)
        {
          
            IP = ip;
            port = _port;
             
         
        }

        internal void open()
        {
            p2Pclient = new Weave.TCPClient.P2Pclient(Weave.TCPClient.DataType.bytes);
            p2Pclient.receiveServerEventbit += P2Pclient_receiveServerEventbit;
            if (!p2Pclient.start(IP, port, false))
            {
                throw new Exception("连接失败！");
            }
        }

        private void P2Pclient_receiveServerEventbit(byte command, byte[] data)
        {
            switch (command)
            {
                case 0x01:
                    rowsdata = data;

                    break;
                case 0x02:
                    rowsdata = GZIP.Decompress(data); 
                    break;
                case 0xfe:
                    error = System.Text.Encoding.UTF8.GetString(data);
                    break;

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
