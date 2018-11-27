using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WeavingDBLogical
{
   public unsafe class logical
    {






     internal   object [] gethtabledtjson(JObject obj, head[] dhead)
        {
            //Hashtable coll = new Hashtable();
            Object[] objs = new object[dhead.Length];
            foreach (var item in dhead)
            {
                // JTokenType jtt = item.Value.Type;
                if (obj[item.key] != null)
                {
                    object objc = insertmarshal(obj[item.key]);
                    objs[item.index] = objc;
                }
                //  datamode dm = new datamode(jtt, objc); 
                //coll.Add(item., objc);
            }


            return objs;
        }
        internal unsafe void*[] gethtabledtjsontointptr(JObject obj, head[] dhead)
        {
            //Hashtable coll = new Hashtable();
            void*[] objs = new void*[dhead.Length];
            foreach (var item in dhead)
            {
                 byte jtt = item.type;
                if (obj[item.key] != null)
                {
                    //object objc = insertmarshal(obj[item.key]);


                    try
                    {
                        {
                            if (jtt == 6)
                            {


                                int p = Convert.ToInt32(obj[item.key].ToString());
                                int nSizeOfPerson = Marshal.SizeOf(p);
                                IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                                Marshal.StructureToPtr(p, intPtr, true);
                                objs[item.index] = intPtr.ToPointer();
                            }
                            else if (jtt == 9)
                            {

                                bool p = Convert.ToBoolean(obj[item.key].ToString());
                                int nSizeOfPerson = Marshal.SizeOf(p);
                                IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                                Marshal.StructureToPtr(p, intPtr, true);
                                objs[item.index] = intPtr.ToPointer(); ;

                            }
                            else if (jtt == 7)
                            {

                                double p = Convert.ToDouble(obj[item.key].ToString());
                                int nSizeOfPerson = Marshal.SizeOf(p);
                                IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                                Marshal.StructureToPtr(p, intPtr, true);
                                objs[item.index] = intPtr.ToPointer();



                            }
                            else if (jtt == 12)
                            {

                                long p = Convert.ToDateTime(obj[item.key].ToString()).ToFileTimeUtc();

                                int nSizeOfPerson = Marshal.SizeOf(p);
                                IntPtr intPtr = Marshal.AllocHGlobal(nSizeOfPerson);
                                Marshal.StructureToPtr(p, intPtr, true);
                                objs[item.index] = intPtr.ToPointer(); ;

                            }
                            else if (jtt == 8)
                            {

                                // char[] p = ((String)objc).ToCharArray().;
                             
                                char* p = (char*)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(obj[item.key].ToString()).ToPointer();
                                objs[item.index] = p;
                            }
                            else
                            {

                                // char[] p = ((String)objc).ToCharArray().;
                                string gzip=  (obj[item.key].ToString());
                            
                                char* p = (char*)System.Runtime.InteropServices.Marshal.StringToHGlobalAnsi(gzip).ToPointer();
                                objs[item.index] = p;
                            }

                        }
                    }
                    catch {
                        Marshal.FreeHGlobal((IntPtr)objs[item.index]);
                    }
                  
                    
                }
                 
            }


            return objs;
        }
        public  string GZipDecompressString(string zippedString)
        {
            if (string.IsNullOrEmpty(zippedString) || zippedString.Length == 0)
            {
                return "";
            }
            else
            {
                byte[] zippedData = Convert.FromBase64String(zippedString.ToString());
                return (string)(System.Text.Encoding.UTF8.GetString(Decompress(zippedData)));
            }
        }
        /// <summary>
        /// ZIP解压
        /// </summary>
        /// <param name="zippedData"></param>
        /// <returns></returns>
          byte[] Decompress(byte[] zippedData)
        {
            MemoryStream ms = new MemoryStream(zippedData);
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Decompress);
            MemoryStream outBuffer = new MemoryStream();
            byte[] block = new byte[1024];
            while (true)
            {
                int bytesRead = compressedzipStream.Read(block, 0, block.Length);
                if (bytesRead <= 0)
                    break;
                else
                    outBuffer.Write(block, 0, bytesRead);
            }
            compressedzipStream.Close();
            return outBuffer.ToArray();
        }
        public  string GZipCompressString(string rawString)
        {
            if (string.IsNullOrEmpty(rawString) || rawString.Length == 0)
            {
                return "";
            }
            else
            {
                byte[] rawData = System.Text.Encoding.UTF8.GetBytes(rawString.ToString());
                byte[] zippedData = Compress(rawData);
                return (string)(Convert.ToBase64String(zippedData));
            }

        }
          byte[] Compress(byte[] rawData)
        {
            MemoryStream ms = new MemoryStream();
            GZipStream compressedzipStream = new GZipStream(ms, CompressionMode.Compress, true);
            compressedzipStream.Write(rawData, 0, rawData.Length);
            compressedzipStream.Close();
            return ms.ToArray();
        }
        internal head[] gethead(JObject obj)
        {
            head [] coll = new head[obj.Count];
            int i = 0;
            foreach (var item in obj)
            {
                // JTokenType jtt = item.Value.Type;
                //     object objc = insertmarshal(item.Value);
                //  datamode dm = new datamode(jtt, objc); 
                // coll.Add(item.Key, objc);
                head hh = new head();
                coll[i] = hh;
                coll[i].key = item.Key;
                coll[i].index = i;
                coll[i].type = (byte)item.Value.Type;
                i++;
            }


            return coll;
        }
        internal head[] gethead(JObject obj, head[] heads)
        {
            //  head[] coll = new head[obj.Count];
            head[] temp = heads;
            foreach (var item in obj)
            {
                bool bb = false;
                foreach (head hd in heads)
                {
                    if (item.Key == hd.key)
                    {
                        bb = true;
                        break;
                    }
                    //coll[i].key = item.Key;
                    //coll[i].index = i;

                }
                if (!bb)
                {
                    temp = new head[heads.Length+1];
                    heads.CopyTo(temp, 0);
                    temp[temp.Length-1] = new head();
                    temp[temp.Length-1].key = item.Key;
                    temp[temp.Length-1].index = temp.Length-1;
                    temp[temp.Length - 1].type = (byte)item.Value.Type;

                    heads = temp;
                }
            }


            return temp;
        }
        internal object insertmarshal(JToken Value)
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

 

        internal ConcurrentQueue<void*[]> listutem = new ConcurrentQueue<void*[]>();
        internal Hashtable gethtable(object obj)
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
      
        internal bool logicaljudgement(byte[] sst, bool[] bbs)
        {
            bool rb;
            rb = bbs[0];
            int len = sst.Length;
            for (int i = 0; i < len; i++)
            {
                if (sst[i] == 0)
                    rb = rb && bbs[i + 1];
                if (sst[i] ==1)
                    rb = rb || bbs[i + 1];
            }

            return rb;
        }
        internal byte[] logicalSplit(string[] sst)
        {
            List<byte> lists = new List<byte>();
            foreach (string s in sst)
            {
                if (s == "&&" )
                    lists.Add(0);
                if (s == "||")
                    lists.Add(1);
            }
            return lists.ToArray();
        }
        internal string Stringtonosymbol(String _sqlsst)
        {
            if (_sqlsst == "''")
                return "";
            Regex r = new Regex("'(.+)'|''"); // 定义一个Regex对象实例
            var m= r.Match(_sqlsst);
            
             
                if (m.Success)
                {
                  return m.Value.Substring(1, m.Value.Length-2);
                }
            return _sqlsst;
        }
            internal string[] sqltolist(String _sqlsst)
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
          string []   sst = _sqlsst.Split(' ');
            for (int ssti = 0; ssti < sst.Length; ssti++)
                sst[ssti] = string.Format(sst[ssti], sspr);
            return sst;
        }
        Regex r = new Regex("==|>=|<=|<|>"); // 定义一个Regex对象实例
        internal string logicalContrastSplit(string s)
        {

            Match m = r.Match(s); // 在字符串中匹配
            if (m.Success)
            {
                return m.Value;
            }
            //foreach (string s in sst)

            return "";
        }
        internal void*[][] sort(void*[][] objsall, head orhe,int order)
        {
            if(order==0)
               objsall = QuickSort(objsall, orhe, 0, objsall.Length - 1,true);
            else
               objsall = QuickSort(objsall, orhe, 0, objsall.Length - 1,false);
            return objsall;
        }

         


        private unsafe int Division(void*[][] list, head orhe, int left, int right,bool ord)
        {

            while (left < right)
            {
                void* p1 = list[left][orhe.index];
                void*[] listvoid = list[left];

                bool bba = false;
                if (orhe.type == 6)
                {
                    //int value = (int)(*(int*)p1);
                    int num = (int)(*(int*)p1);


                    int left1 = (int)(*(int*)list[left + 1][orhe.index]);
                    bba = num > left1;

                }
                else if (orhe.type == 9)
                {
                    //  bool value = (bool)(*(bool*)p1);
                    byte num = Convert.ToByte((bool)(*(bool*)p1));


                    byte left1 = Convert.ToByte((bool)(*(bool*)list[left + 1][orhe.index]));
                    bba = num > left1;
                }
                else if (orhe.type == 7)
                {
                    //  double value = (double)(*(double*)p1);
                    double num = (double)(*(double*)p1);


                    double left1 = (double)(*(double*)list[left + 1][orhe.index]);
                    bba = num > left1;


                }
                else if (orhe.type == 12)
                {


                    long num = (long)(*(long*)p1);


                    long left1 = (long)(*(long*)list[left + 1][orhe.index]);
                    bba = num > left1;

                }
                else if (orhe.type == 8)
                {
                    string vv = Marshal.PtrToStringAnsi((IntPtr)p1);
                    if (vv != "")
                    {
                        int num = Asc(vv.Substring(0, 1));
                        string vv2 = Marshal.PtrToStringAnsi((IntPtr)list[left + 1][orhe.index]);

                        long left1 = Asc(vv2.Substring(0, 1));
                        bba = num > left1;

                    }
                }



                if (bba== ord)
                {
                    list[left] = list[left + 1];
                    list[left + 1] = listvoid;
                    left++;
                }
                else
                {
                    void*[] temp = list[right];
                    list[right] = list[left + 1];
                    list[left + 1] = temp;
                    right--;
                }
                
            }
            
            return left; //指向的此时枢轴的位置
        }
        private void*[][] QuickSort(void*[][] list, head orhe, int left, int right,bool ord)
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
            int intAsciiCode = (int)asciiEncoding.GetBytes(character)[0];
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
    }
}
