﻿using System;

namespace WeavingDB.Client
{
    public class Clientcontrol
    {
        Weave.TCPClient.P2Pclient p2Pclient;
        readonly string IP = "";
        readonly int port = 0;
        byte[] rowsdata;
        public bool finsh = false;
        public string error = "";
        public int resttime = 0;
        public int timeout = 60;
        public Clientcontrol(string ip, int _port)
        {
            IP = ip;
            port = _port;
        }

        internal bool Open()
        {
            if (p2Pclient != null)
            {
                p2Pclient.ReceiveServerEventbit -= P2Pclient_receiveServerEventbit;
                p2Pclient.ErrorMge -= P2Pclient_ErrorMge;
            }
          
            p2Pclient = new Weave.TCPClient.P2Pclient(Weave.TCPClient.DataType.bytes);
            p2Pclient.resttime = resttime;
            p2Pclient.ReceiveServerEventbit += P2Pclient_receiveServerEventbit;
            p2Pclient.ErrorMge += P2Pclient_ErrorMge;
            if (!p2Pclient.Start(IP, port, false))
            {
                return false;
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
                    case 0x81://select
                        if (data.Length > 1)
                            rowsdata = GZIP.Decompress(data);
                        else
                            rowsdata = new byte[0];
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
                    case 0x13:
                        rowsdata = data;
                        break;
                    case 0x14:
                        rowsdata =  GZIP.Decompress(data); 
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

        internal void Close()
        {
            p2Pclient.Stop();
        }

        public byte[] Send(byte command, byte[] data)
        {
            error = "";
            finsh = false;

            if (p2Pclient.Send(command, GZIP.Compress(data)))
            {
                DateTime dt = DateTime.Now;
                while (!finsh)
                {
                    System.Threading.Thread.Yield();
                    if ((DateTime.Now - dt).TotalSeconds > timeout)
                    {
                        p2Pclient.Stop();
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
