using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;

namespace SelectorForm
{
    class DB
    {
        public static Dictionary<String, SQLiteHelper> connDict_ = new Dictionary<String, SQLiteHelper>();
        static DB()
        {
            RegConn(Dist.GlobalDBFile(), "g");
        }
        public static SQLiteHelper Global()
        {
            return connDict_["g"];
        }
        public static SQLiteHelper RegConn(String sDBPath, String sKey)
        {
            SQLiteConnection conn = new SQLiteConnection(String.Format("Data Source={0};Version=3;", sDBPath));
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(conn);
            SQLiteHelper sh = new SQLiteHelper(cmd);
            connDict_[sKey] = sh;
            return sh;
        }
        public static void SetSysInfo(SQLiteHelper sh, String sKey, String sVa)
        {
            sh.Execute(String.Format("Update SysInfo Set Val = '{0}' Where Key = '{1}'", sVa, sKey));
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
    }
}
