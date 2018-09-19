using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class UUDownStrategy : TodayBuyTomorowSellStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "UUDown";
        }
        public override float bounusLimit()
        {
            return 0.095f;
        }
        #endregion

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigDate)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0 || zf < -0.06)
            {
                return null;
            }
            if (!dsh.IsReal())
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
            bool bMeetTradeSigAllready = false;
            bool bMeetRealUp = false;
            float sigDateVol = 0;
            float otherMaxVol = 0;
            bool bHasUpShadowTooHight = false;
            bool bHasDownShadowTooLow = false;
            float otherMaxZF = 0;
            float otherMaxC = 0;
            float otherMaxH = 0;
            float sumOfSigDateZF = 0;
            float totalUp = 0;
            float totalDown = 0;
            for (int i = 1; i < 8; ++i)
            {
                var curOf = dsh.Ref(Info.OF, i);
                var curHf = dsh.Ref(Info.HF, i);
                var curZf = dsh.Ref(Info.ZF, i);
                var curPreZf = dsh.Ref(Info.ZF, i + 1);
                var vol = dsh.Ref(Info.V, i);
                if (curOf < 0.03 && curZf > 0.03 && curZf < 0.095 &&
                    dsh.Ref(Info.OF, i + 1) < 0.03 && dsh.Ref(Info.ZF, i + 1) > 0.03)
                {
                    for (int j = i+2; j < i+5; j++)
                    {
                        if (dsh.Ref(Info.ZF, j) < 0)
                            break;

                        if (dsh.Ref(Info.ZF, j) > 0.05)
                        {
                            return null;
                        }
                    }
                    float preVol = dsh.Ref(Info.V, i+1);
                    float chkVol = Math.Max(vol, preVol);
                    for (int j = i + 2; j < i + 6; ++j)
                    {
                        if (dsh.Ref(Info.V, j) > chkVol * 2)
                        {
                            return null;
                        }
                    }
                    sumOfSigDateZF = curZf + dsh.Ref(Info.ZF, i+1);
                    if (sumOfSigDateZF > 0.1)
                    {
                        iSigDateIndex = i;
                        sigDateVol = Math.Max(dsh.Ref(Info.V, i + 1), vol);
                        break;
                    }
                }
                otherMaxVol = Math.Max(vol, otherMaxVol);
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
                if (curZf < -0.03)
                {
                    bMeetTradeSigAllready = true;
                }
                else if (curZf > 0.012)
                {
                    bMeetRealUp = true;
                }

                if (curZf > 0)
                {
                    totalUp += curZf;
                }
                else
                {
                    totalDown += curZf;
                }
                if (dsh.UpShadow(i) > 0.03)
                {
                    bHasUpShadowTooHight = true;
                }

                if (dsh.DownShadow(i) < -0.03)
                {
                    bHasDownShadowTooLow = true;
                }
                otherMaxZF = Math.Max(otherMaxZF, curZf);
                otherMaxC = Math.Max(dsh.Ref(Info.C, i), otherMaxC);
                otherMaxH = Math.Max(dsh.Ref(Info.H, i), otherMaxH);
            }
            if (iSigDateIndex == -1)
            {
                return null;
            }
            sigDate = dsh.Date(iSigDateIndex).ToString();
            if (dsh.Ref(Info.C) < dsh.Ref(Info.L, iSigDateIndex))
            {
                return null;
            }
            if (dsh.Ref(Info.ZF) + Math.Min(dsh.Ref(Info.ZF, iSigDateIndex), dsh.Ref(Info.ZF, iSigDateIndex+1)) < -0.01)
            {
                return null;
            }
            if (totalDown + totalUp < -0.01)
            {
                return null;
            }
            if (nUpCount < 2)
            {
                return null;
            }

            if (/*nUStopCount > 0 ||*//* nDStopCount > 0 || */bMeetTradeSigAllready || !bMeetRealUp)
            {
                return null;
            }

            float maxUpF = (otherMaxC - dsh.Ref(Info.C, iSigDateIndex)) / dsh.Ref(Info.C, iSigDateIndex);
            if (maxUpF < 0.015/* || maxUpF > 0.04*/)
            {
                return null;
            }
            if (sigDateVol < otherMaxVol * 1.2)
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
            if (bHasUpShadowTooHight || bHasDownShadowTooLow)
            {
                return null;
            }
            //             if (dsh.Ref(Info.OF) < -0.02)
            //             {
            //                 return null;
            //             }

            if (
                 dsh.Ref(Info.ZF, 2) < 0.005 &&
                 dsh.Ref(Info.ZF, 3) < 0.005)
            {
                return null;
            }

            //             if (dsh.Ref(Info.ZF, iSigDateIndex-1) < 0 &&
            //                  dsh.Ref(Info.ZF, iSigDateIndex - 2) < 0)
            //             {
            //                 return null;
            //             }

            if (dsh.Ref(Info.C, iSigDateIndex) < dsh.Ref(Info.O, iSigDateIndex))
            {
                return null;
            }
            if (dsh.Ref(Info.C, iSigDateIndex + 1) < dsh.Ref(Info.O, iSigDateIndex + 1))
            {
                return null;
            }
 
           
            var delta = (dsh.Ref(Info.C) - dsh.Ref(Info.O, 1)) / dsh.Ref(Info.C, 1);
            if (delta > -0.01)
            {
                return null;
            }
            var ret = new Dictionary<String, String>();
            ret[String.Format("sumSig/{0}", sumOfSigDateZF>0.12 ? "1": "0")] = "";
            ret[String.Format("delta/{0}", delta < -0.02 ? "1" : "0")] = "";
            ret[String.Format("maxUp/{0}", maxUpF > 0.02 ? "1" : "0")] = "";
            ret[String.Format("diffzf/{0}", otherMaxZF + zf > -0.02 ? "1" : "0")] = "";
            return ret;
        }
    }
}
