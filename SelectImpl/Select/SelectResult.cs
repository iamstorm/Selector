using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SelectImpl
{
    public class SelectItem
    {
        public int date_;
        public String code_;
        public String strategyName_;
        public Dictionary<String, String> rateItemDict_;
        public List<SelectItem> allSelectItems_;

        public SelectItem()
        {

        }
        public SelectItem(SelectItem item)
        {
            date_ = item.date_;
            code_ = item.code_;
            strategyName_ = item.strategyName_;
            rateItemDict_ = item.rateItemDict_;
        }
        public static SelectItem DontBuy(int date)
        {
            SelectItem dontBuy = new SelectItem();
            dontBuy.date_ = date;
            dontBuy.strategyName_ = "dontBuy";
            return dontBuy;
        }
        public static SelectItem MissBuy(int date)
        {
            SelectItem missBuy = new SelectItem();
            missBuy.date_ = date;
            missBuy.strategyName_ = "miss";
            return missBuy;
        }
        public static ColumnInfo[] ShowColumnInfos
        {
            get {
                return new ColumnInfo[]
                {
                    new ColumnInfo() { name_ = "date", width_ = 60 },
                    new ColumnInfo() { name_ = "code", width_ = 50 },
                    new ColumnInfo() { name_ = "name", width_ = 60 },
                    new ColumnInfo() { name_ = "zf", width_ = 60 },
                    new ColumnInfo() { name_ = "bonus", width_ = 60 },
                    new ColumnInfo() { name_ = "nsh", width_ = 60 },
                    new ColumnInfo() { name_ = "nsc", width_ = 60 },
                    new ColumnInfo() { name_ = "hrate", width_ = 60 },
                    new ColumnInfo() { name_ = "dbuysuc", width_ = 60 },
                    new ColumnInfo() { name_ = "szzs", width_ = 60 },
                    new ColumnInfo() { name_ = "sellspan", width_ = 60 },
                    new ColumnInfo() { name_ = "close", width_ = 60 },
                    new ColumnInfo() { name_ = "strategy", width_ = 200 },
                    new ColumnInfo() { name_ = "rate", width_ = 60 },
                    new ColumnInfo() { name_ = "hscount", width_ = 60 },
                    new ColumnInfo() { name_ = "strarate", width_ = 60 },
                    new ColumnInfo() { name_ = "itemrate", width_ = 60 },
                };
            }
        }

        public static String[] ShowColumnList
        {
            get
            {
                return (from info in ShowColumnInfos
                 select info.name_).ToArray<String>();
            }
        }
        public String getColumnVal(String colName, Stock stock, HistoryData straData)
        {
            if (colName == "date")
            {
                return date_.ToString();
            }
            else if (colName == "strategy")
            {
                return strategyName_;
            }
            if (stock == null)
            {
                return "";
            }
            if (colName == "code")
            {
                return code_;
            }
            else if (colName == "name")
            {
                return stock == null ? "" : stock.name_;
            }
            else if (colName == "zf")
            {
                return stock.zfSee(date_);
            }
            else if (colName == "bonus")
            {
                bool bSellWhenMeetMyBounusLimit;
                int sellDate;
                return App.grp_.computeBonus(stock, date_, out bSellWhenMeetMyBounusLimit, out sellDate);
            }
            else if (colName == "nsh")
            {
                bool bSellWhenMeetMyBounusLimit;
                int sellDate;
                App.grp_.computeBonus(stock, date_, out bSellWhenMeetMyBounusLimit, out sellDate);
                if (sellDate == -1 || !bSellWhenMeetMyBounusLimit)
                {
                    return "";
                }
                return Utils.ToBonus(stock.hf(sellDate));
            }
            else if (colName == "nsc")
            {
                bool bSellWhenMeetMyBounusLimit;
                int sellDate;
                App.grp_.computeBonus(stock, date_, out bSellWhenMeetMyBounusLimit, out sellDate);
                if (sellDate == -1 || !bSellWhenMeetMyBounusLimit)
                {
                    return "";
                }
                return Utils.ToBonus(stock.zf(sellDate));
            }
            else if (colName == "hrate")
            {
                List<SelectItem> daySelectItems = SelectResult.OfDate(date_, allSelectItems_);
                int nPlusCount = 0;
                int nAllCount = 0;
                foreach (var item in daySelectItems)
                {
                    var bonus = item.getColumnVal("bonus");
                    if (bonus == "")
                    {
                        continue;
                    }
                    if (Utils.GetBonusValue(bonus) > 0)
                    {
                        ++nPlusCount;
                    }
                    ++nAllCount;
                }
                if (nAllCount == 0)
                {
                    return "";
                }
                else
                {
                    return (nPlusCount * 1.0f / nAllCount).ToString("F2");
                }
            }
            else if (colName == "dbuysuc")
            {
                if (strategyName_ != "dontBuy")
                {
                    return "";
                }
                List<SelectItem> daySelectItems = SelectResult.OfDate(date_, allSelectItems_);
                int nNobuySucCount = 0;
                foreach (var item in daySelectItems)
                {
                    String bonus = item.getColumnVal("bonus");
                    if (Utils.GetBonusValue(bonus) < 0)
                    {
                        nNobuySucCount++;
                    }
                }
                return (nNobuySucCount * 1.0f/ daySelectItems.Count).ToString("F02");
            }
            else if (colName == "szzs")
	        {
                bool bSellWhenMeetMyBounusLimit;
                int sellDate;
                App.grp_.computeBonus(stock, date_, out bSellWhenMeetMyBounusLimit, out sellDate);
                return sellDate == -1 ? "" : App.ds_.envBonus(sellDate);
	        }
            else if (colName == "sellspan")
            {
                bool bSellWhenMeetMyBounusLimit;
                int sellDate;
                App.grp_.computeBonus(stock, date_, out bSellWhenMeetMyBounusLimit, out sellDate);
                if (sellDate == -1)
                {
                    return "not yet";
                }
                else
                {
                    return Utils.DateSpan(date_, sellDate);
                }
            }
            else if (colName == "close")
            {
                return App.ds_.realVal(Info.C, code_, date_).ToString("F3");
            }
            else if (colName == "rate")
            {
                return RateComputer.TotalRate(RateComputer.HistoryRate(straData), RateComputer.ComputerRate(strategyName_, rateItemDict_)).ToString();
            }
            else if (colName == "hscount")
            {
                return straData == null ? "0" : straData.selectCount_.ToString();
            }
            else if (colName == "strarate")
            {
                return RateComputer.HistoryRate(straData).ToString();
            }
            else if (colName == "itemrate")
            {
                return RateComputer.ComputerRate(strategyName_, rateItemDict_).ToString();
            }
            else
            {
                throw new ArgumentException("想要显示无效的列值！");
            }
        }
        public String getColumnVal(String colName)
        {
            Stock stock = code_ == null ? null : App.ds_.sk(code_);
            HistoryData straData = App.asset_.straData(strategyName_);
            return getColumnVal(colName, stock, straData);
        }
       
        public ListViewItem toListViewItem(ListView lv, int iItemIndex, int nCount)
        {
            Stock stock = code_ == null ? null : App.ds_.sk(code_);
            HistoryData straData = App.asset_.straData(strategyName_);
            ListViewItem lvi = new ListViewItem(String.Format("{0}/{1}", iItemIndex+1, nCount));
            lvi.UseItemStyleForSubItems = false;
            Color rowColor = Color.Empty;
            if (stock != null)
            {
                if (stock.zf(date_) > 0)
                {
                    rowColor = Color.Red;
                }
                else
                {
                    rowColor = Color.Green;
                }
            }
            foreach (String colName in ShowColumnList)
            {
                ListViewItem.ListViewSubItem lvsi = new ListViewItem.ListViewSubItem();
                lvsi.Text = getColumnVal(colName, stock, straData);
                lvsi.ForeColor = rowColor;
                if (lvsi.Text != "" && (colName == "bonus" || colName == "nsh" || colName == "nsc" || colName == "szzs"))
                {
                    if (Utils.GetBonusValue(lvsi.Text) > 0)
                    {
                        lvsi.BackColor = Color.Red;
                        lvsi.ForeColor = Color.White;
                    }
                    else
                    {
                        lvsi.BackColor = Color.Green;
                        lvsi.ForeColor = Color.White;
                    }
                }
                lvi.SubItems.Add(lvsi);
            }
            return lvi;
        }
    }
    public class SelectResult
    {
        public List<SelectItem> selItems_ = new List<SelectItem>();
      
        public static List<SelectItem> OfDate(int date, List<SelectItem> selItems)
        {
            List<SelectItem> retList = new List<SelectItem>();
            foreach (SelectItem item in selItems)
            {
                if (item.date_ == date)
                {
                    retList.Add(item);
                }
            }
            return retList;
        }
    }
}
