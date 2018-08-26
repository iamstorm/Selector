using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace SelectImpl
{
    public static class StrategySetting
    {
        public static String MissStrategyName = "miss";
        public static String DontbuyStrategyName = "dontBuy";
    }
    public enum FocusOn
    {
        FO_New,
        FO_Old,
        FO_All
    }
    public interface IStrategy
    {
#region meta data
        String verTag();
        String name();
        Dictionary<String, String> paramters();
        FocusOn focusOn();
#endregion
        String dbKey();
        SQLiteHelper sh();


        Dictionary<String, String> setup();

        Dictionary<String, String> select(DataStoreHelper dsh, Dictionary<String, String> param, ref String sigDate);
        String computeBonus(Stock stock, int buyDate, out bool bSellWhenMeetMyBounusLimit, out int sellDate);
        float bounusLimit();
    }

}
