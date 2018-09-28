using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class SigEnvDownStrategy : CloseBuyStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "SigEnvDown";
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

            if (zf > 0 || zf < -0.06)
            {
                return null;
            }
            if (dsh.Ref(Info.OF, 1) < -0.03)
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
            bool bMeetTradeSigAllready = false;
            for (int i = 1; i < 8; ++i)
            {
                var curOf = dsh.Ref(Info.OF, i);
                var curHf = dsh.Ref(Info.HF, i);
                var curZf = dsh.Ref(Info.ZF, i);
                var curPreZf = dsh.Ref(Info.ZF, i + 1);
                var vol = dsh.Ref(Info.V, i);
                if (selectPre(dsh.newDsh(i), selectMode, ref sigInfo) != null)
                {
                    iSigDateIndex = i;
                    sigDateVol = vol;
                    sigZF = curZf;
                    for (int j = i + 1; j < i + 5; ++j)
                    {
                        if (dsh.Ref(Info.V, j) > vol * 2 && dsh.Ref(Info.ZF, j) < 0)
                        {
                            return null;
                        }
                     }
                    break;
                }
                if (curOf < -0.04)
                {
                    return null;
                }
                if (curZf > 0 && dsh.IsReal(i))
                {
                    nUpCount++;
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
            if (sigDateVol > otherMaxVol * 2)
            {
                return null;
            }
            if (otherMaxZF + zf > 0)
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
            var ret = new Dictionary<String, String>();
            ret[String.Format("delta/{0}", delta < -0.02 ? "1" : "0")] = "";
            ret[String.Format("maxUp/{0}", maxUpF > 0.02 ? "1" : "0")] = "";
            ret[String.Format("prezf/{0}", dsh.Ref(Info.ZF, 1) > 0 ? "1" : "0")] = "";
            ret[String.Format("maxco/{0}", dsh.Ref(Info.CO, 1) / dsh.Ref(Info.C, 1) > 0.01 ? "1" : "0")] = "";
            return ret;
        }
    }
}
