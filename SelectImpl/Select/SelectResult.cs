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
        public int buyNormlizePrice_;

        public String sigInfo_;
        public Dictionary<String, String> rateItemDict_;
        public List<SelectItem> allSelectItems_;
        public bool iamBuyItem_;
        public int sameDayStrategySelCount_;
        public int sameDaySelCount_;
        
        public static ColumnInfo[] ShowColumnInfos
        {
            get {
                return new ColumnInfo[]
                {
                    new ColumnInfo() { name_ = "date", width_ = 60 },
                    new ColumnInfo() { name_ = "siginfo", width_ = 60 },
                    new ColumnInfo() { name_ = "code", width_ = 50 },
                    new ColumnInfo() { name_ = "name", width_ = 60 },
                    new ColumnInfo() { name_ = "zf", width_ = 60 },
                    new ColumnInfo() { name_ = "bonus", width_ = 60 },
                    new ColumnInfo() { name_ = "nsh", width_ = 60 },
                    new ColumnInfo() { name_ = "nsl", width_ = 60 },
                    new ColumnInfo() { name_ = "nso", width_ = 60 },
                    new ColumnInfo() { name_ = "nsc", width_ = 60 },
                    new ColumnInfo() { name_ = "hrate", width_ = 60 },
                    new ColumnInfo() { name_ = "envbonus", width_ = 60 },
                    new ColumnInfo() { name_ = "sellspan", width_ = 60 },
                    new ColumnInfo() { name_ = "tradespan", width_ = 60 },
                    new ColumnInfo() { name_ = "close", width_ = 60 },
                    new ColumnInfo() { name_ = "strategy", width_ = 200 },
                    new ColumnInfo() { name_ = "pubrank", width_ = 60 },
                    new ColumnInfo() { name_ = "prirank", width_ = 60 },
                    new ColumnInfo() { name_ = "trcount", width_ = 60 },
                    new ColumnInfo() { name_ = "selcount", width_ = 60 },
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
        Dictionary<String, String> colValCacheDict_ = new Dictionary<string, string>();
        public String getColumnVal(String colName, Stock stock, HistoryData straData, bool bSearchCache = true)
        {
            if (bSearchCache)
            {
                String cacheVal;
                if (colValCacheDict_.TryGetValue(colName, out cacheVal))
                {
                    return cacheVal;
                }
            }
            IStrategy stra = App.grp_.strategy(strategyName_);

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
                String zf = stock.zfSee(date_);
                colValCacheDict_[colName] = zf;
                return zf;
            }
            else if (colName == "bonus")
            {
                BuySellInfo info;
                String bonus = stra.computeBonus(this, stock, date_, out info);
                colValCacheDict_[colName] = bonus;
                return bonus;
            }
            else if (colName == "nsh")
            {
                BuySellInfo info;
                stra.computeBonus(this, stock, date_, out info);
                if (info.sellDate_ == -1)
                {
                    colValCacheDict_[colName] = "";
                    return "";
                }
                String bonus = Utils.ToBonus(stock.hf(info.sellDate_));
                colValCacheDict_[colName] = bonus;
                return bonus;
            }
            else if (colName == "nsl")
            {
                BuySellInfo info;
                stra.computeBonus(this, stock, date_, out info);
                if (info.sellDate_ == -1)
                {
                    colValCacheDict_[colName] = "";
                    return "";
                }
                String bonus = Utils.ToBonus(stock.lf(info.sellDate_));
                colValCacheDict_[colName] = bonus;
                return bonus;
            }

            else if (colName == "nso")
            {
                BuySellInfo info;
                stra.computeBonus(this, stock, date_, out info);
                if (info.sellDate_ == -1)
                {
                    colValCacheDict_[colName] = "";
                    return "";
                }
                String bonus = Utils.ToBonus(stock.of(info.sellDate_));
                colValCacheDict_[colName] = bonus;
                return bonus;
            }
            else if (colName == "nsc")
            {
                BuySellInfo info;
                stra.computeBonus(this, stock, date_, out info);
                if (info.sellDate_ == -1 || !info.bSellWhenMeetMyBounusLimit_)
                {
                    colValCacheDict_[colName] = "";
                    return "";
                }
                String bonus = Utils.ToBonus(stock.zf(info.sellDate_));
                colValCacheDict_[colName] = bonus;
                return bonus;
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
                String ret;
                if (nAllCount == 0)
                {
                    ret = "";
                }
                else
                {
                    ret = (nPlusCount * 1.0f / nAllCount).ToString("F2");
                }
                colValCacheDict_[colName] = ret;
                return ret;
            }
            else if (colName == "envbonus")
	        {
                BuySellInfo info;
                stra.computeBonus(this, stock, date_, out info);
                String ret = info.sellDate_ == -1 ? "" : App.ds_.envBonus(info.sellDate_);
                colValCacheDict_[colName] = ret;
                return ret;
	        }
            else if (colName == "sellspan")
            {
                BuySellInfo info;
                stra.computeBonus(this, stock, date_, out info);
                String ret;
                if (info.sellDate_ == -1)
                {
                    ret = "not yet";
                }
                else
                {
                    ret = Utils.DateSpan(date_, info.sellDate_);
                }
                colValCacheDict_[colName] = ret;
                return ret;
            }
            else if (colName == "tradespan")
            {
                BuySellInfo info;
                stra.computeBonus(this, stock, date_, out info);
                String ret;
                if (info.tradeSpan_ == -1)
                {
                    ret = "not yet";
                }
                else
                {
                    ret = info.tradeSpan_.ToString();
                }
                colValCacheDict_[colName] = ret;
                return ret;
            }
            else if (colName == "close")
            {
                String ret = App.ds_.realVal(Info.C, code_, date_).ToString("F3");
                colValCacheDict_[colName] = ret;
                return ret;
            }
            else if (colName == "pubrank")
            {
                if (straData == null)
                {
                    return "-1";
                }
//                 float priRank = Utils.ToType<float>(getColumnVal("prirank"));
//                 float priPercent = priRank / straData.rank_;
//                 return (priPercent * 55.0f + 45.0f * straData.rank_ / 100.0f).ToString("F0");
                return straData.rank_.ToString();
            }
            else if (colName == "prirank")
            {
                String ret;
                if (straData == null)
                {
                    ret = "-1";
                }
                else
                {
                    ret = RateComputer.ComputerRank(strategyName_, rateItemDict_).ToString();
                }
                colValCacheDict_[colName] = ret;
                return ret;
            }
            else if (colName == "trcount")
            {
                return straData == null ? "0" : straData.nTradeCount_.ToString();
            }
            else if (colName == "selcount")
            {
                return straData == null ? "0" : straData.nGoodSampleSelectCount_.ToString();
            }
            else if (colName == "siginfo")
            {
                return sigInfo_;
            }
            else if (colName == "a")
            {
                String ret = App.ds_.Ref(Info.A, stock.dataList_, App.ds_.index(stock, date_), 0).ToString("F0");
                colValCacheDict_[colName] = ret;
                return ret;
            }
            else
            {
                throw new ArgumentException("想要显示无效的列值: " + colName);
            }
        }
        public String getColumnVal(String colName)
        {
            String cacheVal;
            if (colValCacheDict_.TryGetValue(colName, out cacheVal))
            {
                return cacheVal;
            }
            Stock stock = code_ == null ? null : App.ds_.sk(code_);
            HistoryData straData = App.asset_.straData(strategyName_);
            return getColumnVal(colName, stock, straData, false);
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
                if (lvsi.Text != "" && (colName == "bonus" || colName == "nsh" || colName == "nsc" || colName == "envbonus"))
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
      
        public static List<SelectItem> OfDate(int date, List<SelectItem> selItems, bool bMustHasBonus = false)
        {
            List<SelectItem> retList = new List<SelectItem>();
            foreach (SelectItem item in selItems)
            {
                if (item.date_ != date)
                {
                    continue;
                }
                if (bMustHasBonus)
                {
                    var bonus = item.getColumnVal("bonus");
                    if (bonus == "")
                    {
                        continue;
                    }
                }
                retList.Add(item);
            }
            return retList;
        }
    }
}
