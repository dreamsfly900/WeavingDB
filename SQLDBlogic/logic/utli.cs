using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WeavingDB;
using WeavingDB.Logical;

namespace SQLDBlogic.logic
{
  public unsafe class utli
    {
        /// <summary>
        /// 分解SQL语句
        /// </summary>
      public static Contrastmode wherelogical(string sqlsst, Head [] dhead)
        {
          var  sst = Sqltolist(sqlsst);
          var  logical = LogicalSplit(sst);

           
                  int   glen = 0;
                foreach (string sr in sst)
                {
                    string Contrast = LogicalContrastSplit(sr);
                    String[] sscon = sr.Split(new string[] { Contrast }, StringSplitOptions.None);

                    if (sscon.Length == 2)
                    {
                        glen++;
                    }
                }
            Contrastmode mts = new Contrastmode();
            mts.mtsContrast = new byte[glen];
            mts. mtssscondata = new void*[glen];
            mts.collindex = new short[glen];
            mts. hindex = new byte[glen];
            mts.mtslen= new int[glen];
            mts.logical = logical;

          //  if (listu.Count > 0)

            //  gethtable(listu[0]);
            int gglen = 0;
                int ss = 0;
                foreach (string sr in sst)
                {
                    string Contrast = LogicalContrastSplit(sr);
                    String[] sscon = sr.Split(new string[] { Contrast }, StringSplitOptions.None);

                    if (sscon.Length == 2)
                    {
                     //
                        //  mtsContrast[gglen] = Contrast;
                        if (Contrast == ">=")
                        {
                        mts.mtsContrast[gglen] = 0;
                        }
                        if (Contrast == "<=")
                        {
                        mts.mtsContrast[gglen] = 1;
                        }
                        if (Contrast == "==")
                        {
                        mts.mtsContrast[gglen] = 2;
                        }
                        if (Contrast == ">")
                        {
                        mts.mtsContrast[gglen] = 3;
                        }
                        if (Contrast == "<")
                        {
                        mts.mtsContrast[gglen] = 4;
                        }
                        if (Contrast == ">like<")
                        {
                        mts.mtsContrast[gglen] = 5;
                        }
                        if (Contrast == "!=")
                        {
                        mts.mtsContrast[gglen] = 6;
                        }
                        // mtssscon[gglen] = sscon[1];

                        foreach (Head hd in dhead)
                        {
                            if (hd.key == sscon[0])
                            {
                            mts.hindex[gglen] = hd.type;
                            mts.collindex[gglen] = (short)hd.index;
                                sscon[1] = Stringtonosymbol(sscon[1]);
                            int len = 0;
                            mts.mtssscondata[gglen] = DBDataHead.getdata(hd.type, sscon[1],ref len);
                            mts.mtslen[gglen] = len;
                                break;
                            }

                        }

                        gglen++;
                    }
                    ss++;
                }

            return mts;

        }
        internal static string Stringtonosymbol(string _sqlsst)
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
        internal static byte[] LogicalSplit(string[] sst)
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
        internal static string[] Sqltolist(String _sqlsst)
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
        static Regex r = new Regex(">like<|==|>=|<=|<|>"); // 定义一个Regex对象实例
        internal static string LogicalContrastSplit(string s)
        {

            Match m = r.Match(s); // 在字符串中匹配
            if (m.Success)
            {
                return m.Value;
            }
            //foreach (string s in sst)

            return "";
        }
        public static byte[] TToBytes<T>(T obj)
        {
            var bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        public static unsafe IntPtr Tobytes(byte[] write)
        {
            IntPtr write_data = Marshal.AllocHGlobal(write.Length);
            Marshal.Copy(write, 0, write_data, write.Length);
            return write_data;
        }
        /// <summary>
        /// 比较逻辑
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="p1">数据1</param>
        /// <param name="mtsContrast">比较类型0>=，1<=,2==,3>,4<,5like,6!=</param>
        /// <param name="valuep1">数据2</param>
        /// <returns></returns>
        public static unsafe bool CompareLogical(byte type, void* p1,int mtsContrast, void* valuep1,int len=-1,int lenvalue=-1)
        {
           
            // void* p1 = listu[i].dtable2[collindex[ci]];
            if (p1 == null)
                return false;
            if ((IntPtr)p1 == IntPtr.Zero)
                return false;
            if ((IntPtr)valuep1 == IntPtr.Zero)
                return false;
            bool conbb = false;

            if (type == 6)
            {
                int value = (int)(*(int*)p1);
                int sconvalue = (int)(*(int*)valuep1);
                if (mtsContrast == 0)
                {

                    return (value) >= sconvalue;
                }
                else if (mtsContrast == 1)
                {
                    return (value) <= sconvalue;
                }
                else if (mtsContrast == 2)
                {
                    return sconvalue == (value);
                }
                else if (mtsContrast == 3)
                {
                    return (value) > sconvalue;
                }
                else if (mtsContrast == 4)
                {
                    return (value) < sconvalue;
                }
                else if (mtsContrast == 6)
                {
                    return (value) != sconvalue;
                }
            }
            else if (type == 9)
            {
                bool value = (bool)(*(bool*)p1);
                bool sconvalue = (bool)(*(bool*)valuep1);
                if (mtsContrast == 2)
                {
                    return sconvalue == (value);
                }
                else if (mtsContrast == 0)
                {

                    return (value).CompareTo(sconvalue) >= 0;
                }
                else if (mtsContrast == 1)
                {
                    return (value).CompareTo(sconvalue) <= 0;
                }

                else if (mtsContrast == 3)
                {
                    return  (value).CompareTo(sconvalue) > 0;
                }
                else if (mtsContrast == 4)
                {
                    return (value).CompareTo(sconvalue) < 0;
                }
                else if (mtsContrast == 6)
                {
                    return (value) != sconvalue;
                }
            }
            else if (type == 7)
            {
                double value = (double)(*(double*)p1);
                double sconvalue = (double)(*(double*)valuep1);
               if (mtsContrast == 0)
                {
                    return (value) >= sconvalue;
                }
                else if (mtsContrast == 1)
                {
                    return (value) <= sconvalue;
                }
                else if (mtsContrast == 2)
                {
                    return sconvalue == (value);
                }
                else if (mtsContrast == 3)
                {
                    return (value) > sconvalue;
                }
                else if (mtsContrast == 4)
                {
                    return (value) < sconvalue;
                }
                else if (mtsContrast == 6)
                {
                    return (value) != sconvalue;
                }

            }
            else if (type == 12)
            {

                // conbb[bi] = Contrast<DateTime>(Convert.ToDateTime(st), Convert.ToDateTime(value), mtsContrast[ci]);
                long value = (long)(*(long*)p1);
                long sconvalue = (long)(*(long*)valuep1);
                if (mtsContrast == 0)
                {
                    return value >= sconvalue;
                }
                else if (mtsContrast == 1)
                {
                    return value <= sconvalue;
                }
                else if (mtsContrast == 2)
                {
                    return value == sconvalue;
                }
                else if (mtsContrast == 3)
                {
                    return (value) > sconvalue;
                }
                else if (mtsContrast == 4)
                {
                    return (value) < sconvalue;
                }
                else if (mtsContrast == 6)
                {
                    return (value) != sconvalue;
                }
            }
            else if (type == 8)
            {
                
             
                if (mtsContrast == 0)
                {
                    if (len != -1)
                    {
                        if (len > lenvalue)
                        {
                            return true;

                        }
                        else
                        {
                            
                            string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                            string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                            return (value).CompareTo(sconvalue) >= 0;
                        }
                    }
                    else
                    {
                        string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                        string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                        return (value).CompareTo(sconvalue) >= 0;
                    }


                 
                }
                else if (mtsContrast == 1)
                {
                    if (len != -1)
                    {
                        if (len < lenvalue)
                        {
                            return true;

                        }
                        else
                        {
                            string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                            string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                            return (value).CompareTo(sconvalue) <= 0;
                        }
                    }
                    else
                    {
                        string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                        string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                        return (value).CompareTo(sconvalue) <= 0;
                    }
                   
                }

                else if (mtsContrast == 3)
                {
                    if (len != -1)
                    {
                        if (len > lenvalue)
                        {
                            return true;

                        }
                        else
                        {
                            string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                            string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                            return (value).CompareTo(sconvalue) > 0;
                        }
                    }
                    else
                    {
                        string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                        string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                        return (value).CompareTo(sconvalue) > 0;
                    }
                   
                }
                else if (mtsContrast == 4)
                {
                    if (len != -1)
                    {
                        if (len < lenvalue)
                        {
                            return true;
                        }
                        else {
                            string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                            string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                            return (value).CompareTo(sconvalue) < 0;
                        }
                          

                    }
                    else
                    {
                        string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                        string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                        return (value).CompareTo(sconvalue) < 0;
                    }

                }
                else if (mtsContrast == 2)
                {
                    if (len != -1)
                    {
                        if (len == lenvalue)
                        {
                            string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                            string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                            return Sunday.strcontains(sconvalue, (value));
                        }
                        else
                            return false;
                    }
                    else
                    {
                        string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                        string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                        return Sunday.strcontains(sconvalue, (value));
                    }
                   
                }

                else if (mtsContrast == 6)
                {
                    if (len != -1)
                    {
                        if (len == lenvalue)
                        {
                            string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                            string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                            return (value) != sconvalue;
                        }
                        else
                            return true;
                    }
                    else
                    {
                        string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                        string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                        return (value) != sconvalue;
                    }
                   
                }
                else if (mtsContrast == 5)
                {
                    string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                    string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                    //sconvalue = sconvalue.Replace("%", "(.*)").Replace("_", "(.+){1}");
                    //conbb = Stringtonosymbol(value, "^" + sconvalue + "$");
                    if (Sunday.strSunday(value, sconvalue, 0) == 0)
                        return  true;
                    else
                        return false;
                }
            }
            else
            {
                conbb = false;
                // throw new Exception("不支持的逻辑判断。");
            }
            return conbb;
        }


        public static unsafe byte[] Tobyte(byte* write_data, int data_len)
        {

            byte[] write = new byte[data_len];
            Marshal.Copy((IntPtr)write_data, write, 0, write.Length);
            return write;
        }
        private static T BytesToT<T>(byte[] bytes)
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
        #region 排序

        internal static ListDmode[] Sort(ListDmode[] objsall, Head orhe, int order)
        {
            if (order == 0)
                objsall = QuickSort(objsall, orhe, 0, objsall.Length - 1, true);
            else
                objsall = QuickSort(objsall, orhe, 0, objsall.Length - 1, false);
            return objsall;
        }

        private static unsafe int Division(ListDmode[] list, Head orhe, int left, int right, bool ord)
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
        internal static JProperty GetHashtable(string key, byte type, void* p1, int len)
        {
           
          
            try
            {
                if (type == 6)
                {
                    return new JProperty(key, *(int*)p1);
                }
                else if (type == 9)
                {
                    return new JProperty(key, *(bool*)p1);
                 
                }
                else if (type == 7)
                {
                   
                    return new JProperty(key, *(double*)p1);
                }
                else if (type == 12)
                {
                   
                    return new JProperty(key, DateTime.FromFileTime(*(long*)p1));
                }
                else if (type == 8)
                {
                
                    return new JProperty(key, Marshal.PtrToStringAnsi((IntPtr)p1));
                }
                else if (type == 10)
                {
                    return null;
                }
                else
                {
                    byte[] abc = Tobyte((byte*)p1, len);

                    string temp = BytesToT<string>(GZIP.Decompress(abc));

                   object obj = Newtonsoft.Json.JsonConvert.DeserializeObject(temp);
                    return new JProperty(key, obj);
                }
            }
            catch
            {
                return null;
            }
        
        }
        private static ListDmode[] QuickSort(ListDmode[] list, Head orhe, int left, int right, bool ord)
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

        internal static int Asc(string character) /*字符转化为ASCII*/
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            int intAsciiCode = asciiEncoding.GetBytes(character)[0];
            return (intAsciiCode);
        }

        internal static string Chr(int asciiCode) /*ASCII 转化为 字符*/
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
