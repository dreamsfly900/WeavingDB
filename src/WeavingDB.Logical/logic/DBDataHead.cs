
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using WeavingDB;
using WeavingDB.Logical;

namespace SQLDBlogic.logic
{
 
    public unsafe class DBDataHead
    {
       public static  void*[] Gethtabledtjsontointptr(JObject obj, Head[] dhead, ref int[] lensInts, ref IntPtr[] objIntPtrs)
        {
            void*[] objs = new void*[dhead.Length];
            objIntPtrs = new IntPtr[dhead.Length];
            lensInts = new int[dhead.Length];
            foreach (var item in dhead)
            {
                byte jtt = item.type;
                if (obj[item.key] != null)
                {
                    try
                    {
                        int len = 0;
                        objs[item.index] = getdata(jtt, obj[item.key], ref len);
                        lensInts[item.index] = len;
                        
                    }
                    catch
                    {
                        Marshal.FreeHGlobal((IntPtr)objs[item.index]);
                    }
                }
            }
            return objs;
        }
        public static void* getdata(byte jtt, JToken obj,ref int len)
        {
            try
            {
                if (jtt == 10)
                {
                    return null;
                }
                else if (jtt == 6)
                {
                    int p = Convert.ToInt32(obj.ToString());
                    int nSizeOfPerson = Marshal.SizeOf(p);
                    IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                    Marshal.StructureToPtr(p, intPtr, true);
                    len = 4;
                    return intPtr.ToPointer();
                    //objIntPtrs[item.index] = intPtr;
                    //lensInts[item.index] = 4;
                }
                else if (jtt == 9)
                {
                    bool p = Convert.ToBoolean(obj.ToString());
                    int nSizeOfPerson = Marshal.SizeOf(p);
                    IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                    Marshal.StructureToPtr(p, intPtr, true);
                    len = 1;
                    return  intPtr.ToPointer();
                    //objIntPtrs[item.index] = intPtr;
                    //lensInts[item.index] = 1;
                }
                else if (jtt == 7)
                {
                    double p = Convert.ToDouble(obj.ToString());
                    int nSizeOfPerson = Marshal.SizeOf(p);
                    IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                    Marshal.StructureToPtr(p, intPtr, true);
                    len = 8;
                    return intPtr.ToPointer();
                    //objIntPtrs[item.index] = intPtr;
                    //lensInts[item.index] = 8;
                }
                else if (jtt == 12)
                {
                    //lensInts[item.index] = 8;
                    len = 8;
                    long p = Convert.ToDateTime(obj.ToString()).ToFileTime();

                    int nSizeOfPerson = Marshal.SizeOf(p);
                    IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                    Marshal.StructureToPtr(p, intPtr, true);
                    return intPtr.ToPointer();
                  //  objIntPtrs[item.index] = intPtr;
                }
                else if (jtt == 8)
                {
                    string str = obj.ToString();
                    len= System.Text.Encoding.Default.GetBytes(str.ToCharArray()).Length;
                    IntPtr p = Marshal.StringToHGlobalAnsi(str);
                     return p.ToPointer();
                  //  objIntPtrs[item.index] = p;
                }
                else
                {
                    byte[] p = GZIP.Compress(utli.TToBytes(obj.ToString()));
                    len = p.Length;
                    IntPtr p1 = utli.Tobytes(p);
                     return p1.ToPointer();
                    //lensInts[item.index] = p.Length;
                }
            }
            catch
            {
                //  Marshal.FreeHGlobal((IntPtr)objs[item.index]);
                return null;
            }
        }
        public static void* getdata(byte jtt, object obj,ref int len)
        {
            try
            {
                //int len = 0;
                if (jtt == 10)
                {
                    return null;
                }
                else if (jtt == 6)
                {
                    int p = Convert.ToInt32(obj.ToString());
                    int nSizeOfPerson = Marshal.SizeOf(p);
                    IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                    Marshal.StructureToPtr(p, intPtr, true);
                    len = 4;
                    return intPtr.ToPointer();
                    //objIntPtrs[item.index] = intPtr;
                    //lensInts[item.index] = 4;
                }
                else if (jtt == 9)
                {
                    bool p = Convert.ToBoolean(obj.ToString());
                    int nSizeOfPerson = Marshal.SizeOf(p);
                    IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                    Marshal.StructureToPtr(p, intPtr, true);
                    len = 1;
                    return intPtr.ToPointer();
                    //objIntPtrs[item.index] = intPtr;
                    //lensInts[item.index] = 1;
                }
                else if (jtt == 7)
                {
                    double p = Convert.ToDouble(obj.ToString());
                    int nSizeOfPerson = Marshal.SizeOf(p);
                    IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                    Marshal.StructureToPtr(p, intPtr, true);
                    len = 8;
                    return intPtr.ToPointer();
                    //objIntPtrs[item.index] = intPtr;
                    //lensInts[item.index] = 8;
                }
                else if (jtt == 12)
                {
                    //lensInts[item.index] = 8;
                    len = 8;
                    long p = Convert.ToDateTime(obj.ToString()).ToFileTime();

                    int nSizeOfPerson = Marshal.SizeOf(p);
                    IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                    Marshal.StructureToPtr(p, intPtr, true);
                    return intPtr.ToPointer();
                    //  objIntPtrs[item.index] = intPtr;
                }
                else if (jtt == 8)
                {
                    string str = obj.ToString();
                    len = System.Text.Encoding.Default.GetBytes(str.ToCharArray()).Length;
                    IntPtr p = Marshal.StringToHGlobalAnsi(str);
                    return p.ToPointer();
                    //  objIntPtrs[item.index] = p;
                }
                else
                {
                    byte[] p = GZIP.Compress(utli.TToBytes(obj.ToString()));
                    len = p.Length;
                    IntPtr p1 = utli.Tobytes(p);
                    return p1.ToPointer();
                    //lensInts[item.index] = p.Length;
                }
            }
            catch
            {
                //  Marshal.FreeHGlobal((IntPtr)objs[item.index]);
                return null;
            }
        }
        
        public static Head[] Gethead(JObject obj)
        {
            Head[] coll = new Head[obj.Count];
            int i = 0;
            foreach (var item in obj)
            {
                if (item.Key != "" && item.Key != null)
                {
                    Head hh = new Head();
                    coll[i] = hh;
                    coll[i].key = item.Key;
                    coll[i].index = i;
                    coll[i].type = (byte)item.Value.Type;
                    i++;
                }
            }

            return coll;
        }

        public static Head[] Gethead(JObject obj, Head[] heads)
        {
            Head[] temp = heads;
            foreach (var item in obj)
            {
                bool bb = false;
                foreach (Head hd in heads)
                {
                    if (item.Key == hd.key)
                    {
                        if (hd.type == 10)
                        {
                            hd.type = (byte)item.Value.Type;
                        }

                        if ((byte)item.Value.Type == 10)
                        {
                            bb = true;
                            break;
                        }

                        if ((byte)item.Value.Type == hd.type)
                        {
                            bb = true;
                            break;
                        }
                        else
                        {
                            throw new Exception("数据类型不匹配。");
                        }

                    }
                }
                if (!bb)
                {
                    temp = new Head[heads.Length + 1];
                    heads.CopyTo(temp, 0);
                    temp[temp.Length - 1] = new Head
                    {
                        key = item.Key,
                        index = temp.Length - 1,
                        type = (byte)item.Value.Type
                    };

                    heads = temp;
                }
            }
            return temp;
        }
    }
}
