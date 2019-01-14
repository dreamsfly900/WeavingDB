using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeavingDBLogical
{
   public unsafe class DBmanage
    {
        ConcurrentDictionary<String, byte[]> CDKV = new ConcurrentDictionary<string, byte[]>();
        ConcurrentDictionary<String, long> CDKVlong = new ConcurrentDictionary<string, long>();
        ConcurrentDictionary<String, liattable> CDtable = new ConcurrentDictionary<string, liattable>();
        string path = "";
        public DBmanage()
        {
            path=System.Threading.Thread.GetDomain().BaseDirectory;
            if (!System.IO.Directory.Exists(path+"KVDATA"))
            {
                System.IO.Directory.CreateDirectory(path+"KVDATA");
            }
            load(path + "KVDATA");
            int noselecttimeout = 0, notimeout = 0;
            noselecttimeout =Convert.ToInt32( System.Configuration.ConfigurationManager.AppSettings["KVnoselecttimeout"]);
            if (noselecttimeout != 0)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(dataout), noselecttimeout);
            }
            else
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(save), 0);
            }
            notimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["noselecttimeout"]);
            if (notimeout != 0)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(jsondataout), notimeout);
            }
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(bdnull), null);
            System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(DBLogical.freequeue), null);
             
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
                                head[] hhed=  CDtable[key].datahead;
                                for (int j = listdate.Count; j > 0; j--)
                                {
                                    
                                    double ss = (DateTime.Now - DateTime.FromFileTime(listdate[j].dt)).TotalSeconds;
                                    if (ss > timeout)
                                    {
                                        lock (listdate[i])
                                        {
                                            for (int ig = 0; ig < hhed.Length; ig++)
                                            {
                                                if (listdate[i].dtable2[hhed[ig].index] == null)
                                                    continue;
                                                IntPtr pp = (IntPtr)listdate[i].dtable2[hhed[ig].index];
                                                if (pp == IntPtr.Zero)
                                                    continue;
                                                freedata fd = new freedata();
                                                fd.ptr = (IntPtr)listdate[i].dtable2[hhed[ig].index];
                                                fd.type = hhed[ig].type;
                                                DBLogical.allfree.Enqueue(fd);
                                            }
                                            listdate[j] = null;
                                        }
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
                            else
                            {
                                saveone(key, utc);
                            }
                        }
                    }
                }
                catch { }
            }
        }

        void load(string pathd)
        {
            string[] files = Directory.GetFiles(pathd);
            foreach (string file in files)
            {
                string temp = file.Replace(pathd + @"\", "").Replace(".bin","");
                loadone(temp);

            }
        }
        void loadone(string key)
        {
            System.IO.FileStream fs = null;
            try
            {
                
                fs = new System.IO.FileStream(path + @"KVDATA\" + key + ".bin", System.IO.FileMode.OpenOrCreate);
                if (Createtable(key))
                {
                    byte[] utc = new byte[8];
                    fs.Read(utc, 0, 8);
                    long len = fs.Length - 8;
                    long sh = (long)System.BitConverter.ToUInt64(utc, 0);
                    byte[] data = new byte[len];
                    fs.Read(data, 0, (int)len);
                    set(key, data, sh);
                }
                
               
            }
            catch { }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }
        void saveone(string key,long utc)
        {
            System.IO.FileStream fs = null;
            try
            {
                fs = new System.IO.FileStream(path + @"KVDATA\" + key + ".bin", System.IO.FileMode.OpenOrCreate);
                int lenth = CDKV[key].Length;
                byte[] shi = System.BitConverter.GetBytes(utc);
                // long sh = (long)System.BitConverter.ToUInt64(shi, 0);
                fs.Write(shi, 0, 8);
                fs.Write(CDKV[key], 0, lenth);
              
            }
            catch { }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
        }
        void save(object obj)
        {
            while (true)
            {
                System.Threading.Thread.Sleep(1000);
                try
                {
                    //持久化保存
                    string[] keys = CDKVlong.Keys.ToArray();
                    int len = keys.Length;
                    for (int i = 0; i < len; i++)
                    {
                        if (CDKVlong.ContainsKey(keys[i]))
                        {
                            string key = keys[i];
                            long utc = CDKVlong[key];

                            saveone(key, utc);
                            
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

                    listDmode[] objsall = dblo.selecttiem(list.datas, sql, list.datahead);
                        count = objsall.Length;
                        Hashtable[] objbb2 = dblo.viewdata(objsall, order, coll, pageindex, pagesize, list.datahead);
                        return Newtonsoft.Json.JsonConvert.SerializeObject(objbb2);
                    
                 
                }
                catch
                { return ""; }
            }

            return "";
        }

        internal string[] selctekey(string keyp)
        {
            keyp = keyp.Replace("*", "(.+)").Replace("?", "(.+){1}");
            List<String> list = new List<string>();
            foreach (String key in CDKV.Keys)
            {
                // Stringtonosymbol("123", "^12(.+){1}3$");
                if (Stringtonosymbol(key, "^" + keyp+"$"))
                    list.Add(key);
            }
            return list.ToArray();
        }
        internal bool Stringtonosymbol(String _sqlsst,string rstr)
        {
            
            Regex r = new Regex(rstr); // 定义一个Regex对象实例
            var m = r.Match(_sqlsst);


            if (m.Success)
            {
                return true;
            }
            return false;
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
                try
                {
                    string file = path + "KVDATA" + key + ".bin";
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
        public bool set(string key, byte[] vlaue,long utc)
        {

            if (CDKV.ContainsKey(key))
            {
                CDKVlong[key] =  utc;
                CDKV[key] = vlaue;
                return true;
            }
            else
            {
                CDKVlong.TryAdd(key, utc);
                return CDKV.TryAdd(key, vlaue);
            }
        }
    }
}
