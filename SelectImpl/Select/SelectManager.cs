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
        public SelectResult select(DataStoreHelper dsh, int date, List<IStrategy> strategyList, SelectHint hint = null)
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
                foreach (IStrategy stra in strategyList)
                {
                    Dictionary<String, String> param = stra.setup();
                    paramList.Add(param);
                }
            }
            dsh.iSZIndex_ = App.ds_.index(App.ds_.szListData_, date);
            foreach (Stock sk in App.ds_.stockList_)
            {
                int iDateIndexHint = -1;
                if (hint != null)
                {
                    iDateIndexHint = hint.nextWantedIndexHintDict_[sk];
                }
                dsh.setStock(sk);
                for (int i = 0; i < strategyList.Count; ++i)
                {
                    IStrategy stra = strategyList[i];
                    Dictionary<String, String> rateItemDict = null;
                    String sigDate = "";
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

                        FocusOn fon = stra.focusOn();
                        int beforDateCount = sk.dataList_.Count - iIndex;
                        bool isNewStock = beforDateCount < Setting.MyNewStockLimit;
                        switch (fon)
                        {
                            case FocusOn.FO_Old:
                                if (isNewStock)
                                {
                                    continue;
                                }
                                break;
                            case FocusOn.FO_All:
                                break;
                            case FocusOn.FO_New:
                                if (!isNewStock)
                                {
                                    continue;
                                }
                                break;
                            default:
                                throw new ArgumentException("Unknown focusOn");
                        }

                        rateItemDict = stra.select(dsh, paramList[i], ref sigDate);
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
                    selItem.sigDate_ = sigDate;
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
            List<IStrategy> strategyList = new List<IStrategy>();
            strategyList.Add(App.grp_.strategy("UStopDown"));
            strategyList.Add(App.grp_.strategy("UUDown"));
            strategyList.Add(App.grp_.strategy("EveryThreeUp"));
            SelectResult re = select(dsh, Utils.NowDate(), strategyList);
            foreach (var item in re.selItems_)
            {
                item.allSelectItems_ = re.selItems_;
            }
            var buyItem = App.grp_.makeDeside(re.selItems_, Utils.NowDate(), RankBuyDesider.buyer_);
            buyItem.iamBuyItem_ = true;
            re.selItems_.Sort(delegate(SelectItem lhs, SelectItem rhs)
            {
                if (lhs.iamBuyItem_)
                {
                    return -1;
                }
                if (rhs.iamBuyItem_)
                {
                    return 1;
                }
                return Utils.ToType<int>(lhs.getColumnVal("prirank")).CompareTo(Utils.ToType<int>(rhs.getColumnVal("prirank")));
            });

            return re;
        }
    }
}
