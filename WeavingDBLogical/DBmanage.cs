using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WeavingDBLogical
{
   public unsafe class DBmanage
    {
        ConcurrentDictionary<String, byte[]> CDKV = new ConcurrentDictionary<string, byte[]>();
        ConcurrentDictionary<String, long> CDKVlong = new ConcurrentDictionary<string, long>();
        ConcurrentDictionary<String, liattable> CDtable = new ConcurrentDictionary<string, liattable>();
       
       public DBmanage()
        {
            int noselecttimeout = 0, notimeout = 0;
            noselecttimeout =Convert.ToInt32( System.Configuration.ConfigurationManager.AppSettings["KVnoselecttimeout"]);
            if (noselecttimeout != 0)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(dataout), noselecttimeout);
            }
            notimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["noselecttimeout"]);
            if (notimeout != 0)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(jsondataout), notimeout);
            }
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(bdnull), null);

        }
        void bdnull(object obj)
        {
            // DBLogical.delnull()
            //System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(), _listu);
          //  int timeout = (int)obj;
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                try
                {
                    string[] keys = CDtable.Keys.ToArray();
                    int len = keys.Length;
                    for (int i = 0; i < len; i++)
                    {
                        if (CDtable.ContainsKey(keys[i]))
                        {
                            try
                            {
                                string key = keys[i];
                                //List<listDmode> listdate = CDtable[key].datas;
                                if (!CDtable[key].deleterun)
                                {
                                    CDtable[key].deleterun = true;
                                    System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DBLogical.delnull), CDtable[key]);
                                }
                            }
                            catch { }

                        }
                    }
                }
                catch { }
            }
        }
        void jsondataout(object obj)
        {
            int timeout = (int)obj;
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                try
                {
                    string[] keys = CDtable.Keys.ToArray();
                    int len = keys.Length;
                    for (int i = 0; i < len; i++)
                    {
                        if (CDtable.ContainsKey(keys[i]))
                        {
                            try
                            {
                                string key = keys[i];
                                List<listDmode> listdate = CDtable[key].datas;
                                for (int j = listdate.Count; j > 0; j--)
                                {
                                    
                                    double ss = (DateTime.Now - DateTime.FromFileTime(listdate[j].dt)).TotalSeconds;
                                    if (ss > timeout)
                                    {
                                        for (int ig = 0; ig < listdate[j].dtable2.Length; ig++)
                                            Marshal.FreeHGlobal((IntPtr)listdate[i].dtable2[ig]);
                                        listdate.Remove(listdate[j]);
                                    }
                                }
                            }
                            catch { }

                        }
                    }
                }
                catch { }
            }
        }
        void dataout(object obj)
        {
            int timeout = (int)obj;
            while (true)
            {
                System.Threading.Thread.Sleep(1000);

                try
                {
                    string[] keys = CDKVlong.Keys.ToArray();
                    int len = keys.Length;
                    for (int i = 0; i < len; i++)
                    {
                        if (CDKVlong.ContainsKey(keys[i]))
                        {
                            string key = keys[i];
                            long utc = CDKVlong[key];
                            double ss = (DateTime.Now - DateTime.FromFileTime(utc)).TotalSeconds;
                            if (ss > timeout)
                            {
                                Remove(key);
                            }
                        }
                    }
                }
                catch { }
            }
        }
        /// <summary>
        /// 创建非关型数据表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Createtable(string key)
        {
            if (!CDtable.ContainsKey(key))
               return CDtable.TryAdd(key, new liattable());
            return false;
        }
        /// <summary>
        /// 清除非关型数据表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool deletetable(string key)
        {
            if (CDtable.ContainsKey(key))
            {
                try
                {
                    liattable list;
                    bool b = CDtable.TryRemove(key, out list);
                    new DBLogical().cleardata(list.datas, list.datahead);
                    list.datahead = null;
                    list = null;
                    return b;
                }
                catch
                { }
            }
            return false;
        }
        /// <summary>
        /// 向非关表中插入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool insettabledata(string key, string data)
        {
            if (CDtable.ContainsKey(key))
            {
                DBLogical dblo = new DBLogical();
                liattable list = CDtable[key];
               JObject job=  JObject.Parse(data);
                
                lock (list.datas)
                {
                    list.datas.Add(dblo.insertintoJson(job, ref list.datahead));
                  
                }
                return true;
            }
          
            return false;
        }
        /// <summary>
        /// 向非关表中插入数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool insettabledataArray(string key, string data)
        {
            if (CDtable.ContainsKey(key))
            {
                DBLogical dblo = new DBLogical();
                liattable list = CDtable[key];
                JArray job = JArray.Parse(data);

                lock (list.datas)
                {
                    foreach (JObject item in job)
                        list.datas.Add(dblo.insertintoJson(item, ref list.datahead));
                   
                }
                return true;
            }

            return false;
        }

        /// <summary>
        ///  从非关数据表查询数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sql"></param>
        /// <param name="order"></param>
        /// <param name="page"></param>
        /// <param name="viewlen"></param>
        /// <param name="coll"></param>
        /// <returns></returns>
        public string selecttabledata(string key, string sql, byte order, int pageindex, int pagesize ,out int count, string coll="")
        {
            count = 0;
            if (CDtable.ContainsKey(key))
            {
                try
                {
                    DBLogical dblo = new DBLogical();
                    liattable list = CDtable[key];
                   
                        void*[][] objsall = dblo.selecttiem(list.datas, sql, list.datahead);
                        count = objsall.Length;
                        Hashtable[] objbb2 = dblo.viewdata(objsall, order, coll, pageindex, pagesize, list.datahead);
                        return Newtonsoft.Json.JsonConvert.SerializeObject(objbb2);
                    
                 
                }
                catch
                { return ""; }
            }

            return "";
        }

        public bool updatetabledata(string key, string sql, string data)
        {
            if (CDtable.ContainsKey(key))
            {
                try
                {
                    DBLogical dblo = new DBLogical();
                    liattable list = CDtable[key];
                    JObject job= JObject.Parse(data);
                    
                    dblo.updatedata(list.datas, sql, list.datahead, job);
                    //insettabledata(key, data);
                    return true;


                }
                catch(Exception ee)
                {
                    throw ee;
                }
            }

            return false;
        }
        public bool deletetabledata(string key, string sql)
        {
            if (CDtable.ContainsKey(key))
            {
                try
                {
                    if (sql == "")
                        return false;
                    DBLogical dblo = new DBLogical();
                    liattable list = CDtable[key];

                    dblo.deletedata(list.datas, sql, list.datahead);
                    return true;


                }
                catch
                { return false; }
            }

            return false;
        }
        /// <summary>
        /// 通过KEY获取V值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public byte[] get(string key)
        {
            try
            {
                if (CDKV.ContainsKey(key))
                {
                    CDKVlong[key] = DateTime.Now.ToFileTime();
                    return CDKV[key];
                }
            }
            catch
            { }
            return null;
           
        }
        /// <summary>
        /// 移除K-V数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Remove(string key)
        {
            byte[] b;long lo;
            if (CDKV.ContainsKey(key))
            {
                CDKVlong.TryRemove(key, out lo);
                return CDKV.TryRemove(key, out b);
            }
            return false;
        }
        /// <summary>
        /// 设置K-V数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vlaue"></param>
        /// <returns></returns>
        public bool set(string key, byte[] vlaue)
        {
          
            if (CDKV.ContainsKey(key))
            {
                CDKVlong[key] = DateTime.Now.ToFileTime();
                CDKV[key] = vlaue;
                return true;
            }
            else
            {
                CDKVlong.TryAdd(key, DateTime.Now.ToFileTime());
                return CDKV.TryAdd(key, vlaue);
            }
        }

    }
}
