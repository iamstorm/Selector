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
            bool bShowProgress = date == Utils.NowDate();
            if (bShowProgress) {
                App.host_.uiStartProcessBar();
            }
            SelectResult re = new SelectResult();
            dsh.iSZIndex_ = App.ds_.index(App.ds_.szListData_, date);
            for (int isk = 0; isk < App.ds_.stockList_.Count; ++isk ) {
                Stock sk = App.ds_.stockList_[isk];
                if (bShowProgress) {
                    App.host_.uiSetProcessBar(String.Format("正在检测是否选择{0}", sk.code_), isk * 100 / App.ds_.stockList_.Count);
                }
                int iDateIndexHint = -1;
                if (hint != null) {
                    iDateIndexHint = hint.nextWantedIndexHintDict_[sk];
                }
                dsh.setStock(sk);
                for (int i = 0; i < strategyList.Count; ++i) {
                    IStrategy stra = strategyList[i];
                    Dictionary<String, String> rateItemDict = null;
                    String sigInfo = "";
                    try {
                        int iIndex = App.ds_.index(sk, date, iDateIndexHint);
                        if (iIndex == -1) {
                            continue;
                        }
                        if (hint != null) {
                            hint.nextWantedIndexHintDict_[sk] = iIndex + 1;
                        }
                        dsh.iIndex_ = iIndex;

                        if (dsh.dataList_[iIndex] == Data.NowInvalidData)
                            continue;

                        FocusOn fon = stra.focusOn();
                        int beforDateCount = sk.dataList_.Count - iIndex;
                        bool isNewStock = beforDateCount < Setting.MyNewStockLimit;
                        switch (fon) {
                            case FocusOn.FO_Old:
                                if (isNewStock) {
                                    continue;
                                }
                                break;
                            case FocusOn.FO_All:
                                break;
                            case FocusOn.FO_New:
                                if (!isNewStock) {
                                    continue;
                                }
                                break;
                            default:
                                throw new ArgumentException("Unknown focusOn");
                        }
                        if (dsh.MA(Info.A, 5, 1) < 20000) {
                            continue;
                        }

                        rateItemDict = stra.select(dsh, selectMode, ref sigInfo);
                        if (rateItemDict == null) {
                            continue;
                        }
                    } catch (DataException /*ex*/) {
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
            Dictionary<String, int> strategySelCountDict = new Dictionary<String, int>();
            foreach (var item in re.selItems_)
            {
                if (strategySelCountDict.ContainsKey(item.strategyName_))
                {
                    ++strategySelCountDict[item.strategyName_];
                }
                else
                {
                    strategySelCountDict.Add(item.strategyName_, 1);
                }
            }
            foreach (var item in re.selItems_)
            {
                item.sameDayStrategySelCount_ = strategySelCountDict[item.strategyName_];
                item.sameDaySelCount_ = re.selItems_.Count;
            }
            if (bShowProgress) {
                App.host_.uiFinishProcessBar();
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
            if (buyItem != null)
            {
                buyItem.iamBuyItem_ = true;
            }
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
