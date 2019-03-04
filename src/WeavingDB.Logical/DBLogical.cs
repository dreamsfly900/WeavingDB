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
using WeavingDB.Logical;

namespace WeavingDBLogical
{
    public unsafe class DBLogical : Logical
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
        /// <summary>
        /// 将JSON转成要插入的指针和与其对应的数据类型标识
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dhead"></param>
        /// <returns></returns>
        public unsafe ListDmode insertintoJson(JObject obj, ref Head[] dhead)
        {

            try
            {
                if (dhead == null)
                {
                    dhead = Gethead(obj);
                }
                else
                {
                    dhead = Gethead(obj, dhead);
                }

                ListDmode ld = new ListDmode();
                // ld.dtable = gethtabledtjson(obj as JObject, dhead);
                int[] lens = new int[0];
                ld.dtable2 = Gethtabledtjsontointptr(obj, dhead, ref lens);
                ld.LenInts = lens;
                return ld;
            }
            catch (Exception e)
            {
                throw new Exception("数据插入有误" + e.Message);
            }
        }

        List<ListDmode> listu;
        byte[] logical; string[] sst; String sqlsst; Head[] dhead; int maxlen = 0;
        bool[] numbb;
        byte[] mtsContrast;
        void*[] mtssscon;
        short[] collindex;
        byte[] hindex;
        int glen = 0;
        /// <summary>
        /// 清理数据表
        /// </summary>
        /// <param name="_listu"></param>
        /// <param name="_dhead"></param>
        public unsafe void cleardata(List<ListDmode> _listu, Head[] _dhead)
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
                        if (_listu[i].dtable2[_dhead[ig].index] == null)
                            continue;
                        lock (_listu[i])
                        {
                            byte type = _dhead[ig].type;
                            IntPtr pp = (IntPtr)_listu[i].dtable2[_dhead[ig].index];


                            _listu[i].dtable2[_dhead[ig].index] = IntPtr.Zero.ToPointer();
                            _listu[i].LenInts[dhead[ig].index] = 0;
                            if (pp != IntPtr.Zero)
                            {
                                Freedata fd = new Freedata();
                                fd.ptr = pp;
                                fd.type = type;
                                allfree.Enqueue(fd);
                            }
                        }


                    }

                _listu.Clear();
            }
        }
        /// <summary>
        /// 有条件的删除表数据
        /// </summary>
        /// <param name="_listu"></param>
        /// <param name="_sqlsst"></param>
        /// <param name="_dhead"></param>
        public unsafe void deletedata(List<ListDmode> _listu, String _sqlsst, Head[] _dhead)
        {
            try
            {
                //listsindex = new List<int>();
                // listutem = new List<void*[]>();
                sqlsst = _sqlsst;
                dhead = _dhead;

                listu = _listu;

                wherelogical();


                delLogicaltiem(_listu.Count, 0, _dhead);
                for (int gi = 0; gi < mtssscon.Length; gi++)
                {
                    void* p = mtssscon[gi];

                    Marshal.FreeHGlobal((IntPtr)p);
                }


            }
            catch (Exception ex)
            { throw ex; }

        }
        /// <summary>
        /// 有条件的更新数据
        /// </summary>
        /// <param name="_listu"></param>
        /// <param name="_sqlsst"></param>
        /// <param name="_dhead"></param>
        /// <param name="job"></param>
        public unsafe void updatedata(List<ListDmode> _listu, String _sqlsst, Head[] _dhead, JObject job)
        {
            try
            {

                //listsindex = new List<int>();
                // listutem = new List<void*[]>();
                sqlsst = _sqlsst;
                if (sqlsst == "")
                {
                    ListDmode dmode; Head[] hhead = new Head[0];


                    for (int i = 0; i < _listu.Count; i++)
                    {
                        if (i < listu.Count && listu[i] != null)
                        {
                            lock (_listu[i])
                            {
                                dmode = insertintoJson(job, ref hhead);
                                for (int ig = 0; ig < _dhead.Length; ig++)
                                {

                                    if (_dhead[ig].index >= listu[i].dtable2.Length)
                                        continue;
                                    byte type = _dhead[ig].type;

                                    for (int igg = 0; igg < hhead.Length; igg++)
                                    {
                                        if (hhead[igg].key == _dhead[ig].key)
                                        {

                                            if (type == hhead[igg].type)
                                            {
                                                IntPtr pp = (IntPtr)listu[i].dtable2[_dhead[ig].index];
                                                listu[i].dtable2[_dhead[ig].index] = IntPtr.Zero.ToPointer();
                                                //listu[i].dtable2[_dhead[ig].index] = IntPtr.Zero.ToPointer();
                                                if (pp != IntPtr.Zero)
                                                {
                                                    Freedata fd = new Freedata();
                                                    fd.ptr = pp;
                                                    fd.type = type;
                                                    allfree.Enqueue(fd);
                                                }
                                                listu[i].dtable2[_dhead[ig].index] = dmode.dtable2[hhead[igg].index];
                                                listu[i].LenInts[_dhead[ig].index] = dmode.LenInts[hhead[igg].index];
                                                dmode.dtable2[hhead[igg].index] = IntPtr.Zero.ToPointer();
                                            }


                                        }
                                    }



                                }
                                for (int igga = 0; igga < hhead.Length; igga++)
                                {
                                    if (hhead[igga].index >= dmode.dtable2.Length)
                                        continue;
                                    IntPtr pp = (IntPtr)dmode.dtable2[hhead[igga].index];
                                    if (pp != IntPtr.Zero)
                                    {
                                        Freedata fd = new Freedata();
                                        fd.ptr = pp;
                                        fd.type = hhead[igga].type;
                                        allfree.Enqueue(fd);
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


                    updataLogicaltiem(_listu.Count, 0, _dhead, job);
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

        public static ConcurrentQueue<Freedata> allfree = new ConcurrentQueue<Freedata>();
        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LocalFree(IntPtr mem);
        /// <summary>
        /// 垃圾回收，彻底从内存中移除数据所占用的空间并释放。
        /// </summary>
        /// <param name="obj"></param>
        public static void freequeue(object obj)
        {
            while (true)
            {
                int count = allfree.Count;
                while (count > 0)
                {
                    if (!allfree.IsEmpty)
                    {
                        Freedata fd = new Freedata();
                        allfree.TryDequeue(out fd);
                        IntPtr pp = fd.ptr;
                        byte type = fd.type;
                        //if (pp != IntPtr.Zero && type != 6 && type != 9 && type != 7 && type != 12 && type != 8)
                        //{

                        //    binaryvoid byv2 = new binaryvoid();
                        //    Marshal.PtrToStructure((IntPtr)pp, byv2);

                        //    LocalFree(byv2.data);
                        //}
                        try
                        {

                            LocalFree(pp);
                            // Marshal.FreeHGlobal(pp);

                        }
                        catch { }
                    }
                    count--;
                }
                System.Threading.Thread.Sleep(500);
            }
        }
        /// <summary>
        /// 移除表链中的null行
        /// </summary>
        /// <param name="oo"></param>
        public static void delnull(object oo)
        {
            try
            {
                Liattable _listu = oo as Liattable;
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
        /// <summary>
        /// 分解SQL语句并压入等待判断的条件中
        /// </summary>
        void wherelogical()
        {
            sst = Sqltolist(sqlsst);
            logical = LogicalSplit(sst);

            {
                glen = 0;
                foreach (string sr in sst)
                {
                    string Contrast = LogicalContrastSplit(sr);
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
                    string Contrast = LogicalContrastSplit(sr);
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
                        if (Contrast == ">like<")
                        {
                            mtsContrast[gglen] = 5;
                        }
                        // mtssscon[gglen] = sscon[1];

                        foreach (Head hd in dhead)
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

        /// <summary>
        /// 有条件的查询表中数据，并返回指针，查询是多线程的，数据是无排序的
        /// </summary>
        /// <param name="_listu"></param>
        /// <param name="_sqlsst"></param>
        /// <param name="_dhead"></param>
        /// <param name="_maxlen"></param>
        /// <returns></returns>
        public unsafe ListDmode[] selecttiem(List<ListDmode> _listu, String _sqlsst, Head[] _dhead, int _maxlen = 100000)
        {

            //listsindex = new List<int>();
            listutem = new ConcurrentQueue<ListDmode>();

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
                        listutem.Enqueue(listu[i]);
                        // listlen.Add(listu[i].LenInts);
                    }
                }

            }
            else
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


            //listutem = new ConcurrentQueue<void*[]>();
            return listutem.ToArray(); ;
        }

        public Hashtable[] viewdata(ListDmode[] objsall, byte order, string ordercol, int indexlen, int viewlen, Head[] datahead)
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
                    Head orhe = null;
                    foreach (Head h in datahead)
                    {
                        if (h.key == ordercol)
                        {
                            orhe = h;
                            break;
                        }
                    }
                    if (orhe == null)
                        return null;


                    objsall = Sort(objsall, orhe, order);

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
                            foreach (Head h in datahead)
                            {
                                if (objsall[i].dtable2.Length > h.index)
                                {
                                    object obj = GetHashtable(h.key, h.type, objsall[i].dtable2[h.index], objsall[i].LenInts[h.index]);
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
            catch
            {
                numbb[index] = true;
            }
            numbb[index] = true;
        }



        unsafe void delLogicaltiem(int listulen, int listindex, Head[] _dhead)
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
                    lock (listu[i])
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
                                        if (mtsContrast[ci] == 5)
                                        {
                                            string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                                            string sconvalue = Marshal.PtrToStringAnsi((IntPtr)mtssscon[ci]);
                                            sconvalue = sconvalue.Replace("%", "(.+)").Replace("_", "(.+){1}");
                                            conbb[bi] = Stringtonosymbol(value, "^" + sconvalue + "$");
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
                                allb = Logicaljudgement(logical, conbb);
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

                                    listu[i].dtable2[_dhead[ig].index] = IntPtr.Zero.ToPointer();

                                    listu[i].LenInts[_dhead[ig].index] = 0;
                                    try
                                    {
                                        if (pp != IntPtr.Zero)
                                        {
                                            Freedata fd = new Freedata();
                                            fd.ptr = pp;
                                            fd.type = type;
                                            allfree.Enqueue(fd);
                                        }


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
        unsafe void updataLogicaltiem(int listulen, int listindex, Head[] _dhead, JObject job)
        {

            // List<void*[]> dellist = new List<void*[]>();
            if (logical.Length < 0)
            {
                return;
            }


            ListDmode dmode; Head[] hhead = new Head[0];

            // dmode = insertintoJson(job, ref hhead);
            bool[] conbb = new bool[logical.Length + 1];
            short coblen = (short)conbb.Length;
            bool allb = false;

            for (int i = listindex; i < listulen; i++)
            {
                if (i < listu.Count && listu[i] != null)
                {
                    lock (listu[i])
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
                                        if (mtsContrast[ci] == 5)
                                        {
                                            string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                                            string sconvalue = Marshal.PtrToStringAnsi((IntPtr)mtssscon[ci]);
                                            sconvalue = sconvalue.Replace("%", "(.+)").Replace("_", "(.+){1}");
                                            conbb[bi] = Stringtonosymbol(value, "^" + sconvalue + "$");

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
                                allb = Logicaljudgement(logical, conbb);
                            if (allb)
                            {
                                //  List<IntPtr> dellist = new List<IntPtr>();

                                dmode = insertintoJson(job, ref hhead);
                                for (int ig = 0; ig < _dhead.Length; ig++)
                                {
                                    byte type = _dhead[ig].type;
                                    if (_dhead[ig].index >= listu[i].dtable2.Length)
                                        continue;
                                    for (int igg = 0; igg < hhead.Length; igg++)
                                    {
                                        if (hhead[igg].key == _dhead[ig].key)
                                        {

                                            if (type == hhead[igg].type)
                                            {

                                                IntPtr pp = (IntPtr)listu[i].dtable2[_dhead[ig].index];

                                                listu[i].dtable2[_dhead[ig].index] = IntPtr.Zero.ToPointer();
                                                if (pp != IntPtr.Zero)
                                                {
                                                    Freedata fd = new Freedata();
                                                    fd.ptr = pp;
                                                    fd.type = type;
                                                    allfree.Enqueue(fd);
                                                }

                                                listu[i].dtable2[_dhead[ig].index] = dmode.dtable2[hhead[igg].index];
                                                listu[i].LenInts[_dhead[ig].index] = dmode.LenInts[hhead[igg].index];
                                                dmode.dtable2[hhead[igg].index] = IntPtr.Zero.ToPointer();


                                            }
                                            //else
                                            //{ throw new Exception("数据列，类型不匹配。"); }

                                        }
                                    }

                                }

                                for (int igga = 0; igga < hhead.Length; igga++)
                                {
                                    IntPtr pp = (IntPtr)dmode.dtable2[hhead[igga].index];
                                    if (pp != IntPtr.Zero)
                                    {
                                        Freedata fd = new Freedata();
                                        fd.ptr = pp;
                                        fd.type = hhead[igga].type;
                                        allfree.Enqueue(fd);
                                    }
                                }



                            }


                        }
                        catch
                        {
                            throw new Exception("不支持的逻辑判断。");
                        }
                    }

                }
            }


            GC.Collect();

            //  System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(del), dellist);
            //   return listutem.ToArray();

        }


        unsafe void Logicaltiem(int listulen, int listindex)
        {


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
                                    if (mtsContrast[ci] == 5)
                                    {
                                        string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                                        string sconvalue = Marshal.PtrToStringAnsi((IntPtr)mtssscon[ci]);
                                        sconvalue = sconvalue.Replace("%", "(.+)").Replace("_", "(.+){1}");
                                        conbb[bi] = Stringtonosymbol(value, "^" + sconvalue + "$");
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
                            allb = Logicaljudgement(logical, conbb);
                        if (allb)
                        {
                            listu[i].dt = DateTime.Now.ToFileTime();
                            //listu[i].dtable
                            listutem.Enqueue(listu[i]);

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
