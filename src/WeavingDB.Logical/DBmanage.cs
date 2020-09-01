using Newtonsoft.Json.Linq;
using SQLDBlogic.logic;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;

namespace WeavingDB.Logical
{
    public unsafe class DBmanage
    {
        readonly ConcurrentDictionary<string, byte[]> CDKV = new ConcurrentDictionary<string, byte[]>();
        readonly ConcurrentDictionary<string, long> CDKVlong = new ConcurrentDictionary<string, long>();
        readonly ConcurrentDictionary<string, long> CDKVlongtimeout = new ConcurrentDictionary<string, long>();
        readonly ConcurrentDictionary<string, Liattable> CDtable = new ConcurrentDictionary<string, Liattable>();
        readonly ConcurrentQueue<string> savekey = new ConcurrentQueue<string>();
        readonly string path = "";
        bool KVRemove = false;
        int noselecttimeout = 0, notimeout = 0;
        public DBmanage()
        {
            path = Thread.GetDomain().BaseDirectory;
            if (!Directory.Exists(path + "KVDATA"))
            {

                Directory.CreateDirectory(path + "KVDATA");
            }
            if (!Directory.Exists(path + "TDATA"))
            {

                Directory.CreateDirectory(path + "TDATA");
            }
            try
            {
                KVRemove = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["KVRemove"]);
            }
            catch { }
              noselecttimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["KVnoselecttimeout"]);
            if (noselecttimeout != 0)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Dataout), noselecttimeout);
            }
            else
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Save), 0);
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(Datatimeout), 0);
            notimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["noselecttimeout"]);
          
            if (notimeout != 0)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(Jsondataout), notimeout);
            }
            Load(path + "KVDATA");
            Loadtable(path + "TDATA");
           // ThreadPool.QueueUserWorkItem(new WaitCallback(Bdnull), null);
          //  ThreadPool.QueueUserWorkItem(new WaitCallback(DBLogical.freequeue), null);
        }

        private void Datatimeout(object state)
        {
           
            while (true)
            {
                Thread.Sleep(1000);

                try
                {
                    string[] keys = CDKVlongtimeout.Keys.ToArray();
                    int len = keys.Length;
                    for (int i = 0; i < len; i++)
                    {
                        if (CDKVlongtimeout.ContainsKey(keys[i]))
                        {
                            string key = keys[i];
                            long utc = CDKVlongtimeout[key];
                            double ss = (DateTime.Now - DateTime.FromFileTime(utc)).TotalSeconds;
                            if (ss > 0)
                            {
                                CDKVlongtimeout.TryRemove(key, out utc);
                                if (KVRemove)
                                    Remove(key);
                                else
                                  clear(key);
                            }
                        }
                    }
                    
                }
                catch { }
            }
        }

      
        /// <summary>
        /// 数据表中的数据超过存储时限，清理
        /// </summary>
        /// <param name="obj"></param>
        void Jsondataout(object obj)
        {
            if (!Directory.Exists(path + "TLOG"))
            {

                Directory.CreateDirectory(path + "TLOG");
            }
            int timeout = (int)obj;
            while (true)
            {
                Thread.Sleep(1000);
                try
                {
                    System.IO.StreamWriter fi = new StreamWriter(path + "TLOG/" + DateTime.Now.ToString("yyyyMMddHH")+".log");
                    
                    string[] keys = CDtable.Keys.ToArray();
                    int len = keys.Length;
                    for (int i = 0; i < len; i++)
                    {
                        if (CDtable.ContainsKey(keys[i]))
                        {
                            try
                            {
                                string key = keys[i];
                                List<ListDmode> listdate = CDtable[key].datas;
                                Head[] hhed = CDtable[key].datahead;
                                 var tree=  CDtable[key].tree;
                                for (int j = listdate.Count; j > 0; j--)
                                {
                                    double ss = (DateTime.Now - DateTime.FromFileTime(listdate[j].dt)).TotalSeconds;
                                    if (ss > timeout)
                                    {
                                        lock (listdate[j])
                                        {
                                            for (int ig = 0; ig < hhed.Length; ig++)
                                            {
                                                if (listdate[j].dtable2[hhed[ig].index] == null)
                                                    continue;
                                                IntPtr pp = (IntPtr)listdate[j].dtable2[hhed[ig].index];
                                                if (pp == IntPtr.Zero)
                                                    continue;
                                                fi.WriteLine("移除超时："+ hhed[ig].key+ "type:"+ hhed[ig].type+"行:"+ j);
                                                tree[hhed[ig].key].searchremove(pp.ToPointer(), hhed[ig].type, hhed[ig].index);
                                                listdate[j].dtable2[hhed[ig].index] = null;
                                                Marshal.FreeHGlobal(pp);
                                            }
                                            listdate[j].dtable2 = null;
                                            listdate[j] = null;
                                            listdate.RemoveAt(j);
                                            j = listdate.Count;
                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    fi.Close();
                }
                catch { }
            }
        }

        void Dataout(object obj)
        {
            int timeout = (int)obj;
            while (true)
            {
                Thread.Sleep(1000);

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
                            if (utc != 0)
                            {
                                double ss = (DateTime.Now - DateTime.FromFileTime(utc)).TotalSeconds;
                                if (ss > timeout)
                                {
                                    if (KVRemove)
                                        Remove(key);
                                    else
                                        clear(key);
                                     
                                }
                            }
                        }
                    }
                    if (!savekey.IsEmpty)
                    {
                        len = savekey.Count;
                        for (int i = 0; i < len; i++)
                        {
                            savekey.TryDequeue(out string key);
                            long utc = CDKVlong[key];
                            if (utc != 0)
                                Saveone(key, utc);
                        }
                    }
                }
                catch { }
            }
        }
        void Loadtable(string pathd)
        {
            string[] files = Directory.GetFiles(pathd);
            foreach (string file in files)
            {
                string temp = file.Replace(pathd + @"\", "").Replace(".bin", "");
                Createtable(temp);
            }
        }
        void Load(string pathd)
        {
            string[] files = Directory.GetFiles(pathd);
            foreach (string file in files)
            {
                string temp = file.Replace(pathd + @"\", "").Replace(".bin", "");

                Loadone(temp);
            }
        }

        void Loadone(string key)
        {
            FileStream fs = null;
            try
            {
                 fs = new FileStream(path + @"KVDATA\" + key + ".bin", FileMode.Open ,FileAccess.ReadWrite, FileShare.Read);
                
                  //  if (Createtable(key))
                    {
                        byte[] utc = new byte[8];
                        fs.Read(utc, 0, 8);
                        long len = fs.Length - 8;
                        long sh = (long)BitConverter.ToUInt64(utc, 0);
                        byte[] data = new byte[len];
                        fs.Read(data, 0, (int)len);
                        if (noselecttimeout != 0)
                        {
                            double ss = (DateTime.Now - DateTime.FromFileTime(sh)).TotalSeconds;
                        if (ss > noselecttimeout)
                        {
                            Set(key, data, DateTime.Now.ToFileTime());
                        }
                        else
                            Set(key, data, sh);

                        }
                        else
                            Set(key, data, sh);
                    }
                 
            }
            catch(Exception e)
            {
                //System.IO.StreamWriter sw = new StreamWriter(path + "//log//log.tt");
                //sw.WriteLine(e.Message);
                //sw.Close();
                
            }
            finally
            {
                try
                {
                    if (fs != null)
                        fs.Close();
                }
                catch { }
            }
        }

        void Saveone(string key, long utc)
        {
            FileStream fs = null;
            try
            {
                if (File.Exists(path + @"KVDATA\" + key + ".bin"))
                {
                    try {
                        File.Delete(path + @"KVDATA\" + key + ".bin");
                    }
                    catch
                    { }
                }
                fs = new FileStream(path + @"KVDATA\" + key + ".bin", FileMode.OpenOrCreate);
                 
                int lenth = CDKV[key].Length;
                
                byte[] shi = BitConverter.GetBytes(utc);
                fs.Write(shi, 0, 8);
                fs.Write(CDKV[key], 0, lenth);
               
            }
            catch { }
            finally
            {
                try
                {
                    if (fs != null)
                        fs.Close();
                }
                catch { }
            }
        }

        void Save(object obj)
        {
            while (true)
            {
                Thread.Sleep(1000);
                try
                {
                    //持久化保存
                    if (!savekey.IsEmpty)
                    {
                        int len = savekey.Count;
                        for (int i = 0; i < len; i++)
                        {
                            savekey.TryDequeue(out string key);
                            if (CDKVlong.ContainsKey(key))
                            {

                                long utc = CDKVlong[key];
                                if(utc!=0)
                                Saveone(key, utc);

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
            {
                if (!File.Exists(path + @"TDATA\" + key + ".bin"))
                {

                    using (FileStream fs = System.IO.File.Create(path + @"TDATA\" + key + ".bin"))
                    { } 
                }
                return CDtable.TryAdd(key, new Liattable());
            }
            return false;
        }

        /// <summary>
        /// 清除非关型数据表
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Deletetable(string key)
        {
            if (CDtable.ContainsKey(key))
            {
                try
                {
                    if (File.Exists(path + @"TDATA\" + key + ".bin"))
                    {

                        System.IO.File.Delete(path + @"TDATA\" + key + ".bin");
                      
                    }
                    bool b = CDtable.TryRemove(key, out Liattable list);
                    new DBLogic().cleardata(list.datas, list.datahead, list);
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
        public bool Insettabledata(string key, string data)
        {
            if (CDtable.ContainsKey(key))
            {
                DBLogic dblo = new DBLogic();
                Liattable list = CDtable[key];
                JObject job = JObject.Parse(data);

                lock (list.datas)
                {
                    var tee = dblo.insertintoJson(job, ref list.datahead);
                    list.datas.Add(tee);
                    dblo.insertintoIndex(job, tee, list.datahead, ref list.tree);
                  //  list.datas.Add(dblo.insertintoJson(job, ref list.datahead));
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
        public bool InsettabledataArray(string key, string data)
        {
            try
            {
                if (CDtable.ContainsKey(key))
                {
                    DBLogic dblo = new DBLogic();
                    Liattable list = CDtable[key];
                    JArray job = JArray.Parse(data);

                    lock (list.datas)
                    {
                        foreach (JObject item in job)
                        {
                            var tee = dblo.insertintoJson(item, ref list.datahead);
                            list.datas.Add(tee);
                            dblo.insertintoIndex(item, tee, list.datahead, ref list.tree);
                        }
                    }
                    return true;
                }
            }
            catch
            { }

            return false;
        }

        /// <summary>
        ///  从非关数据表查询数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sql"></param>
        /// <param name="order"></param>
        /// <param name="pageindex"></param>
        /// <param name="pagesize"></param>
        /// <param name="count"></param>
        /// <param name="coll"></param>
        /// <returns></returns>
        public string Selecttabledata(string key, string sql, byte order, int pageindex, int pagesize, out int count, string coll = "",string veiwcoll="")
        {
            count = 0;
            if (CDtable.ContainsKey(key))
            {
                try
                {
                    DBLogic dblo = new DBLogic();
                    Liattable list = CDtable[key];

                    ListDmode[] objsall = dblo.selecttiem(list.datas, sql, list.datahead, list);
                    count = objsall.Length;
                    JObject[] objbb2 = dblo.viewdata(objsall, list.datahead, veiwcoll, order, coll, pageindex, pagesize);
                    return Newtonsoft.Json.JsonConvert.SerializeObject(objbb2);
                }
                catch
                { return ""; }
            }

            return "";
        }
        public string Selectcount(string key, string sql, out int count)
        {
            count = 0;
            if (CDtable.ContainsKey(key))
            {
                try
                {
                    DBLogic dblo = new DBLogic();
                    Liattable list = CDtable[key];

                    ListDmode[] objsall = dblo.selecttiem(list.datas, sql, list.datahead, list);
                    count = objsall.Length;
                 //   JObject[] objbb2 = dblo.viewdata(objsall, order, coll, pageindex, pagesize, list.datahead);
                    return Newtonsoft.Json.JsonConvert.SerializeObject(count);
                }
                catch
                { return Newtonsoft.Json.JsonConvert.SerializeObject(0); }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(0);
        }
        internal string[] Selctekey(string keyp)
        {
            try
            {
                keyp = keyp.Replace("*", "(.+)").Replace("?", "(.+){1}");
                List<string> list = new List<string>();
                foreach (string key in CDKV.Keys)
                {
                    if (Stringtonosymbol(key, "^" + keyp + "$"))
                        list.Add(key);
                }
                return list.ToArray();
            }
            catch { }
            return new string[0];
        }

        internal bool Stringtonosymbol(string _sqlsst, string rstr)
        {
            Regex r = new Regex(rstr); // 定义一个Regex对象实例
            var m = r.Match(_sqlsst);

            if (m.Success)
            {
                return true;
            }
            return false;
        }

        public bool Updatetabledata(string key, string sql, string data)
        {
            if (CDtable.ContainsKey(key))
            {
                try
                {
                    DBLogic dblo = new DBLogic();
                    Liattable list = CDtable[key];
                    JObject job = JObject.Parse(data);

                    dblo.updatedata(list.datas, sql, list.datahead, job, list);
                    return true;
                }
                catch (Exception ee)
                {
                    //throw ee;
                }
            }

            return false;
        }

        public bool Deletetabledata(string key, string sql)
        {
            if (CDtable.ContainsKey(key))
            {
                try
                {
                   
                    DBLogic dblo = new DBLogic();
                    Liattable list = CDtable[key];

                    dblo.deletedata(list.datas, sql, list.datahead, list);
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
        public byte[] Get(string key)
        {
            try
            {
                if (CDKV.ContainsKey(key))
                {
                    if(CDKVlong[key]!=0)
                     CDKVlong[key] = DateTime.Now.ToFileTime();
                    return CDKV[key];
                }
                else
                {
                    try
                    {
                        string file = path + "KVDATA\\" + key + ".bin";
                        if (File.Exists(file))
                        {
                            Loadone(key);
                            if (CDKV.ContainsKey(key))
                            {
                                if (CDKVlong[key] != 0)
                                    CDKVlong[key] = DateTime.Now.ToFileTime();
                                return CDKV[key];
                            }
                            // return Get(key);
                        }
                    }
                    catch (Exception e){
                        //System.IO.StreamWriter sw = new StreamWriter(path + "//log//log.tt");
                        //sw.WriteLine(e.Message);
                        //sw.Close();
                    }
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
            byte[] b; long lo;
            if (CDKV.ContainsKey(key))
            {
                try
                {
                    string file = path + "KVDATA\\" + key + ".bin";
                    if (File.Exists(file))
                    {
                        File.Delete(file);
                    }
                }
                catch { }
                CDKVlong.TryRemove(key, out lo);
                return CDKV.TryRemove(key, out b);
            }
            return false;
        }
        /// <summary>
        /// 移除K-V数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool clear(string key)
        {
            try
            {
                byte[] b; long lo;
                if (CDKV.ContainsKey(key))
                {

                    CDKVlong.TryRemove(key, out lo);
                    return CDKV.TryRemove(key, out b);
                }
            }
            catch
            { }
            return false;
        }
        /// <summary>
        /// 设置K-V数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vlaue"></param>
        /// <returns></returns>
        public bool Set(string key, byte[] vlaue)
        {
            try
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
            catch { }
            finally { savekey.Enqueue(key); }
            return true;
        }
        public bool Set(string key, byte[] vlaue,int timeout)
        {
            try
            {
                if (CDKV.ContainsKey(key))
                {
                    CDKVlong[key] = 0;
                    CDKV[key] = vlaue;
                    CDKVlongtimeout[key]= DateTime.Now.AddMinutes(timeout).ToFileTime();
                    return true;
                }
                else
                {
                    CDKVlong.TryAdd(key, 0);
                    CDKVlongtimeout.TryAdd(key, DateTime.Now.AddMinutes(timeout).ToFileTime());
                    return CDKV.TryAdd(key, vlaue);
                }
            }
            catch { }
            finally {
                savekey.Enqueue(key); }
            return true;
        }
        public bool Set(string key, byte[] vlaue, long utc)
        {
            try
            {
                if (CDKV.ContainsKey(key))
                {
                    CDKVlong[key] = utc;
                    CDKV[key] = vlaue;
                    return true;
                }
                else
                {
                    CDKVlong.TryAdd(key, utc);
                    return CDKV.TryAdd(key, vlaue);
                }
            }
            catch { return true; }
        }
    }
}
