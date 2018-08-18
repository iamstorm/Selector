using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectorForm
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
        public BuyItem makeDeside(List<SelectItem> selectItems)
        {
            return null;
        }
        public List<BuyItem> desideToBuy(RegressResult re)
        {
            List<BuyItem> buyitems = new List<BuyItem>();
            IList<int> dateList = Utils.TraverTimeDay(re.startDate_, re.endDate_);
            foreach (int date in dateList)
            {
                BuyItem buy = makeDeside(re.ofDate(date));
                if (buy != null)
                {
                    buyitems.Add(buy);
                }
            }
            return re.buyItems_ = buyitems;
        }
    }
}
