using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace SelectImpl
{
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
    public class BuySellInfo
    {
        public bool bSellWhenMeetMyBounusLimit_ = true;
        public int sellDate_ = -1;
        public int tradeSpan_ = -1;
        public String allBonus_ = "";
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
        String computeBonus(SelectItem item, Stock stock, int buyDate, out BuySellInfo info);
        float bonusLimit();
    }

    public interface IStrategy_LF : IStrategy
    {
        float buyLimit();
    }
    public interface IStrategy_LF_Minute : IStrategy
    {
    }
}
