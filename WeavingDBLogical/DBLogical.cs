using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeavingDBLogical
{
    public unsafe class DBLogical: logical
    {

        ////
        //// 摘要:
        ////     No token type has been set.
        //None = 0,
        ////
        //// 摘要:
        ////     A JSON object.
        //Object = 1,
        ////
        //// 摘要:
        ////     A JSON array.
        //Array = 2,
        ////
        //// 摘要:
        ////     A JSON constructor.
        //Constructor = 3,
        ////
        //// 摘要:
        ////     A JSON object property.
        //Property = 4,
        ////
        //// 摘要:
        ////     A comment.
        //Comment = 5,
        ////
        //// 摘要:
        ////     An integer value.
        //Integer = 6,
        ////
        //// 摘要:
        ////     A float value.
        //Float = 7,
        ////
        //// 摘要:
        ////     A string value.
        //String = 8,
        ////
        //// 摘要:
        ////     A boolean value.
        //Boolean = 9,
        ////
        //// 摘要:
        ////     A null value.
        //Null = 10,
        ////
        //// 摘要:
        ////     An undefined value.
        //Undefined = 11,
        ////
        //// 摘要:
        ////     A date value.
        //Date = 12,
        ////
        //// 摘要:
        ////     A raw JSON value.
        //Raw = 13,
        ////
        //// 摘要:
        ////     A collection of bytes value.
        //Bytes = 14,
        ////
        //// 摘要:
        ////     A Guid value.
        //Guid = 15,
        ////
        //// 摘要:
        ////     A Uri value.
        //Uri = 16,
        ////
        //// 摘要:
        ////     A TimeSpan value.
        //TimeSpan = 17

      public unsafe listDmode insertintoJson(JObject obj,ref head [] dhead)
        {
            
            try
            {
                if (dhead == null)
                {
                    dhead = gethead(obj);
                }
                else
                {
                    dhead = gethead(obj, dhead);
                }
                listDmode ld = new listDmode();
               // ld.dtable = gethtabledtjson(obj as JObject, dhead);
                ld.dtable2= gethtabledtjsontointptr(obj, dhead);
                return ld;
            }
            catch(Exception e) { throw new Exception("数据插入有误"+ e.Message); }
        }

        List<listDmode> listu;
        byte[] logical; string[] sst; String sqlsst; head[] dhead;int maxlen = 0;
        bool[] numbb;
        byte[] mtsContrast;
        void*[] mtssscon;
        short[] collindex;
        byte[] hindex;
        int glen = 0;
        public unsafe void cleardata(List<listDmode> _listu, head[] _dhead)
        {
            lock (_listu)
            {
                for (int i = 0; i < _listu.Count; i++)
                    for (int ig = 0; ig < _dhead.Length; ig++)
                    {
                     
                        if (_listu[i] == null)
                            continue;
                        if (_dhead[ig].index >= _listu[i].dtable2.Length)
                            continue;
                        if(_listu[i].dtable2[_dhead[ig].index]==null)
                            continue;
                        byte type = _dhead[ig].type;
                        IntPtr pp=(IntPtr)_listu[i].dtable2[_dhead[ig].index];
                        if(pp== IntPtr.Zero)
                            continue;
                        if (type != 6 && type != 9 && type != 7 && type != 12 && type != 8)
                        {

                            binaryvoid byv2 = new binaryvoid();
                            Marshal.PtrToStructure((IntPtr)pp, byv2);
                            
                              Marshal.FreeHGlobal(byv2.data);
                        }
                        try
                        {

                            Marshal.FreeHGlobal(pp);
                        }
                        catch { }
                         
                     
                    }

                _listu.Clear();
            }
        }

         public unsafe void deletedata(List<listDmode> _listu, String _sqlsst, head[] _dhead)
        {
            try
            {
                //listsindex = new List<int>();
               // listutem = new List<void*[]>();
                sqlsst = _sqlsst;
                dhead = _dhead;
        
                listu = _listu;

                wherelogical();

           
                delLogicaltiem(_listu.Count,0, _dhead);
                for (int gi = 0; gi < mtssscon.Length; gi++)
                {
                    void* p = mtssscon[gi];

                    Marshal.FreeHGlobal((IntPtr)p);
                }

                
            }
            catch (Exception ex)
            { throw ex; }

        }
        public unsafe void updatedata(List<listDmode> _listu, String _sqlsst, head[] _dhead,JObject job)
        {
            try
            {
                
                //listsindex = new List<int>();
                // listutem = new List<void*[]>();
                sqlsst = _sqlsst;
                if (sqlsst == "")
                {
                    listDmode dmode; head[] hhead=new head[0];
                    
                     dmode = insertintoJson(job, ref hhead);
                    for (int i = 0; i < _listu.Count; i++)
                    {
                        if (i < listu.Count && listu[i] != null)
                        {
                            for (int ig = 0; ig < _dhead.Length; ig++)
                            {
                               
                                if (_dhead[ig].index >= listu[i].dtable2.Length)
                                    continue;
                                byte type = _dhead[ig].type;
                                for (int igg = 0; igg < hhead.Length; igg++)
                                {
                                    if (hhead[igg].key == _dhead[ig].key)
                                    {
                                        bool bba = false;
                                        if (type == hhead[igg].type)
                                        {
                                            IntPtr pp = (IntPtr)listu[i].dtable2[_dhead[ig].index];
                                            if (type != 6 && type != 9 && type != 7 && type != 12 && type != 8)
                                            {

                                                binaryvoid byv2 = new binaryvoid();
                                                Marshal.PtrToStructure((IntPtr)pp, byv2);

                                                Marshal.FreeHGlobal(byv2.data);
                                            }

                                            Marshal.FreeHGlobal(pp);
                                            listu[i].dtable2[_dhead[ig].index] = dmode.dtable2[hhead[igg].index];
                                            bba = true;
                                        }
                                        else
                                        { throw new Exception("数据列，类型不匹配。"); }
                                        if(bba)
                                          dmode = insertintoJson(job, ref hhead);
                                    }
                                }

                            }
                        }
                    }
                }
                else
                {
                    dhead = _dhead;

                    listu = _listu;

                    wherelogical();


                    updataLogicaltiem(_listu.Count, 0, _dhead,job);
                    for (int gi = 0; gi < mtssscon.Length; gi++)
                    {
                        void* p = mtssscon[gi];

                        Marshal.FreeHGlobal((IntPtr)p);
                    }

                }
            }
            catch (Exception ex)
            { throw ex; }

        }
        public static void delnull(object oo)
        {
            try
            {
                liattable _listu = oo as liattable;
                for (int i = 0; i < _listu.datas.Count; i++)
                    if (_listu.datas.Count > i && _listu.datas[i] == null)
                    {
                        _listu.datas.RemoveAt(i);

                        i = i - 1;
                    }
                _listu.deleterun = false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        void wherelogical()
        {
            sst = sqltolist(sqlsst);
            logical = logicalSplit(sst);

            {
                glen = 0;
                foreach (string sr in sst)
                {
                    string Contrast = logicalContrastSplit(sr);
                    String[] sscon = sr.Split(new string[] { Contrast }, StringSplitOptions.None);

                    if (sscon.Length == 2)
                    {
                        glen++;
                    }
                }
                mtsContrast = new byte[glen];
                mtssscon = new void*[glen];
                collindex = new short[glen];
                hindex = new byte[glen];

            }
            if (listu.Count > 0)
            {
                //  gethtable(listu[0]);
                int gglen = 0;
                int ss = 0;
                foreach (string sr in sst)
                {
                    string Contrast = logicalContrastSplit(sr);
                    String[] sscon = sr.Split(new string[] { Contrast }, StringSplitOptions.None);

                    if (sscon.Length == 2)
                    {
                        Contrastmode mts = new Contrastmode();
                        //  mtsContrast[gglen] = Contrast;
                        if (Contrast == ">=")
                        {
                            mtsContrast[gglen] = 0;
                        }
                        if (Contrast == "<=")
                        {
                            mtsContrast[gglen] = 1;
                        }
                        if (Contrast == "==")
                        {
                            mtsContrast[gglen] = 2;
                        }
                        if (Contrast == ">")
                        {
                            mtsContrast[gglen] = 3;
                        }
                        if (Contrast == "<")
                        {
                            mtsContrast[gglen] = 4;
                        }
                        // mtssscon[gglen] = sscon[1];

                        foreach (head hd in dhead)
                        {
                            if (hd.key == sscon[0])
                            {
                                hindex[gglen] = hd.type;
                                collindex[gglen] = (short)hd.index;
                                unsafe
                                {
                                    if (hindex[gglen] == 6)
                                    {
                                        int p = Convert.ToInt32(sscon[1]);
                                        int nSizeOfPerson = Marshal.SizeOf(p);
                                        IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                                        Marshal.StructureToPtr(p, intPtr, true);
                                        mtssscon[gglen] = intPtr.ToPointer();
                                    }
                                    else if (hindex[gglen] == 9)
                                    {
                                        bool p = Convert.ToBoolean(sscon[1]);
                                        int nSizeOfPerson = Marshal.SizeOf(p);
                                        IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                                        Marshal.StructureToPtr(p, intPtr, true);
                                        mtssscon[gglen] = intPtr.ToPointer();

                                    }
                                    else if (hindex[gglen] == 7)
                                    {

                                        double p = Convert.ToDouble(sscon[1]);
                                        int nSizeOfPerson = Marshal.SizeOf(p);
                                        IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                                        Marshal.StructureToPtr(p, intPtr, true);
                                        mtssscon[gglen] = intPtr.ToPointer();



                                    }
                                    else if (hindex[gglen] == 12)
                                    {
                                        sscon[1] = Stringtonosymbol(sscon[1]);
                                        //sst[ss + 1] = Stringtonosymbol(sst[ss + 1]);
                                        long p = Convert.ToDateTime(sscon[1]).ToFileTime();
                                        int nSizeOfPerson = Marshal.SizeOf(p);
                                        IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                                        Marshal.StructureToPtr(p, intPtr, true);
                                        mtssscon[gglen] = intPtr.ToPointer();

                                    }
                                    else if (hindex[gglen] == 8)
                                    {
                                        sscon[1] = Stringtonosymbol(sscon[1]);
                                        // char[] p = ((String)objc).ToCharArray().;
                                        char* p = (char*)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(sscon[1]).ToPointer();
                                        mtssscon[gglen] = p;
                                    }
                                    else
                                    {
                                        throw new Exception("不支持的逻辑判断");
                                    }

                                }
                                break;
                            }

                        }

                        gglen++;
                    }
                    ss++;
                }

            }

        }
        List<int> listsindex = new List<int>();
        public unsafe void*[][] selecttiem(List<listDmode> _listu, String _sqlsst, head[] _dhead, int _maxlen= 100000)
        {
            
                //listsindex = new List<int>();
                listutem = new ConcurrentQueue<void*[]>();
           
                sqlsst = _sqlsst;
                dhead = _dhead;
                maxlen = _maxlen;
                listu = _listu;
            if (_sqlsst == "")
            {
                int len = _listu.Count;
                for (int i = 0; i < len; i++)
                {
                    if (i < listu.Count && listu[i] != null)
                    {
                        listu[i].dt = DateTime.Now.ToFileTime();
                        //listu[i].dtable
                        listutem.Enqueue(listu[i].dtable2);
                    }
                }

            } else
            {
                wherelogical();
                int num = listu.Count % maxlen == 0 ? listu.Count / maxlen : (listu.Count / maxlen) + 1;
                if (listu.Count < maxlen)
                    num = 1;
                numbb = new bool[num];
                for (int ih = 0; ih < num; ih++)
                {
                    numbb[ih] = false;
                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(gg), ih);
                    //Logicaltiem((ih + 1) * maxlen, ih * maxlen);

                }
                try
                {

                    bool nbb = false;
                    while (!nbb)
                    {
                        nbb = false;
                        int nbi = 0;
                        if ((num - 1) == 0)
                        {
                            if (numbb[0]) break;
                        }
                        else
                            while (nbi < (num - 1))
                            {
                                nbb = numbb[nbi] && numbb[nbi + 1];
                                nbi++;
                            }
                    }

                    for (int gi = 0; gi < mtssscon.Length; gi++)
                    {
                        void* p = mtssscon[gi];

                        Marshal.FreeHGlobal((IntPtr)p);
                    }
                }
                // void*[][] listall = new void*[0][listsindex.Count];
                catch (Exception ex)
                { throw ex; }
            }
           
            void*[][] tempp= listutem.ToArray();
            //listutem = new ConcurrentQueue<void*[]>();
            return tempp;
        }

        public  Hashtable[] viewdata(void*[][] objsall, byte order,string ordercol,int indexlen, int viewlen, head[] datahead)
        {
            List<Hashtable> alllist = new List<Hashtable>();
            Hashtable[] temphtt = alllist.ToArray();
            try
            {
                if (ordercol == "")
                {

                }
                else
                {
                    head orhe = null;
                    foreach (head h in datahead)
                    {
                        if (h.key == ordercol)
                        {
                            orhe = h;
                            break;
                        }
                    }
                    if (orhe == null)
                        return null;


                    objsall = sort(objsall, orhe, order);

                }

                if (viewlen <= 0)
                    viewlen = objsall.Length;
                if ((indexlen * viewlen) >= objsall.Length)
                {
                    alllist = new List<Hashtable>();
                    return temphtt;
                }
                int count = indexlen * viewlen;
                int lens = ((indexlen + 1) * viewlen) > objsall.Length ? objsall.Length : ((indexlen + 1) * viewlen);
                lens = lens - (indexlen * viewlen);
                for (int i = count; i < count + lens; i++)
                {
                    try
                    {
                        if (objsall[i] != null)
                        {
                            Hashtable ht = new Hashtable();
                            foreach (head h in datahead)
                            {
                                if (objsall[i].Length > h.index)
                                {
                                    object obj = getHashtable(h.key, h.type, objsall[i][h.index]);
                                    ht.Add(h.key, obj);
                                }
                            }
                            alllist.Add(ht);
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
                 temphtt = alllist.ToArray();
                alllist = new List<Hashtable>();
                return temphtt;
            }
            catch
            {
                return null;
            }
            finally
            {
                  
            }
           

        

        }
  
        void gg(object objmode)
        {
            int index = (int)objmode;
            try
            {
                Logicaltiem((index + 1) * maxlen, index * maxlen);

            }
            catch  {
                numbb[index] = true;
            }
            numbb[index] = true;
        }

      

        unsafe void delLogicaltiem(int listulen, int listindex, head[] _dhead)
        {

           // List<void*[]> dellist = new List<void*[]>();
            if (logical.Length < 0)
            {
                return;
            }
            


            bool[] conbb = new bool[logical.Length + 1];
            short coblen = (short)conbb.Length;
            bool allb = false;

            for (int i = listindex; i < listulen; i++)
            {
               
                if (i < listu.Count && listu[i] != null)
                {

                    byte bi = 0;

                    int llen = logical.Length;
                    allb = false;
                    for (int ci = 0; ci < glen; ci++)
                    {
                        try
                        {
                            if (collindex[ci] != -99)
                            {
                                //  object value = (listu[i].dtable[collindex[ci]]);
                              
                                void* p1 = listu[i].dtable2[collindex[ci]];
                                if (p1 == null)
                                    break;
                                conbb[bi] = false;

                                if (hindex[ci] == 6)
                                {
                                    int value = (int)(*(int*)p1);
                                    int sconvalue = (int)(*(int*)mtssscon[ci]);
                                    if (mtsContrast[ci] == 0)
                                    {

                                        conbb[bi] = (value) >= sconvalue;
                                    }
                                    if (mtsContrast[ci] == 1)
                                    {
                                        conbb[bi] = (value) <= sconvalue;
                                    }
                                    if (mtsContrast[ci] == 2)
                                    {
                                        conbb[bi] = sconvalue == (value);
                                    }
                                    if (mtsContrast[ci] == 3)
                                    {
                                        conbb[bi] = (value) > sconvalue;
                                    }
                                    if (mtsContrast[ci] == 4)
                                    {
                                        conbb[bi] = (value) < sconvalue;
                                    }

                                }
                                else if (hindex[ci] == 9)
                                {
                                    bool value = (bool)(*(bool*)p1);
                                    bool sconvalue = (bool)(*(bool*)mtssscon[ci]);
                                    if (mtsContrast[ci] == 2)
                                    {
                                        conbb[bi] = sconvalue == (value);
                                    }
                                }
                                else if (hindex[ci] == 7)
                                {
                                    double value = (double)(*(double*)p1);
                                    double sconvalue = (double)(*(double*)mtssscon[ci]);
                                    if (mtsContrast[ci] == 0)
                                    {
                                        conbb[bi] = (value) >= sconvalue;
                                    }
                                    if (mtsContrast[ci] == 1)
                                    {
                                        conbb[bi] = (value) <= sconvalue;
                                    }
                                    if (mtsContrast[ci] == 2)
                                    {
                                        conbb[bi] = sconvalue == (value);
                                    }
                                    if (mtsContrast[ci] == 3)
                                    {
                                        conbb[bi] = (value) > sconvalue;
                                    }
                                    if (mtsContrast[ci] == 4)
                                    {
                                        conbb[bi] = (value) < sconvalue;
                                    }

                                }
                                else if (hindex[ci] == 12)
                                {

                                    // conbb[bi] = Contrast<DateTime>(Convert.ToDateTime(st), Convert.ToDateTime(value), mtsContrast[ci]);
                                    long value = (long)(*(long*)p1);
                                    long sconvalue = (long)(*(long*)mtssscon[ci]);
                                    if (mtsContrast[ci] == 0)
                                    {
                                        conbb[bi] = value >= sconvalue;
                                    }
                                    if (mtsContrast[ci] == 1)
                                    {
                                        conbb[bi] = value <= sconvalue;
                                    }
                                    if (mtsContrast[ci] == 2)
                                    {
                                        conbb[bi] = value == sconvalue;
                                    }
                                    if (mtsContrast[ci] == 3)
                                    {
                                        conbb[bi] = (value) > sconvalue;
                                    }
                                    if (mtsContrast[ci] == 4)
                                    {
                                        conbb[bi] = (value) < sconvalue;
                                    }
                                }
                                else if (hindex[ci] == 8)
                                {


                                    if (mtsContrast[ci] == 2)
                                    {
                                        string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                                        string sconvalue = Marshal.PtrToStringAnsi((IntPtr)mtssscon[ci]);
                                        conbb[bi] = sconvalue == (value);
                                    }

                                }
                                else
                                {
                                    throw new Exception("不支持的逻辑判断。");
                                }
                                bi++;
                            }

                        }
                        catch { break; }
                    }
                    try
                    {

                       
                        if (coblen == 1)
                            allb = conbb[0];
                        else
                            allb = logicaljudgement(logical, conbb);
                        if (allb)
                        {
                            //  List<IntPtr> dellist = new List<IntPtr>();
                            for (int ig = 0; ig < _dhead.Length; ig++)
                            {
                             
                                if (_dhead[ig].index >= listu[i].dtable2.Length)
                                    continue;
                                byte type = _dhead[ig].type;
                               
                                if (listu[i] == null)
                                    break;
                                if (listu[i].dtable2[_dhead[ig].index] == null)
                                    continue;
                                 IntPtr pp = (IntPtr)listu[i].dtable2[_dhead[ig].index];
                                

                                
                                if (type != 6 && type != 9 && type != 7 && type != 12 && type != 8)
                                {

                                    binaryvoid byv2 = new binaryvoid();
                                    Marshal.PtrToStructure((IntPtr)pp, byv2);

                                    Marshal.FreeHGlobal(byv2.data);
                                }
                                try
                                {
                                    Marshal.FreeHGlobal(pp);
                                    pp = IntPtr.Zero;
                                }
                                catch { }
                              
                              

                            }
                         //   dellist.Add(listu[i].dtable2);

                            //listu.RemoveAt(i);
                            listu[i] = null;
                         //   i = i - 1;
                            // listu[i].dt = DateTime.Now;
                            //listu[i].dtable
                            //listutem.Add(listu[i].dtable2);
                            // listsindex.Add(i);


                        }


                    }
                    catch
                    {
                        throw new Exception("不支持的逻辑判断。");
                    }
                    //if (oo != null)
                    //{
                    //    listutem.Add(oo);
                    //}
                }
            }


            GC.Collect();

            //  System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(del), dellist);
            //   return listutem.ToArray();

        }
        unsafe void updataLogicaltiem(int listulen, int listindex, head[] _dhead,JObject job)
        {

            // List<void*[]> dellist = new List<void*[]>();
            if (logical.Length < 0)
            {
                return;
            }
            

            listDmode dmode; head[] hhead = new head[0];

            dmode = insertintoJson(job, ref hhead);
            bool[] conbb = new bool[logical.Length + 1];
            short coblen = (short)conbb.Length;
            bool allb = false;

            for (int i = listindex; i < listulen; i++)
            {
                if (i < listu.Count && listu[i] != null)
                {

                    byte bi = 0;

                    int llen = logical.Length;
                    allb = false;
                    for (int ci = 0; ci < glen; ci++)
                    {
                        try { 

                        if (collindex[ci] != -99)
                        {
                            //  object value = (listu[i].dtable[collindex[ci]]);
                            void* p1 = listu[i].dtable2[collindex[ci]];
                            if (p1 == null)
                                break;
                            conbb[bi] = false;

                            if (hindex[ci] == 6)
                            {
                                int value = (int)(*(int*)p1);
                                int sconvalue = (int)(*(int*)mtssscon[ci]);
                                if (mtsContrast[ci] == 0)
                                {

                                    conbb[bi] = (value) >= sconvalue;
                                }
                                if (mtsContrast[ci] == 1)
                                {
                                    conbb[bi] = (value) <= sconvalue;
                                }
                                if (mtsContrast[ci] == 2)
                                {
                                    conbb[bi] = sconvalue == (value);
                                }
                                if (mtsContrast[ci] == 3)
                                {
                                    conbb[bi] = (value) > sconvalue;
                                }
                                if (mtsContrast[ci] == 4)
                                {
                                    conbb[bi] = (value) < sconvalue;
                                }

                            }
                            else if (hindex[ci] == 9)
                            {
                                bool value = (bool)(*(bool*)p1);
                                bool sconvalue = (bool)(*(bool*)mtssscon[ci]);
                                if (mtsContrast[ci] == 2)
                                {
                                    conbb[bi] = sconvalue == (value);
                                }
                            }
                            else if (hindex[ci] == 7)
                            {
                                double value = (double)(*(double*)p1);
                                double sconvalue = (double)(*(double*)mtssscon[ci]);
                                if (mtsContrast[ci] == 0)
                                {
                                    conbb[bi] = (value) >= sconvalue;
                                }
                                if (mtsContrast[ci] == 1)
                                {
                                    conbb[bi] = (value) <= sconvalue;
                                }
                                if (mtsContrast[ci] == 2)
                                {
                                    conbb[bi] = sconvalue == (value);
                                }
                                if (mtsContrast[ci] == 3)
                                {
                                    conbb[bi] = (value) > sconvalue;
                                }
                                if (mtsContrast[ci] == 4)
                                {
                                    conbb[bi] = (value) < sconvalue;
                                }

                            }
                            else if (hindex[ci] == 12)
                            {

                                // conbb[bi] = Contrast<DateTime>(Convert.ToDateTime(st), Convert.ToDateTime(value), mtsContrast[ci]);
                                long value = (long)(*(long*)p1);
                                long sconvalue = (long)(*(long*)mtssscon[ci]);
                                if (mtsContrast[ci] == 0)
                                {
                                    conbb[bi] = value >= sconvalue;
                                }
                                if (mtsContrast[ci] == 1)
                                {
                                    conbb[bi] = value <= sconvalue;
                                }
                                if (mtsContrast[ci] == 2)
                                {
                                    conbb[bi] = value == sconvalue;
                                }
                                if (mtsContrast[ci] == 3)
                                {
                                    conbb[bi] = (value) > sconvalue;
                                }
                                if (mtsContrast[ci] == 4)
                                {
                                    conbb[bi] = (value) < sconvalue;
                                }
                            }
                            else if (hindex[ci] == 8)
                            {


                                if (mtsContrast[ci] == 2)
                                {
                                    string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                                    string sconvalue = Marshal.PtrToStringAnsi((IntPtr)mtssscon[ci]);
                                    conbb[bi] = sconvalue == (value);
                                }

                            }
                            else
                            {
                                throw new Exception("不支持的逻辑判断。");
                            }
                            bi++;
                        }

                        }
                        catch { break; }
                    }
                    try
                    {


                        if (coblen == 1)
                            allb = conbb[0];
                        else
                            allb = logicaljudgement(logical, conbb);
                        if (allb)
                        {
                            //  List<IntPtr> dellist = new List<IntPtr>();
                            for (int ig = 0; ig < _dhead.Length; ig++)
                            {
                                byte type = _dhead[ig].type;
                                if (_dhead[ig].index >= listu[i].dtable2.Length)
                                    continue;
                                for (int igg = 0; igg < hhead.Length; igg++)
                                {
                                    if (hhead[igg].key == _dhead[ig].key)
                                    {
                                        bool bba = false;
                                        if (type == hhead[igg].type)
                                        {
                                          
                                            IntPtr pp = (IntPtr)listu[i].dtable2[_dhead[ig].index];
                                            if (type != 6 && type != 9 && type != 7 && type != 12 && type != 8)
                                            {

                                                binaryvoid byv2 = new binaryvoid();
                                                Marshal.PtrToStructure((IntPtr)pp, byv2);

                                                Marshal.FreeHGlobal(byv2.data);
                                            }
                                           // void* gg = dmode.dtable2[hhead[igg].index];
                                           // if ()
                                            Marshal.FreeHGlobal(pp);
                                            listu[i].dtable2[_dhead[ig].index] = dmode.dtable2[hhead[igg].index];
                                        

                                            bba = true;
                                        }
                                        else
                                        { throw new Exception("数据列，类型不匹配。"); }
                                        if (bba)
                                            dmode = insertintoJson(job, ref hhead);
                                    }
                                }

                            }
                          
                        



                        }


                    }
                    catch
                    {
                        throw new Exception("不支持的逻辑判断。");
                    }
                    //if (oo != null)
                    //{
                    //    listutem.Add(oo);
                    //}
                }
            }


            GC.Collect();

            //  System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(del), dellist);
            //   return listutem.ToArray();

        }

        //void del(object oo)
        //{
        //    try
        //    {
        //        List<void*[]> dellist = oo as List<void*[]>;

        //        for (int ig = 0; ig < dellist.Count; ig++)
        //            for (int ig2 = 0; ig2 < dellist[ig].Length; ig2++)
        //                Marshal.FreeHGlobal((IntPtr)dellist[ig][ig2]);
        //        //   listu.Remove(ld);
        //        dellist = null;
        //        GC.Collect();
        //    }
        //    catch(Exception e)
        //    {
        //        throw e;
        //    }
        //}
        unsafe void Logicaltiem(int listulen,int listindex)
        {
           
         
            if (logical.Length < 0)
            {
                return ;
            }
         
          
          
         
            bool[] conbb = new bool[logical.Length + 1];
            short coblen = (short)conbb.Length;
            bool allb = false;
          
            for (int i = listindex; i < listulen; i++)
            {
                if (i < listu.Count && listu[i] != null)
                {

                    byte bi = 0;

                    int llen = logical.Length;
                    allb = false;
                    for (int ci = 0; ci < glen; ci++)
                    {

                        try
                        {
                            if (collindex[ci] != -99)
                            {
                                //  object value = (listu[i].dtable[collindex[ci]]);
                                void* p1 = listu[i].dtable2[collindex[ci]];
                                if (p1 == null)
                                    break;
                                conbb[bi] = false;

                                if (hindex[ci] == 6)
                                {
                                    int value = (int)(*(int*)p1);
                                    int sconvalue = (int)(*(int*)mtssscon[ci]);
                                    if (mtsContrast[ci] == 0)
                                    {

                                        conbb[bi] = (value) >= sconvalue;
                                    }
                                    if (mtsContrast[ci] == 1)
                                    {
                                        conbb[bi] = (value) <= sconvalue;
                                    }
                                    if (mtsContrast[ci] == 2)
                                    {
                                        conbb[bi] = sconvalue == (value);
                                    }
                                    if (mtsContrast[ci] == 3)
                                    {
                                        conbb[bi] = (value) > sconvalue;
                                    }
                                    if (mtsContrast[ci] == 4)
                                    {
                                        conbb[bi] = (value) < sconvalue;
                                    }

                                }
                                else if (hindex[ci] == 9)
                                {
                                    bool value = (bool)(*(bool*)p1);
                                    bool sconvalue = (bool)(*(bool*)mtssscon[ci]);
                                    if (mtsContrast[ci] == 2)
                                    {
                                        conbb[bi] = sconvalue == (value);
                                    }
                                }
                                else if (hindex[ci] == 7)
                                {
                                    double value = (double)(*(double*)p1);
                                    double sconvalue = (double)(*(double*)mtssscon[ci]);
                                    if (mtsContrast[ci] == 0)
                                    {
                                        conbb[bi] = (value) >= sconvalue;
                                    }
                                    if (mtsContrast[ci] == 1)
                                    {
                                        conbb[bi] = (value) <= sconvalue;
                                    }
                                    if (mtsContrast[ci] == 2)
                                    {
                                        conbb[bi] = sconvalue == (value);
                                    }
                                    if (mtsContrast[ci] == 3)
                                    {
                                        conbb[bi] = (value) > sconvalue;
                                    }
                                    if (mtsContrast[ci] == 4)
                                    {
                                        conbb[bi] = (value) < sconvalue;
                                    }

                                }
                                else if (hindex[ci] == 12)
                                {

                                    // conbb[bi] = Contrast<DateTime>(Convert.ToDateTime(st), Convert.ToDateTime(value), mtsContrast[ci]);
                                    long value = (long)(*(long*)p1);
                                    long sconvalue = (long)(*(long*)mtssscon[ci]);
                                    if (mtsContrast[ci] == 0)
                                    {
                                        conbb[bi] = value >= sconvalue;
                                    }
                                    if (mtsContrast[ci] == 1)
                                    {
                                        conbb[bi] = value <= sconvalue;
                                    }
                                    if (mtsContrast[ci] == 2)
                                    {
                                        conbb[bi] = value == sconvalue;
                                    }
                                    if (mtsContrast[ci] == 3)
                                    {
                                        conbb[bi] = (value) > sconvalue;
                                    }
                                    if (mtsContrast[ci] == 4)
                                    {
                                        conbb[bi] = (value) < sconvalue;
                                    }
                                }
                                else if (hindex[ci] == 8)
                                {


                                    if (mtsContrast[ci] == 2)
                                    {
                                        string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                                        string sconvalue = Marshal.PtrToStringAnsi((IntPtr)mtssscon[ci]);
                                        conbb[bi] = sconvalue == (value);
                                    }

                                }
                                else
                                {
                                    throw new Exception("不支持的逻辑判断。");
                                }
                                bi++;
                            }
                        }
                        catch { break; }

                    }
                    try
                    {


                        if (coblen == 1)
                            allb = conbb[0];
                        else
                            allb = logicaljudgement(logical, conbb);
                        if (allb)
                        {
                            listu[i].dt = DateTime.Now.ToFileTime();
                            //listu[i].dtable
                            listutem.Enqueue(listu[i].dtable2);
                           // listsindex.Add(i);


                        }


                    }
                    catch
                    {
                        throw new Exception("不支持的逻辑判断。");
                    }
                    //if (oo != null)
                    //{
                    //    listutem.Add(oo);
                    //}
                }
            }


           

         //   return listutem.ToArray();

        }
     
      
    }
}
