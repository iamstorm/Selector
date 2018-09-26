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
        public override float bounusLimit()
        {
            return 0.095f;
        }
        #endregion
        Dictionary<String, String> selectPre(DataStoreHelper dsh, SelectMode selectMode, ref String sigDate)
        {
            var zf = dsh.Ref(Info.ZF);
            if (zf < -0.095 || zf > 0.095)
            {
                return null;
            }
            if (dsh.IsLikeSTStop())
            {
                return null;
            }
            if (dsh.HH(Info.CO, 20) == 0)
            {
                return EmptyRateItemButSel;
            }
            return null;
        }

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigDate)
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
            if (dsh.Ref(Info.OF) > 0.04 || dsh.UpShadow() > 0.04)
            {
                return null;
            }

            int iSigIndex = -1;
            for (int i = 1; i < 8; ++i)
            {
                if (dsh.DownShadow(i) < -0.05)
                {
                    iSigIndex = i;
                    if (dsh.AccZF(8, iSigIndex) > -0.15)
                    {
                        return null;
                    }
                    break;
                }
            }
            if (iSigIndex == -1)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 1) > 0.05)
            {
                return null;
            }
            if (dsh.Ref(Info.C) < dsh.Ref(Info.H, iSigIndex))
            {
                return null;
            }

            return EmptyRateItemButSel;
        }
    }
}
