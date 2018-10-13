using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class VUpDownStrategy : CloseBuyStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "VUpDown";
        }
        #endregion

        float vRate_ = 2f;

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0 || zf < -0.095)
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
            if (dsh.Ref(Info.LF) < -0.06)
            {
                return null;
            }
            int iSigDateIndex = -1;
            int nUpCount = 0;
            int nDownCount = 0;
            int nUStopCount = 0;
            int nDStopCount = 0;
            bool bMeetTradeSigAllready = false;
            bool bMeetRealUp = false;
            float sigDateVol = 0;
            float otherMaxVol = 0;
            bool bHasUpShadowTooHight = false;
            bool bHasDownShadowTooLow = false;
            float otherMaxZF = 0;
            float otherMinZF = float.MaxValue;
            float otherMaxC = 0;
            float otherMaxH = 0;
            float sumOfSigDateZF = 0;
            float otherMaxDownV = 0;
            float minSigV = float.MaxValue; 
            float totalUp = 0;
            float totalDown = 0;
            for (int i = 1; i < 8; ++i)
            {
                var curOf = dsh.Ref(Info.OF, i);
                var curHf = dsh.Ref(Info.HF, i);
                var curZf = dsh.Ref(Info.ZF, i);
                var curPreZf = dsh.Ref(Info.ZF, i + 1);
                var vol = dsh.Ref(Info.V, i);
                var preVol = dsh.Ref(Info.V, i + 1);
                if (dsh.Ref(Info.OF, i) > 0.03 && curZf < 0)
                {
                    return null;
                }
                float ma5 = dsh.MA(Info.V, 10, i + 2);
                float hhv = dsh.Ref(Info.V, dsh.HH(Info.V, 5, i + 2));
                if (curZf > 0.01 && curPreZf > 0.01 && vol > 1.3 * hhv && preVol > 1.3 * hhv && (vol < vRate_ * preVol && preVol < vRate_ * vol) && vol / ma5 > vRate_ &&
                 preVol / ma5 > vRate_ && dsh.Ref(Info.ZF, i + 2) < 0.05
                    && dsh.Ref(Info.V, i + 2) / dsh.MA(Info.V, 5, i + 3) < vRate_)
                {
                    for (int j = i + 2; j < i + 7; j++)
                    {
                        if (dsh.Ref(Info.ZF, j) < -0.095)
                            return null;
                    }
                    if (dsh.AccZF(2, i) < 0.05)
                    {
                        return null;
                    }
                    {
                        iSigDateIndex = i;
                        sigDateVol = Math.Max(dsh.Ref(Info.V, i + 1), vol);
                        minSigV = Math.Min(dsh.Ref(Info.V, i + 1), vol);
                        break;
                    }
                }
                otherMaxVol = Math.Max(vol, otherMaxVol);
                if (curZf > 0.005 && dsh.IsReal(i))
                {
                    nUpCount++;
                }
                else if (curZf < -0.005 && dsh.IsReal(i))
                {
                    nDownCount++;
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
                if (curZf > 0)
                {
                    totalUp += curZf;
                }
                else
                {
                    totalDown += curZf;
                }
                if (curZf > 0.012)
                {
                    bMeetRealUp = true;
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
                otherMinZF = Math.Min(otherMinZF, curZf);
                otherMaxC = Math.Max(dsh.Ref(Info.C, i), otherMaxC);
                otherMaxH = Math.Max(dsh.Ref(Info.H, i), otherMaxH);
                if (curZf < 0)
                {
                    otherMaxDownV = Math.Max(otherMaxDownV, vol);
                }
            }
            if (iSigDateIndex == -1)
            {
                return null;
            }
  //          sigInfo = dsh.Date(iSigDateIndex).ToString();
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
            sigInfo = (otherMaxZF + zf).ToString("F4");

            if (otherMaxZF + zf < -0.02)
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

            if (dsh.Ref(Info.C, iSigDateIndex) < dsh.Ref(Info.O, iSigDateIndex))
            {
                return null;
            }
            if (dsh.Ref(Info.C, iSigDateIndex + 1) < dsh.Ref(Info.O, iSigDateIndex + 1))
            {
                return null;
            }
            if (Math.Min(dsh.Ref(Info.ZF, iSigDateIndex), dsh.Ref(Info.ZF, iSigDateIndex + 1)) + dsh.Ref(Info.ZF, iSigDateIndex + 2) < -0.01)
            {
                return null;
            }


            var delta = (dsh.Ref(Info.C) - dsh.Ref(Info.O, 1)) / dsh.Ref(Info.C, 1);

            if (delta > -0.01)
            {
                return null;
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
            
            var ret = new Dictionary<String, String>();
            ret[String.Format("sumSig/{0}", sumOfSigDateZF > 0.12 ? "1" : "0")] = "";
            ret[String.Format("delta/{0}", delta < -0.02 ? "1" : "0")] = "";
            ret[String.Format("maxUp/{0}", maxUpF > 0.02 ? "1" : "0")] = "";
            ret[String.Format("maxZF/{0}", otherMaxZF + zf > -0.01 ? "1" : "0")] = "";
            ret[String.Format("diffzf/{0}", otherMaxZF + zf > -0.02 ? "1" : "0")] = "";
            return ret;
        }
    }
}
