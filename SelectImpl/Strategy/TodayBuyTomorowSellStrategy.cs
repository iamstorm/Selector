﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace SelectImpl
{
    public abstract class TodayBuyTomorowSellStrategy : BaseStrategyImpl
    {
        public String computeBonus(Stock stock, int buyDate, out bool bSellWhenMeetMyBounusLimit, out int sellDate)
        {
            IStrategy stra = (IStrategy)this;
            float bonusLimit = stra.bounusLimit();
            sellDate = stock.nextTradeDate(buyDate);
            float onemoney = 1.0f;
            bSellWhenMeetMyBounusLimit = true;
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
                    if (of > bonusLimit)//开盘超出盈利顶额
                    {
                        onemoney *= 1 + of;
                        break;
                    }
                    if (hf > bonusLimit)//达到盈利顶额
                    {
                        onemoney *= 1 + bonusLimit;
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
    }
}
