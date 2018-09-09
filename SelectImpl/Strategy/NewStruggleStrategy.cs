using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class NewStruggleStrategy : BaseStrategyImpl, IStrategy
    {
        #region meta data
        String IStrategy.verTag()
        {
            return "1.0";
        }
        String IStrategy.name()
        {
            return "NewStruggle";
        }
        FocusOn IStrategy.focusOn()
        {
            return FocusOn.FO_New;
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
        String IStrategy.computeBonus(Stock stock, int buyDate, out bool bSellWhenMeetMyBounusLimit, out int sellDate)
        {
            IStrategy stra = (IStrategy)this;
            sellDate = stock.nextTradeDate(buyDate);
            float firstDayBonusLimit = 0.045f;
            float otherDayBonusLimit = 0.045f;
            float onemoney = 1.0f;
            bSellWhenMeetMyBounusLimit = true;
            if (sellDate == -1 || (Utils.NowIsTradeDay() && sellDate > Utils.NowDate()))
            {
                return "";// 还未遇到交易日，不知道盈亏
            }
            bool bFirstSellDate = true;
            int nDaySpan = 0;
            do
            {
                float of = stock.of(sellDate);
                float hf = stock.hf(sellDate);
                float zf = stock.zf(sellDate);
                float bonusLimit = bFirstSellDate ? firstDayBonusLimit : otherDayBonusLimit;
                if (hf < -0.11 || hf > 0.111)
                {//除权了
                    return "";
                }
                if (Utils.IsDownStop(hf))//一直跌停
                {
                    sellDate = stock.nextTradeDate(sellDate);
                    onemoney *= 1 + hf;
                    bSellWhenMeetMyBounusLimit = false;
                    bFirstSellDate = false;
                    ++nDaySpan;
                    continue;
                }
                //今天没一直跌停，大概率能卖出
                if (bSellWhenMeetMyBounusLimit)
                {
                    if (onemoney * (1 + of) - 1 > bonusLimit)//开盘超出盈利顶额
                    {
                        onemoney *= 1 + of;
                        break;
                    }
                    if (onemoney * (1 + hf) - 1 > bonusLimit)//达到盈利顶额
                    {
                        onemoney = 1 + bonusLimit;
                        break;
                    }
                    if (Utils.IsDownStop(zf))//尾盘跌停，卖不了
                    {
                        bSellWhenMeetMyBounusLimit = false;
                    }
                    onemoney *= 1 + zf;
                    bFirstSellDate = false;
                    ++nDaySpan;
                    if (nDaySpan > 1)
                    {
                        break;
                    }
                    sellDate = stock.nextTradeDate(sellDate);
                }
                else
                {//逃命模式
                    if (!Utils.IsDownStop(of))//开盘没跌停
                    {//跑了
                        onemoney *= 1 + of;
                        break;
                    }
                    //开盘跌停但因为没一直跌停，所以肯定可以以跌停价卖出
                    onemoney *= 1 - 0.095f;
                    break;
                }
            } while (sellDate != -1);
            return Utils.ToBonus((onemoney - 1 - 0.002f) * 0.5f);
        }
        Dictionary<String, String> selectForHStrugle(DataStoreHelper dsh, float topMostC)
        {
            if (dsh.MaxCO(1) > dsh.MaxCO())
            {
                bool bHasGreen = false;
                for (int i = 1; i <= 2; i++)
                {
                    if (dsh.Ref(Info.C, i) < dsh.Ref(Info.O, i))
                    {
                        bHasGreen = true;
                        break;
                    }
                }
                if (!bHasGreen)
                {
                    return null;
                }
            }
            for (int i = 1; i <= 2; i++)
            {
                if (!dsh.IsReal(i))
                {
                    return null;
                }
            }
            var test = dsh.Date().ToString();
            var upRate = (dsh.Ref(Info.H) - topMostC) / topMostC;
            if ((dsh.Ref(Info.H, 1) - topMostC) / topMostC < 0 &&
             upRate > 0.01/* && dsh.UpShadow() > 0.02*/ &&
                dsh.MaxCO() < topMostC && dsh.Ref(Info.ZF) > 0)
            {
                return EmptyRateItemButSel;
            }
            return null;
        }
        Dictionary<String, String> IStrategy.select(DataStoreHelper dsh, Dictionary<String, String> param, ref String sigDate)
        {
            IStrategy stra = (IStrategy)this;
            var zf = dsh.Ref(Info.ZF);
            if (zf > 0.095 || zf < -0.095)
            {
                return null;
            }
            if (dsh.stock_.code_ == "603713")
            {
                int a = 3;
                a = 5;
            }
            if (dsh.birthCount() > 40)
            {
                return null;
            }
            int iOpenUpStopIndex = -1;
            int nDayCount = 0;
            for (int i = dsh.dataList_.Count-2; i > 0; --i)
            {
                if (dsh.ds_.Ref(Info.ZF, dsh.dataList_, i) < 0.095)
                {
                    iOpenUpStopIndex = i - dsh.iIndex_;
                    break;
                }
                ++nDayCount;
            }
            if (nDayCount < 3/* && Utils.Year(dsh.Date()) == 2005*/)
            {
                return null;
            }
            if (iOpenUpStopIndex == -1)
            {
                return null;
            }
            int iTopMostIndex = -1;
            iTopMostIndex = iOpenUpStopIndex + 1;
            float topMostSimpleC = Math.Max(dsh.Ref(Info.O, iTopMostIndex), dsh.Ref(Info.C, iTopMostIndex));
            int iSerarchBeginIndex = -1;
            for (int i = iOpenUpStopIndex; i > 0; --i)
            {
                float curTop = Math.Max(dsh.Ref(Info.O, i), dsh.Ref(Info.C, i));
                if (curTop > topMostSimpleC)
                {
                    topMostSimpleC = curTop;
                    iTopMostIndex = i;
                }
                if (dsh.Ref(Info.ZF, i) < 0 && dsh.MaxCO(i) > dsh.MaxCO(i-1))
                {
                    iSerarchBeginIndex = i;
                    break;
                }
            }
            if (iTopMostIndex == -1 || iSerarchBeginIndex == -1)
            {
                return null;
            }

//             float topMostSecondC = topMostSimpleC;
//             int iTopSecondIndex = -1;
//             for (int i = iSerarchBeginIndex-1; i > 0; --i)
//             {
//                 if (dsh.Ref(Info.ZF, i) < -0.02 && dsh.IsRealDown(i))
//                 {
//                     iSearchIndex2 = i;
//                     break;
//                 }
//                 topMostSecondC = Math.Max(topMostSecondC, dsh.Ref(Info.C, i));
//                 iTopSecondIndex = i;
//             }
//             if (iSearchIndex2 == -1)
//             {
//                 return null;
//             }
//             if (iTopSecondIndex != -1 && topMostSecondC > topMostSimpleC)
//             {
//                 topMostSecondC = Math.Max(topMostSecondC, dsh.MaxCO(iTopSecondIndex - 1));
//                 topMostC = topMostSecondC;
//                 iTopMostIndex = iTopSecondIndex;
//             }
//             else
//             {
//                 topMostC = topMostSimpleC;
//             }
            for (int i = 3; i >= 1; --i )
            {
                bool bContinue = true;
                if (dsh.Ref(Info.ZF, iTopMostIndex-i) > 0.095)
                {
                    for (int j = iTopMostIndex-i; j > 0; --j)
                    {
                        if (dsh.Ref(Info.ZF, j) < 0)
                        {
                            bContinue = false;
                            break;
                        }
                        float curTop = Math.Max(dsh.Ref(Info.O, j), dsh.Ref(Info.C, j));
                        if (curTop > topMostSimpleC)
                        {
                            topMostSimpleC = curTop;
                            iTopMostIndex = iTopMostIndex - i;
                        }
                    }
                }
                if (!bContinue)
                    break;
            }

            int iSearchIndex2 = iSerarchBeginIndex;
            float topMostC = topMostSimpleC;
            sigDate = dsh.Date(iTopMostIndex).ToString();

            int iStruggleUpIndex = -1;
            int iTwoUpIndex = -1;
            for (int i = iSearchIndex2; i >= 0; --i)
            {
                if (iStruggleUpIndex == -1)
                {
                    if (dsh.Ref(Info.C, i + 2) < topMostC && (dsh.Ref(Info.C, i + 1) - topMostC) / topMostC > 0)
                    {
                        iStruggleUpIndex = i;
                    }
                }
//                 if (dsh.Ref(Info.C, i + 2) < topMostC && (dsh.Ref(Info.C, i + 1) - topMostC) / topMostC > 0.005 &&
//                     dsh.Ref(Info.ZF, i) > 0.01)
//                 {
//                     iTwoUpIndex = i;
//                     break;
//                 }
            }
            if (iStruggleUpIndex == -1)
            {
                return null;
            }
            if (iTwoUpIndex == iStruggleUpIndex && iTwoUpIndex == 0)
            {
                for (int i = 1;  i < 4;  i++)
                {
                    if (dsh.Ref(Info.ZF, i) > 0.095)
                    {
                        return null;
                    }
                    else if (dsh.Ref(Info.ZF, i) < 0)
                    {
                        break;
                    }
                }
                return EmptyRateItemButSel;
            }
//             for (int i = 1; i <= 2; i++)
//             {
//                 if (selectForHStrugle(dsh.newDsh(i), topMostC) != null)
//                 {
//                     return null;
//                 }
//             }

            return selectForHStrugle(dsh, topMostC);
        }
    }
}
