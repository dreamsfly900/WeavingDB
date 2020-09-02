using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeavingDB.Logical;

namespace WeavingDB.Logical
{
  public  class BinaryData
    {
        public static void WriteTableHead(string path,string tablename, Liattable Ltb)
        {
            if (!Directory.Exists(path + "TDATA"))
            {

                Directory.CreateDirectory(path + "TDATA");
            }
            FileStream fileStream = new FileStream(path + @"TDATA\" + tablename + ".bin", FileMode.OpenOrCreate, FileAccess.Write);
            BinaryWriter bw = new BinaryWriter(fileStream);
            bw.Write(Ltb.datahead.Length);
            foreach (Head hd in Ltb.datahead)
            {
                bw.Write(hd.type);
                bw.Write(hd.index);
               // bw.Write(hd.key.Length);
                bw.Write(hd.key);
            }
            bw.Write(Ltb.tree.Count);
            foreach (string key in Ltb.tree.Keys)
            {
                bw.Write(key);
            }
            bw.Flush();
            bw.Close();
            fileStream.Close();
        }
        public static Liattable  ReadTableHead(string path, string tablename)
        {
            if (!Directory.Exists(path + "TDATA"))
            {

                Directory.CreateDirectory(path + "TDATA");
            }
            try
            {
                Liattable lib = new Liattable();
                FileStream fileStream = new FileStream(path + @"TDATA\" + tablename + ".bin", FileMode.Open, FileAccess.Read);
                BinaryReader bw = new BinaryReader(fileStream);
                int len = bw.ReadInt32();
                Head[] list = new Head[len];
                for (int i = 0; i < len; i++)
                {
                    Head hd = new Head();
                    hd.type = bw.ReadByte();
                    hd.index = bw.ReadInt32();
                    //  int strlen = bw.ReadInt32();
                    hd.key = bw.ReadString();
                    list[i] = hd;
                }
                len = bw.ReadInt32();
                for (int i = 0; i < len; i++)
                {
                    string key = bw.ReadString();
                    lib.tree.Add(key, new BPTree(100));
                }
                bw.Close();
                fileStream.Close();
                lib.datahead = list;
                return lib;
            }
            catch
            {
                return new Liattable();
            }
        }
        public static SafeFileHandle getWriteDataHandle(string path, string tablename, Head[] heads, ListDmode data)
        {
            FileStream fileStream = new FileStream(path + @"TDATA\" + tablename + ".data", FileMode.Open, FileAccess.ReadWrite);
           return  fileStream.SafeFileHandle;
        }
        public static void WriteData(SafeFileHandle fileintptr,Head[] heads,ListDmode data)
        {
            FileStream fileStream = new FileStream(fileintptr,FileAccess.Write);

          
        }

    }
}
