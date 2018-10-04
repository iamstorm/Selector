using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace SelectImpl
{
    public struct DateSelectItem
    {
        public int date_;
        public List<SelectItem> selItems_;
        public List<SelectItem> goodSampleSelItems_;
        public SelectItem getBuyItem()
        {
            foreach (var item in selItems_)
            {
                if (item.iamBuyItem_)
                {
                    return item;
                }
            }
            return null;
        }
    }
    public struct DateRange
    {
        public int startDate_;
        public int endDate_;
    }
    public enum RunMode
    {
        RM_Raw,
        RM_Asset
    }
    public class RegressResult
    {
        public RunMode runMode_;
        public String name_;
        public String solutionName_;
        public String dateRangeName_;
        public List<DateRange> dateRangeList_;
        public List<IStrategy> strategyList_;
        public List<SelectItem> selItems_ = new List<SelectItem>();
        public List<SelectItem> buyItems_;
        public HistoryData reHistory_;
        public String TotalBonus
        {
            get 
            {
                float totalBous = 0;
                foreach (var item in buyItems_)
                {
                    var bonus = item.getColumnVal("bonus");
                    if (bonus == "")
                    {
                        continue;
                    }
                    totalBous += Utils.GetBonusValue(bonus);
                }
                return totalBous.ToString("F2") + "%";
            }
        }
        public int EndDate
        {
            get
            {
                return dateRangeList_.Last().endDate_;
            }
        }
        public String SelectFormName
        {
            get
            {
                return name_ + "_Select";
            }
        }
        public String BuyFormName
        {
            get
            {
                return name_ + "_Buy";
            }
        }
        public String StatisBonusFormName
        {
            get
            {
                return name_ + "_Bonus";
            }
        }
        public String StatisStrategyDataFormName
        {
            get
            {
                return name_ + "_StraData";
            }
        }
        public String StatisRateItemFormName
        {
            get
            {
                return name_ + "_RateItem";
            }
        }
        public String StatisHistoryFormName
        {
            get
            {
                return name_ + "_History";
            }
        }
        public String[] AllFormNames
        {
            get
            {
                return new String[] { SelectFormName, BuyFormName, 
                    StatisBonusFormName, StatisStrategyDataFormName, StatisRateItemFormName, StatisHistoryFormName };
            }
        }
        public int getTradeDayCount()
        {
            int nTradeDayCount = 0;
            List<int> dateList = Utils.TraverTimeDay(dateRangeList_);
            foreach (int date in dateList)
            {
                if (Utils.IsTradeDay(date))
                {
                    ++nTradeDayCount;
                }
            }
            return nTradeDayCount;
        }
        public static float GetUpRate(List<SelectItem> items)
        {
            int nUpCount = 0;
            int nTotalValidCount = 0;
            foreach (var item in items)
            {
                var bonus = item.getColumnVal("bonus");
                if (bonus == "")
                {
                    continue;
                }
                if (Utils.GetBonusValue(bonus) > 0)
                {
                    ++nUpCount;
                }
                ++nTotalValidCount;
            }
            if (nTotalValidCount == 0)
            {
                return 0;
            }
            return nUpCount * 1.0f / nTotalValidCount;
        }
        public static List<SelectItem> GetGoodSampleSelectItems(List<SelectItem> dayItems)
        {
            dayItems.Sort(delegate(SelectItem lhs, SelectItem rhs)
            {
                var lhsBonus = lhs.getColumnVal("bonus");
                var rhsBonus = rhs.getColumnVal("bonus");
                if (lhsBonus == "")
                {
                    return 1;
                }
                if (rhsBonus == "")
                {
                    return -1;
                }
                float lhsBonusValue = Utils.GetBonusValue(lhsBonus);
                float rhsBonusValue = Utils.GetBonusValue(rhsBonus);
                return lhsBonusValue.CompareTo(rhsBonusValue);
            });
            List<SelectItem> goodSamplesItems;
            if (dayItems.Count <= 10)
            {
                goodSamplesItems = dayItems;
            }
            else
            {
                goodSamplesItems  = new List<SelectItem>();
                int delta = dayItems.Count / 10;
                for (int i = 0; i < 10; ++i )
                {
                    goodSamplesItems.Add(dayItems[i*delta]);
                }
            }
            return goodSamplesItems;
        }
        public static List<DateSelectItem> ToDaySelectItemList(List<SelectItem> selItems, List<DateRange> dateRangeList)
        {
            List<DateSelectItem> retList = new List<DateSelectItem>();
            List<int> dateList = Utils.TraverTimeDay(dateRangeList);
            dateList.Reverse();
            foreach (int date in dateList)
            {
                if (!Utils.IsTradeDay(date))
                {
                    continue;
                }
                DateSelectItem item = new DateSelectItem();
                item.date_ = date;
                var rawitems = SelectResult.OfDate(date, selItems, true);
                Dictionary<String, int> uniqueItemDict = new Dictionary<String, int>();
                item.selItems_ = new List<SelectItem>();
                foreach (var ritem in rawitems)
                {
                    if (!uniqueItemDict.ContainsKey(ritem.code_))
                    {
                        item.selItems_.Add(ritem);
                        uniqueItemDict[ritem.code_] = 0;
                    }
                }

                item.goodSampleSelItems_ = GetGoodSampleSelectItems(item.selItems_);
                retList.Add(item);
            }
            return retList;
        }
        public static Dictionary<String, List<SelectItem>> ToRateItemDict(List<SelectItem> selItems)
        {
            Dictionary<String, List<SelectItem>> rateItemDict = new Dictionary<String, List<SelectItem>>();
            foreach (var item in selItems)
            {
                foreach (var kv in item.rateItemDict_)
                {
                    List<SelectItem> itemList;
                    if (rateItemDict.TryGetValue(kv.Key, out itemList))
                    {
                        itemList.Add(item);
                    }
                    else
                    {
                        itemList = new List<SelectItem>();
                        itemList.Add(item);
                        rateItemDict[kv.Key] = itemList;
                    }
                }
            }
            return rateItemDict;
        }
        public static void GetUpDownOccurCountForRateItem(List<SelectItem> selItems, List<String> rateItemKeyList, out List<int> occurInUpItemList, out List<int> occurInDownItemList, out int nUpSelCount, out int nDownSelCount)
        {
            occurInUpItemList = new List<int>();
            occurInDownItemList = new List<int>();
            foreach (var item in rateItemKeyList)
	        {
		        occurInUpItemList.Add(0);
		        occurInDownItemList.Add(0);
	        }
            nUpSelCount = nDownSelCount = 0;
            foreach (var item in selItems)
            {
                var bonus = item.getColumnVal("bonus");
                if (bonus == "")
                {
                    continue;
                }
                float bonusValue = Utils.GetBonusValue(bonus);
                if (bonusValue > 0)
                {
                    ++nUpSelCount;
                }
                else
                {
                    ++nDownSelCount;
                }
                for (int i = 0; i < rateItemKeyList.Count; ++i)
                {
                    if (item.rateItemDict_.ContainsKey(rateItemKeyList[i]))
                    {
                        if (bonusValue > 0)
	                    {
                            occurInUpItemList[i] += 1;
	                    } else
                        {
                            occurInDownItemList[i] += 1;
                        }
                    }
                }
            }
        }
    }
}
