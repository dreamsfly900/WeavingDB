﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeavingDBLogical
{
  //  public enum Dtype { Dint, Dfloat, Ddouble, Dbyte, DDecimal, Dstring, Ddatetime, BufferedStream };
   
    public unsafe class listDmode
    {
        //public object[] dtable;
        public void* [] dtable2;
        public DateTime dt = DateTime.Now;
    }
    public class liattable
    {
       public   List<listDmode> datas = new List<listDmode>();
        public head [] datahead;
    }
    public class head
    {
        public string  key;
        public int index;
        public byte type;//int,float,bool,date,string
    }
    public class Contrastmode
    {
        public int collindex =-99;
        public string[] sscon;
        public string Contrast;
    }
}
