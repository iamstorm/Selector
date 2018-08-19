using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using SelectImpl;

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
    }
}
