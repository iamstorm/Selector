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
    public enum SelectMode
    {
        SM_Regress,
        SM_SelectOpen,
        SM_SelectInDay,
        SM_SelectClose
    }
    public interface IStrategy
    {
#region meta data
        String verTag();
        String name();
        FocusOn focusOn();
#endregion
        String dbKey();
        SQLiteHelper sh();


        Dictionary<String, String> select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo);
        String computeBonus(Stock stock, int buyDate, out bool bSellWhenMeetMyBounusLimit, out int sellDate);
        float bounusLimit();
    }

    public interface IStrategy_LF : IStrategy
    {
        float buyLimit();
        float ofBonusLimit();
    }
}
