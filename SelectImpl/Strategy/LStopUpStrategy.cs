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
        FocusOn IStrategy.focusOn()
        {
            return FocusOn.FO_Old;
        }
        public float bounusLimit()
        {
            return 0.095f;
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

        Dictionary<String, String> selectForCrossLL(DataStoreHelper dsh, Dictionary<String, String> param, int iIndex, out int iLLIndex)
        {
            iLLIndex = -1;
            var zf = dsh.Ref(Info.ZF, iIndex);
            if (zf < -0.095)
            {
                return null;
            }

            iLLIndex = dsh.LL(Info.L, 120, iIndex+1);
            if (iLLIndex < 20)
            {
                return null;
            }
            if (!dsh.IsLowPeak(Info.C, iLLIndex, 10))
            {
                return null;
            }
            if (dsh.Ref(Info.C, iIndex) > dsh.Ref(Info.L, iLLIndex))
            {
                return null;
            }
            if (dsh.Ref(Info.C, iIndex+1) < dsh.Ref(Info.C, iLLIndex))
            {
                return null;
            }
            float maxC = 0;
            for (int i = iIndex+1; i < iLLIndex; i++)
            {
                maxC = Math.Max(maxC, dsh.Ref(Info.C, i));
            }
            if ((maxC - dsh.Ref(Info.C, iIndex)) / dsh.Ref(Info.C, iIndex) < 0.2)
            {
                return null;
            }
            return EmptyRateItemButSel;
        }
        Dictionary<String, String> selectForVol(DataStoreHelper dsh, int iIndex, Dictionary<String, String> param, ref String sigDate)
        {
            var zf = dsh.Ref(Info.ZF, iIndex);
            var preZF = dsh.Ref(Info.ZF, iIndex + 1);
            if (zf > -0.03 || zf < -0.095)
            {
                return null;
            }

            int iMaxVolIndex = -1;
            float maxVol = 0;
            for (int i = iIndex + 1; i < iIndex+15; i++)
            {
                float vol = dsh.Ref(Info.V, i);
                if (vol > maxVol)
                {
                    maxVol = vol;
                    iMaxVolIndex = i;
                }
            }
            if (iMaxVolIndex < iIndex+7)
            {
                return null;
            }
            float preVol = maxVol;
            float minVol = float.MaxValue;
            for (int i = iMaxVolIndex - 1; i > iIndex; i--)
            {
                float vol = dsh.Ref(Info.V, i);
                if (vol > preVol * 1.1)
                {
                    return null;
                }
                preVol = vol;
                minVol = Math.Min(minVol, vol);
                if (dsh.Ref(Info.ZF, i) < -0.095)
                {
                    return null;
                }
            }
            if (maxVol < minVol * 3)
            {
                return null;
            }
            if (dsh.Ref(Info.V, iIndex) < dsh.Ref(Info.V, iIndex+1) * 2)
            {
                return null;
            }

            return EmptyRateItemButSel;
        }
        Dictionary<String, String> selectForLargeSample(DataStoreHelper dsh, Dictionary<String, String> param, ref String sigDate)
        {
            var zf = dsh.Ref(Info.ZF);
            var preZF = dsh.Ref(Info.ZF, 1);
            if (zf <= 0.0 || zf > 0.0099)
            {
                return null;
              }
//              if (zf < 0.002 || zf > 0.008)
//              {
//                  return null;
//              }
        if (preZF <= 0)
            {
                return null;
            }

        if (dsh.Ref(Info.V) > dsh.Ref(Info.V, 1))
        {
            return null;
        }
            float maxZF = 0;
            float minZF = 0;
            int nUpCount = 0;
            float totalZF = 0;
            for (int i = 1; i < 8; i++)
            {
                maxZF = Math.Max(maxZF, dsh.Ref(Info.ZF, i));
                minZF = Math.Min(minZF, dsh.Ref(Info.ZF, i));
              //  if (dsh.Ref(Info.ZF, i) > 0)
                if (dsh.Ref(Info.C, i) > dsh.Ref(Info.O, i))
                {
                    ++nUpCount;
                }
                totalZF += dsh.Ref(Info.ZF, i);
            }
            if (nUpCount > 4)
            {
                return null;
            }
            
            if (totalZF < -0.05 || totalZF > 0.05)
            {
                return null;
            }
//              for (int i = 1; i < 4; i++)
//              {
//                  if (dsh.Ref(Info.ZF, i) > 0.07)
//                  {
//                      return null;
//                  }
//                  if (dsh.Ref(Info.ZF, i) > 0 && dsh.Ref(Info.C, i) <= dsh.Ref(Info.O, i))
//                  {
//                      return null;
//                  }
//              }
             int nDownContinue = 0;
             float downContinueZF = 0;
             float upZF = 0;
             bool bMeetDownNow = false;
             for (int i = 0; i < 10; i++)
             {
                 if (dsh.Ref(Info.ZF, i) <= 0)
                 {
                     ++nDownContinue;
                     bMeetDownNow = true;
                     downContinueZF += dsh.Ref(Info.ZF, i);
//                      if (dsh.Ref(Info.OF, i) < -0.015)
//                      {
//                          return null;
//                      }
                 }
                 else
                 {
                     if (bMeetDownNow)
                     {
                         break;
                     }
                     upZF += dsh.Ref(Info.ZF, i);
                     nDownContinue = 0;
                 }
             }

             if (nDownContinue > 2)
             {
                 return null;
             }
//               if (upZF > 0.08)
//               {
//                   return null;
//               }
//              if (upZF + downContinueZF < 0)
//              {
//                  return null;
//              }

//             float delta = dsh.Ref(Info.C) - dsh.MA(Info.C, 5);
//             if (delta / dsh.Ref(Info.C) < 0.02 || delta / dsh.Ref(Info.C) > 0.04)
//             {
//                 return null;
//             }
             if (dsh.Ref(Info.C) <= dsh.Ref(Info.O))
             {
                 return null;
             }
            if (dsh.Ref(Info.OF) < -0.04)
            {
                return null;
            }
            int nUpStopCount = 0;
            float upV = 0, downV = 0;
            int upCount = 0, downCount = 0;
            float maxC = 0, minC = float.MaxValue;
            int upContinueCount = 0;
            int nHasUp3Count = 0;
            for (int i = 1; i < 15; i++)
            {
                float curZF = dsh.Ref(Info.ZF, i);
                maxC = Math.Max(maxC, dsh.Ref(Info.C, i));
                minC = Math.Min(minC, dsh.Ref(Info.C, i));
                if (curZF > 0.095)
                {
                    ++nUpStopCount;
                }
                if (dsh.Ref(Info.ZF, i) > 0)
                {
                    upV += dsh.Ref(Info.V, i);
                    ++upCount;
                }
                else
                {
                    downV += dsh.Ref(Info.V, i);
                    ++downCount;
                }
                if (curZF > 0)
                {
                    upContinueCount++;
                }
                else
                {
                    if (upContinueCount > 2)
                    {
                        ++nHasUp3Count;
                    }
                    upContinueCount = 0;
                }
            }
            if (nUpStopCount > 2)
            {
                return null;
            }
//             if (nHasUp3Count >= 2)
//             {
//                 return null;
//              }
//             if (dsh.Ref(Info.HF) < 0.02)
//             {
//                 return null;
//             }
            for (int i = 1; i < 4; i++)
            {
                float curOF = dsh.Ref(Info.OF, i);
                if (curOF > 0.02)
                {
                    return null;
                }
                if (dsh.UpShadow(i) > 0.03)
                {
                    return null;
                }
                if (!dsh.IsReal(i))
                {
                    return null;
                }
            }

            if (dsh.Ref(Info.ZF, 2) < 0.03)
             {
                 return null;
             }

            if (dsh.Ref(Info.ZF, 3) > 0.04)
            {
                return null;
            }

            if (dsh.Ref(Info.ZF, 4) > 0)
            {
                return null;
            }
            if (dsh.Ref(Info.A) < 1000)
            {
                return null;
            }
//              bool bFound = false;
//              for (int i = 1; i < 5; i++)
//              {
//                  if (dsh.DownShadow(i) <  -0.04 && dsh.Ref(Info.ZF, i) < 0)
//                  {
//                      bFound = true;
//                      break;
//                  }
//              }
//              if (bFound)
//              {
//                  return null;
//              }
            return EmptyRateItemButSel;
         
            return EmptyRateItemButSel;
        }
        Dictionary<String, String> selectForHH(DataStoreHelper dsh, Dictionary<String, String> param, ref String sigDate)
        {
            if (dsh.Ref(Info.ZF, 1) > 0.095)
            {
                return null;
            }
            if (dsh.Ref(Info.ZF) > 0.095)
            {
                return null;
             }
            if (dsh.Ref(Info.ZF) > -0.02 || !dsh.IsRealDown())
            {
                return null;
            }
            int iHHIndex = dsh.HH(Info.C, 90, 1);
            if (iHHIndex < 20)
            {
                return null;
            }
            if (!dsh.IsHighPeak(Info.C, iHHIndex, 10))
            {
                return null;
            }
            if (dsh.Ref(Info.C) > dsh.Ref(Info.C, iHHIndex))
            {
                return null;
            }
            if (dsh.Ref(Info.C)*1.03 < dsh.Ref(Info.C, iHHIndex))
            {
                return null;
            }
            for (int i = 1; i < 5; i++)
            {
                if (dsh.Ref(Info.C, i) > dsh.Ref(Info.C, iHHIndex))
                {
                    return null;
                }
            }
            sigDate = dsh.Date(iHHIndex).ToString();
            return EmptyRateItemButSel;
        }
        Dictionary<String, String> selectForV(DataStoreHelper dsh)
        {
            var zf = dsh.Ref(Info.ZF);

            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (dsh.Ref(Info.C, 3) > dsh.MA(Info.C, 10, 3))
            {
                return null;
            }
            if (dsh.Ref(Info.ZF, 1) < 0 || dsh.Ref(Info.ZF, 2) < 0)
            {
                return null;
            }
            for (int i = 0; i <= 2; i++)
            {
                if (!dsh.IsReal(i))
                {
                    return null;
                }
                if (dsh.Ref(Info.ZF, i) > 0.095)
                {
                    return null;
                }
            }
            if (dsh.Ref(Info.V, 2) / dsh.MA(Info.V, 5, 3) > 2 &&
                dsh.Ref(Info.V, 1) / dsh.MA(Info.V, 5, 3) > 2)
            {

            }
            else
            {
                return null;
            }
            if (zf < 0 || zf > 0.05)
            {
                return null;
            }
            //             if (dsh.Ref(Info.V) > Math.Max(dsh.Ref(Info.V, 2), dsh.Ref(Info.V, 1)))
            //             {
            //                 return null;
            //             }
            return EmptyRateItemButSel;
        }
        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, Dictionary<String, String> param, ref String sigDate)
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

            if (dsh.Ref(Info.A) < 1000)
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
            bool bMeetRealDown = false;
            float sigDateVol = 0;
            float otherMaxVol = 0;
            bool bHasUpShadowTooHight = false;
            bool bHasDownShadowTooLow = false;
            float otherMaxZF = 0;
            float otherMinZF = float.MaxValue;
            float otherMaxC = 0;
            float otherMaxH = 0;
            float sumOfSigDateZF = 0;
            float minV = float.MaxValue;
            float otherMaxDownV = 0;
            float minSigV = float.MaxValue;
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
                if (curZf > 0.01 && curPreZf > 0.01 && vol > 1.3 * hhv && preVol > 1.3 * hhv && (vol < 2 * preVol && preVol < 2 * vol) && vol / ma5 > 2 &&
                 preVol / ma5 > 2 && dsh.Ref(Info.ZF, i + 2) < 0.05
                    && dsh.Ref(Info.V, i + 2) / dsh.MA(Info.V, 5, i + 3) < 2)
                {
                    for (int j = i + 2; j < i + 7; j++)
                    {
                        if (dsh.Ref(Info.ZF, j) < -0.095)
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
                
                if (curZf > 0.012)
                {
                    bMeetRealUp = true;
                }
                if (curZf < -0.012)
                {
                    bMeetRealDown = true;
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
                minV = Math.Min(vol, minV);
                if (curZf < 0)
                {
                    otherMaxDownV = Math.Max(otherMaxDownV, vol);
                }
            }
            if (iSigDateIndex == -1)
            {
                return null;
            }
//             if (otherMaxDownV > minSigV)
//             {
//                 return null;
//             }
            sigDate = dsh.Date(iSigDateIndex).ToString();
            if (zf < 0)
            {
                if (nUpCount < 2)
                {
                    return null;
                }
            }
            else
            {
                if (nDownCount < 2)
                {
                    return null;
                }
            }
            if (zf < 0)
            {
                if (/*nUStopCount > 0 ||*//* nDStopCount > 0 || */bMeetTradeSigAllready || !bMeetRealUp)
                {
                    return null;
                }
            }
            else
            {
                if (/*nUStopCount > 0 ||*//* nDStopCount > 0 || */bMeetTradeSigAllready || !bMeetRealDown)
                {
                    return null;
                }
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
            if (zf < 0)
            {
                if (otherMaxZF + zf > 0)
                {
                    return null;
                }
            } else
            {
                 if (otherMinZF + zf  > 0)
                 {
                     return null;
                 }
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
            if (zf < 0)
            {
                if (
                 dsh.Ref(Info.ZF, 2) < 0.005 &&
                 dsh.Ref(Info.ZF, 3) < 0.005)
                {
                    return null;
                }
            }
            else
            {
                if (
                         dsh.Ref(Info.ZF, 2) > 0.01 &&
                         dsh.Ref(Info.ZF, 3) > 0.01)
                {
                    return null;
                }
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

             if (zf < 0)
             {
                 if (delta > -0.01)
                 {
                     return null;
                 }
             }
             else
             {
                 if (delta < 0.01)
                 {
                     return null;
                 }
             }
            var ret = new Dictionary<String, String>();
            ret[String.Format("sumSig/{0}", sumOfSigDateZF > 0.12 ? "1" : "0")] = "";
    //        ret[String.Format("delta/{0}", delta < -0.02 ? "1" : "0")] = "";
            ret[String.Format("maxUp/{0}", maxUpF > 0.02 ? "1" : "0")] = "";
            ret[String.Format("diffzf/{0}", otherMaxZF + zf > -0.02 ? "1" : "0")] = "";
            return ret;
        }
    }
}
