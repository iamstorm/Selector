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

        public static String[] ShowColumnList()
        {
            return new String[] {
                "date", "code", "name", "zf", "bonus", "close", "strategy", "rate", "hscount", "rateKey"
            };
        }
        public Object getCellValue(DataGridViewRow row, String colName, Stock stock, StrategyData straData)
        {
            DataGridViewCell cell = row.Cells[colName];
            if (colName == "date")
            {
                return date_;
            }
            else if (colName == "code")
            {
                return code_;
            }
            else if (colName == "name")
            {
                return stock.name_;
            }
            else if (colName == "zf")
            {
                if (stock.zf(date_) > 0)
                {
                    row.DefaultCellStyle.ForeColor = Color.Red;
                }
                else
                {
                    row.DefaultCellStyle.ForeColor = Color.Green;
                }
                return stock.zfSee(date_);
            }
            else if (colName == "bonus")
            {
                int nextDate = stock.nextDate(date_);
                if (nextDate == -1)
                {
                    return "";
                }
                else
                {
                    if (stock.zf(nextDate) > 0)
                    {
                        cell.Style.BackColor = Color.Red;
                        cell.Style.ForeColor = Color.White;
                    }
                    else
                    {
                        cell.Style.BackColor = Color.Green;
                        cell.Style.ForeColor = Color.White;
                    }
                    return stock.zfSee(nextDate);
                }
            }
            else if (colName == "close")
            {
                return App.ds_.realVal(Info.C, code_, date_);
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
                return straData == null ? 0 : straData.selectCount_;
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
    }
    public class BuyItem
    {
        public SelectItem bySelectItem_;
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

    }
}
