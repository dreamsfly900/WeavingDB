using Microsoft.Win32.SafeHandles;
using SQLDBlogic.logic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace WeavingDB.Logical
{
    public class BinaryData
    {
        public static void WriteTableHead(string path, string tablename, Liattable Ltb)
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
        public static Liattable ReadTableHead(string path, string tablename)
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

        public static byte[] headsToByte(Head[] heads)
        {
            ushort len = (ushort)heads.Length;
            List<byte> datas = new List<byte>();
            byte[] lenbit = BitConverter.GetBytes(len);
            datas.AddRange(lenbit);
            foreach (Head head in heads)
            {
                byte[] temp = System.Text.Encoding.UTF8.GetBytes(head.key);
                byte[] templenbit = BitConverter.GetBytes(temp.Length);
            //    byte[] temptype = BitConverter.GetBytes(head.type);
                datas.AddRange(templenbit);
                datas.AddRange(temp);
                datas.Add(head.type);

            }

            return datas.ToArray();
        }
        public static Head[] ByteToheads(byte[] datas, ref int offsetall)
        {
            ushort len = BitConverter.ToUInt16(datas, 0);
            offsetall += 2;
            Head[] heads = new Head[len];
            int offset = 0;
            for (int i = 0; i < len; i++)
            {
                heads[i] = new Head();
                int nnlen= BitConverter.ToInt32(datas, 2 + (i * 4) + (i * 1) + offset);
             
                heads[i].key = System.Text.Encoding.UTF8.GetString(datas, 2 +4+ (i * 4) + (i * 1) + offset, nnlen);
                heads[i].type = datas[2 + 4+(i * 4) + (i * 1) + offset+ nnlen];
                heads[i].index = i;
              
                offset += nnlen;
                offsetall += nnlen;
            }
            offsetall +=  (len * 4) + (len * 1) ;
            //   offsetall
            return heads;
        }
        public unsafe static byte[] DmodeToByte(Head[] heads, ListDmode data)
        {

            byte[] dataslist = new byte[0];
            //byte[] lenbit = BitConverter.GetBytes(offset);
            //dataslist.AddRange(lenbit);
            if (data != null && data.dtable2 != null)
                {
                    for (int i = 0; i < heads.Length; i++)
                    {
                     
                        // 按照类型写入；
                        byte[] dta= GetHashtablebyte(heads[i].type, data.dtable2[i], data.LenInts[i]);
                       byte[] lenbit = new byte[0];
                        if (heads[i].type != 6 && heads[i].type != 7 && heads[i].type != 9 && heads[i].type != 12)
                        {
                            int len = data.LenInts[i];
                            lenbit = BitConverter.GetBytes(len);
                           
                        }
                    byte[] temp = new byte[lenbit.Length + dta.Length];
                    Array.Copy(lenbit, 0, temp, 0, lenbit.Length);
                    Array.Copy(dta, 0, temp, lenbit.Length, dta.Length);
                    byte[] datas = new byte[dataslist.Length+ temp.Length];
                    Array.Copy(dataslist, 0, datas, 0, dataslist.Length);
                    Array.Copy(temp, 0, datas, dataslist.Length, temp.Length);
                    dataslist = datas;
                    //dataslist.AddRange(lenbit);
                    //dataslist.AddRange(dta);
                }
                }
            
            return dataslist;
        }
        public unsafe static ListDmode ByteToDmode(Head[] heads, byte[] data ,ref int offset )
        {
            ListDmode listDmode = new ListDmode();
            listDmode.dtable2 = new void*[heads.Length];
            listDmode.LenInts = new int[heads.Length];
          
                for (int i = 0; i < heads.Length; i++)
                {

                // 按照类型写入；
                    int len = 0;
                    void* dta = GetbyteTointprt(heads[i].type, data,ref offset,ref len);
                    listDmode.dtable2[i] = dta;
                     listDmode.LenInts[i] = len;

                }
            

            return listDmode;
        }
        unsafe static void* GetbyteTointprt(byte type, byte[] data,ref int offset,ref int len)
        {



            try
            {
                if (type == 6)
                {
                    
                    int p = BitConverter.ToInt32(data, offset);
                    int nSizeOfPerson = Marshal.SizeOf(p);
                    IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                    Marshal.StructureToPtr(p, intPtr, true);
                    offset += 4;
                    len = 4;
                    return intPtr.ToPointer();
                }
                else if (type == 9)
                {
                   
                    bool p = BitConverter.ToBoolean(data, offset);
                    int nSizeOfPerson = Marshal.SizeOf(p);
                    IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                    Marshal.StructureToPtr(p, intPtr, true);
                    offset += 1;
                    len = 1;
                    return intPtr.ToPointer();

                }
                else if (type == 7)
                {
                  
                    double p = BitConverter.ToDouble(data, offset);
                    int nSizeOfPerson = Marshal.SizeOf(p);
                    IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                    Marshal.StructureToPtr(p, intPtr, true);
                    offset += 8;
                    len = 8;
                    return intPtr.ToPointer();

                }
                else if (type == 12)
                {
                   
                    long p = BitConverter.ToInt64(data, offset);
                    int nSizeOfPerson = Marshal.SizeOf(p);
                    IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                    Marshal.StructureToPtr(p, intPtr, true);
                    offset += 8;
                    len = 8;
                    return intPtr.ToPointer();
                   
                }
                else if (type == 8)
                {
                    int lens = BitConverter.ToInt32(data, offset);
                    len = lens;
                    string str= System.Text.Encoding.Default.GetString(data, offset + 4, lens);
                    offset += 4+ lens;
                    IntPtr p = Marshal.StringToHGlobalAnsi(str);
                    return p.ToPointer();


                }
              
                else
                {
                    int lens = BitConverter.ToInt32(data, offset);
                    len = lens;
                    byte[] abc = new byte[lens];
                    Array.Copy(data, offset + 4, abc, 0, lens);
                    offset += 4 + lens;
                    IntPtr p1 = utli.Tobytes(abc);
                    return p1.ToPointer();

 

                }
            }
            catch
            {
                return null;
            }


        }
        unsafe static byte[] GetHashtablebyte(byte type, void* p1, int len)
        {



            try
            {
                if (type == 6)
                {
                    int p = *(int*)p1;
                    return BitConverter.GetBytes(p);
                }
                else if (type == 9)
                {
                    bool p = *(bool*)p1;
                    return BitConverter.GetBytes(p);

                }
                else if (type == 7)
                {
                    double p = *(double*)p1;
                    return BitConverter.GetBytes(p);
                    
                }
                else if (type == 12)
                {
                    long p = *(long*)p1;
                    return BitConverter.GetBytes(p);

                }
                else if (type == 8)
                {
                    string str = Marshal.PtrToStringAnsi((IntPtr)p1);
                    return System.Text.Encoding.Default.GetBytes(str.ToCharArray());
                   

                }
                else if (type == 10)
                {
                    return null;
                }
                else
                {
                    byte[] abc =utli.Tobyte((byte*)p1, len);
                    //GZIP.Decompress(abc);


                    return abc;

                }
            }
            catch
            {
                return null;
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
