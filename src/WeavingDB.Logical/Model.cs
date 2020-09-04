using System;
using System.Collections.Generic;

namespace WeavingDB.Logical
{
    
    public class Liattable
    {
        public List<ListDmode> datas = new List<ListDmode>();
        public Head[] datahead;
        public Dictionary<string, BPTree> tree = new Dictionary<string, BPTree>();
        public bool deleterun = false;
        public IntPtr filedata = IntPtr.Zero;
    }
   
}
