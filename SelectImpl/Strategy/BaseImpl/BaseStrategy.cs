using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace SelectImpl
{
    public abstract class BaseStrategyImpl
    {
        public string dbKey()
        {
            IStrategy stra = (IStrategy)this;
            return "$stra_" + stra.name();
        }
        public SQLiteHelper sh()
        {
            return DB.connDict_[dbKey()];
        }
        virtual public FocusOn focusOn()
        {
            return FocusOn.FO_All;
        }
        virtual public float bounusLimit()
        {
            return Setting.MyDefBounusLimit;
        }
        public static Dictionary<String, String> EmptyRateItemButSel = new Dictionary<string, string>();
    }
}
