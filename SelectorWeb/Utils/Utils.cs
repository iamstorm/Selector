using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace SelectorWeb.Utils
{
    public class U
    {
        public static int Date(DateTime dateTime)
        {
            return ToType<int>(dateTime.ToString("yyyyMMdd"));
        }
        public static int NowDate()
        {
            return Date(DateTime.Now);
        }
        public static T ToType<T>(object obj)
        {
            return (T)Convert.ChangeType(obj.ToString(), typeof(T));
        }
        public static bool IsTradeDay(int date)
        {
            return DB.Global().ExecuteScalar<int>(String.Format("Select Count(1) From trade_date Where cal_date = '{0}'", date)) > 0;
        }
        public static bool NowIsTradeDay()
        {
            return IsTradeDay(NowDate());
        }
        public static void SetSysInfo(SQLiteHelper sh, String sKey, String sVal, bool bInsertIfNotExist = true)
        {
            int ret = sh.Execute(String.Format("Update SysInfo Set Val = '{0}' Where Key = '{1}'", sVal, sKey));
            if (bInsertIfNotExist && ret == 0)
            {
                sh.Execute(String.Format("Insert Into SysInfo(Key, Val) Values ('{0}', '{1}')", sKey, sVal));
            }
        }
        public static String GetSysInfo(SQLiteHelper sh, String sKey, String sDefVal = "")
        {
            object obj = sh.ExecuteScalar(String.Format("Select Val From SysInfo Where Key = '{0}'", sKey));
            if (obj == null)
            {
                return sDefVal;
            }
            else
            {
                return obj.ToString();
            }
        }
        public static String ToTimeDesc(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}