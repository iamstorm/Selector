using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class SelectHint
    {
        public Dictionary<Stock, int> nextWantedIndexHintDict_ = new Dictionary<Stock,int>();
    }
    public class SelectManager
    {
        public SelectResult select(DataStoreHelper dsh, SelectMode selectMode, int date, List<IStrategy> strategyList, SelectHint hint = null)
        {
            SelectResult re = new SelectResult();
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
                    String sigInfo = "";
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

                        rateItemDict = stra.select(dsh, selectMode, ref sigInfo);
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
                    selItem.sigInfo_ = sigInfo;
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
//             List<IStrategy> strategyList = new List<IStrategy>();
//             strategyList.Add(App.grp_.strategy("LStopUp"));

            List<IStrategy> strategyList;
            SelectMode selectMode = SelectMode.SM_SelectInDay;
            DateTime curTime = DateTime.Now;

            if (Utils.IsOpenTime(curTime.Hour, curTime.Minute))
            {
                strategyList = App.Solution("lf_good").straList_;
                selectMode = SelectMode.SM_SelectOpen;
            }
            else if (Utils.IsCloseTime(curTime.Hour, curTime.Minute))
            {
                strategyList = App.Solution("close_good").straList_;
                selectMode = SelectMode.SM_SelectClose;
            }
            else if (Utils.IsTradeTime(curTime.Hour, curTime.Minute))
            {
                strategyList = App.Solution("lf_good").straList_;
                selectMode = SelectMode.SM_SelectInDay;
            }
            else
            {
                strategyList = App.Solution("close_good").straList_;
                selectMode = SelectMode.SM_SelectClose;
            }
            if (strategyList.Count == 0)
            {
                throw new Exception("No strategy provide!");
            }
            SelectResult re = select(dsh, selectMode, Utils.NowDate(), strategyList);
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
