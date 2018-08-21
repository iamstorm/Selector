using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
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
            return new String[] {  };
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

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, Dictionary<String, String> param)
        {
            if (dsh.Ref(Info.ZF) < -0.095)
            {
                return new Dictionary<String, String>();
            }
            return null;
        }
    }
}
