using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SelectImpl
{
    public class StrategyAsset
    {
        public Dictionary<String, HistoryData> straDataDict_;
        public Dictionary<String, HistoryData> straRaItemData_;

        public StrategyAsset()
        {
        }
        public void readAssetFromDB()
        {
            straDataDict_ = new Dictionary<String, HistoryData>();
            straRaItemData_ = new Dictionary<String, HistoryData>();
            DataTable straHisTable = DB.Global().Select("Select * From stra_his");
            foreach (DataRow row in straHisTable.Rows)
            {
                String straName = row["straName"].ToString();

                straDataDict_[straName] = HistoryData.FromDataRow(row);
            }
            foreach(IStrategy stra in App.grp_.strategyList_)
            {
                if (!straDataDict_.ContainsKey(stra.name()))
                {
                    straDataDict_.Add(stra.name(), null);
                }
                DataTable dt = stra.sh().Select("Select * From rateitem_his");
                foreach (DataRow row in dt.Rows)
                {
                    String rateItemKey = row["rateItemKey"].ToString();

                    straRaItemData_[rateItemKey] = HistoryData.FromDataRow(row);
                }
            }
        }
        public static void WriteStrategyAsset(IStrategy stra, HistoryData straData, Dictionary<String, HistoryData> straRaItemData)
        {
            DB.Global().Execute(String.Format("Delete From stra_his Where straname = '{0}'", stra.name()));
            Dictionary<String, Object> straDataDict = straData.toDictionary(stra.verTag());
            straDataDict["straname"] = stra.name();
            DB.Global().Insert("stra_his", straDataDict);
            stra.sh().Execute("Delete From rateitem_his");
            if (straRaItemData != null)
            {
                foreach (var kv in straRaItemData)
                {
                    Dictionary<String, Object> dataDict = kv.Value.toDictionary(stra.verTag());
                    dataDict["rateItemKey"] = kv.Key;
                    stra.sh().Insert("rateitem_his", dataDict);
                }
            }
        }

        public HistoryData straData(String straName)
        {
            return straDataDict_[straName];
        }
        public HistoryData straRateItemData(String rateItemKey)
        {
            HistoryData data;
            if (straRaItemData_.TryGetValue(rateItemKey, out data))
            {
                return data;
            }
            else
            {
                return null;
            }
        }
        public static float GetBackBonus(List<SelectItem> allItems)
        {
            allItems.Sort(delegate(SelectItem lhs, SelectItem rhs)
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
            float allBackBonusValue = 0;
            Dictionary<int, int> backBonusDataDict = new Dictionary<int, int>();
            for (int i = 0; i < allItems.Count; ++i)
            {
                SelectItem item = allItems[i];
                var bonus = item.getColumnVal("bonus");
                if (bonus == "")
                {
                    continue;
                }
                float allbonusValue = Utils.GetBonusValue(bonus);
                if (allbonusValue >= 0)
                {
                    break;
                }
                if (backBonusDataDict.ContainsKey(item.date_))
                {
                    continue;
                }
                backBonusDataDict[item.date_] = 0;
                allBackBonusValue += allbonusValue;
                if (backBonusDataDict.Count >= 10)
                {
                    break;
                }
            }
            if (backBonusDataDict.Count > 0)
            {
                return  allBackBonusValue / backBonusDataDict.Count;
            }
            else
            {
                return 0;
            }
        }
        public static HistoryData GetHistoryData(List<DateSelectItem> dateItems,
            int iStartIndex, int nCount, RunMode runMode)
        {
            List<SelectItem> allItems = new List<SelectItem>();
            for (int i = 0; i < nCount; i++)
            {
                if (iStartIndex + i >= dateItems.Count)
                {
                    break;
                }
                DateSelectItem dayData = dateItems[iStartIndex + i];
                allItems.AddRange(dayData.selItems_);
            }
            HistoryData data = new HistoryData();

            data.backBonusValue_ = GetBackBonus(allItems);
            for (int i = 0; i < allItems.Count; ++i)
            {
                SelectItem item = allItems[i];
                var bonus = item.getColumnVal("bonus");
                if (bonus == "")
                {
                    continue;
                }
                data.nHoldStockDays_ += Utils.ToType<int>(item.getColumnVal("tradespan"));
                float bonusValue = Utils.GetBonusValue(bonus);
                data.allBonusValue_ += bonusValue;
            }

            allItems.Clear();
            for (int i = 0; i < nCount; i++)
            {
                if (iStartIndex + i >= dateItems.Count)
                {
                    break;
                }
                DateSelectItem dayData = dateItems[iStartIndex + i];
                allItems.AddRange(dayData.goodSampleSelItems_);
            }
            data.gbackBonusValue_ = GetBackBonus(allItems);
            for (int i = 0; i < allItems.Count; ++i)
            {
                SelectItem item = allItems[i];
                var bonus = item.getColumnVal("bonus");
                if (bonus == "")
                {
                    continue;
                }
                data.nGoodSampleHoldStockDays_ += Utils.ToType<int>(item.getColumnVal("tradespan"));
                float bonusValue = Utils.GetBonusValue(bonus);
                data.allGoodSampleBonusValue_ += bonusValue;
            }


            data.nDayCount_ = iStartIndex + nCount - 1 >= dateItems.Count ?
                dateItems.Count - iStartIndex : nCount;
            int date0 = dateItems[iStartIndex].date_;
            int date1 = dateItems[iStartIndex + data.nDayCount_ - 1].date_;
            data.startDate_ = Math.Min(date0, date1);
            data.endDate_ = Math.Max(date0, date1);
            for (int i = 0; i < nCount; i++)
            {
                if (iStartIndex + i >= dateItems.Count)
                {
                    break;
                }
                DateSelectItem dayData = dateItems[iStartIndex + i];
                foreach (var selItem in dayData.selItems_)
                {
                    var bonus = selItem.getColumnVal("bonus");
                    if (bonus == "")
                    {
                        continue;
                    }
                    float bounusValue = Utils.GetBonusValue(bonus);
                    var envBonus = selItem.getColumnVal("envbonus");
                    if (envBonus != "")
                    {
                        float envBonusValue = Utils.GetBonusValue(envBonus);
                        if (envBonusValue < 0)
                        {
                            ++data.nAntiEnvCheckCount_;
                            if (bounusValue > 0)
                            {
                                ++data.nAntiEnvCount_;
                            }
                        }
                    }
                }
                data.nGoodSampleSelectCount_ += dayData.goodSampleSelItems_.Count;
                data.nAllSampleSelectCount_ += dayData.selItems_.Count;
                data.nDayMaxSelectCount_ = Math.Max(data.nDayMaxSelectCount_, dayData.selItems_.Count);
                foreach (var selItem in dayData.selItems_)
                {
                    var bonus = selItem.getColumnVal("bonus");
                    if (bonus == "")
                    {
                        continue;
                    }
                    float bounusValue = Utils.GetBonusValue(bonus);
                    if (bounusValue > 0)
                    {
                        ++data.nSelectSucCount_;
                    }
                }
                if (runMode == RunMode.RM_Asset)
                {
                    var buyItem = dayData.getBuyItem();
                    if (buyItem != null)
                    {
                        var bonus = buyItem.getColumnVal("bonus");
                        if (bonus != "")
                        {
                            float bounusValue = Utils.GetBonusValue(bonus);
                            if (bounusValue > 0)
                            {
                                data.nTradeSucCount_++;
                            }
                            data.tradeBonusValue_ += bounusValue;
                        }
                        ++data.nTradeCount_;
                    }

                    float dayAllBonusValue = 0;
                    foreach (var item in dayData.selItems_)
                    {
                        var bonus = item.getColumnVal("bonus");
                        if (bonus != "")
                        {
                            float bonusValue = Utils.GetBonusValue(bonus);
                            dayAllBonusValue += bonusValue;
                        }
                    }
                    if (dayAllBonusValue > 0)
                    {
                        data.nTradeAllSucCount_++;
                    }
                }
                else
                {
                    if (dayData.selItems_.Count > 0)
                    {
                        float dayAllBonusValue = 0;
                        foreach (var item in dayData.selItems_)
                        {
                            var bonus = item.getColumnVal("bonus");
                            if (bonus != "")
                            {
                                float bonusValue = Utils.GetBonusValue(bonus);
                                dayAllBonusValue += bonusValue;
                            }
                        }
                        if (dayAllBonusValue > 0)
                        {
                            data.nTradeSucCount_++;
                            data.nTradeAllSucCount_++;
                        }
                        data.tradeBonusValue_ += dayAllBonusValue / dayData.selItems_.Count;
                        if (dayData.selItems_.Count > 0)
                        {
                            data.nTradeCount_++;
                        }
                    }
                }
            }
            data.refrestStatistics();
            return data;
        }
        public bool collectRateItemHistory(RegressResult re,
            out Dictionary<String, HistoryData> strategyRateItemHistoryData,
            out List<HistoryData> dataList,
            out List<String> rateItemList
            )
        {
            Dictionary<String, List<SelectItem>> rateItemDict = RegressResult.ToRateItemDict(re.selItems_);
            var sortDict = from objDic in rateItemDict orderby objDic.Key select objDic;
            dataList = new List<HistoryData>();
            rateItemList = new List<string>();
            strategyRateItemHistoryData = new Dictionary<string, HistoryData>();
            foreach (var kv in sortDict)
            {
                if (kv.Value.Count < 20)
                {
                    continue;
                }
                rateItemList.Add(kv.Key);
                List<DateSelectItem> items = RegressResult.ToDaySelectItemList(kv.Value, re.dateRangeList_);
                HistoryData data = StrategyAsset.GetHistoryData(items, 0, items.Count, RunMode.RM_Raw);
                dataList.Add(data);
            }
            if (dataList.Count == 0)
            {
                return false;
            }
            for (int i = 0; i < rateItemList.Count; ++i)
            {
                strategyRateItemHistoryData[rateItemList[i]] = dataList[i];
            }
            return true;
        }
        public bool writeAssetForAllSingleStrategy(String dateRangeName)
        {
            DB.Global().Execute(String.Format("Delete From stra_his"));
            DB.Global().Execute(String.Format("Delete From stra_opt"));
            foreach (var item in App.autoSolutionSettingList_)
            {
                if (item.name_ == "$All" || item.name_ == "$Tmp")
                {
                    continue;
                }
                RegressResult re = new RegressResult();
                re.runMode_ = RunMode.RM_Raw;
                re.name_ = item.name_+"-"+dateRangeName;
                re.solutionName_ = item.name_;
                re.dateRangeName_ = dateRangeName;
                re.dateRangeList_ = App.DateRange(dateRangeName).rangeList_;
                re.strategyList_ = App.Solution(item.name_).straList_;

                if (re.EndDate >= Utils.LastTradeDay())
                {
                    if (!App.ds_.prepareForSelect())
                    {
                        MessageBox.Show("准备数据工作失败，无法继续执行！");
                        return false;
                    }
                }
                RegressManager regressManager = new RegressManager();
                regressManager.regress(re);

                List<DateSelectItem> items = RegressResult.ToDaySelectItemList(re.selItems_, re.dateRangeList_);
                re.reHistory_ = StrategyAsset.GetHistoryData(items, 0, items.Count, re.runMode_);

                Dictionary<String, HistoryData> strategyRateItemHistoryData;
                List<HistoryData> dataList;
                List<String> rateItemList;
                App.asset_.collectRateItemHistory(re, out strategyRateItemHistoryData, out dataList, out rateItemList);
                StrategyAsset.WriteStrategyAsset(re.strategyList_[0], re.reHistory_, strategyRateItemHistoryData);

                Dictionary<String, Object> straDataDict = re.reHistory_.toDictionary("");
                straDataDict["straname"] = re.solutionName_;
                DB.Global().Insert("stra_opt", straDataDict);
            }
            App.asset_.readAssetFromDB();
            App.host_.uiSetMsg("Write asset completed.");
            return true;
        }
        public void createStraTable(SQLiteHelper sh)
        {
            sh.Execute(@"CREATE TABLE rateitem_his ( 
                                rateItemKey               VARCHAR( 200 )   PRIMARY KEY
                                                                        UNIQUE,
                                tradeSucProbility      NUMERIC( 5, 2 )  NOT NULL,
                                tradeAllSucProbility      NUMERIC( 5, 2 )  NOT NULL,
                                selectSucProbility     NUMERIC( 5, 2 )  NOT NULL,
                                tradeBonusValue             NUMERIC( 5, 2 )  NOT NULL,
                                bTerTradeDay           NUMERIC( 5, 2 )  NOT NULL,  
                                antiRate               NUMERIC( 5, 2 )  NOT NULL,
                                tradeDayRate           NUMERIC( 5, 2 )  NOT NULL,
                                startDate              INT              NOT NULL,
                                endDate                INT              NOT NULL,
                                nDayCount              INT              NOT NULL,
                                nTradeCount            INT              NOT NULL,
                                nAntiEnvCount          INT              NOT NULL,
                                nAntiEnvCheckCount  INT              NOT NULL,
                                nSelectSucCount        INT              NOT NULL,
                                nTradeSucCount         INT              NOT NULL,
                                nTradeAllSucCount            INT  NOT NULL,  

                                bPerTradeDay           NUMERIC( 5, 2 )  NOT NULL,  
                                backBonusValue     NUMERIC( 5, 2 )  NOT NULL,
                                allBonusValue         NUMERIC( 5, 2 )              NOT NULL,
                                nHoldStockDays         INT              NOT NULL,
                                nAllSampleSelectCount INT              NOT NULL,

                                bGPerTradeDay           NUMERIC( 5, 2 )  NOT NULL,  
                                gbackBonusValue     NUMERIC( 5, 2 )  NOT NULL,
                                allGoodBonusValue         NUMERIC( 5, 2 )              NOT NULL,
                                nGoodHoldStockDays         INT              NOT NULL,
                                nGoodSampleSelectCount INT              NOT NULL,

                                nDayMaxSelectCount INT              NOT NULL,
                                nDayPerSelectCount   INT              NOT NULL,
                                rank                   INT              NOT NULL  ,
                                verTag                   VARCHAR( 100 )              NOT NULL 

                            );
                    ");
        }
    }
}
