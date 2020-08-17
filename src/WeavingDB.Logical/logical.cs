using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;

namespace WeavingDB.Logical
{
    public unsafe class Logical
    {
        internal object[] Gethtabledtjson(JObject obj, Head[] dhead)
        {
            object[] objs = new object[dhead.Length];
            foreach (var item in dhead)
            {
                if (obj[item.key] != null)
                {
                    object objc = Insertmarshal(obj[item.key]);
                    objs[item.index] = objc;
                }
            }

            return objs;
        }

        internal object GetHashtable(string key, byte type, void* p1, int len)
        {
            object obj = null;
            try
            {
                if (type == 6)
                {
                    obj = *(int*)p1;
                }
                else if (type == 9)
                {
                    obj = *(bool*)p1;
                }
                else if (type == 7)
                {
                    obj = *(double*)p1;
                }
                else if (type == 12)
                {
                    obj = DateTime.FromFileTime(*(long*)p1);
                }
                else if (type == 8)
                {
                    obj = Marshal.PtrToStringAnsi((IntPtr)p1);
                }
                else if (type == 10)
                {
                    return null;
                }
                else
                {
                    byte[] abc = Tobyte((byte*)p1, len);

                    string temp = BytesToT<string>(GZIP.Decompress(abc));

                    obj = Newtonsoft.Json.JsonConvert.DeserializeObject(temp);
                }
            }
            catch
            {
                return obj;
            }
            return obj;
        }

        private T BytesToT<T>(byte[] bytes)
        {
            using (var ms = new MemoryStream())
            {
                ms.Write(bytes, 0, bytes.Length);
                var bf = new BinaryFormatter();
                ms.Position = 0;
                var x = bf.Deserialize(ms);
                return (T)x;
            }
        }

        private byte[] TToBytes<T>(T obj)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        public unsafe IntPtr Tobytes(byte[] write)
        {
            IntPtr write_data = Marshal.AllocHGlobal(write.Length);
            Marshal.Copy(write, 0, write_data, write.Length);
            return write_data;
        }

        public unsafe byte[] Tobyte(byte* write_data, int data_len)
        {

            byte[] write = new byte[data_len];
            Marshal.Copy((IntPtr)write_data, write, 0, write.Length);
            return write;
        }

        internal unsafe void*[] Gethtabledtjsontointptr(JObject obj, Head[] dhead, ref int[] lensInts, ref IntPtr[] objIntPtrs)
        {
            void*[] objs = new void*[dhead.Length];
           objIntPtrs = new IntPtr[dhead.Length];
            lensInts = new int[dhead.Length];
            foreach (var item in dhead)
            {
                byte jtt = item.type;
                if (obj[item.key] != null)
                {
                    try
                    {
                        if (jtt == 10)
                        {
                            continue;
                        }
                        else if (jtt == 6)
                        {
                            int p = Convert.ToInt32(obj[item.key].ToString());
                            int nSizeOfPerson = Marshal.SizeOf(p);
                            IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                            Marshal.StructureToPtr(p, intPtr, true);
                            objs[item.index] = intPtr.ToPointer();
                            objIntPtrs[item.index] = intPtr;
                            lensInts[item.index] = 4;
                        }
                        else if (jtt == 9)
                        {
                            bool p = Convert.ToBoolean(obj[item.key].ToString());
                            int nSizeOfPerson = Marshal.SizeOf(p);
                            IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                            Marshal.StructureToPtr(p, intPtr, true);
                            objs[item.index] = intPtr.ToPointer();
                            objIntPtrs[item.index] = intPtr;
                            lensInts[item.index] = 1;
                        }
                        else if (jtt == 7)
                        {
                            double p = Convert.ToDouble(obj[item.key].ToString());
                            int nSizeOfPerson = Marshal.SizeOf(p);
                            IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                            Marshal.StructureToPtr(p, intPtr, true);
                            objs[item.index] = intPtr.ToPointer();
                            objIntPtrs[item.index] = intPtr;
                            lensInts[item.index] = 8;
                        }
                        else if (jtt == 12)
                        {
                            lensInts[item.index] = 8;
                            long p = Convert.ToDateTime(obj[item.key].ToString()).ToFileTime();

                            int nSizeOfPerson = Marshal.SizeOf(p);
                            IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                            Marshal.StructureToPtr(p, intPtr, true);
                            objs[item.index] = intPtr.ToPointer();
                            objIntPtrs[item.index] = intPtr;
                        }
                        else if (jtt == 8)
                        {
                            string str = obj[item.key].ToString();
                            lensInts[item.index] = System.Text.Encoding.Default.GetBytes(str.ToCharArray()).Length;
                            IntPtr p = Marshal.StringToHGlobalAnsi(str);
                            objs[item.index] = p.ToPointer();
                            objIntPtrs[item.index] = p;
                        }
                        else
                        {
                            byte[] p = GZIP.Compress(TToBytes(obj[item.key].ToString()));
                            objIntPtrs[item.index] = Tobytes(p);
                            objs[item.index] = objIntPtrs[item.index].ToPointer();
                            lensInts[item.index] = p.Length;
                        }
                    }
                    catch
                    {
                        Marshal.FreeHGlobal((IntPtr)objs[item.index]);
                    }
                }
            }
            return objs;
        }

        internal Head[] Gethead(JObject obj)
        {
            Head[] coll = new Head[obj.Count];
            int i = 0;
            foreach (var item in obj)
            {
                if (item.Key != "" && item.Key != null)
                {
                    Head hh = new Head();
                    coll[i] = hh;
                    coll[i].key = item.Key;
                    coll[i].index = i;
                    coll[i].type = (byte)item.Value.Type;
                    i++;
                }
            }

            return coll;
        }

        internal Head[] Gethead(JObject obj, Head[] heads)
        {
            Head[] temp = heads;
            foreach (var item in obj)
            {
                bool bb = false;
                foreach (Head hd in heads)
                {
                    if (item.Key == hd.key)
                    {
                        if (hd.type == 10)
                        {
                            hd.type = (byte)item.Value.Type;
                        }

                        if ((byte)item.Value.Type == 10)
                        {
                            bb = true;
                            break;
                        }

                        if ((byte)item.Value.Type == hd.type)
                        {
                            bb = true;
                            break;
                        }
                        else
                        {
                            throw new Exception("数据类型不匹配。");
                        }

                    }
                }
                if (!bb)
                {
                    temp = new Head[heads.Length + 1];
                    heads.CopyTo(temp, 0);
                    temp[temp.Length - 1] = new Head
                    {
                        key = item.Key,
                        index = temp.Length - 1,
                        type = (byte)item.Value.Type
                    };

                    heads = temp;
                }
            }
            return temp;
        }

        internal object Insertmarshal(JToken Value)
        {
            object vv;
            if (Value.Type == JTokenType.Integer)
            {
                vv = Convert.ToInt32(Value.ToString());

            }
            else if (Value.Type == JTokenType.Float)
            {
                vv = Convert.ToDouble(Value.ToString());

            }
            else if (Value.Type == JTokenType.Date)
            {
                vv = Convert.ToDateTime(Value.ToString());

            }
            else if (Value.Type == JTokenType.String)
            {
                vv = Value.ToString();

            }

            else if (Value.Type == JTokenType.Boolean)
            {
                vv = Convert.ToBoolean(Value.ToString());

            }
            else vv = Value.ToString();
            return vv;
        }

        internal ConcurrentQueue<ListDmode> listutem = new ConcurrentQueue<ListDmode>();
        internal Hashtable Gethtable(object obj)
        {
            Hashtable coll = new Hashtable();
            Type t = (obj).GetType();
            FieldInfo[] pis = t.GetFields();
            int i = 0;
            foreach (FieldInfo fi in pis)
            {
                coll.Add(fi.Name, i);
                i++;
            }
            return coll;
        }

        internal bool Logicaljudgement(byte[] sst, bool[] bbs)
        {
            bool rb;
            rb = bbs[0];
            int len = sst.Length;
            for (int i = 0; i < len; i++)
            {
                if (sst[i] == 0)
                    rb = rb && bbs[i + 1];
                if (sst[i] == 1)
                    rb = rb || bbs[i + 1];
            }

            return rb;
        }

        internal byte[] LogicalSplit(string[] sst)
        {
            List<byte> lists = new List<byte>();
            foreach (string s in sst)
            {
                if (s == "&&")
                    lists.Add(0);
                if (s == "||")
                    lists.Add(1);
            }
            return lists.ToArray();
        }
        public bool Stringtonosymbol(String _sqlsst, string rstr)
        {
            
            Regex r = new Regex(rstr); // 定义一个Regex对象实例
            
            var m = r.Match(_sqlsst);


            if (m.Success)
            {
                return true;
            }
            return false;
        }
        internal string Stringtonosymbol(string _sqlsst)
        {
            if (_sqlsst == "''")
                return "";
            Regex r = new Regex("'(.+)'|''"); // 定义一个Regex对象实例
            var m = r.Match(_sqlsst);


            if (m.Success)
            {
                return m.Value.Substring(1, m.Value.Length - 2);
            }
            return _sqlsst;
        }

    
        internal string[] Sqltolist(String _sqlsst)
        {
            Regex r = new Regex("'(.+?)'|''"); // 定义一个Regex对象实例
            var ms = r.Matches(_sqlsst);
            int ii = 0;
            foreach (Match m in ms)
            {
                if (m.Success)
                {
                    ii++;
                }
            }
            string[] sspr = new string[ii];
            ii = 0;
            foreach (Match m in ms)
            {
                if (m.Success)
                {
                    sspr[ii] = m.Value;
                    _sqlsst = _sqlsst.Replace(m.Value, "{" + ii + "}");
                    ii++;
                }

            }
            _sqlsst = _sqlsst.Replace(" like ", ">like<");
            string[] sst = _sqlsst.Split(' ');
            for (int ssti = 0; ssti < sst.Length; ssti++)
            {
                sst[ssti] = string.Format(sst[ssti], sspr);
            }
            return sst;
        }
        Regex r = new Regex(">like<|==|>=|<=|<|>"); // 定义一个Regex对象实例
        internal string LogicalContrastSplit(string s)
        {

            Match m = r.Match(s); // 在字符串中匹配
            if (m.Success)
            {
                return m.Value;
            }
            //foreach (string s in sst)

            return "";
        }

        #region 排序

        internal ListDmode[] Sort(ListDmode[] objsall, Head orhe, int order)
        {
            if (order == 0)
                objsall = QuickSort(objsall, orhe, 0, objsall.Length - 1, true);
            else
                objsall = QuickSort(objsall, orhe, 0, objsall.Length - 1, false);
            return objsall;
        }

        private unsafe int Division(ListDmode[] list, Head orhe, int left, int right, bool ord)
        {

            while (left < right)
            {
                void* p1 = list[left].dtable2[orhe.index];
                ListDmode listvoid = list[left];

                bool bba = false;
                if (orhe.type == 6)
                {
                    int num = *(int*)p1;

                    int left1 = *(int*)list[left + 1].dtable2[orhe.index];
                    bba = num > left1;
                }
                else if (orhe.type == 9)
                {
                    byte num = Convert.ToByte(*(bool*)p1);

                    byte left1 = Convert.ToByte(*(bool*)list[left + 1].dtable2[orhe.index]);
                    bba = num > left1;
                }
                else if (orhe.type == 7)
                {
                    double num = *(double*)p1;

                    double left1 = *(double*)list[left + 1].dtable2[orhe.index];
                    bba = num > left1;
                }
                else if (orhe.type == 12)
                {
                    long num = *(long*)p1;
                    long left1 = *(long*)list[left + 1].dtable2[orhe.index];
                    bba = num > left1;
                }
                else if (orhe.type == 8)
                {
                    string vv = Marshal.PtrToStringAnsi((IntPtr)p1);
                    if (vv != "")
                    {
                        int num = Asc(vv.Substring(0, 1));
                        string vv2 = Marshal.PtrToStringAnsi((IntPtr)list[left + 1].dtable2[orhe.index]);

                        long left1 = Asc(vv2.Substring(0, 1));
                        bba = num > left1;

                    }
                }

                if (bba == ord)
                {
                    list[left] = list[left + 1];
                    list[left + 1] = listvoid;
                    left++;
                }
                else
                {
                    ListDmode temp = list[right];
                    list[right] = list[left + 1];
                    list[left + 1] = temp;
                    right--;
                }
            }
            return left; //指向的此时枢轴的位置
        }

        private ListDmode[] QuickSort(ListDmode[] list, Head orhe, int left, int right, bool ord)
        {
            if (left < right)
            {
                int i = Division(list, orhe, left, right, ord);
                //对枢轴的左边部分进行排序
                QuickSort(list, orhe, i + 1, right, ord);
                //对枢轴的右边部分进行排序
                QuickSort(list, orhe, left, i - 1, ord);
                return list;
            }
            return list;
        }

        internal int Asc(string character) /*字符转化为ASCII*/
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            int intAsciiCode = asciiEncoding.GetBytes(character)[0];
            return (intAsciiCode);
        }

        internal string Chr(int asciiCode) /*ASCII 转化为 字符*/
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                throw new Exception("ASCII Code is not valid.");
            }
        }
        #endregion
    }
}
