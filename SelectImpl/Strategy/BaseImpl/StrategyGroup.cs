using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class StrategyGroup
    {
        public List<IStrategy> strategyList_ = new List<IStrategy>();
        public StrategyGroup()
        {
            strategyList_.Add(new TmpStrategy());
            strategyList_.Add(new UStopDownStrategy());
            strategyList_.Add(new UUDownStrategy());
            strategyList_.Add(new EveryThreeUpStategy());
            strategyList_.Add(new NewStruggleStrategy());
            strategyList_.Add(new VUpDownStrategy());
            strategyList_.Add(new LF_UStopDownStrategy());
            strategyList_.Add(new LF_UUDownStrategy());
            strategyList_.Add(new FlatStepStrategy());
            strategyList_.Add(new MaxCOStrategy());
            strategyList_.Add(new DownUpStrategy());
            strategyList_.Add(new EveryDownUpStrategy());
            strategyList_.Add(new EnvUpStrategy());
            strategyList_.Add(new UpDownDownUpStrategy());
            strategyList_.Add(new UpDownUpStrategy());
            strategyList_.Add(new NNDownUpStrategy());
            strategyList_.Add(new LF_M_DUDStrategy());
            strategyList_.Add(new LF_M_NEWStrategy());
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
        public SelectItem makeDeside(List<SelectItem> selectItems, int date, IBuyDesider desider, bool bIsSelect)
        {
            if (selectItems.Count == 0)
            {
                return null;
            }
            if (bIsSelect) {
                DateTime curTime = DateTime.Now;
                var dt = DB.Global().Select(
                    String.Format("Select code, buyNormlizePrice From minute_select Where date = '{0}'", Utils.Date(curTime)));

                Dictionary<String, int> codeBuyNormlizePriceDict = new Dictionary<String, int>();
                foreach (DataRow row in dt.Rows) {
                    codeBuyNormlizePriceDict.Add(row["code"].ToString(), Utils.ToType<int>(row["buyNormlizePrice"].ToString()));
                }

                foreach (var item in selectItems) {
                    bool bMinuteStrategy = item.strategyName_.StartsWith("LF_");
                    if (!bMinuteStrategy) {
                        continue;
                    }
                    if (codeBuyNormlizePriceDict.TryGetValue(item.code_, out item.buyNormlizePrice_)) {
                        item.sigInfo_ = Utils.ToPrice(item.buyNormlizePrice_).ToString();
                        continue;
                    }
                    var close = item.getColumnVal("close");
                    float fclose;
                    if (!float.TryParse(close, out fclose)) {
                        continue;
                    }
                    item.buyNormlizePrice_ = (int)(fclose * Setting.NormalizeRate);
                    item.sigInfo_ = Utils.ToPrice(item.buyNormlizePrice_).ToString();
                    Dictionary<String, Object> selectItem = new Dictionary<String, Object>();
                    selectItem["code"] = item.code_;
                    selectItem["date"] = item.date_;
                    selectItem["name"] = item.getColumnVal("name");
                    selectItem["zf"] = item.getColumnVal("zf");
                    selectItem["bonus"] = item.getColumnVal("bonus");
                    selectItem["close"] = item.getColumnVal("close");
                    selectItem["straname"] = item.strategyName_;
                    selectItem["buyNormlizePrice"] = item.buyNormlizePrice_;
                    selectItem["selecttime"] = Utils.ToTimeDesc(curTime);

                    DB.Global().Insert("minute_select", selectItem);
                }
                foreach (var item in selectItems) {
                    bool bMinuteStrategy = item.strategyName_.StartsWith("LF_");
                    if (!bMinuteStrategy) {
                        continue;
                    }
                    if (!codeBuyNormlizePriceDict.ContainsKey(item.code_)) {
                        continue;
                    }
                    var close = item.getColumnVal("close");
                    DB.Global().Execute(String.Format("Update minute_select Set close = '{0}' Where code = '{1}'", close, item.code_));
                }
            }
            SelectItem buyItem = desider.makeDeside(selectItems);
            
            return buyItem;
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
                else
                {
                    var buyItem = makeDeside(items, date, RankBuyDesider.buyer_, false);
                    if (buyItem != null)
                    {
                        buyItem.iamBuyItem_ = true;
                        ++nTradeCount;
                    }
                    buyitems.Add(buyItem);
                }
                App.host_.uiSetProcessBar(String.Format("{0}:正在回归{1}-{2}，购买阶段：完成{3}的购买, 选择总数：{4}，当前MissBuy：{5}，Buy：{6}",
                    re.solutionName_,
                    dateList.Last(), dateList.First(), date, re.selItems_.Count, nMissCount, nTradeCount),
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
                App.host_.uiSetProcessBar(String.Format("{0}:正在回归{1}-{2}，可能最佳购买阶段：完成{3}的购买, 选择总数：{4}，当前MissBuy：{5}，Buy：{6}",
                re.solutionName_,
                dateList.Last(), dateList.First(), date, re.selItems_.Count, nMissCount, nTradeCount),
                nFinishCount * 100 / nTotalCount);
            }
            App.host_.uiFinishProcessBar();
            return buyitems;
        }

    }
}
