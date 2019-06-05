using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class LF_M_NEWStrategy : LFMBuyStrategy, IStrategy_LF_M
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "LF_M_NEW";
        }
        #endregion

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            IStrategy_LF_M stra = (IStrategy_LF_M)this;

            if (dsh.stock_.code_ != "300780") {
                return null;
            }
            if (dsh.Ref(Info.ZF) > 0.095) {
                return null;
            }

            return EmptyRateItemButSel;
        }
    }
}
