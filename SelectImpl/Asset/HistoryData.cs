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
        public float tradeAllSucProbility_ = -1;
        public float selectSucProbility_ = -1;
        public float tradeBonusValue_;
        public float backBonusValue_;
        public float gbackBonusValue_;
        public float antiRate_ = -1;
        public float tradeDayRate_ = -1;
        public float bPerTradeDay_ = 0;
        public float bGPerTradeDay_ = 0;
        public float bTerTradeDay_ = 0;
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
        public int nTradeAllSucCount_;
        public int nHoldStockDays_;
        public float allBonusValue_;
        public int nGoodSampleHoldStockDays_;
        public float allGoodSampleBonusValue_;
        public int nDayMaxSelectCount_;
        public int nDayPerSelectCount_;
        public int nPlusCount_;
        public int nMinusCount_;
        public int nDeadCount_;
        public int nPartialMinusCount_;
        public float plusRate_;
        public float minusRate_;
        public float deadRate_;
        public float partialMinusRate_;
        public static ColumnInfo[] ShowColumnInfos
        {
            get {
                return new ColumnInfo[]
                {
                    new ColumnInfo() { name_ = "rank", width_ = 60 },
                    new ColumnInfo() { name_ = "prate", width_ = 60 },
                    new ColumnInfo() { name_ = "mrate", width_ = 60 },
                    new ColumnInfo() { name_ = "drate", width_ = 60 },
                    new ColumnInfo() { name_ = "pmrate", width_ = 60 },
                    new ColumnInfo() { name_ = "tsp", width_ = 60 },
                    new ColumnInfo() { name_ = "tasp", width_ = 60 },
                    new ColumnInfo() { name_ = "ssp", width_ = 60 },
                    new ColumnInfo() { name_ = "bonus", width_ = 60 },
                    new ColumnInfo() { name_ = "btertr", width_ = 60 },
                    new ColumnInfo() { name_ = "atr", width_ = 60 },
                    new ColumnInfo() { name_ = "tdr", width_ = 60 },
                    new ColumnInfo() { name_ = "days", width_ = 60 },
                    new ColumnInfo() { name_ = "trades", width_ = 60 },

                    new ColumnInfo() { name_ = "gbpertr", width_ = 60 },
                    new ColumnInfo() { name_ = "gbackbonus", width_ = 60 },
                    new ColumnInfo() { name_ = "gholddays", width_ = 60 },
                    new ColumnInfo() { name_ = "gallbonus", width_ = 60 },

                    new ColumnInfo() { name_ = "bpertr", width_ = 60 },
                    new ColumnInfo() { name_ = "backbonus", width_ = 65 },
                    new ColumnInfo() { name_ = "holddays", width_ = 60 },
                    new ColumnInfo() { name_ = "allbonus", width_ = 60 },

                    new ColumnInfo() { name_ = "maxsel", width_ = 60 },
                    new ColumnInfo() { name_ = "persel", width_ = 60 },
                    new ColumnInfo() { name_ = "samples", width_ = 60 },
                    new ColumnInfo() { name_ = "allselects", width_ = 70 },
                    new ColumnInfo() { name_ = "antic", width_ = 60 },
                    new ColumnInfo() { name_ = "anticc", width_ = 60 },
                    new ColumnInfo() { name_ = "startDate", width_ = 60 },
                    new ColumnInfo() { name_ = "endDate", width_ = 60 },
                };
            }
        }
        public ListViewItem toListViewItem(ListView lv, String slnName)
        {
            ListViewItem lvi = new ListViewItem(slnName);
            lvi.SubItems.Add(rank_.ToString());
            lvi.SubItems.Add(plusRate_.ToString("F5"));
            lvi.SubItems.Add(minusRate_.ToString("F5"));
            lvi.SubItems.Add(deadRate_.ToString("F5"));
            lvi.SubItems.Add(partialMinusRate_.ToString("F5"));
            lvi.SubItems.Add(tradeSucProbility_.ToString("F2"));
            lvi.SubItems.Add(tradeAllSucProbility_.ToString("F2"));
            lvi.SubItems.Add(selectSucProbility_.ToString("F2"));
            lvi.SubItems.Add(tradeBonusValue_.ToString("F2") + "%");
            lvi.SubItems.Add(bTerTradeDay_.ToString("F2") + "%");
            lvi.SubItems.Add(antiRate_.ToString("F2"));
            lvi.SubItems.Add(tradeDayRate_.ToString("F2"));
            lvi.SubItems.Add(nDayCount_.ToString());
            lvi.SubItems.Add(nTradeCount_.ToString());

            lvi.SubItems.Add(bGPerTradeDay_.ToString("F2") + "%");
            lvi.SubItems.Add(gbackBonusValue_.ToString("F2") + "%");
            lvi.SubItems.Add(nGoodSampleHoldStockDays_.ToString());
            lvi.SubItems.Add(allGoodSampleBonusValue_.ToString("F2") + "%");

            lvi.SubItems.Add(bPerTradeDay_.ToString("F2") + "%");
            lvi.SubItems.Add(backBonusValue_.ToString("F2") + "%");
            lvi.SubItems.Add(nHoldStockDays_.ToString());
            lvi.SubItems.Add(allBonusValue_.ToString("F2") + "%");

            lvi.SubItems.Add(nDayMaxSelectCount_.ToString());
            lvi.SubItems.Add(nDayPerSelectCount_.ToString());
            lvi.SubItems.Add(nGoodSampleSelectCount_.ToString());
            lvi.SubItems.Add(nAllSampleSelectCount_.ToString());
            lvi.SubItems.Add(nAntiEnvCount_.ToString());
            lvi.SubItems.Add(nAntiEnvCheckCount_.ToString());
            lvi.SubItems.Add(startDate_.ToString());
            lvi.SubItems.Add(endDate_.ToString());
            return lvi;
        }
        public static HistoryData FromDataRow(DataRow row)
        {
            HistoryData data = new HistoryData();
            data.plusRate_ = Utils.ToType<float>(row["plusRate"]);
            data.minusRate_ = Utils.ToType<float>(row["minusRate"]);
            data.deadRate_ = Utils.ToType<float>(row["deadRate"]);
            data.partialMinusRate_ = Utils.ToType<float>(row["partialMinusRate"]);
            data.nPlusCount_ = Utils.ToType<int>(row["nPlusCount"]);
            data.nMinusCount_ = Utils.ToType<int>(row["nMinusCount"]);
            data.nDeadCount_ = Utils.ToType<int>(row["nDeadCount"]);
            data.nPartialMinusCount_ = Utils.ToType<int>(row["nPartialMinusCount"]);
            data.tradeSucProbility_ = Utils.ToType<float>(row["tradeSucProbility"]);
            data.tradeAllSucProbility_ = Utils.ToType<float>(row["tradeAllSucProbility"]);
            data.selectSucProbility_ = Utils.ToType<float>(row["selectSucProbility"]);

            data.tradeBonusValue_ = Utils.ToType<float>(row["tradeBonusValue"]);
            data.bTerTradeDay_ = Utils.ToType<float>(row["bTerTradeDay"]);

            data.bGPerTradeDay_ = Utils.ToType<float>(row["bGPerTradeDay"]);
            data.gbackBonusValue_ = Utils.ToType<float>(row["gbackBonusValue"]);
            data.allGoodSampleBonusValue_ = Utils.ToType<float>(row["allGoodBonusValue"]);
            data.nGoodSampleHoldStockDays_ = Utils.ToType<int>(row["nGoodHoldStockDays"]);
            data.nGoodSampleSelectCount_ = Utils.ToType<int>(row["nGoodSampleSelectCount"]);


            data.bPerTradeDay_ = Utils.ToType<float>(row["bPerTradeDay"]);
            data.backBonusValue_ = Utils.ToType<float>(row["backBonusValue"]);
            data.allBonusValue_ = Utils.ToType<float>(row["allBonusValue"]);
            data.nHoldStockDays_ = Utils.ToType<int>(row["nHoldStockDays"]);
            data.nAllSampleSelectCount_ = Utils.ToType<int>(row["nAllSampleSelectCount"]);

            data.antiRate_ = Utils.ToType<float>(row["antiRate"]);
            data.tradeDayRate_ = Utils.ToType<float>(row["tradeDayRate"]);
            data.rank_ = Utils.ToType<int>(row["rank"]);
            data.startDate_ = Utils.ToType<int>(row["startDate"]);
            data.endDate_ = Utils.ToType<int>(row["endDate"]);
            data.nDayCount_ = Utils.ToType<int>(row["nDayCount"]);
            data.nTradeCount_ = Utils.ToType<int>(row["nTradeCount"]);
            data.nAntiEnvCount_ = Utils.ToType<int>(row["nAntiEnvCount"]);
            data.nAntiEnvCheckCount_ = Utils.ToType<int>(row["nAntiEnvCheckCount"]);
            data.nSelectSucCount_ = Utils.ToType<int>(row["nSelectSucCount"]);
            data.nTradeSucCount_ = Utils.ToType<int>(row["nTradeSucCount"]);
            data.nTradeAllSucCount_ = Utils.ToType<int>(row["nTradeAllSucCount"]);
            data.nDayMaxSelectCount_ = Utils.ToType<int>(row["nDayMaxSelectCount"]);
            data.nDayPerSelectCount_ = Utils.ToType<int>(row["nDayPerSelectCount"]);
            return data;
        }
        public Dictionary<String, Object> toDictionary(String verTag)
        {
            Dictionary<String, Object> dict = new Dictionary<String, Object>();
            dict["plusRate"] = plusRate_;
            dict["minusRate"] = minusRate_;
            dict["deadRate"] = deadRate_;
            dict["partialMinusRate"] = partialMinusRate_;
            dict["nPlusCount"] = nPlusCount_;
            dict["nMinusCount"] = nMinusCount_;
            dict["nDeadCount"] = nDeadCount_;
            dict["nPartialMinusCount"] = nPartialMinusCount_;
            dict["tradeSucProbility"] = tradeSucProbility_;
            dict["tradeAllSucProbility"] = tradeAllSucProbility_;
            dict["selectSucProbility"] = selectSucProbility_;

            dict["tradeBonusValue"] = tradeBonusValue_;
            dict["bTerTradeDay"] = bTerTradeDay_;

            dict["bGPerTradeDay"] = bGPerTradeDay_;
            dict["gbackBonusValue"] = gbackBonusValue_;
            dict["allGoodBonusValue"] = allGoodSampleBonusValue_;
            dict["nGoodSampleSelectCount"] = nGoodSampleSelectCount_;
            dict["nGoodHoldStockDays"] = nGoodSampleHoldStockDays_;

            dict["bPerTradeDay"] = bPerTradeDay_;
            dict["backBonusValue"] = backBonusValue_;
            dict["allBonusValue"] = allBonusValue_;
            dict["nHoldStockDays"] = nHoldStockDays_;
            dict["nAllSampleSelectCount"] = nAllSampleSelectCount_;

            dict["antiRate"] = antiRate_;
            dict["tradeDayRate"] = tradeDayRate_;
            dict["rank"] = rank_;
            dict["startDate"] = startDate_;
            dict["endDate"] = endDate_;
            dict["nDayCount"] = nDayCount_;
            dict["nTradeCount"] = nTradeCount_;
            dict["nAntiEnvCount"] = nAntiEnvCount_;
            dict["nAntiEnvCheckCount"] = nAntiEnvCheckCount_;
            dict["nSelectSucCount"] = nSelectSucCount_;
            dict["nTradeSucCount"] = nTradeSucCount_;
            dict["nTradeAllSucCount"] = nTradeAllSucCount_;
            dict["nDayMaxSelectCount"] = nDayMaxSelectCount_;
            dict["nDayPerSelectCount"] = nDayPerSelectCount_;
            dict["verTag"] = verTag;
            return dict;
        }
        public void refrestStatistics()
        {
            if (nTradeCount_ == 0)
            {
                tradeSucProbility_ = -1;
                tradeAllSucProbility_ = -1;
                bTerTradeDay_ = 0;
                nDayPerSelectCount_ = 0;
            }
            else
            {
                tradeSucProbility_ = nTradeSucCount_ * 1.0f / nTradeCount_;
                tradeAllSucProbility_ = nTradeAllSucCount_ * 1.0f / nTradeCount_;
                bTerTradeDay_ = tradeBonusValue_ / nTradeCount_;
                nDayPerSelectCount_ = (int)Math.Ceiling(nGoodSampleSelectCount_ * 1.0 / nTradeCount_);
            }
            if (nGoodSampleSelectCount_ == 0)
            {
                selectSucProbility_ = -1;
                plusRate_ = -1;
                minusRate_ = -1;
                deadRate_ = -1;
                partialMinusRate_ = -1;
            }
            else
            {
                selectSucProbility_ = nSelectSucCount_ * 1.0f / nGoodSampleSelectCount_;
                plusRate_ = nPlusCount_ * 1.0f / nGoodSampleSelectCount_;
                minusRate_ = nMinusCount_ * 1.0f / nGoodSampleSelectCount_;
                deadRate_ = nDeadCount_ * 1.0f / nGoodSampleSelectCount_;
                partialMinusRate_ = nPartialMinusCount_ * 1.0f / nGoodSampleSelectCount_;
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
            if (nHoldStockDays_ == 0)
            {
                bPerTradeDay_ = 0;
            } else
            {
                bPerTradeDay_ = allBonusValue_ / nHoldStockDays_;
            }
            if (nGoodSampleHoldStockDays_ == 0)
            {
                bGPerTradeDay_ = 0;
            }
            else
            {
                bGPerTradeDay_ = allGoodSampleBonusValue_ / nGoodSampleHoldStockDays_;
            }
            float probility = tradeSucProbility_ < 0 ? selectSucProbility_ : tradeSucProbility_;
            rank_ = (int)(100 * /*probility * */priority());
        }
        public float priority()
        {
            int plusRank/*25*/, minusRank/*25*/, deadRank/*50*/, partialMinusRank/*10*/;
            plusRank = (int)(plusRate_ * 25);
            minusRank = (int)((1 - minusRate_) * 25);
            deadRank = (int)((1 - deadRate_) * 50);
            partialMinusRank = (int)((1 - partialMinusRate_) * 10);
            return (plusRank + minusRank + deadRank + partialMinusRank) / 110.0f;
//             int bonusValueRank/*40*/, gbackBonusValueRank/*30*/, antiRateRank/*15*/, tradeDayRateRank/*15*/;
//             bonusValueRank = (int)(bGPerTradeDay_ / 2.0f * 40);
//             if (gbackBonusValue_ >= 0)
//             {
//                 gbackBonusValueRank = 30;
//             }
//             else
//             {
//                 gbackBonusValueRank = (int)((20 + gbackBonusValue_) * 30 / 20);
//             }
//             if (antiRate_ > 0)
//             {
//                 antiRateRank = (int)(antiRate_ * 15);
//             }
//             else
//             {
//                 antiRateRank = 5;
//             }
//             tradeDayRateRank = (int)(tradeDayRate_ * 15);
// 
//             return (bonusValueRank + gbackBonusValueRank + antiRateRank + tradeDayRateRank) / 100.0f;
        }
    }
}
