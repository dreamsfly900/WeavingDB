using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using WeavingDB.Logical;

namespace SQLDBlogic.logic
{
  public  class utli
    {
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
        /// <param name="mtsContrast">比较类型0>=，1<=,2==,3>,4<,5like</param>
        /// <param name="valuep1">数据2</param>
        /// <returns></returns>
        public static unsafe bool CompareLogical(byte type, void* p1,int mtsContrast, void* valuep1)
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

                    conbb = (value) >= sconvalue;
                }
                if (mtsContrast == 1)
                {
                    conbb = (value) <= sconvalue;
                }
                if (mtsContrast == 2)
                {
                    conbb = sconvalue == (value);
                }
                if (mtsContrast == 3)
                {
                    conbb = (value) > sconvalue;
                }
                if (mtsContrast == 4)
                {
                    conbb = (value) < sconvalue;
                }

            }
            else if (type == 9)
            {
                bool value = (bool)(*(bool*)p1);
                bool sconvalue = (bool)(*(bool*)valuep1);
                if (mtsContrast == 2)
                {
                    conbb = sconvalue == (value);
                }
                if (mtsContrast == 0)
                {

                    conbb = (value).CompareTo( sconvalue) >= 0;
                }
                if (mtsContrast == 1)
                {
                    conbb = (value).CompareTo(sconvalue) <= 0;
                }
               
                if (mtsContrast == 3)
                {
                    conbb = (value).CompareTo(sconvalue) > 0;
                }
                if (mtsContrast == 4)
                {
                    conbb = (value).CompareTo(sconvalue) < 0;
                }
            }
            else if (type == 7)
            {
                double value = (double)(*(double*)p1);
                double sconvalue = (double)(*(double*)valuep1);
                if (mtsContrast == 0)
                {
                    conbb = (value) >= sconvalue;
                }
                if (mtsContrast == 1)
                {
                    conbb = (value) <= sconvalue;
                }
                if (mtsContrast == 2)
                {
                    conbb = sconvalue == (value);
                }
                if (mtsContrast == 3)
                {
                    conbb = (value) > sconvalue;
                }
                if (mtsContrast == 4)
                {
                    conbb = (value) < sconvalue;
                }

            }
            else if (type == 12)
            {

                // conbb[bi] = Contrast<DateTime>(Convert.ToDateTime(st), Convert.ToDateTime(value), mtsContrast[ci]);
                long value = (long)(*(long*)p1);
                long sconvalue = (long)(*(long*)valuep1);
                if (mtsContrast == 0)
                {
                    conbb = value >= sconvalue;
                }
                if (mtsContrast == 1)
                {
                    conbb = value <= sconvalue;
                }
                if (mtsContrast == 2)
                {
                    conbb = value == sconvalue;
                }
                if (mtsContrast == 3)
                {
                    conbb = (value) > sconvalue;
                }
                if (mtsContrast == 4)
                {
                    conbb = (value) < sconvalue;
                }
            }
            else if (type == 8)
            {
                string value = Marshal.PtrToStringAnsi((IntPtr)p1);
                string sconvalue = Marshal.PtrToStringAnsi((IntPtr)valuep1);
                if (mtsContrast == 0)
                {

                    conbb = (value).CompareTo(sconvalue) >= 0;
                }
                if (mtsContrast == 1)
                {
                    conbb = (value).CompareTo(sconvalue) <= 0;
                }

                if (mtsContrast == 3)
                {
                    conbb = (value).CompareTo(sconvalue) > 0;
                }
                if (mtsContrast == 4)
                {
                    conbb = (value).CompareTo(sconvalue) < 0;
                }
                if (mtsContrast == 2)
                {
                  
                    conbb = Sunday.strcontains(sconvalue, (value));
                }
                if (mtsContrast == 5)
                {
                     
                    //sconvalue = sconvalue.Replace("%", "(.*)").Replace("_", "(.+){1}");
                    //conbb = Stringtonosymbol(value, "^" + sconvalue + "$");
                    if (Sunday.strSunday(value, sconvalue, 0) == 0)
                        conbb = true;
                    else
                        conbb = false;
                }
            }
            else
            {
                conbb = false;
                // throw new Exception("不支持的逻辑判断。");
            }
            return conbb;
        }

    }
}
