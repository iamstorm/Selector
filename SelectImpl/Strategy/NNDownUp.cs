using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class NNDownUpStrategy : CloseBuyStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "NNDownUp";
        }
        #endregion

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (zf < 0.02 || zf > 0.03)
            {
                return null;
            }
            if (dsh.IsLikeSTStop())
            {
                return null;
            }
            if (dsh.Ref(Info.OF) < -0.015)
            {
                return null;
            }
            if (dsh.EveryDown(1) >= 3)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 1) > -0.02 || dsh.Ref(Info.ZF, 1) < -0.05)
            {
                return null;
            }
            if (dsh.UpShadow() > 0.04)
            {
                return null;
            }
            if (dsh.Ref(Info.CO)/dsh.Ref(Info.C, 1) < 0.01)
            {
                return null;
            }
            if (dsh.AccZF(8) > -0.1)
            {
                return null;
            }
            if (dsh.AccZF(2) > 0)
            {
                return null;
            }
            if (dsh.HH(Info.V, 8) == 0)
            {
                return null;
            }
            if (dsh.DownShadow() < -0.04)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 2) < -0.01)
            {
                return null;
            }
            if (!dsh.IsReal())
            {
                return null;
            }
            
            for (int i = 1; i < 8; ++i )
            {
                var curZF = dsh.Ref(Info.ZF, i);
                var curOF = dsh.Ref(Info.OF, i);
                if (curZF < -0.095 || curZF > 0.095)
                {
                    return null;
                }
                if (curOF > 0.04 && dsh.IsGreen(i))
                {
                    return null;
                }
                if (dsh.IsUpStopEveryDay(3, i))
                {
                    return null;
                }
            }
            if (!selectBySZ(dsh))
            {
                return null;
            }

            return EmptyRateItemButSel;
        }
   
    }
}
