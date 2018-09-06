using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SelectImpl;
using System.Windows.Forms.DataVisualization.Charting;

namespace SelectorForm
{
    public enum WantDataType
    {
        WD_BonusData,
        WD_RawHistory,
        WD_RateItemData,
        WD_HistoryData,
    }
    public partial class RegressStatisticsForm : Form
    {
        public WantDataType wd_;
        public RegressResult re_;
        public Dictionary<String, HistoryData> strategyRateItemHistoryData_;
        public RegressStatisticsForm(RegressResult re, WantDataType wd)
        {
            InitializeComponent();
            re_ = re;
            wd_ = wd;

            menuStrip1.Visible = false;
            chart_.Titles.Add("Title");
            string sMode = re.runMode_ == RunMode.RM_Asset ? "Asset mode" : "Raw mode";
            chart_.Titles[0].Text = String.Format("{0} {1}", re.solutionName_, sMode);
            switch (wd_)
            {
                case WantDataType.WD_BonusData:
                    fillBonusChart();
                    break;
                case WantDataType.WD_RawHistory:
                    fillRawHistoryChart();
                    break;
                case WantDataType.WD_RateItemData:
                    fillRateItemChart();
                    menuStrip1.Visible = true;
                    break;
                case WantDataType.WD_HistoryData:
                    fillHistoryDataChart();
                    if (re_.reHistory_ == null)
                    {
                        break;
                    }
                    historyView.Columns.Add("solution", 80);
                    LUtils.InitListView(historyView, HistoryData.ShowColumnInfos);
                    historyView.Items.Add(re_.reHistory_.toListViewItem(historyView, re_.solutionName_));
                    break;
                default:
                    throw new ArgumentException("Unknown data type!");
            }

        }
        public void selItemsHistoryDataToChart(Chart chart, List<SelectItem> selItems, List<DateRange> dateRangeList, RunMode runMode, out ChartXUnit xUnit)
        {
            List<HistoryData> dataList = new List<HistoryData>();
            List<DateSelectItem> items = RegressResult.ToDaySelectItemList(selItems, dateRangeList);
            xUnit = SUtils.DetermineXUnit(items.Count);
            if (items.Count == 0)
            {
                return;
            }
            if (xUnit == ChartXUnit.CXU_Year)
            {
                items.Reverse();
                int startDate = items[0].date_;
                int endDate = items[items.Count - 1].date_;
                int startYear = Utils.Year(startDate);
                int endYear = Utils.Year(endDate);
                int iSearchStartHint = 0;
                for (int iYear = startYear; iYear <= endYear; iYear++)
                {
                    int iStartIndex = -1;
                    int nDaySpan = 0;
                    for (int i = iSearchStartHint; i < items.Count; i++)
                    {
                        if (iStartIndex == -1 && Utils.Year(items[i].date_) == iYear)
                        {
                            iStartIndex = i;
                        }
                        if (iStartIndex != -1)
                        {
                            if (Utils.Year(items[i].date_) == iYear)
                            {
                                nDaySpan++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    iSearchStartHint = iStartIndex + nDaySpan;
                    HistoryData data = StrategyAsset.GetHistoryData(items, iStartIndex, nDaySpan, runMode);
                    dataList.Add(data);
                }
            }
            else
            {
                int nDaySpan = SUtils.ToDayCount(xUnit);
                int iStartIndex = 0;
                do
                {
                    if (iStartIndex + nDaySpan - 1 > items.Count)
                    {
                        break;
                    }
                    HistoryData data = StrategyAsset.GetHistoryData(items, iStartIndex, nDaySpan, runMode);
                    dataList.Add(data);
                    iStartIndex += nDaySpan;
                } while (iStartIndex < items.Count);
                dataList.Reverse();
            }

            List<String> xDescList = new List<string>();
            foreach (var item in dataList)
            {
                xDescList.Add(SUtils.ToXDesc(xUnit, item.startDate_, item.endDate_));
            }
            SUtils.FillHistoryDataToChart(chart, dataList, xDescList);
            re_.reHistory_ = HistoryData.MergeHistory(dataList);
        }

        public void fillHistoryDataChart()
        {
            if (re_.selItems_.Count == 0)
            {
                return;
            }
            ChartXUnit xUnit;
            selItemsHistoryDataToChart(chart_, re_.selItems_, re_.dateRangeList_, re_.runMode_, out xUnit);
            chart_.Titles[0].Text  = chart_.Titles[0].Text + ", " + SUtils.ToUnitName(xUnit) + ", " + re_.getTradeDayCount() + "days";
        }
        void fillBonusChart()
        {
            List<BonusRange> bonusRangeList = new List<BonusRange>();
            for (int i = -10; i < 10; i++)
            {
                bonusRangeList.Add(new BonusRange(i, i + 1));
            }
            List<SelectItem> selItems = re_.runMode_ == RunMode.RM_Asset ? re_.buyItems_ : re_.selItems_;
            foreach (var item in selItems)
            {
                var bonus = item.getColumnVal("bonus");
                if (bonus != "")
                {
                    float bonusValue = Utils.GetBonusValue(bonus);
                    int iIndex = (int)Math.Floor(bonusValue) + 10;
                    if (iIndex < 0)
                    {
                        iIndex = 0;
                    }
                    else if (iIndex >= bonusRangeList.Count)
                    {
                        iIndex = bonusRangeList.Count - 1;
                    }
                    bonusRangeList[iIndex].nTradeCount_ += 1;
                }
                var nsh = item.getColumnVal("nsh");
                if (nsh != "")
                {
                    float nshValue = Utils.GetBonusValue(nsh);
                    int iIndex = (int)Math.Floor(nshValue) + 10;
                    if (iIndex < 0)
                    {
                        iIndex = 0;
                    }
                    else if (iIndex >= bonusRangeList.Count)
                    {
                        iIndex = bonusRangeList.Count - 1;
                    }
                    bonusRangeList[iIndex].nHfCount_ += 1;
                }
                var nsc = item.getColumnVal("nsc");
                if (nsc != "")
                {
                    float nscValue = Utils.GetBonusValue(nsc);
                    int iIndex = (int)Math.Floor(nscValue) + 10;
                    if (iIndex < 0)
                    {
                        iIndex = 0;
                    }
                    else if (iIndex >= bonusRangeList.Count)
                    {
                        iIndex = bonusRangeList.Count - 1;
                    }
                    bonusRangeList[iIndex].nCfCount_ += 1;
                }
            }
            SUtils.FillBonusChart(chart_, bonusRangeList);
        }
        void fillRateItemChart()
        {
            if (re_.runMode_ != RunMode.RM_Raw)
            {
                return;
            }
//             List<DateSelectItem> dayItems = RegressResult.ToDaySelectItemList(re_.selItems_, re_.dateRangeList_);
//             List<SelectItem> validItems = new List<SelectItem>();
//             foreach (var item in dayItems)
//             {
//                 if (item.selItems_.Count > 1)
//                 {
//                     validItems.AddRange(item.selItems_);
//                 }
//             }

            Dictionary<String, List<SelectItem>> rateItemDict = RegressResult.ToRateItemDict(re_.selItems_);
            var sortDict = from objDic in rateItemDict orderby objDic.Key select objDic;
            List<HistoryData> dataList = new List<HistoryData>();
            List<String> rateItemList = new List<string>();
            foreach (var kv in sortDict)
            {
                if (kv.Value.Count < 20)
                {
                    continue;   
                }
                rateItemList.Add(kv.Key);
                List<DateSelectItem> items = RegressResult.ToDaySelectItemList(kv.Value, re_.dateRangeList_);
                HistoryData data = StrategyAsset.GetHistoryData(items, 0, items.Count, RunMode.RM_Raw);
                dataList.Add(data);
            }
//             List<int> occurInUpItemList, occurInDownItemList;
//             int nUpSelCount, nDownSelCount;
//             RegressResult.GetUpDownOccurCountForRateItem(re_.selItems_, rateItemList, 
//                 out occurInUpItemList, out occurInDownItemList, out nUpSelCount, out nDownSelCount);
//             for (int i = 0; i < rateItemList.Count; i++)
//             {
//                 dataList[i].recalProbilityForRateItem(occurInUpItemList[i], occurInDownItemList[i], nUpSelCount, nDownSelCount);
//             }

            if (dataList.Count == 0)
            {
                return;
            }
            SUtils.FillHistoryDataToChart(chart_, dataList, rateItemList);
            strategyRateItemHistoryData_ = new Dictionary<string, HistoryData>();
            for (int i = 0; i < rateItemList.Count; ++i )
            {
                strategyRateItemHistoryData_[rateItemList[i]] = dataList[i];
            }
        }
        void fillRawHistoryChart()
        {
            if (re_.runMode_ != RunMode.RM_Asset)
            {
                return;
            }
            ChartXUnit xUnit;
            selItemsHistoryDataToChart(chart_, re_.selItems_, re_.dateRangeList_, RunMode.RM_Raw, out xUnit);
        }

        private void straListView__SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void writeAssetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (re_.runMode_ != RunMode.RM_Raw)
            {
                MessageBox.Show("Current is not raw mode, can't write asset!", "Selector");
                return;
            }
            StrategyAsset.WriteStrategyAsset(re_.strategyList_[0], re_.reHistory_, strategyRateItemHistoryData_);
            App.asset_.readAssetFromDB();
            MessageBox.Show("Save success!", "Selector");
        }

    }
}
