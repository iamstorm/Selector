using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class LStopUpStrategy : CloseBuyStrategy, IStrategy
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
        public override float bonusLimit()
        {
            return 0.052f;
        }
        public override float firstDayBonusLimit()
        {
            return 0.095f;
        }
        public override int buySpan()
        {
            return 1;
        }
        #endregion
      
        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if ( zf > -0.02)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 1) > -0.05)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 2) < 0.03)
            {
                return null;
            }
            return EmptyRateItemButSel;
        }
    }
}
