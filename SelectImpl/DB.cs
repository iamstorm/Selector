using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using SelectImpl;

namespace SelectImpl
{
    public class DB
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
            SQLiteHelper sh;
            if (connDict_.TryGetValue(sKey, out sh))
            {
                return sh;
            }
            SQLiteConnection conn = new SQLiteConnection(String.Format("Data Source={0};Version=3;", sDBPath));
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand(conn);
            sh = new SQLiteHelper(cmd);
            connDict_[sKey] = sh;
            return sh;
        }
        public static void UnRegConn(String sKey)
        {
            SQLiteHelper sh;
            if (connDict_.TryGetValue(sKey, out sh))
            {
                sh.cmd.Connection.Dispose();
                sh.cmd.Dispose();
            }
        }
    }
}
