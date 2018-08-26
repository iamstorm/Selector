using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class LStopUpStrategy : TodayBuyTomorowSellStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "LStopUp";
        }
        Dictionary<String, String> IStrategy.paramters()
        {
            return null;
        }
        #endregion
        Dictionary<String, String> IStrategy.setup()
        {
            return null;
        }

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, Dictionary<String, String> param, ref String sigDate)
        {
            var zf = dsh.Ref(Info.ZF);
            if (zf > -0.095 && zf < -0.05)
            {
                return EmptyRateItemButSel;
            }
            return null;
        }
    }
}
