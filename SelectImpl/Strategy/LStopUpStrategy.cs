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
        public float bounusLimit()
        {
            return 0.095f;
        }
        #endregion

        Dictionary<String, String> selectForCrossLL(DataStoreHelper dsh, bool bSelectMode, int iIndex, out int iLLIndex)
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
        Dictionary<String, String> selectForVol(DataStoreHelper dsh, int iIndex, bool bSelectMode, ref String sigDate)
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
        Dictionary<String, String> selectForLargeSample(DataStoreHelper dsh, bool bSelectMode, ref String sigDate)
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
        }
        Dictionary<String, String> selectForHH(DataStoreHelper dsh, bool bSelectMode, ref String sigDate)
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
        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, bool bSelectMode, ref String sigDate)
        {
            var zf = dsh.Ref(Info.ZF);
            if (zf > 0.095)
            {
                return EmptyRateItemButSel;
            }
            else
            {
                return null;
            }
            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (!dsh.IsReal())
            {
                return null;
            }
//             for (int i = 1; i < 8; i++)
// 			{
// 			    
// 			}
//             float ma5 = dsh.MA(Info.V, 1);
//             if (dsh.Ref(Info.V) > )
//             {
//                 
//             }
            return null;
        }
    }
}
