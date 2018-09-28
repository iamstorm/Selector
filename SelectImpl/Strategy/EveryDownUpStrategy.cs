using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class EveryDownUpStrategy : CloseBuyStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "EveryDownUp";
        }
        #endregion
        
        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (zf > 0.03 || zf < 0.02)
            {
                return null;
            }
            if (!dsh.IsReal())
            {
                return null;
            }
            if (dsh.UpShadow() > 0.04)
            {
                return null;
            }
            var of = dsh.Ref(Info.OF);
            if (of > 0.02 || of < -0.04)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 1) > -0.005)
            {
                return null;
            }

            if (dsh.Ref(Info.ZF, 2) < 0.005/* || !dsh.IsReal(2)*/)
            {
                return null;
            }
            if (dsh.Ref(Info.V) > dsh.Ref(Info.V, 1) * 1.5)
            {
                return null;
            }
            if ((dsh.Ref(Info.C) - dsh.Ref(Info.C, 2)) / dsh.Ref(Info.C, 1) > 0.01)
            {
                return null;
            }
            if (dsh.Ref(Info.C) > dsh.Ref(Info.O, 1))
            {
                return null;
            }
            if ((dsh.Ref(Info.L) - dsh.Ref(Info.L, 2)) / dsh.Ref(Info.C, 1) > 0.01)
            {
                return null;
            }
            int nTotalDownDays = dsh.EveryDown(3);
            if (nTotalDownDays < 4)
            {
                return null;
            }
            if (dsh.EveryUp(3 + nTotalDownDays) > nTotalDownDays)
            {
                return null;
            }
            float totalZF = dsh.AccZF(nTotalDownDays, 3);
            if (totalZF > -0.1)
            {
                return null;
            }

            return EmptyRateItemButSel;
        }
    }
}
