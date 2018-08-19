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
        public SelectItem makeDeside(List<SelectItem> selectItems)
        {
            SelectItem buyItem = null;
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
                List<SelectItem> items = re.ofDate(date);
                if (items.Count == 0)
                {
                    if (Utils.IsTradeDay(date))
                    {
                        buyitems.Add(makeDeside(items));
                    }
                    continue;
                }
                buyitems.Add(makeDeside(items));
            }
            return re.buyItems_ = buyitems;
        }
    }
}
