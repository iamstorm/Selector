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
        public override float bounusLimit()
        {
            return 0.095f;
        }
        #endregion
        Dictionary<String, String> selectPre(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            //              if (zf < 0.02 || zf > 0.03)
            //              {
            //                  return null;
            //              }
            if (zf < 0.02)
            {
                return null;
            }
            if (dsh.Ref(Info.HF) > 0.095)
            {
                return null;
            }
            if (dsh.SZRef(Info.ZF) > -0.015)
            {
                return null;
            }
            if (!dsh.IsReal())
            {
                return null;
            }
            if (dsh.UpShadow() < 0.04)
            {
                return null;
            }
             if (dsh.DownShadow() < -0.04)
             {
                 return null;
             }
             var of = dsh.Ref(Info.OF);
             if (of > 0.02 || of < -0.04)
             {
                 return null;
             }
            return EmptyRateItemButSel;
        }

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
            if (dsh.Ref(Info.ZF, 1) < 0)
            {
                return null;
            }
            int iSigIndex = -1;
            var maxCO = float.MinValue;
            var minZF = float.MaxValue;
            for (int i = 1; i < 8; ++i )
            {
                var curZF = dsh.Ref(Info.ZF, i);
                if (curZF < 0 && dsh.HH(Info.CO, 10, i) == i && dsh.Ref(Info.CO, i) / dsh.Ref(Info.CO, dsh.HH(Info.CO, 10, i + 1, -1)) > 3)
                {
                    iSigIndex = i;
                    if (curZF < -0.095)
                    {
                        return null;
                    }
                    break;
                }
                if (i > 1 && curZF > 0)
                {
                    return null;
                }
                minZF = Math.Min(minZF, curZF);
                if (curZF < 0)
                {
                    maxCO = Math.Max(maxCO, dsh.Ref(Info.CO, i));
                }
            }
            if (iSigIndex == -1)
            {
                return null;
            }
            if (maxCO > dsh.Ref(Info.CO, iSigIndex))
            {
                return null;
            }
            if (dsh.Ref(Info.CO) * 2 > dsh.Ref(Info.CO, iSigIndex))
            {
                return null;
            }
            if (minZF < dsh.Ref(Info.ZF, iSigIndex))
            {
                return null;
            }
            return EmptyRateItemButSel;
        }
    }
}
