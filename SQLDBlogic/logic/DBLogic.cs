using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using WeavingDB.Logical;

namespace SQLDBlogic.logic
{
    public unsafe  class DBLogic
    {

        #region 数据插入
        /// <summary>
        /// 建立索引
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ld"></param>
        /// <param name="dhead"></param>
        /// <param name="trees"></param>
        public unsafe void insertintoIndex(JObject obj,ListDmode ld, Head[] dhead, ref Dictionary<string, BPTree> trees)
        {
            for (int i = 0; i < ld.dtable2.Length; i++)
            {
                int len = 0;
                void* key = DBDataHead.getdata(dhead[i].type, obj[dhead[i].key], ref len);
                if (trees.ContainsKey(dhead[i].key))
                {

                    trees[dhead[i].key].insert(trees[dhead[i].key].root, key, ld, dhead[i].type);
                }
                else
                {
                    BPTree tree = new BPTree(4);

                    tree.insert(tree.root, key, ld, dhead[i].type);

                    trees.Add(dhead[i].key, tree);
                }
            }



        }
        [DllImport("kernel32", SetLastError = true)]
        static extern IntPtr LocalFree(IntPtr mem);
        ///// <summary>
        ///// 垃圾回收，彻底从内存中移除数据所占用的空间并释放。
        ///// </summary>
        ///// <param name="obj"></param>
        //void freequeue(object obj)
        //{
        //    //     while (true)
        //    {
        //        if (obj == null)
        //            return;
        //        ConcurrentQueue<Freedata> allfree = obj as ConcurrentQueue<Freedata>;
        //        int count = allfree.Count;

        //        while (!allfree.IsEmpty)
        //        {
        //            Freedata fd = new Freedata();
        //            allfree.TryDequeue(out fd);
        //            IntPtr pp = fd.ptr;
        //            byte type = fd.type;
                   
        //            try
        //            {

        //                //LocalFree(pp);
        //                if (pp != IntPtr.Zero)
        //                {

        //                    LocalFree(pp);
        //                }
        //                else 
        //                { }

        //            }
        //            catch (Exception e)
        //            {

        //            }
        //        }
                 
        //       // System.Threading.Thread.Sleep(500);
        //    }
        //}
        /// <summary>
        /// 数据插入到内存
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dhead"></param>
        /// <param name="trees"></param>
        /// <returns></returns>
        public unsafe ListDmode insertintoJson(JObject obj, ref Head[] dhead)
        {

            try
            {
                if (dhead == null)
                {
                    dhead = DBDataHead.Gethead(obj);
                }
                else
                {
                    dhead = DBDataHead.Gethead(obj, dhead);
                }

                ListDmode ld = new ListDmode();
                // ld.dtable = gethtabledtjson(obj as JObject, dhead);
                IntPtr[] intps = null;
                int[] lens = new int[0];
                ld.dtable2 = DBDataHead.Gethtabledtjsontointptr(obj, dhead, ref lens, ref intps);
                ld.LenInts = lens;
           

                return ld;
            }
            catch (Exception e)
            {
                throw new Exception("数据插入有误" + e.Message);
            }
        }

        #endregion

        ConcurrentQueue<Freedata> allfree = new ConcurrentQueue<Freedata>();
        #region 数据删除
        public unsafe int deletedata(List<ListDmode> _listu, String sqlsst, Head[] _dhead, Liattable ltable)
        {
            ConcurrentQueue<Freedata> allfree = null;
            var mylist = new ListDmode[0];
            if (sqlsst != "")
            {
                var list = selecttiem(_listu, sqlsst, _dhead, ltable);

                mylist = list;
            }
            else
            {

                mylist = _listu.ToArray();
            }
            if (mylist == null)
                return 0;
            int count = mylist.Length;
            if (count > 0)
            {
                lock (_listu)
                {
                    allfree = delete(mylist, _dhead, ltable);
                    int i = 0;

                    while (i < _listu.Count)
                    {
                        if (_listu[i].dtable2 == null)
                        {
                            _listu.RemoveAt(i);
                        }
                        else
                        {
                            i++;
                        }


                    }
                }
                //freequeue(allfree);
                //   System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(freequeue), allfree);
                return count;
            }
            else
                return 0;
        }
        ConcurrentQueue<Freedata> delete(ListDmode[] _listu, Head[] _dhead, Liattable ltable)
        {
            int count = 0;

            for (int i = 0; i < _listu.Length; i++)
            {
                if (i < _listu.Length && _listu[i] != null)
                {
                    lock (_listu[i])
                    {
                        if (_listu[i].dtable2 == null)
                            continue;
                        for (int ig = 0; ig < _dhead.Length; ig++)
                        {
                           
                            IntPtr pp = (IntPtr)_listu[i].dtable2[ig];
                            byte type = _dhead[ig].type;
                            ltable.tree[_dhead[ig].key].searchremove(_listu[i].dtable2[ig], type, ig);
                            if (pp != IntPtr.Zero)
                            {
                                count++;

                                Marshal.FreeHGlobal(pp);
                                //Freedata fd = new Freedata();
                                //fd.ptr = pp;
                                //fd.type = type;
                                //allfree.Enqueue(fd);
                            }
                            _listu[i].dtable2[ig] = IntPtr.Zero.ToPointer();

                        }
                        _listu[i].dtable2 = null;
                        // _listu[i] = null;

                    }
                }
            }
            return allfree;
        }
        #endregion
        #region 数据更新
        /// <summary>
        /// 数据插入
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dhead"></param>
        /// <param name="trees"></param>
        /// <returns></returns>
        public unsafe int updatedata(List<ListDmode> _listu, String sqlsst, Head[] _dhead, JObject job, Liattable ltable)
        {
          
            var mylist = new ListDmode[0];
            if (sqlsst != "")
            {
               var list=  selecttiem(_listu, sqlsst, _dhead, ltable);

                mylist = list;
            }
            else
            {

                mylist = _listu.ToArray();
            }
            if (mylist == null)
                return 0;
            int count = mylist.Length;
            if (count > 0)
            {
                allfree = Update(mylist, _dhead, job, ltable);
                //freequeue(allfree);
               // System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(freequeue), allfree);
                return count;
            }
            return 0;
        }

        ConcurrentQueue<Freedata> Update(ListDmode[] _listu, Head[] _dhead, JObject job, Liattable ltable)
        {
            int count = 0;
          
            ListDmode dmode; Head[] hhead = new Head[0];
            for (int i = 0; i < _listu.Length; i++)
            {
                if (i < _listu.Length && _listu[i] != null)
                {
                    lock (_listu[i])
                    {
                        if (_listu[i].dtable2 == null)
                            continue;
                        dmode = insertintoJson(job, ref hhead);
                     
                        for (int ig = 0; ig < _dhead.Length; ig++)
                        {

                            if (_dhead[ig].index >= _listu[i].dtable2.Length)
                                continue;
                            byte type = _dhead[ig].type;

                            for (int igg = 0; igg < hhead.Length; igg++)
                            {
                                if (hhead[igg].key == _dhead[ig].key)
                                {

                                    if (type == hhead[igg].type)
                                    {
                                        IntPtr pp = (IntPtr)_listu[i].dtable2[_dhead[ig].index];
                                        ltable.tree[hhead[igg].key].searchremove(_listu[i].dtable2[_dhead[ig].index], type, _dhead[ig].index);
                                        if (pp != IntPtr.Zero)
                                        {
                                            count++;
                                            //Freedata fd = new Freedata();
                                            //fd.ptr = pp;
                                            //fd.type = type;
                                            //allfree.Enqueue(fd);
                                            Marshal.FreeHGlobal(pp);
                                        }
                                        _listu[i].dtable2[_dhead[ig].index] = dmode.dtable2[hhead[igg].index];
                                        _listu[i].LenInts[_dhead[ig].index] = dmode.LenInts[hhead[igg].index];
                                        int len = 0;
                                        void* key = DBDataHead.getdata(_dhead[ig].type, job[_dhead[ig].key], ref len);
                                        ltable.tree[hhead[igg].key].insert(ltable.tree[hhead[igg].key].root, key, _listu[i], type);
                                       
                                        //  dmode.dtable2[hhead[igg].index] = IntPtr.Zero.ToPointer();
                                    }
                                }
                            }



                        }
                    

                    }
                }
            }
            return allfree;
        }
        #endregion 

        #region 数据查询

        /// <summary>
        /// 数据总行数查询
        /// </summary>
        /// <param name="_listu"></param>
        /// <param name="_sqlsst"></param>
        /// <param name="_dhead"></param>
        /// <param name="ltable"></param>
        /// <returns></returns>
        public unsafe long SelectCount(List<ListDmode> _listu, String _sqlsst, Head[] _dhead, Liattable ltable)
        {
            ListDmode[] data = selecttiem(_listu, _sqlsst, _dhead, ltable);
            long count = data.LongLength;
            return count;
        }

        List<ListDmode> saobiao(List<ListDmode> listu,int mtsContrast,byte datatype,void* data,int index,int len)
        {
            List<ListDmode> list = new List<ListDmode>();
            for (int i = 0; i < listu.Count; i++)
            {
               
                if (utli.CompareLogical(datatype, listu[i].dtable2[index], mtsContrast, data, len, listu[i].LenInts[index]))
                {
                    list.Add(listu[i]);
                }
            }
            return list;
        }
    /// <summary>
    ///  通过SQL语句查询
    /// </summary>
    /// <param name="listu"></param>
    /// <param name="sqlsst"></param>
    /// <param name="dhead"></param>
    /// <param name="ltable"></param>
    /// <returns></returns>
        public ListDmode[] selecttiem(List<ListDmode> listu, string sqlsst, Head[] dhead, Liattable ltable)
        {
            List<ListDmode> listutem = new List<ListDmode>();
            if (sqlsst == "")
            {
                int len = listu.Count;
                for (int i = 0; i < len; i++)
                {
                    if (i < listu.Count && listu[i] != null)
                    {
                        listu[i].dt = DateTime.Now.ToFileTime();
                        //listu[i].dtable
                        listutem.Add(listu[i]);
                        // listlen.Add(listu[i].LenInts);
                    }
                }

            }
            else
            {
                Contrastmode contm = utli.wherelogical(sqlsst, dhead);
               
               
                for (int i = 0; i < contm.mtsContrast.Length; i++)
                {
                    if (i == 0 )
                    {
                      

                        if (contm.mtsContrast[i] != 5)
                        {
                            List<ListDmode> list = new List<ListDmode>();
                            list = ltable.tree[dhead[contm.collindex[i]].key].searcheQualto(ltable.tree[dhead[contm.collindex[i]].key].root,
                                       contm.mtssscondata[i], contm.hindex[i], contm.mtsContrast[i]);
                            if (list == null)
                                return new ListDmode[0];
                            listutem.AddRange(list);
                        }
                        else
                        {
                            listutem = saobiao(listutem, contm.mtsContrast[i], contm.hindex[i], contm.mtssscondata[i], contm.collindex[i], contm.mtslen[i]);
                            if (listutem == null)
                                return new ListDmode[0];
                        }
                       

                    }
                    else
                    { 
                        int b = i - 1;

                        if (b >= 0 && b < contm.logical.Length)
                        {
                            if (contm.logical[b] == 0)
                            {

                                listutem = saobiao(listutem, contm.mtsContrast[i], contm.hindex[i], contm.mtssscondata[i], contm.collindex[i], contm.mtslen[i]);
                                if (listutem == null)
                                    return new ListDmode[0];
                            }
                            else if (contm.logical[b] == 1)
                            {
                            
                                if (contm.mtsContrast[i] != 5)
                                {
                                    List<ListDmode> list = new List<ListDmode>();
                                    list = ltable.tree[dhead[contm.collindex[i]].key].searcheQualto(ltable.tree[dhead[contm.collindex[i]].key].root,
                                               contm.mtssscondata[i], contm.hindex[i], contm.mtsContrast[i]);
                                    if (list != null)
                                        listutem = chongfu(listutem, list);
                                }
                                else
                                {
                                    listutem = saobiao(listutem, contm.mtsContrast[i], contm.hindex[i], contm.mtssscondata[i], contm.collindex[i], contm.mtslen[i]);
                                    if (listutem == null)
                                        return new ListDmode[0];
                                } 
                                 
                            }
                        }
                    } 
                }
            }
            return listutem.ToArray();
        }

        internal void cleardata(List<ListDmode> datas, Head[] datahead, Liattable ltable)
        {
            delete(datas.ToArray(), datahead,  ltable);
        }

        public JObject[] viewdata(ListDmode[] objsall, Head[] datahead,byte order=0, string ordercol="", int indexlen=0, int viewlen=0)
        {
            //List<Hashtable> alllist = new List<Hashtable>();
            JObject[] temphtt = new JObject[0];
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


                    objsall = utli.Sort(objsall, orhe, order);

                }

                if (viewlen <= 0)
                    viewlen = objsall.Length;
                if ((indexlen * viewlen) >= objsall.Length)
                {
                    //alllist = new List<Hashtable>();
                    return temphtt;
                }
                int count = indexlen * viewlen;
                int lens = ((indexlen + 1) * viewlen) > objsall.Length ? objsall.Length : ((indexlen + 1) * viewlen);
                lens = lens - (indexlen * viewlen);
                string[] ksys = new string[datahead.Length];
                byte[] types = new byte[datahead.Length];
                int[] indexs = new int[datahead.Length];
                int ik = 0;
                foreach (Head h in datahead)
                {
                    ksys[ik] = h.key;
                    types[ik] = h.type;
                    indexs[ik] = h.index;
                    ik++;
                }
                temphtt = new JObject[count + lens];
                for (int i = count; i < count + lens; i++)
                {
                    try
                    {
                        if (objsall[i] != null)
                        {
                            JObject jo = new JObject();


                            for (int hi = 0; hi < datahead.Length; hi++)
                            {
                                if (objsall[i].dtable2.Length > indexs[hi])
                                {
                                    //  DateTime dt = DateTime.Now;

                                    
                                    JProperty obj =utli. GetHashtable(ksys[hi], types[hi], objsall[i].dtable2[indexs[hi]], objsall[i].LenInts[indexs[hi]]);
                                    //  DateTime dt2 = DateTime.Now;
                                    //if ((dt2 - dt).TotalMilliseconds > 1)
                                    //{
                                    //    Console.WriteLine("耗时：" + (dt2 - dt).TotalMilliseconds + "毫秒--查询后的数据：");
                                    //}
                                    if(obj!=null)
                                    jo.Add(obj );
                                }
                            }
                            temphtt[i] = jo;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
                //temphtt = alllist.ToArray();
                //alllist = new List<Hashtable>();
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

        BarCodeComparer Comparer = new BarCodeComparer();
        public class BarCodeComparer : IComparer<ListDmode>
        {
            public int Compare(ListDmode x, ListDmode y)
            {
                 if (x.dtable2[0]== y.dtable2[0])
                    return 0;
                else if(x.dtable2[0]> y.dtable2[0])
                    return 1;
                else
                    return -1;
            }
        }
        List<ListDmode> chongfu(List<ListDmode> listutem, List<ListDmode> list)
        {
            listutem.Sort(Comparer);
            for (int s = 0; s < list.Count; s++)
            {

                if (listutem.BinarySearch(list[s], Comparer) >= 0)
                {
                    listutem.Remove(list[s]);
                }
            }
            listutem.AddRange(list.ToArray());
            return listutem;
        }
        #endregion

    }
}
