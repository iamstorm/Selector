using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace SelectImpl
{
    public abstract class LF_TodayBuyTomorowSellStrategy : BaseStrategyImpl
    {
        public String computeBonus(Stock stock, int buyDate, out bool bSellWhenMeetMyBounusLimit, out int sellDate)
        {
            IStrategy_LF stra = (IStrategy_LF)this;
            float buyLimit = stra.buyLimit();
            float ofBonusLimit = stra.ofBonusLimit();
            float bonusLimit = stra.bounusLimit();
            float zfOfBuyDate = App.ds_.Ref(Info.ZF, stock.dataList_, App.ds_.index(stock, buyDate));
            bSellWhenMeetMyBounusLimit = true;
            sellDate = stock.nextTradeDate(buyDate);
            float onemoney = 1.0f;
            float buyZF = zfOfBuyDate - buyLimit;
            onemoney *= 1 + buyZF;
            if (sellDate == -1 || (Utils.NowIsTradeDay() && sellDate > Utils.NowDate()))
            {
                return "";// 还未遇到交易日，不知道盈亏
            }
            do
            {
                float of = stock.of(sellDate);
                float hf = stock.hf(sellDate);
                float zf = stock.zf(sellDate);
                if (hf < -0.11 || hf > 0.111)
                {//除权了
                    return "";
                }
                if (Utils.IsDownStop(hf))//一直跌停
                {
                    sellDate = stock.nextTradeDate(sellDate);
                    onemoney *= 1 + hf;
                    bSellWhenMeetMyBounusLimit = false;
                    continue;
                }
                //今天没一直跌停，大概率能卖出
                if (bSellWhenMeetMyBounusLimit)
                {
                    if (of + buyZF > ofBonusLimit)//开盘超出盈利顶额
                    {
                        onemoney *= 1 + of;
                        break;
                    }
                    if (hf + buyZF > bonusLimit)//达到盈利顶额
                    {
                        onemoney = 1 + bonusLimit;
                        break;
                    }
                    if (Utils.IsDownStop(zf))//尾盘跌停，卖不了
                    {
                        sellDate = stock.nextTradeDate(sellDate);
                        onemoney *= 1 + zf;
                        bSellWhenMeetMyBounusLimit = false;
                        continue;
                    }
                    onemoney *= 1 + zf;//尾盘卖了
                    break;
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
            return Utils.ToBonus(onemoney - 1 - 0.002f);
        }
        public bool ofDownLimit(DataStoreHelper dsh, float downLimit)
        {
            IStrategy_LF stra = (IStrategy_LF)this;
            float buyLimit = stra.buyLimit();
            float ofDown = dsh.Ref(Info.OF) - buyLimit;
            if (ofDown <= downLimit)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public bool meetBuyChance(DataStoreHelper dsh, SelectMode selectMode)
        {
            IStrategy_LF stra = (IStrategy_LF)this;
            float buyLimit = stra.buyLimit();
            switch (selectMode)
            {
                case SelectMode.SM_Regress:
                    return dsh.Ref(Info.LF) < buyLimit;
                case SelectMode.SM_SelectOpen:
                    return true;
                default:
                    return dsh.Ref(Info.ZF) < buyLimit + 0.005f;
            }
        }
    }
}
