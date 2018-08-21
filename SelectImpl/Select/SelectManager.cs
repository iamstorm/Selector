using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class SelectHint
    {
        public Dictionary<Stock, int> nextWantedIndexHintDict_ = new Dictionary<Stock,int>();
        public List<Dictionary<String, String>> straParamList_ = new List<Dictionary<String, String>>();
    }
    public class SelectManager
    {
        public SelectResult select(DataStoreHelper dsh, int date, SelectHint hint = null)
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
                dsh.setStock(sk);
                for (int i = 0; i < straList.Count; ++i)
                {
                    IStrategy stra = straList[i];
                    Dictionary<String, String> rateItemDict = null;
                    try
                    {
                        int iIndex = App.ds_.index(sk, date, iDateIndexHint);
                        if (iIndex == -1)
                        {
                            continue;
                        }
                        if (hint != null)
                        {
                            hint.nextWantedIndexHintDict_[sk] = iIndex + 1;
                        }
                        dsh.iIndex_ = iIndex;

                        if (dsh.dataList_[iIndex] == Data.NowInvalidData)
                            continue;

                        rateItemDict = stra.select(dsh, paramList[i]);
                        if (rateItemDict == null)
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
                    selItem.rateItemDict_ = rateItemDict;
                    re.selItems_.Add(selItem);
                }
            }

            return re;
        }
        public SelectResult selectNow()
        {
            DataStoreHelper dsh = new DataStoreHelper();
            SelectResult re = select(dsh, Utils.NowDate());
            foreach (var item in re.selItems_)
            {
                item.allSelectItems_ = re.selItems_;
            }
            return re;
        }
    }
}
