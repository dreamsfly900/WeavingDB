using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace WeavingDB.Logical
{
    public unsafe class BinaryData
    {
        public static byte[] EncodeBinaryData(Head[] heads, ListDmode[] dmodes,int count=0,int countlen=0,int [] vecol=null)
        {

            List<byte> listall = new List<byte>();
            byte[] datas = headsToByte(heads);
            listall.AddRange(datas);
            //byte[] all = new byte[datas.Length];
            //Array.Copy(datas, 0, all, 0, datas.Length);
            if (countlen == 0)
                countlen = dmodes.Length;
            for (int i = count; i < countlen; i++)
            {
                if (dmodes[i] != null)
                {
                    byte[] row= DmodeToByte(heads, dmodes[i], vecol );
                    listall.AddRange(row);
                    //byte[] temp = new byte[all.Length + row.Length];
                    //Array.Copy(all, 0, temp, 0, all.Length);
                    //Array.Copy(row, 0, temp, all.Length, row.Length);
                    //all = temp;
                }
            }
            return listall.ToArray();
        }
        public static ListDmode[]  DecodeBinaryData(byte[] datas)
        {
            int offset = 0;
            Head[] heads= ByteToheads(datas, ref offset);
            List<ListDmode> list = new List<ListDmode>();
            while (true)
            {
                if(offset>= datas.Length)
                    return list.ToArray();
                ListDmode Dmode= ByteToDmode(heads, datas, ref offset);
                list.Add(Dmode);
            }
        }
        public static JArray DecodeBinaryDataJson(byte[] datas)
        {
            int offset = 0;
            Head[] heads = ByteToheads(datas, ref offset);
            JArray list = new JArray();
            while (true)
            {
                if (offset >= datas.Length)
                    return list;
                JObject Dmode = ByteToJObject(heads, datas, ref offset);
                list.Add(Dmode);
            }
            //return list;
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
        public unsafe static byte[] DmodeToByte(Head[] heads, ListDmode data, int[] vecol = null)
        {
            List<byte> listall = new List<byte>();
           // byte[] dataslist = new byte[0];
            //byte[] lenbit = BitConverter.GetBytes(offset);
            //dataslist.AddRange(lenbit);
            if (data != null && data.dtable2 != null)
                {
                    for (int i = 0; i < heads.Length; i++)
                    {

                    byte[] dta;
                        // 按照类型写入；
                        if (vecol!=null && vecol.Length>0)
                            dta= GetHashtablebyte(heads[i].type, data.dtable2[vecol[i]], data.LenInts[vecol[i]]);
                        else
                            dta = GetHashtablebyte(heads[i].type, data.dtable2[i], data.LenInts[i]);
                     byte[] lenbit = new byte[0];
                        if (heads[i].type != 6 && heads[i].type != 7 && heads[i].type != 9 && heads[i].type != 12)
                        {
                        int len = 0;
                        if (vecol != null && vecol.Length > 0)
                            len = data.LenInts[vecol[i]];
                        else
                            len = data.LenInts[i];
                        lenbit = BitConverter.GetBytes(len);
                        listall.AddRange(lenbit);
                    }
                   
                    byte[] temp = new byte[lenbit.Length + dta.Length];
                    //Array.Copy(lenbit, 0, temp, 0, lenbit.Length);
                    //Array.Copy(dta, 0, temp, lenbit.Length, dta.Length);
                    //byte[] datas = new byte[dataslist.Length+ temp.Length];
                    //Array.Copy(dataslist, 0, datas, 0, dataslist.Length);
                    //Array.Copy(temp, 0, datas, dataslist.Length, temp.Length);
                    //dataslist = datas;
                    //dataslist.AddRange(lenbit);
                    listall.AddRange(dta);
                }
                }
            
            return listall.ToArray();
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
        public unsafe static JObject ByteToJObject(Head[] heads, byte[] data, ref int offset)
        {
            JObject listDmode = new JObject();
            

            for (int i = 0; i < heads.Length; i++)
            {

                // 按照类型写入；
                int len = 0;
                JProperty dta = GetbyteToJProperty(heads[i].key,heads[i].type, data, ref offset, ref len);
                listDmode.Add(dta);

            }


            return listDmode;
        }
        public static JProperty GetbyteToJProperty(string key,byte type, byte[] data, ref int offset, ref int len)
        {


            try
            {
                if (type == 6)
                {
                    int p = BitConverter.ToInt32(data, offset);
                   
                    offset += 4;
                    len = 4;
                  
                    return new JProperty(key, p);
                }
                else if (type == 9)
                {
                    bool p = BitConverter.ToBoolean(data, offset);
                  
                    offset += 1;
                    len = 1;
                    return new JProperty(key, p);

                }
                else if (type == 7)
                {
                    double p = BitConverter.ToDouble(data, offset);
                   
                    offset += 8;
                    len = 8;
                    return new JProperty(key, p);
                }
                else if (type == 12)
                {
                    long p = BitConverter.ToInt64(data, offset);
                   
                    offset += 8;
                    len = 8;
                    return new JProperty(key, DateTime.FromFileTime(p));
                }
                else if (type == 8)
                {
                    int lens = BitConverter.ToInt32(data, offset);
                    len = lens;
                    string str = System.Text.Encoding.Default.GetString(data, offset + 4, lens);
                    offset += 4 + lens;
                    return new JProperty(key, str);
                }
                else if (type == 10)
                {
                    return null;
                }
                else
                {
                    int lens = BitConverter.ToInt32(data, offset);
                    len = lens;
                    byte[] abc = new byte[lens];
                    Array.Copy(data, offset + 4, abc, 0, lens);
                    offset += 4 + lens;
                    
                    string temp = BytesToT<string>(GZIP.Decompress(abc));

                    object obj = Newtonsoft.Json.JsonConvert.DeserializeObject(temp);
                    return new JProperty(key, obj);
                }
            }
            catch
            {
                return null;
            }

        }
        public static T BytesToT<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length);
                var bf = new BinaryFormatter();
                ms.Position = 0;
                var x = bf.Deserialize(ms);
                return (T)x;
            }
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
                    IntPtr p1 = Tobytes(abc);
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
                if ((IntPtr)p1 == IntPtr.Zero)
                    return new byte[0];
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
                    byte[] abc =Tobyte((byte*)p1, len);
                    //GZIP.Decompress(abc);


                    return abc;

                }
            }
            catch
            {
                return null;
            }


        }
        public static unsafe byte[] Tobyte(byte* write_data, int data_len)
        {

            byte[] write = new byte[data_len];
            Marshal.Copy((IntPtr)write_data, write, 0, write.Length);
            return write;
        }
      
        public static unsafe IntPtr Tobytes(byte[] write)
        {
            IntPtr write_data = Marshal.AllocHGlobal(write.Length);
            Marshal.Copy(write, 0, write_data, write.Length);
            return write_data;
        }
    }
}
