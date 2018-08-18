using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectorForm
{
    public class LStopUpStrategy : BaseStrategyImpl, IStrategy
    {
        #region meta data
        int IStrategy.version()
        {
            return 1;
        }
        String IStrategy.name()
        {
            return "LStopUp";
        }
        Dictionary<String, String> IStrategy.paramters()
        {
            return null;
        }
        String[] IStrategy.rateItemNames()
        {
            return new String[] { "1", "2" };
        }
        bool IStrategy.focusOnNew()
        {
            return false;
        }
        #endregion
        Dictionary<String, String> IStrategy.setup()
        {
            return null;
        }

        String[] IStrategy.select(DataStore store, Stock stock, int iIndex, Dictionary<String, String> param)
        {
            DataStoreHelper dsh = store.helper(stock.code_, iIndex);
            if (dsh.Ref(Info.ZF)<-0.095)
            {
                return new String[0];
            }
            return null;
        }
        int IStrategy.rate(String[] rateItems)
        {
            return 1;
        }

    }
}
