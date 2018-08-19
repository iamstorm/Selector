using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectorForm
{
    public class SelectHint
    {
        public Dictionary<Stock, int> nextWantedIndexHintDict_ = new Dictionary<Stock,int>();
        public List<Dictionary<String, String>> straParamList_ = new List<Dictionary<String, String>>();
    }
    public class SelectManager
    {
        public SelectResult select(int date, SelectHint hint = null)
        {
            SelectResult re = new SelectResult();
            List<Dictionary<String, String> > paramList;
            if (hint != null)
            {
                paramList = hint.straParamList_;
            }
            else
            {
                paramList = new List<Dictionary<String, String>>();
                foreach (IStrategy stra in App.grp_.strategyList_)
                {
                    Dictionary<String, String> param = stra.setup();
                    paramList.Add(param);
                }
            }
            List<IStrategy> straList = App.grp_.strategyList_;
            foreach (Stock sk in App.ds_.stockList_)
            {
                int iDateIndexHint = -1;
                if (hint != null)
                {
                    iDateIndexHint = hint.nextWantedIndexHintDict_[sk];
                }
                for (int i = 0; i < straList.Count; ++i)
                {
                    IStrategy stra = straList[i];
                    String[] rateItems = null;
                    try
                    {
                        int iIndex = App.ds_.index(sk, date, iDateIndexHint);
                        if (iIndex == -1)
                        {
                            continue;
                        }
                        hint.nextWantedIndexHintDict_[sk] = iIndex + 1;
                        rateItems = stra.select(App.ds_, sk, iIndex, paramList[i]);
                        if (rateItems == null)
                        {
                            continue;
                        }
                    }
                    catch (DataException /*ex*/)
                    {
                        continue;
                    }
                    SelectItem selItem = new SelectItem();
                    selItem.code_ = sk.code_;
                    selItem.date_ = date;
                    selItem.strategyName_ = stra.name();
                    selItem.rateItems_ = rateItems;
                    selItem.rateItemKey_ = Utils.FormatRateItemKey(rateItems);
                    selItem.rate_ = stra.rate(rateItems);
                    re.selItems_.Add(selItem);
                }
            }

            return re;
        }
        public SelectResult selectNow()
        {
            return select(Utils.Date(DateTime.Now));
        }
    }
}
