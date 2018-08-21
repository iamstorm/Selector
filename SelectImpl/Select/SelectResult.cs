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
        public String rateItemKey_;
        public String rate_;
        public List<SelectItem> allSelectItems_;

        public SelectItem()
        {

        }
        public SelectItem(SelectItem item)
        {
            date_ = item.date_;
            code_ = item.code_;
            strategyName_ = item.strategyName_;
            rateItemKey_ = item.rateItemKey_;
            rate_ = item.rate_;
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
                    new ColumnInfo() { name_ = "sellspan", width_ = 60 },
                    new ColumnInfo() { name_ = "close", width_ = 60 },
                    new ColumnInfo() { name_ = "strategy", width_ = 200 },
                    new ColumnInfo() { name_ = "rate", width_ = 60 },
                    new ColumnInfo() { name_ = "hscount", width_ = 60 },
                    new ColumnInfo() { name_ = "rateKey", width_ = 60 },
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
        public String getColumnVal(String colName, Stock stock, StrategyData straData)
        {
            if (colName == "date")
            {
                return date_.ToString();
            }
            else if (colName == "code")
            {
                return code_;
            }
            else if (colName == "name")
            {
                return stock == null ? "" : stock.name_;
            }
            else if (colName == "zf")
            {
                if (stock == null)
                {
                    return "";
                }
                return stock.zfSee(date_);
            }
            else if (colName == "bonus")
            {
                if (stock == null)
                {
                    return "";
                }
                bool bSellWhenMeetMyBounusLimit;
                int sellDate;
                return App.grp_.computeBonus(stock, date_, out bSellWhenMeetMyBounusLimit, out sellDate);
            }
            else if (colName == "nsh")
            {
                if (stock == null)
                {
                    return "";
                }
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
                if (stock == null)
                {
                    return "";
                }
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
                if (stock == null)
                {
                    return "";
                }
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
            else if (colName == "sellspan")
            {
                if (stock == null)
                {
                    return "";
                }
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
                if (code_ == null)
                {
                    return "";
                }
                return App.ds_.realVal(Info.C, code_, date_).ToString("F3");
            }
            else if (colName == "strategy")
            {
                return strategyName_;
            }
            else if (colName == "rate")
            {
                return rate_;
            }
            else if (colName == "hscount")
            {
                return straData == null ? "0" : straData.selectCount_.ToString();
            }
            else if (colName == "rateKey")
            {
                return rateItemKey_;
            }
            else
            {
                throw new ArgumentException("想要显示无效的列值！");
            }
        }
        public String getColumnVal(String colName)
        {
            Stock stock = code_ == null ? null : App.ds_.sk(code_);
            StrategyData straData = App.asset_.straData(strategyName_);
            return getColumnVal(colName, stock, straData);
        }
       
        public ListViewItem toListViewItem(ListView lv, int iItemIndex, int nCount)
        {
            Stock stock = code_ == null ? null : App.ds_.sk(code_);
            StrategyData straData = App.asset_.straData(strategyName_);
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
                if (lvsi.Text != "" && (colName == "bonus" || colName == "nsh" || colName == "nsc"))
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
        public static List<SelectItem> MergeSelectItem(List<SelectItem> selItems)
        {
            Dictionary<Tuple<String, int>, List<SelectItem>> sameCodeDateItemDict = new Dictionary<Tuple<string, int>, List<SelectItem>>();
            foreach (var item in selItems)
            {
                var key = Tuple.Create(item.code_, item.date_);
                List<SelectItem> items;
                if (sameCodeDateItemDict.TryGetValue(key, out items))
	            {
		            items.Add(item);
	            } else 
                {
                    items = new List<SelectItem>();
                    items.Add(item);
                    sameCodeDateItemDict[key] = items;
                }
            }
            List<SelectItem> retList = new List<SelectItem>();
            foreach (var kv in sameCodeDateItemDict)
            {
                if (kv.Value.Count < 2)
                {
                    retList.Add((kv.Value[0]));
                }
                else
                {
                    List<String> straList = new List<String>();
                    List<String> rateList = new List<String>();
                    List<String> rateItemKeyList = new List<String>();
                    for (int i = 0; i < kv.Value.Count; ++i )
                    {
                        straList.Add(kv.Value[i].strategyName_);
                        rateList.Add(kv.Value[i].rate_);
                        rateItemKeyList.Add(kv.Value[i].rateItemKey_);
                    }
                    var newItem = new SelectItem(kv.Value[0]);
                    newItem.strategyName_ = String.Join(",", straList);
                    newItem.rate_ = String.Join(",", rateList);
                    newItem.rateItemKey_ = String.Join(",", rateItemKeyList);
                    retList.Add(newItem);
                }
            }
            return retList;
        }
        public static List<SelectItem> SplitSelectItem(List<SelectItem> selItems)
        {
            List<SelectItem> retList = new List<SelectItem>();
            foreach(var item in selItems)
            {
                String[] straList = item.strategyName_.Split(',');
                if (straList.Length == 1)
                {
                    retList.Add(item);
                }
                else
                {
                    String[] rateList = item.rate_.Split(',');
                    String[] rateItemKeyList = item.rateItemKey_.Split(',');
                    for (int i = 0; i < straList.Length; i++)
                    {
                        var newItem = new SelectItem(item);
                        newItem.strategyName_ = straList[0];
                        newItem.rate_ = rateList[0];
                        newItem.rateItemKey_ = rateItemKeyList[0];
                        retList.Add(newItem);
                    }
                }
            }
            return retList;
        }

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
