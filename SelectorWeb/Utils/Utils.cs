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
        public static bool IsTradeDay(SQLiteHelper sh, int date)
        {
            return sh.ExecuteScalar<int>(String.Format("Select Count(1) From trade_date Where cal_date = '{0}'", date)) > 0;
        }
        public static bool NowIsTradeDay(SQLiteHelper sh)
        {
            return IsTradeDay(sh, NowDate());
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
    public class SH : SQLiteHelper, IDisposable
    {
        SQLiteConnection conn_;
        public SH()
        {
            conn_ = new SQLiteConnection(String.Format("Data Source={0};Pooling=true;Version=3;", Dist.GlobalDBFile()));
            conn_.Open();
            cmd = new SQLiteCommand(conn_);
        }
        ~SH()      
        { 
            // 为了保持代码的可读性性和可维护性,千万不要在这里写释放非托管资源的代码 
            // 必须以Dispose(false)方式调用,以false告诉Dispose(bool disposing)函数是从垃圾回收器在调用Finalize时调用的 
            Dispose(false); 
        } 
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {//释放托管状态(托管对象)。
                    cmd.Dispose();
                    conn_.Dispose();
                }

                //释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                //将大型字段设置为 null。
                disposedValue = true;

                if (disposing)
                    GC.SuppressFinalize(this);   
            }
        }
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
        }
    }
}