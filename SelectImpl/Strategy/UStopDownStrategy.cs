using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class UStopDownStrategy : TodayBuyTomorowSellStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "UStopDown";
        }
        public float bounusLimit()
        {
            return 0.032f;
        }
        Dictionary<String, String> IStrategy.paramters()
        {
            return null;
        }
        #endregion
        Dictionary<String, String> IStrategy.setup()
        {
            return null;
        }

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, Dictionary<String, String> param, ref String sigDate)
        {
            var zf = dsh.Ref(Info.ZF);
            if (zf > -0.03 || zf < -0.05)
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
            bool bHasHFTooHight = false;
            bool bHasLFTooLow = false;
            float otherMaxZF = 0;
            float otherMaxC = 0;
            for (int i = 1; i < 8; ++i )
            {
                var curOf = dsh.Ref(Info.OF, i);
                var curHf = dsh.Ref(Info.HF, i);
                var curZf = dsh.Ref(Info.ZF, i);
                var curPreZf = dsh.Ref(Info.ZF, i+1);
                var vol = dsh.Ref(Info.V, i);
                if (curOf < 0.095 && curHf > 0.095 && curZf < 0.095 && curZf > 0.01 && curPreZf < 0.095)
                {
                    iSigDateIndex = i;
                    sigDateVol = vol;
                    break;
                }
                otherMaxVol = Math.Max(vol, otherMaxVol);
                if (curZf > 0)
                {
                    nUpCount++;
                }
                if (curZf > 0.095)
                {
                    nUStopCount++;
                } else if (curZf < -0.095)
                {
                    nDStopCount++;
                }
                if (curZf < -0.03)
                {
                    bMeetTradeSigAllready = true;
                }
                else if (curZf > 0.015)
                {
                    bMeetRealUp = true;
                }
//                 if (curHf > 0.05)
//                 {
//                     bHasHFTooHight = true;
//                 }
                if (dsh.UpShadow(i) > 0.03)
                {
                    bHasHFTooHight = true;
                }

                if (dsh.DownShadow(i) < -0.03)
                {
                    bHasLFTooLow = true;
                }
                otherMaxZF = Math.Max(otherMaxZF, curZf);
                otherMaxC = Math.Max(dsh.Ref(Info.C, i), otherMaxC);
            }
            if (iSigDateIndex == -1)
            {
                return null;
            }
            if (nUStopCount > 0 || nDStopCount > 0 || bMeetTradeSigAllready || !bMeetRealUp)
            {
                return null;
            }
             if (sigDateVol < otherMaxVol)
             {
                 return null;
             }
            if (dsh.Ref(Info.ZF, iSigDateIndex-1) < 0 &&
                 dsh.Ref(Info.ZF, iSigDateIndex - 2) < 0)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 1) < 0.005 &&
                 dsh.Ref(Info.ZF, 2) < 0.005 &&
                 dsh.Ref(Info.ZF, 3) < 0.005)
            {
                return null;
            }
//             if (Math.Abs(dsh.Ref(Info.C, 1) - dsh.Ref(Info.O, 1)) >
//                 Math.Abs(dsh.Ref(Info.C) - dsh.Ref(Info.O)))
//             {
//                 return null;
//             }
            if (dsh.Ref(Info.C, iSigDateIndex) < dsh.Ref(Info.O, iSigDateIndex))
            {
                return null;
            }
             if (dsh.stock_.code_ == "603099")
             {
                 int a = 3;
                 a = 4;
             }
             var delta = (dsh.Ref(Info.C) - dsh.Ref(Info.O, 1))/dsh.Ref(Info.C, 1);
             if (delta > -0.01)
             {
                 return null;
             }
             if (dsh.Ref(Info.LF) < -0.06)
             {
                 return null;
             }
             if (bHasHFTooHight || bHasLFTooLow)
             {
                 return null;
             }
             if (dsh.Ref(Info.OF) < -0.018)
             {
                 return null;
             }
             if (otherMaxZF + zf > 0)
             {
                 return null;
             }
             if ((otherMaxC - dsh.Ref(Info.C, iSigDateIndex))/dsh.Ref(Info.C, iSigDateIndex) < 0.015)
             {
                 return null;
             }
            sigDate = dsh.Date(iSigDateIndex).ToString();
            if (nUpCount < 2)
            {
                return null;
            }
            var rateItemDict = new Dictionary<string, string>();
            if (bMeetTradeSigAllready)
            {
                rateItemDict["OutDate/Y"] = "";
            }
            else
            {
                rateItemDict["OutDate/N"] = "";
            }
            return rateItemDict;
        }
    }
}
