﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class LF_UStopDownStrategy : LFBuyStrategy, IStrategy_LF
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "LF_UStopDown";
        }
        float IStrategy_LF.buyLimit()
        {
            return -0.05f;
        }
        #endregion

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, SelectMode selectMode, ref String sigInfo)
        {
            IStrategy_LF stra = (IStrategy_LF)this;
            if (!meetBuyChance(dsh, selectMode))
            {
                return null;
            }

            if (dsh.Ref(Info.OF) > 0.04 || dsh.Ref(Info.OF) < -0.015 || dsh.Ref(Info.ZF, 1) < -0.02 || dsh.Ref(Info.OF, 1) < -0.03)
            {
                return null;
            }
            if (dsh.Ref(Info.CO, 1)/dsh.Ref(Info.C, 1) < 0.01)
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
            bool bHasUpShadowTooHight = false;
            bool bHasDownShadowTooLow = false;
            float otherMaxVol = float.MinValue;
            float otherMaxC = float.MinValue;
            float otherMaxH = float.MinValue;
            float otherMaxZF = float.MinValue;
            float sigDateVol = 0;
            float sigZF = 0;
            float minDownShadow = float.MaxValue;
            float maxUpShadow = float.MinValue;
            float otherMinZF = float.MaxValue;
            float totalUp = 0;
            float totalDown = 0;
            for (int i = 1; i < 8; ++i )
            {
                var curOf = dsh.Ref(Info.OF, i);
                var curHf = dsh.Ref(Info.HF, i);
                var curZf = dsh.Ref(Info.ZF, i);
                var curPreZf = dsh.Ref(Info.ZF, i+1);
                var vol = dsh.Ref(Info.V, i);
                if (curZf > 0.095)
                {
                    return null;
                }
                if (curOf < 0.04 && curHf > 0.095 && curZf < 0.095 && curZf > 0)
                {
                    iSigDateIndex = i;
                    sigDateVol = vol;
                    sigZF = curZf;
                    for (int j = i + 1; j < i + 5; ++j )
                    {
                        if (dsh.Ref(Info.V, j) > vol*2)
                        {
                            return null;
                        }
                    }
                    if (dsh.AccZF(8, i+1) < -0.15)
                    {
                        return null;
                    }
                    if (dsh.AccZF(3, i + 1) > 0.095)
                    {
                        return null;
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
                if (curZf < 0 && dsh.IsReal(i))
                {
                    nDownCount++;
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
                else if (curZf > 0.012)
                {
                    bMeetRealUp = true;
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
                otherMaxC = Math.Max(dsh.Ref(Info.C, i), otherMaxC);
                otherMaxH = Math.Max(dsh.Ref(Info.H, i), otherMaxH);
                otherMaxZF = Math.Max(otherMaxZF, curZf);
                otherMinZF = Math.Min(otherMinZF, curZf);
            }
            if (iSigDateIndex == -1)
            {
                return null;
            }
            sigInfo = dsh.Date(iSigDateIndex).ToString();

            if (dsh.Ref(Info.ZF, iSigDateIndex) + dsh.Ref(Info.ZF, iSigDateIndex+1) < -0.01)
            {
                return null;
            }
            if (sigDateVol > otherMaxVol * 2)
            {
                return null;
            }
            if (otherMinZF > 0.03)
            {
                return null;
            }
            if (totalDown + totalUp < -0.01)
            {
                return null;
            }
            if (otherMaxH < dsh.Ref(Info.H, iSigDateIndex))
            {
                return null;
            }
            if (dsh.Ref(Info.C, 1) * (1 + stra.buyLimit() - 0.01) < dsh.Ref(Info.L, iSigDateIndex))
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
            if (/*nUStopCount > 0 ||*//* nDStopCount > 0 || */bMeetTradeSigAllready || !bMeetRealUp)
            {
                return null;
            }
             float maxUpF = (otherMaxC - dsh.Ref(Info.C, iSigDateIndex)) / dsh.Ref(Info.C, iSigDateIndex);
             if (maxUpF < 0.02/* || maxUpF > 0.04*/)
             {
                 return null;
             }
    
//             if (sigDateVol < otherMaxVol * 1.2)
//             {
//                 return null;
//             }

//             if (dsh.Ref(Info.LF) < -0.06)
//             {
//                 return null;
//             }
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
             if (dsh.Ref(Info.ZF, iSigDateIndex - 1) < 0 && dsh.IsReal(iSigDateIndex - 1) && 
                 dsh.Ref(Info.H, iSigDateIndex - 1) > dsh.Ref(Info.H, iSigDateIndex))
             {
                 return null;
             }
//              if (dsh.Ref(Info.ZF, iSigDateIndex-1) < 0 &&
//                   dsh.Ref(Info.ZF, iSigDateIndex - 2) < 0)
//              {
//                  return null;
//              }
             if (dsh.Ref(Info.C, iSigDateIndex) < dsh.Ref(Info.O, iSigDateIndex))
             {
                 return null;
             }

             var delta = (dsh.Ref(Info.C, 1) * (1 + stra.buyLimit() + 0.01) - dsh.Ref(Info.O, 1)) / dsh.Ref(Info.C, 1);
             if (delta > -0.01)
             {
                 return null;
            }
             var ret = new Dictionary<String, String>();
             ret[String.Format("vol/{0}", sigDateVol < otherMaxVol*1.5 ? "1" : "0")] = "";
   //          ret[String.Format("maxUp/{0}", maxUpF > 0.03 ? "1" : "0")] = "";
             return ret;
        }
    }
}
