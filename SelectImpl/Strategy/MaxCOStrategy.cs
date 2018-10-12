using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class MaxCOStrategy : CloseBuyStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "MaxCO";
        }
        #endregion
        
        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0 || zf < -0.06)
            {
                return null;
            }
            if (dsh.Ref(Info.OF, 1) < -0.03)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 1) > 0)
            {
                return null;
            }

            int iSigDateIndex = -1;
            int nUpCount = 0;
            int nUStopCount = 0;
            int nDStopCount = 0;
            bool bMeetRealUp = false;
            bool bHasUpShadowTooHight = false;
            bool bHasDownShadowTooLow = false;
            float otherMaxVol = float.MinValue;
            float otherMaxZF = float.MinValue;
            float otherMaxC = 0;
            float otherMaxH = 0;
            float sigDateVol = 0;
            float sigZF = 0;
            float minDownShadow = float.MaxValue;
            float maxUpShadow = float.MinValue;
            float totalUp = 0;
            float totalDown = 0;
            float otherMinZF = float.MaxValue;
            float otherMaxCO = float.MinValue;
            bool bMeetTradeSigAllready = false;
            int nNotRealCount = 0;
            for (int i = 1; i < 8; ++i)
            {
                var curOf = dsh.Ref(Info.OF, i);
                var curHf = dsh.Ref(Info.HF, i);
                var curLf = dsh.Ref(Info.LF, i);
                var curZf = dsh.Ref(Info.ZF, i);
                var curPreZf = dsh.Ref(Info.ZF, i + 1);
                var vol = dsh.Ref(Info.V, i);
                var coRate = dsh.Ref(Info.CO, i) / dsh.Ref(Info.C, i + 1);
                if (curZf > 0 && curZf < 0.095 && coRate > 0.05 &&
                    vol > dsh.MA(Info.V, 5, i + 1)  && dsh.HH(Info.CO, 30, i) == i)
                {
                    for (int j = i + 1; j < i + 6; ++j)
                    {
                        if (dsh.Ref(Info.V, j) > vol && dsh.Ref(Info.C, j) < dsh.Ref(Info.O, j))
                        {
                            return null;
                        }
                    }
                    if (dsh.AccZF(8, i + 1) < -0.1)
                    {
                        return null;
                    }
                    if (dsh.AccZF(3, i+1) > 0.095)
                    {
                        return null;
                    }
                    int iMaxVolIndex = dsh.HH(Info.V, 20, i);
                    if (dsh.Ref(Info.C, iMaxVolIndex) > dsh.Ref(Info.O, iMaxVolIndex))
                    {
                        iSigDateIndex = i;
                        sigDateVol = vol;
                        sigZF = curZf;
                        break;
                    }
                }
                if (curOf < -0.04)
                {
                    return null;
                }
                if (curZf > 0 && dsh.IsReal(i))
                {
                    nUpCount++;
                }
                if (!dsh.IsReal(i))
                {
                    ++nNotRealCount;
                }
                if (curZf > 0.095)
                {
                    nUStopCount++;
                }
                else if (curZf < -0.095)
                {
                    nDStopCount++;
                }
                else if (curZf > 0.012)
                {
                    bMeetRealUp = true;
                }
                if (curZf < -0.03 && dsh.Ref(Info.ZF, i - 1) > 0)
                {
                    bMeetTradeSigAllready = true;
                }
                float upShadow = dsh.UpShadow(i);
                float downShadow = dsh.DownShadow(i);
                if (upShadow > maxUpShadow)
                {
                    maxUpShadow = upShadow;
                }
                if (downShadow < minDownShadow)
                {
                    minDownShadow = downShadow;
                }
                if (curZf > 0)
                {
                    totalUp += curZf;
                }
                else
                {
                    totalDown += curZf;
                }
                otherMaxVol = Math.Max(vol, otherMaxVol);
                otherMaxZF = Math.Max(otherMaxZF, curZf);
                otherMaxC = Math.Max(dsh.Ref(Info.C, i), otherMaxC);
                otherMaxH = Math.Max(dsh.Ref(Info.H, i), otherMaxH);
                otherMinZF = Math.Min(otherMinZF, curZf);
                otherMaxCO = Math.Max(otherMaxCO, dsh.Ref(Info.CO, i));
            }
            if (iSigDateIndex == -1)
            {
                return null;
            }
            sigInfo = dsh.Date(iSigDateIndex).ToString();
            if (otherMinZF > 0.02)
            {
                return null;
            }
            if (nNotRealCount > 1)
            {
                return null;
            }
            if (totalDown + totalUp < -0.01)
            {
                return null;
            }
            if (maxUpShadow > 0.03)
            {
                bHasUpShadowTooHight = true;
            }

            if (minDownShadow < -0.03)
            {
                bHasDownShadowTooLow = true;
            }
            if (nUpCount < 2)
            {
                return null;
            }
            if (otherMaxH < dsh.Ref(Info.H, iSigDateIndex))
            {
                return null;
            }

            if (/*nUStopCount > 0 ||*//* nDStopCount > 0 || */ bMeetTradeSigAllready || !bMeetRealUp)
            {
                return null;
            }

            float maxUpF = (otherMaxC - dsh.Ref(Info.C, iSigDateIndex)) / dsh.Ref(Info.C, iSigDateIndex);
            if (maxUpF < 0.015/* || maxUpF > 0.04*/)
            {
                return null;
            }
            if (dsh.Ref(Info.C) < dsh.Ref(Info.L, iSigDateIndex))
            {
                return null;
            }
            if (bHasUpShadowTooHight || bHasDownShadowTooLow)
            {
                return null;
            }
            if (
                 dsh.Ref(Info.ZF, 2) < 0.005 &&
                 dsh.Ref(Info.ZF, 3) < 0.005)
            {
                return null;
            }


            if (sigDateVol < otherMaxVol * 1.2)
            {
                return null;
            }
            if (sigDateVol > otherMaxVol*2)
            {
                return null;
            }
            if (otherMaxZF + zf > 0)
            {
                return null;
            }
            if (otherMaxZF + zf < -0.02)
            {
                return null;
            }

            if (dsh.Ref(Info.LF) < -0.06)
            {
                return null;
            }
            var delta = (dsh.Ref(Info.C) - dsh.Ref(Info.O, 1)) / dsh.Ref(Info.C, 1);
            if (delta > -0.01)
            {
                return null;
            }

            if (!selectBySZ(dsh))
            {
                return null;
            }
            var ret = new Dictionary<String, String>();
            ret[String.Format("delta/{0}", delta < -0.02 ? "1" : "0")] = "";
            ret[String.Format("maxUp/{0}", maxUpF > 0.02 ? "1" : "0")] = "";
            return ret;
        }
    }
}
