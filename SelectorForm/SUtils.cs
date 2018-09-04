using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SelectImpl;
using System.Data;
using System.Windows.Forms.DataVisualization.Charting;
using System.Drawing;

namespace SelectorForm
{
    public enum ChartXUnit
    {
        CXU_Day,
        CXU_Week,
        CXU_Month,
        CXU_Year,
    }
    public struct HistoryCharData
    {
        public String xDesc_;
        public double tradeSucProbility_;
        public double selectSucProbility_;
        public double bonusValue_;
        public double antiRate_;
        public double tradeDayRate_;
        public double dontBuyRate_;
        public HistoryData data;
    }
    public class BonusRange
    {
        public BonusRange(int bMin, int bMax)
        {
            bonusMin_ = bMin;
            bonusMax_ = bMax;
            nTradeCount_ = 0;
            nHfCount_ = 0;
            nCfCount_ = 0;
        }
        public String xDesc()
        {
            return String.Format("{0}%-{1}%", bonusMin_, bonusMax_);
        }
        public int bonusMin_;
        public int bonusMax_;
        public int nTradeCount_;
        public int nHfCount_;
        public int nCfCount_;
    }
    public static class SUtils
    {
        public static ChartXUnit DetermineXUnit(int nDayCount)
        {
            if (nDayCount > 12 * ToDayCount(ChartXUnit.CXU_Month))
            {
                return ChartXUnit.CXU_Year;
            }
            else if (nDayCount > 12 * ToDayCount(ChartXUnit.CXU_Week))
            {
                return ChartXUnit.CXU_Month;
            }
            else if (nDayCount > 12 * ToDayCount(ChartXUnit.CXU_Day))
            {
                return ChartXUnit.CXU_Week;
            }
            else
            {
                return ChartXUnit.CXU_Day;
            }
        }
        public static String ToUnitName(ChartXUnit xUnit)
        {
            switch (xUnit)
            {
                case ChartXUnit.CXU_Day:
                    return "Day";
                case ChartXUnit.CXU_Week:
                    return "Week";
                case ChartXUnit.CXU_Month:
                    return "Month";
                case ChartXUnit.CXU_Year:
                    return "Year";
                default:
                    throw new ArgumentException("Unknown ChartXUnit!");
            }
        }
        public static int ToDayCount(ChartXUnit xUnit)
        {
            switch (xUnit)
            {
                case ChartXUnit.CXU_Day:
                    return 1;
                case ChartXUnit.CXU_Week:
                    return 5;
                case ChartXUnit.CXU_Month:
                    return 22;
                case ChartXUnit.CXU_Year:
                    return 245;
                default:
                    throw new ArgumentException("Unknow xUnit!");
            }
        }
        public static String ToXDesc(ChartXUnit xUnit, int startDate, int endDate)
        {
            if (xUnit == ChartXUnit.CXU_Year)
            {
                return String.Format("{0}", Utils.Year(startDate));
            }
            else
            {
                return String.Format("{0}-{1}", startDate.ToString().Substring(2), endDate.ToString().Substring(2));
            }
        }
        public static void ScaleToRange(DataTable dt, String colName, double dZeroValue, double dMinValue, double dMaxValue, out double dRealMinValue, out double dRealMaxValue)
        {
            dRealMinValue = double.MaxValue;
            dRealMaxValue = double.MinValue;
            double dSpan = dMaxValue - dMinValue;
            foreach (DataRow row in dt.Rows)
            {
                var val = Utils.ToType<double>(row[colName]);
                row[colName] = (val - dZeroValue) * 200/ dSpan;
                dRealMinValue = Math.Min(dRealMinValue, val);
                dRealMaxValue = Math.Max(dRealMaxValue, val);
            }
        }
        struct WantedRange
        {
            public WantedRange(double dZeroValue, double dMinValue, double dMaxValue)
            {
                dZeroValue_ = dZeroValue;
                dMinValue_ = dMinValue;
                dMaxValue_ = dMaxValue;
            }
            public double dZeroValue_;
            public double dMinValue_;
            public double dMaxValue_;
        }
        public static void FillHistoryDataToChart(Chart chart, List<HistoryData> dataList, List<String> xDescList)
        {
            DataTable dt = new DataTable();
            Dictionary<String, WantedRange> valueDict = new Dictionary<String, WantedRange>();
            Dictionary<String, Color> colorDict = new Dictionary<String, Color>();
            dt.Columns.Add("xDesc");
            dt.Columns.Add("bonusValue"); valueDict["bonusValue"] = new WantedRange(0, -20, 20); colorDict["bonusValue"] = Color.Red;
            dt.Columns.Add("tradeSucProbility"); valueDict["tradeSucProbility"] = new WantedRange(0.5, 0, 1); colorDict["tradeSucProbility"] = Color.Orange;
            dt.Columns.Add("selectSucProbility"); valueDict["selectSucProbility"] = new WantedRange(0.5, 0, 1); colorDict["selectSucProbility"] = Color.Blue;
            dt.Columns.Add("tradeDayRate"); valueDict["tradeDayRate"] = new WantedRange(0.5, 0, 1); colorDict["tradeDayRate"] = Color.BurlyWood;
            dt.Columns.Add("antiRate"); valueDict["antiRate"] = new WantedRange(0.2, 0, 1); colorDict["antiRate"] = Color.Violet;
            dt.Columns.Add("goodSamples"); valueDict["goodSamples"] = new WantedRange(0, 0, 10); colorDict["goodSamples"] = Color.Black;
            dt.Columns.Add("rank"); valueDict["rank"] = new WantedRange(50, 0, 100); colorDict["rank"] = Color.DeepSkyBlue;
            for (int i = 0; i < dataList.Count; ++i)
            {
                var item = dataList[i];
                DataRow row = dt.NewRow();
                row["xDesc"] = xDescList[i]; 
                row["bonusValue"] = item.bonusValue_;
                row["tradeSucProbility"] = item.tradeSucProbility_;
                row["selectSucProbility"] = item.selectSucProbility_;
                row["tradeDayRate"] = item.tradeDayRate_;
                row["antiRate"] = item.antiRate_;
                row["goodSamples"] = item.nGoodSampleSelectCount_/500.0f;
                row["rank"] = item.rank_;
                dt.Rows.Add(row);
            }
            Legend infoLegend = new Legend();
            for (int i = 1; i < dt.Columns.Count; ++i)
            {
                var col = dt.Columns[i];
                WantedRange range = valueDict[col.ColumnName];
                double dRealMinValue, dRealMaxValue;
                ScaleToRange(dt, col.ColumnName, range.dZeroValue_, range.dMinValue_, range.dMaxValue_, out dRealMinValue, out dRealMaxValue);
                var series = new Series();
                series.Name = col.ColumnName;
                series.XValueMember = "xDesc";
                series.YValueMembers = col.ColumnName;
                series.ToolTip = col.ColumnName;
                chart.Series.Add(series);
                series.Color = colorDict[col.ColumnName];
                if (col.ColumnName == "bonusValue")
                {
                    infoLegend.CustomItems.Add(series.Color, String.Format("{0}: {1}% - {2}%", col.ColumnName, dRealMinValue.ToString("F2"), dRealMaxValue.ToString("F2")));
                }
                else
                {
                    infoLegend.CustomItems.Add(series.Color, String.Format("{0}: {1} - {2}", col.ColumnName, dRealMinValue.ToString("F2"), dRealMaxValue.ToString("F2")));
                }
            }
            infoLegend.Docking = Docking.Bottom;
            chart.Legends.Add(infoLegend);
            chart.DataSource = dt;
        }
        public static void FillBonusChart(Chart chart, List<BonusRange> rangeList)
        {
            List<BonusRange> validBonusRangeList = new List<BonusRange>();
            int iMinIndex = rangeList.Count, iMaxIndex = -1;
            for (int i = 0; i < rangeList.Count; i++)
            {
                if (rangeList[i].nTradeCount_ > 0 || rangeList[i].nHfCount_ > 0 || rangeList[i].nCfCount_ > 0)
                {
                    iMinIndex = Math.Min(iMinIndex, i);
                    iMaxIndex = Math.Max(iMaxIndex, i);
                }
            }
            if (iMinIndex == rangeList.Count || iMaxIndex == -1)
            {
                return;
            }
            for (int i = iMinIndex; i <= iMaxIndex; i++)
            {
                validBonusRangeList.Add(rangeList[i]);
            }
            Dictionary<String, Color> colorDict = new Dictionary<string, Color>();
            DataTable dt = new DataTable();
            dt.Columns.Add("xDesc");
            dt.Columns.Add("tradeCount"); colorDict["tradeCount"] = Color.Blue;
            dt.Columns.Add("hfCount"); colorDict["hfCount"] = Color.Red;
            dt.Columns.Add("cfCount"); colorDict["cfCount"] = Color.Green;
            foreach (var range in validBonusRangeList)
            {
                var row = dt.NewRow();
                row["xDesc"] = range.xDesc();
                row["tradeCount"] = range.nTradeCount_;
                row["hfCount"] = range.nHfCount_;
                row["cfCount"] = range.nCfCount_;
                dt.Rows.Add(row);
            }
            for (int i = 1; i < dt.Columns.Count; ++i)
            {
                var col = dt.Columns[i];
                var series = new Series();
                series.Name = col.ColumnName;
                series.XValueMember = "xDesc";
                series.YValueMembers = col.ColumnName;
                series.ToolTip = col.ColumnName;
                chart.Series.Add(series);
                series.Color = colorDict[col.ColumnName];
            }
            chart.DataSource = dt;
        }
    }
}
