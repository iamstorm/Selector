using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class UpDownDownUpStrategy : CloseBuyStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "UpDownDownUp";
        }
        #endregion

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (zf < 0.01)
            {
                return null;
            }
            if (dsh.IsLikeSTStop())
            {
                return null;
            }
            if (dsh.DownShadow() < -0.04)
            {
                return null;
            }

            if ((dsh.Ref(Info.C, 1) - dsh.Ref(Info.L, 3)) / dsh.Ref(Info.C, 1) > 0)
            {
                return null;
            }
            if (dsh.Ref(Info.HL, 1) > dsh.Ref(Info.HL, 2) * 2)
            {
                return null;
            }
            if (dsh.Ref(Info.HL, 2) > dsh.Ref(Info.HL, 1) * 2)
            {
                return null;
            }
            if (dsh.Ref(Info.C) < dsh.Ref(Info.O, 1))
            {
                return null;
            }
            if ((dsh.Ref(Info.C, 1) - dsh.Ref(Info.O, 5))/dsh.Ref(Info.C, 1) > 0.01)
            {
                return null;
            }
            if (dsh.Ref(Info.L) < dsh.Ref(Info.L, 1) && dsh.Ref(Info.H) > dsh.Ref(Info.H, 1))
            {
                return null;
            }

            if (dsh.AccZF(2, 1) < -0.1)
            {
                return null;
            }
            if (dsh.AccZF(3)  > 0.02)
            {
                return null;
            }
            if (dsh.AccZF(2) < -0.04)
            {
                return null;
            }
            var accZF6 = dsh.AccZF(6);
            if (accZF6 > 0.08)
            {
                return null;
            }
            int nDownCount = 2;
            if (dsh.EveryDown(1) != nDownCount)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, nDownCount+1) < 0.01)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, dsh.HH(Info.ZF, 6)) < 0.03)
            {
                return null;
            }

            if (dsh.Ref(Info.ZF, nDownCount + 2) > -0.01)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, nDownCount + 3) < 0.01)
            {
                return null;
            }
            var upV = dsh.Acc(Info.V, 6, 0, 1);
            var downV = dsh.Acc(Info.V, 6, 0, -1);
            if (upV > downV * 1.5)
            {
                return null;
            }

            for (int i = 0; i <= 5; ++i)
            {
                var curOF = dsh.Ref(Info.OF, i);
                var curZF = dsh.Ref(Info.ZF, i);
                if (!dsh.IsReal(i))
                {
                    return null;
                }
                if (curZF > 0.095 || curZF < -0.095)
                {
                    return null;
                }
                if (curOF > 0.02 && curZF < 0)
                {
                    return null;
                }
                if (curOF < -0.02 && curZF < 0)
                {
                    return null;
                }
                if (dsh.Ref(Info.CO, i) / dsh.Ref(Info.C, i+1) < 0.006)
                {
                    return null;
                }
            }

            var szZF = dsh.SZRef(Info.ZF);
            if (szZF > 0 && szZF < 0.011 && dsh.SZAcc(Info.ZF, 7, 1) > 0.03)
            {
                return null;
            }
            if (!selectBySZ(dsh))
            {
                return null;
            }
            return EmptyRateItemButSel;
        }
   
    }
}
