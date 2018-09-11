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
            strategyList_.Add(new UStopDownStrategy());
            strategyList_.Add(new UUDownStrategy());
            strategyList_.Add(new EveryThreeUpStategy());
            strategyList_.Add(new NewStruggleStrategy());
            strategyList_.Add(new VUpDownStrategy());
        }
        public IStrategy strategy(String straName)
        {
            foreach (var stra in strategyList_)
            {
                if (stra.name() == straName)
                {
                    return stra;
                }
            }
            return null;
        }
        public SelectItem makeDeside(List<SelectItem> selectItems, int date, IBuyDesider desider)
        {
            if (selectItems.Count == 0)
            {
                return SelectItem.MissBuy(date);
            }
            SelectItem buyItem = desider.makeDeside(selectItems);
            
            return buyItem == null ? SelectItem.DontBuy(selectItems[0].date_) : buyItem;
        }
        public List<SelectItem> desideToBuy(RegressResult re)
        {
            App.host_.uiStartProcessBar();
            List<SelectItem> buyitems = new List<SelectItem>();
            List<int> dateList = Utils.TraverTimeDay(re.dateRangeList_);
            dateList.Reverse();
            int nFinishCount = 0;
            int nTotalCount = dateList.Count;
            int nMissCount = 0;
            int nDontCount = 0;
            int nTradeCount = 0;
            foreach (int date in dateList)
            {
                nFinishCount++;
                List<SelectItem> items = SelectResult.OfDate(date, re.selItems_);
                if (items.Count == 0)
                {
                    if (Utils.IsTradeDay(date))
                    {
        //                buyitems.Add(makeDeside(items, date, RankBuyDesider.buyer_));
                        ++nMissCount;
                    }
                    continue;
                }
                else
                {
                    var buyItem = makeDeside(items, date, RankBuyDesider.buyer_);
                    if (buyItem.isRealSelectItem)
                    {
                        buyItem.iamBuyItem_ = true;
                        ++nTradeCount;
                    }
                    else
                    {
                        ++nDontCount;
                    }
                    buyitems.Add(buyItem);
                }
                App.host_.uiSetProcessBar(String.Format("正在回归{0}-{1}，购买阶段：完成{2}的购买, 选择总数：{3}，当前MissBuy：{4}，DontBuy：{5}，Buy：{6}",
                    dateList.Last(), dateList.First(), date, re.selItems_.Count, nMissCount, nDontCount, nTradeCount),
                    nFinishCount * 100 / nTotalCount);
            }
            App.host_.uiFinishProcessBar();
            return re.buyItems_ = buyitems;
        }
        public List<SelectItem> buyMostBonusPerDay(RegressResult re)
        {
            App.host_.uiStartProcessBar();
            List<SelectItem> buyitems = new List<SelectItem>();
            List<int> dateList = Utils.TraverTimeDay(re.dateRangeList_);
            dateList.Reverse();
            int nFinishCount = 0;
            int nTotalCount = dateList.Count;
            int nMissCount = 0;
            int nTradeCount = 0;
            foreach (int date in dateList)
            {
                nFinishCount++;
                List<SelectItem> items = SelectResult.OfDate(date, re.selItems_);
                if (items.Count == 0)
                {
                    if (Utils.IsTradeDay(date))
                    {
                        ++nMissCount;
                    }
                    continue;
                }
                float maxBonusValue = -11;
                SelectItem maxBonusItem = null;
                foreach (var item in items)
                {
                    var bonus = item.getColumnVal("bonus");
                    if (bonus == "")
                    {
                        continue;
                    }
                    var bonusValue = Utils.GetBonusValue(bonus);
                    if (bonusValue > maxBonusValue)
                    {
                        maxBonusValue = bonusValue;
                        maxBonusItem = item;
                    }
                }
                if (maxBonusItem == null)
                {
                    buyitems.Add(items[0]);
                }
                else
                {
                    buyitems.Add(maxBonusItem);
                }
                ++nTradeCount;
                App.host_.uiSetProcessBar(String.Format("正在回归{0}-{1}，可能最佳购买阶段：完成{2}的购买, 选择总数：{3}，当前MissBuy：{4}，Buy：{5}",
                dateList.Last(), dateList.First(), date, re.selItems_.Count, nMissCount, nTradeCount),
                nFinishCount * 100 / nTotalCount);
            }
            App.host_.uiFinishProcessBar();
            return buyitems;
        }

    }
}
