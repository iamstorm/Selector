using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class StrategyGroup
    {
        public List<IStrategy> strategyList_ = new List<IStrategy>();
        public StrategyGroup()
        {
            strategyList_.Add(new LStopUpStrategy());

            foreach (IStrategy stra in strategyList_)
            {
                string name = stra.name();
            }
        }
        public SelectItem makeDeside(List<SelectItem> selectItems, int date)
        {
            SelectItem buyItem = null;
            if (selectItems.Count == 0)
            {
                return SelectItem.MissBuy(date);
            }
            if (selectItems.Count == 1)
            {
                buyItem = selectItems[0];
            }
            else
            {
                int maxRate = 0;
                SelectItem maxRateItem = null;
                foreach (var item in selectItems)
                {
                    int rate = Utils.ToType<int>(item.rate_);
                    if (rate > maxRate)
                    {
                        maxRateItem = item;
                        maxRate = rate;
                    }
                }
                buyItem = maxRateItem;
            }
            return buyItem == null ? SelectItem.DontBuy(selectItems[0].date_) : buyItem;
        }
        public List<SelectItem> desideToBuy(RegressResult re)
        {
            List<SelectItem> buyitems = new List<SelectItem>();
            List<int> dateList = Utils.TraverTimeDay(re.startDate_, re.endDate_);
            dateList.Reverse();
            foreach (int date in dateList)
            {
                List<SelectItem> items = SelectResult.OfDate(date, re.selItems_);
                if (items.Count == 0)
                {
                    if (Utils.IsTradeDay(date))
                    {
                        buyitems.Add(makeDeside(items, date));
                    }
                    continue;
                }
                buyitems.Add(makeDeside(items, date));
            }
            return re.buyItems_ = buyitems;
        }
        public String computeBonus(Stock stock, int buyDate, out bool bSellWhenMeetMyBounusLimit,out int sellDate)
        {
            sellDate = stock.nextTradeDate(buyDate);
            float onemoney = 1.0f;
            bSellWhenMeetMyBounusLimit = true;
            if (sellDate == -1)
	        {
		        return "";// 还未遇到交易日，不知道盈亏
	        }
            do
            {
                float of = stock.of(sellDate);
                float hf = stock.hf(sellDate);
                float zf = stock.zf(sellDate);
                if (Utils.IsDownStop(hf))//一直跌停
                {
                    sellDate = stock.nextTradeDate(sellDate);
                    onemoney *= 1+hf;
                    bSellWhenMeetMyBounusLimit = false;
                    continue;
                }
                //今天没一直跌停，大概率能卖出
                if (bSellWhenMeetMyBounusLimit)
                {
                    if (of > Setting.MyBounusLimit)//开盘超出盈利顶额
                    {
                        onemoney *= 1+of;
                        break;
                    }
                    if (hf > Setting.MyBounusLimit)//达到盈利顶额
                    {
                        onemoney *= 1 + Setting.MyBounusLimit;
                        break;
                    }
                    if (Utils.IsDownStop(zf))//尾盘跌停，卖不了
                    {
                        sellDate = stock.nextTradeDate(sellDate);
                        onemoney *= 1+zf;
                        bSellWhenMeetMyBounusLimit = false;
                        continue;
                    }
                    onemoney *= 1+zf;//尾盘卖了
                    break;
                }
                else
                {//逃命模式
                    if (!Utils.IsDownStop(of))//开盘没跌停
                    {//跑了
                        onemoney *= 1+of;
                        break;
                    }
                    //开盘跌停但因为没一直跌停，所以肯定可以以跌停价卖出
                    onemoney *= 1-0.095f;
                    break;
                }
            } while (sellDate != -1);
            return Utils.ToBonus(onemoney - 1);
        }
    }
}
