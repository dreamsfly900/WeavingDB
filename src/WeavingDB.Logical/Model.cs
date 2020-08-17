using System;
using System.Collections.Generic;

namespace WeavingDB.Logical
{
    public unsafe class ListDmode
    {
        public void*[] dtable2;
        public IntPtr[] dtable;
        public IntPtr* dtableone;
        public int[] LenInts;
        public long dt = DateTime.Now.ToFileTime();
    }

    public class Liattable
    {
        public List<ListDmode> datas = new List<ListDmode>();
        public Head[] datahead;
        public bool deleterun = false;
    }

    public class Head
    {
        public string key;
        public int index;
        public byte type;//int,float,bool,date,string
    }

    public class Contrastmode
    {
        public int collindex = -99;
        public string[] sscon;
        public string Contrast;
    }

    public class Freedata
    {
        public IntPtr ptr;
        public byte type;

    }
}
