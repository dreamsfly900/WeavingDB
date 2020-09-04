using System;
using System.Collections.Generic;

namespace WeavingDB.Logical
{
    public class Freedata
    {
        public IntPtr ptr;
        public byte type;

    }
    public unsafe class Contrastmode
    {
        public byte[] mtsContrast;//比较类型
        public void*[] mtssscondata;//比较数据
        public short[] collindex;//列索引
        public byte[] hindex;//数据类型
        public byte[] logical;
        public int[] mtslen;
    }
    public class Liattable
    {
        public List<ListDmode> datas = new List<ListDmode>();
        public Head[] datahead;
        public Dictionary<string, BPTree> tree = new Dictionary<string, BPTree>();
        public bool deleterun = false;
        public IntPtr filedata = IntPtr.Zero;
    }
    public unsafe class ListDmode
    {
        public void*[] dtable2;
        //public IntPtr[] dtable;
        //public IntPtr* dtableone;
        public int[] LenInts;
        public long dt = DateTime.Now.ToFileTime();
    }
    public class Head
    {
        public string key;
        public int index;
        public byte type;//int,float,bool,date,string
    }
}
