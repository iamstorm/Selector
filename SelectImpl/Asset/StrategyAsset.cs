using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SQLite;

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
            straDataDict_.Add(StrategySetting.DontbuyStrategyName, null);
            straDataDict_.Add(StrategySetting.MissStrategyName, null);
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
        public static HistoryData GetHistoryData(List<DateSelectItem> dateItems,
            int iStartIndex, int nCount, RunMode runMode)
        {
            HistoryData data = new HistoryData();
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
                foreach (var selItem in dayData.goodSampleSelItems_)
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
                    if (buyItem != null && buyItem.isRealSelectItem)
                    {
                        var bonus = buyItem.getColumnVal("bonus");
                        if (bonus != "")
                        {
                            float bounusValue = Utils.GetBonusValue(bonus);
                            if (bounusValue > 0)
                            {
                                data.nTradeSucCount_++;
                            }
                            data.bonusValue_ += bounusValue;
                        }
                        ++data.nTradeCount_;
                    }
                    if (buyItem != null && buyItem.strategyName_ == StrategySetting.DontbuyStrategyName)
                    {
                        var candidateList = RankBuyDesider.buyer_.getAllPriCompeteSucList(dayData.selItems_);
                        foreach (var candidate in candidateList)
                        {
                            var bonus = candidate.getColumnVal("bonus");
                            if (bonus == "")
                            {
                                continue;
                            }
                            float bounusValue = Utils.GetBonusValue(bonus);
                            if (bounusValue > 0)
                            {
                                ++data.nDontBuyButUp_;
                            }
                            else
                            {
                                ++data.nDontBuyAndDown_;
                            }
                        }
                    }
                }
                else if (dayData.selItems_.Count > 0)
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
                    }
                    data.bonusValue_ += dayAllBonusValue / dayData.selItems_.Count;
                    if (dayData.selItems_.Count > 0)
                    {
                        data.nTradeCount_++;
                    }
                }
            }
            data.refrestStatistics();
            return data;
        }
        public void createStraTable(SQLiteHelper sh)
        {
            sh.Execute(@"CREATE TABLE rateitem_his ( 
                                rateItemKey               VARCHAR( 200 )   PRIMARY KEY
                                                                        UNIQUE,
                                tradeSucProbility      NUMERIC( 5, 2 )  NOT NULL,
                                selectSucProbility     NUMERIC( 5, 2 )  NOT NULL,
                                bonusValue             NUMERIC( 5, 2 )  NOT NULL,
                                antiRate               NUMERIC( 5, 2 )  NOT NULL,
                                tradeDayRate           NUMERIC( 5, 2 )  NOT NULL,
                                dontBuyRate            NUMERIC( 5, 2 )  NOT NULL,
                                startDate              INT              NOT NULL,
                                endDate                INT              NOT NULL,
                                nDayCount              INT              NOT NULL,
                                nTradeCount            INT              NOT NULL,
                                nGoodSampleSelectCount INT              NOT NULL,
                                nAllSampleSelectCount INT              NOT NULL,
                                nAntiEnvCount          INT              NOT NULL,
                                nAntiEnvCheckCount  INT              NOT NULL,
                                nSelectSucCount        INT              NOT NULL,
                                nTradeSucCount         INT              NOT NULL,
                                nDontBuyAndDown        INT              NOT NULL,
                                nDontBuyButUp          INT              NOT NULL,
                                bPerTradeDay           NUMERIC( 5, 2 )  NOT NULL,  
                                rank                   INT              NOT NULL  ,
                                verTag                   VARCHAR( 100 )              NOT NULL 

                            );
                    ");
        }
    }
}
