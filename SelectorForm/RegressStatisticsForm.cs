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
        WD_StrategyData,
        WD_RateItemData,
        WD_HistoryData,
    }
    public partial class RegressStatisticsForm : Form
    {
        public WantDataType wd_;
        public RegressResult re_;
        public Dictionary<String, HistoryData> strategyRateItemHistoryData_;
        public int sortColumn_;
        public RegressStatisticsForm(RegressResult re, WantDataType wd)
        {
            InitializeComponent();
            re_ = re;
            wd_ = wd;

            menuStrip1.Visible = false;
            writeAssetToolStripMenuItem.Visible = false;
            writeAsOptimizeToolStripMenuItem.Visible = false;
            chart_.Titles.Add("Title");
            string sMode = re.runMode_ == RunMode.RM_Asset ? "Asset mode" : "Raw mode";
            chart_.Titles[0].Text = String.Format("{0} {1}", re.solutionName_, sMode);
            switch (wd_)
            {
                case WantDataType.WD_BonusData:
                    fillBonusChart();
                    break;
                case WantDataType.WD_StrategyData:
                    fillStrategyDataChart();
                    break;
                case WantDataType.WD_RateItemData:
                    fillRateItemChart();
                    menuStrip1.Visible = true;
                    writeAssetToolStripMenuItem.Visible = true;
                    break;
                case WantDataType.WD_HistoryData:
                    fillHistoryDataChart();
                    menuStrip1.Visible = true;
                    writeAsOptimizeToolStripMenuItem.Visible = true;
                    if (re_.reHistory_ == null)
                    {
                        break;
                    }
                    historyView.Columns.Add("solution", 80);
                    LUtils.InitListView(historyView, HistoryData.ShowColumnInfos);
                    historyView.Items.Add(re_.reHistory_.toListViewItem(historyView, re_.solutionName_));

                    addSolutionBestDataToHistoryView(re_.solutionName_, "best");
                    break;
                default:
                    throw new ArgumentException("Unknown data type!");
            }

        }
        void addSolutionBestDataToHistoryView(String sName, String sNameInHistoryView)
        {
            DataTable straHisTable = DB.Global().Select(String.Format("Select * From stra_opt Where straname = '{0}'", sName));
            foreach (DataRow row in straHisTable.Rows)
            {
                HistoryData data = HistoryData.FromDataRow(row);
                historyView.Items.Add(data.toListViewItem(historyView, sNameInHistoryView));
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
        void fillStrategyDataChart()
        {
            if (re_.strategyList_.Count < 2)
            {
                return;
            }
            Dictionary<String, int> straBuyItemCount = new Dictionary<String, int>();
            Dictionary<String, float> straBonusDict = new Dictionary<String, float>();
            int nTotalCount = 0;
            float totalBonus = 0;
            foreach (var item in re_.selItems_)
            {
                if (!item.iamBuyItem_)
                {
                    continue;
                }
                ++nTotalCount;
                if (straBuyItemCount.ContainsKey(item.strategyName_))
                {
                    straBuyItemCount[item.strategyName_]++;
                }
                else
                {
                    straBuyItemCount[item.strategyName_] = 1;
                }
                var bonus = item.getColumnVal("bonus");
                if (bonus != "")
                {
                    float bonusValue = Utils.GetBonusValue(bonus);
                    if (straBonusDict.ContainsKey(item.strategyName_))
                    {
                        straBonusDict[item.strategyName_] += bonusValue;
                    }
                    else
                    {
                        straBonusDict[item.strategyName_] = bonusValue;
                    }
                    totalBonus += bonusValue;
                }
            }
            historyView.Columns.Add("strategy", 80);
            LUtils.InitListView(historyView, HistoryData.ShowColumnInfos);
            foreach (var kv in straBuyItemCount)
            {
                addSolutionBestDataToHistoryView("$" + kv.Key, kv.Key);
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("xDesc");
            dt.Columns.Add("count");
            dt.Columns.Add("bonusValue"); 
            foreach (var kv in straBuyItemCount)
            {
                DataRow row = dt.NewRow();
                row["xDesc"] = kv.Key;
                row["count"] = kv.Value;
                float bonus;
                if (straBonusDict.TryGetValue(kv.Key, out bonus))
                {
                    row["bonusValue"] = bonus;
                }
                dt.Rows.Add(row);
            }
            chart_.ChartAreas[0].Name = dt.Columns[1].ColumnName;
            ChartArea area = new ChartArea(dt.Columns[2].ColumnName);
            chart_.ChartAreas.Add(area);
            for (int i = 1; i < dt.Columns.Count; ++i)
            {
                var col = dt.Columns[i];
                var series = new Series();
                series.Name = col.ColumnName;
                series.XValueMember = "xDesc";
                series.YValueMembers = col.ColumnName;
                series.ToolTip = col.ColumnName;
                series.ChartType = SeriesChartType.Pie;

                series.ChartArea = col.ColumnName;

                chart_.Series.Add(series);
            }
            chart_.DataSource = dt;
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

        private void writeAsOptimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DB.Global().Execute(String.Format("Delete From stra_opt Where straname = '{0}'", re_.solutionName_));
            Dictionary<String, Object> straDataDict = re_.reHistory_.toDictionary("");
            straDataDict["straname"] = re_.solutionName_;
            DB.Global().Insert("stra_opt", straDataDict);
            MessageBox.Show("Save success!", "Selector");
        }

        private void historyView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LUtils.OnColumnClick(historyView, e.Column, ref sortColumn_);
        }

    }
}
