using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class EveryThreeUpStategy : TodayBuyTomorowSellStrategy, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "EveryThreeUp";
        }
        public FocusOn focusOn()
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
        const int SearchDayCount = 15;
        const int EveryUpCount = 3;
        Dictionary<String, String> selectFor(DataStoreHelper dsh, Dictionary<String, String> param, ref String sigDate, int iIndex, out bool isTwoDownMode)
        {
            isTwoDownMode = false;
            var zf = dsh.Ref(Info.ZF, iIndex);
            if (zf > -0.05 || zf < -0.095)
            {
                return null;
            }
            if (!dsh.IsRealDown())
            {
                return null;
            }
            if (zf < -0.09 && dsh.Ref(Info.ZF, iIndex+1) < -0.08)
            {
                return null;
            }

            int iSigDateIndex = -1;
            int nUStopCount = 0;
            int nDStopCount = 0;
            float sigDateVol = 0;
            float sigZF = 0;
            int nMeetMinus5PercentCount = 0;
            int iFirstMinus5PercentIndex = -1;
            int iTwoMinus5PercentIndex = -1;
            int nPlus5PercentCount = 0;
            for (int i = iIndex + 1; i < SearchDayCount; ++i)
            {
                var curOf = dsh.Ref(Info.OF, i);
                var curHf = dsh.Ref(Info.HF, i);
                var curZf = dsh.Ref(Info.ZF, i);
                var curLf = dsh.Ref(Info.LF, i);
                var curPreZf = dsh.Ref(Info.ZF, i + 1);
                var vol = dsh.Ref(Info.V, i);
                if (dsh.IsUpStopEveryDay(EveryUpCount, i))
                {
                    iSigDateIndex = i;
                    sigDateVol = vol;
                    sigZF = curZf;
                    break;
                }
            }
            for (int i = iSigDateIndex + EveryUpCount; i < iSigDateIndex + EveryUpCount + SearchDayCount; i++)
            {
                if (dsh.IsUpStopEveryDay(EveryUpCount, i))
                {
                    return null;
                }
            }
            for (int i = iSigDateIndex - 1; i > iIndex; --i)
            {
                var curOf = dsh.Ref(Info.OF, i);
                var curHf = dsh.Ref(Info.HF, i);
                var curZf = dsh.Ref(Info.ZF, i);
                var curLf = dsh.Ref(Info.LF, i);
                var curPreZf = dsh.Ref(Info.ZF, i + 1);
                var vol = dsh.Ref(Info.V, i);
                if (curZf > 0.095)
                {
                    nUStopCount++;
                }
                else if (curZf < -0.095)
                {
                    nDStopCount++;
                }
                if (curZf < -0.05)
                {
                    if (iFirstMinus5PercentIndex == -1)
                    {
                        iFirstMinus5PercentIndex = i;
                    }
                    ++nMeetMinus5PercentCount;
                    if (nMeetMinus5PercentCount == 2)
                    {
                        iTwoMinus5PercentIndex = i;
                    }
                }
                if (curZf > 0.06)
                {
                    ++nPlus5PercentCount;
                }
            }
            if (iSigDateIndex == -1)
            {
                return null;
            }
            int orgThreshold = 3;
            int nThreshold = orgThreshold;
            if (iTwoMinus5PercentIndex != -1)
            {
                bool bHasUp = false;
                for (int i = iIndex + 1; i < iTwoMinus5PercentIndex; i++)
                {
                    if (dsh.Ref(Info.ZF, i) > 0.01)
                    {
                        bHasUp = true;
                        break;
                    }
                }
                if (!bHasUp)
                {
                    nThreshold++;
                }
             }
            if (nThreshold == orgThreshold)
            {
                if (dsh.Ref(Info.ZF, 1 + iIndex) < 0 &&
                     dsh.Ref(Info.ZF, 2 + iIndex) < 0)
                {
                    nThreshold++;
                }
            }

//             if (nDStopCount > 1)
//             {
//                 return null;
//             }
            nThreshold += nPlus5PercentCount;
            nThreshold += nDStopCount;

            if (dsh.UpShadow(1) > 0.04 && dsh.Ref(Info.ZF, 1) < 0 && dsh.Ref(Info.ZF) > -0.08)
            {
                ++nThreshold;
            }
            //  nThreshold += (int)Math.Ceiling(nPlus5PercentCount*0.5);
            if (nMeetMinus5PercentCount < nThreshold - 1)
            {
                return null;
            }
            
            if (nMeetMinus5PercentCount > nThreshold - 1)
            {
                if (dsh.Ref(Info.ZF, iIndex + 1) > 0)
                {
                    return null;
                }
            }
            var preZF = dsh.Ref(Info.ZF, iIndex + 1);
            if (preZF > 0.02)
            {
                isTwoDownMode = true;
                return null;
            }
//             if (dsh.Ref(Info.C, iIndex) > dsh.Ref(Info.O, iIndex))
//             {
//                 return null;
//             }
            var ret = new Dictionary<String, String>();
            ret[String.Format("Threshold/{0}", nThreshold)] = "";
            ret[String.Format("Meet/{0}", nMeetMinus5PercentCount == nThreshold - 1 ? "1" : "0")] = "";
            return ret;
        }

        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, Dictionary<String, String> param, ref String sigDate)
        {
            float zf = dsh.Ref(Info.ZF);
            bool bIsTwoDownMode = false;
            Dictionary<String, String> ret;
            if (zf < -0.04 && zf > -0.095)
            {
                ret = selectFor(dsh, param, ref sigDate, 1, out bIsTwoDownMode);
                if (ret == null && bIsTwoDownMode)
                {
                    var retDict = new Dictionary<String, String>();
                    retDict[String.Format("TwoDownMode/1")] = "";
                    return retDict;
                }
            }
            ret = selectFor(dsh, param, ref sigDate, 0, out bIsTwoDownMode);
            if (ret == null)
            {
                return null;
            }
            ret[String.Format("TwoDownMode/0")] = "";
            return ret;
        }
    }
}
