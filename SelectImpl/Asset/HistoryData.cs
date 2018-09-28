using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace SelectImpl
{
    public class HistoryData
    {
        public float tradeSucProbility_ = -1;

        public float selectSucProbility_ = -1;
        public float bonusValue_;
        public float antiRate_ = -1;
        public float tradeDayRate_ = -1;
        public float dontBuyRate_ = 1;//不买确实跌了/不买却涨了
        public float bPerTradeDay_ = 0;
        public int rank_;

        public int startDate_;
        public int endDate_;
        public int nDayCount_;
        public int nTradeCount_;
        public int nAllSampleSelectCount_;
        public int nGoodSampleSelectCount_;
        public int nAntiEnvCount_;
        public int nAntiEnvCheckCount_;
        public int nSelectSucCount_;
        public int nTradeSucCount_;
        public int nDontBuyAndDown_;
        public int nDontBuyButUp_;
        public static ColumnInfo[] ShowColumnInfos
        {
            get {
                return new ColumnInfo[]
                {
                    new ColumnInfo() { name_ = "rank", width_ = 60 },
                    new ColumnInfo() { name_ = "tsp", width_ = 60 },
                    new ColumnInfo() { name_ = "ssp", width_ = 60 },
                    new ColumnInfo() { name_ = "bonus", width_ = 60 },
                    new ColumnInfo() { name_ = "atr", width_ = 60 },
                    new ColumnInfo() { name_ = "tdr", width_ = 60 },
                    new ColumnInfo() { name_ = "dbr", width_ = 60 },
                    new ColumnInfo() { name_ = "days", width_ = 60 },
                    new ColumnInfo() { name_ = "trades", width_ = 60 },
                    new ColumnInfo() { name_ = "bpertr", width_ = 60 },
                    new ColumnInfo() { name_ = "samples", width_ = 60 },
                    new ColumnInfo() { name_ = "allselects", width_ = 70 },
                    new ColumnInfo() { name_ = "antic", width_ = 60 },
                    new ColumnInfo() { name_ = "anticc", width_ = 60 },
                    new ColumnInfo() { name_ = "dbad", width_ = 60 },
                    new ColumnInfo() { name_ = "dbbu", width_ = 60 },
                    new ColumnInfo() { name_ = "startDate", width_ = 60 },
                    new ColumnInfo() { name_ = "endDate", width_ = 60 },
                };
            }
        }
        public ListViewItem toListViewItem(ListView lv, String slnName)
        {
            ListViewItem lvi = new ListViewItem(slnName);
            lvi.SubItems.Add(rank_.ToString());
            lvi.SubItems.Add(tradeSucProbility_.ToString("F2"));
            lvi.SubItems.Add(selectSucProbility_.ToString("F2"));
            lvi.SubItems.Add(bonusValue_.ToString("F2")+"%");
            lvi.SubItems.Add(antiRate_.ToString("F2"));
            lvi.SubItems.Add(tradeDayRate_.ToString("F2"));
            lvi.SubItems.Add(dontBuyRate_.ToString("F2"));
            lvi.SubItems.Add(nDayCount_.ToString());
            lvi.SubItems.Add(nTradeCount_.ToString());
            lvi.SubItems.Add(bPerTradeDay_.ToString("F2") + "%");
            lvi.SubItems.Add(nGoodSampleSelectCount_.ToString());
            lvi.SubItems.Add(nAllSampleSelectCount_.ToString());
            lvi.SubItems.Add(nAntiEnvCount_.ToString());
            lvi.SubItems.Add(nAntiEnvCheckCount_.ToString());
            lvi.SubItems.Add(nDontBuyAndDown_.ToString());
            lvi.SubItems.Add(nDontBuyButUp_.ToString());
            lvi.SubItems.Add(startDate_.ToString());
            lvi.SubItems.Add(endDate_.ToString());
            return lvi;
        }
        public static HistoryData FromDataRow(DataRow row)
        {
            HistoryData data = new HistoryData();
            data.tradeSucProbility_ = Utils.ToType<float>(row["tradeSucProbility"]);
            data.selectSucProbility_ = Utils.ToType<float>(row["selectSucProbility"]);
            data.bonusValue_ = Utils.ToType<float>(row["bonusValue"]);
            data.antiRate_ = Utils.ToType<float>(row["antiRate"]);
            data.tradeDayRate_ = Utils.ToType<float>(row["tradeDayRate"]);
            data.dontBuyRate_ = Utils.ToType<float>(row["dontBuyRate"]);
            data.rank_ = Utils.ToType<int>(row["rank"]);
            data.startDate_ = Utils.ToType<int>(row["startDate"]);
            data.endDate_ = Utils.ToType<int>(row["endDate"]);
            data.nDayCount_ = Utils.ToType<int>(row["nDayCount"]);
            data.nTradeCount_ = Utils.ToType<int>(row["nTradeCount"]);
            data.bPerTradeDay_ = Utils.ToType<float>(row["bPerTradeDay"]);
            data.nGoodSampleSelectCount_ = Utils.ToType<int>(row["nGoodSampleSelectCount"]);
            data.nAllSampleSelectCount_ = Utils.ToType<int>(row["nAllSampleSelectCount"]);
            data.nAntiEnvCount_ = Utils.ToType<int>(row["nAntiEnvCount"]);
            data.nAntiEnvCheckCount_ = Utils.ToType<int>(row["nAntiEnvCheckCount"]);
            data.nSelectSucCount_ = Utils.ToType<int>(row["nSelectSucCount"]);
            data.nTradeSucCount_ = Utils.ToType<int>(row["nTradeSucCount"]);
            data.nDontBuyAndDown_ = Utils.ToType<int>(row["nDontBuyAndDown"]);
            data.nDontBuyButUp_ = Utils.ToType<int>(row["nDontBuyButUp"]);
            return data;
        }
        public Dictionary<String, Object> toDictionary(String verTag)
        {
            Dictionary<String, Object> dict = new Dictionary<String, Object>();
            dict["tradeSucProbility"] = tradeSucProbility_;
            dict["selectSucProbility"] = selectSucProbility_;
            dict["bonusValue"] = bonusValue_;
            dict["antiRate"] = antiRate_;
            dict["tradeDayRate"] = tradeDayRate_;
            dict["dontBuyRate"] = dontBuyRate_;
            dict["rank"] = rank_;
            dict["startDate"] = startDate_;
            dict["endDate"] = endDate_;
            dict["nDayCount"] = nDayCount_;
            dict["nTradeCount"] = nTradeCount_;
            dict["bPerTradeDay"] = bPerTradeDay_;
            dict["nGoodSampleSelectCount"] = nGoodSampleSelectCount_;
            dict["nAllSampleSelectCount"] = nAllSampleSelectCount_;
            dict["nAntiEnvCount"] = nAntiEnvCount_;
            dict["nAntiEnvCheckCount"] = nAntiEnvCheckCount_;
            dict["nSelectSucCount"] = nSelectSucCount_;
            dict["nTradeSucCount"] = nTradeSucCount_;
            dict["nDontBuyAndDown"] = nDontBuyAndDown_;
            dict["nDontBuyButUp"] = nDontBuyButUp_;
            dict["verTag"] = verTag;
            return dict;
        }
        public static HistoryData MergeHistory(List<HistoryData> dataList)
        {
            HistoryData allData = new HistoryData();

            allData.startDate_ = (from data in dataList
                                  orderby data.startDate_
                                  select data.startDate_).ToList().FirstOrDefault();
            allData.endDate_ = (from data in dataList
                                orderby data.endDate_ descending
                                select data.endDate_).ToList().FirstOrDefault();

            foreach (var data in dataList)
            {
                allData.bonusValue_ += data.bonusValue_;
                allData.nDayCount_ += data.nDayCount_;
                allData.nTradeCount_ += data.nTradeCount_;
                allData.nGoodSampleSelectCount_ += data.nGoodSampleSelectCount_;
                allData.nAllSampleSelectCount_ += data.nAllSampleSelectCount_;
                allData.nAntiEnvCount_ += data.nAntiEnvCount_;
                allData.nAntiEnvCheckCount_ += data.nAntiEnvCheckCount_;
                allData.nSelectSucCount_ += data.nSelectSucCount_;
                allData.nTradeSucCount_ += data.nTradeSucCount_;
                allData.nDontBuyAndDown_ += data.nDontBuyAndDown_;
                allData.nDontBuyButUp_ += data.nDontBuyButUp_;
            }
            allData.refrestStatistics();
            return allData;
        }
        public void refrestStatistics()
        {
            if (nTradeCount_ == 0)
            {
                tradeSucProbility_ = -1;
            }
            else
            {
                tradeSucProbility_ = nTradeSucCount_ * 1.0f / nTradeCount_;
            }
            if (nAllSampleSelectCount_ == 0)
            {
                selectSucProbility_ = -1;
            }
            else
            {
                selectSucProbility_ = nSelectSucCount_ * 1.0f / nAllSampleSelectCount_;
            }
            if (nAntiEnvCheckCount_ == 0)
            {
                antiRate_ = 0;
            }
            else
            {
                antiRate_ = nAntiEnvCount_ * 1.0f / nAntiEnvCheckCount_;
            }
            tradeDayRate_ = nTradeCount_ * 1.0f/ nDayCount_;
            if (nDontBuyAndDown_ == 0 && nDontBuyButUp_ == 0)
            {
                dontBuyRate_ = 1;
            }
            else if (nDontBuyButUp_ == 0)
            {
                dontBuyRate_ = 1000;
            }
            else
            {
                dontBuyRate_ = nDontBuyAndDown_ * 1.0f / nDontBuyButUp_;
            }
            if (nTradeCount_ == 0)
            {
                bPerTradeDay_ = 0;
            } else
            {
                bPerTradeDay_ = bonusValue_ / nTradeCount_;
            }
            float probility = tradeSucProbility_ < 0 ? selectSucProbility_ : tradeSucProbility_;
            rank_ = (int)(100 * probility * priority());
        }
        public float priority()
        {
            int bonusValueRank/*30*/, antiRateRank/*35*/, tradeDayRateRank/*15*/, dontBuyRank/*20*/;
            bonusValueRank = (int)(bPerTradeDay_ / 2.0f * 30);
            if (antiRate_ > 0)
            {
                antiRateRank = (int)(antiRate_ * 35);
            }
            else
            {
                antiRateRank = 10;
            }
            tradeDayRateRank = (int)(tradeDayRate_ * 15);
            dontBuyRank = (int)(dontBuyRate_ * 20);

            return (bonusValueRank + antiRateRank + tradeDayRateRank + dontBuyRank) / 100.0f;
        }
        public void recalProbilityForRateItem(int occurInUpItem, int occurInDownItem, int nUpSelCount, int nDownSelCount)
        {
            if (nGoodSampleSelectCount_ == 0)
	        {
		        tradeSucProbility_ = selectSucProbility_ = -1;
	        } else {
                float upProbility = nUpSelCount*1.0f/nGoodSampleSelectCount_;
                float downProbility = nDownSelCount*1.0f/nGoodSampleSelectCount_;
                float rateItemInUpSelProbility = nUpSelCount == 0 ? 1 : occurInUpItem*1.0f/nUpSelCount;
                float rateItemInDownSelProbility = nDownSelCount == 0 ? 1: occurInDownItem*1.0f/nDownSelCount;
                tradeSucProbility_ = selectSucProbility_ =
                    (rateItemInUpSelProbility*upProbility)/(rateItemInUpSelProbility*upProbility + rateItemInDownSelProbility*downProbility);
            }
        }
    }
}
