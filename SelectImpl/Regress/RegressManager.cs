using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace SelectImpl
{
    public class RegressManager
    {
        public DataStore ds_;
        public void regress(RegressResult regressRe)
        {
            DataStoreHelper dsh = new DataStoreHelper();
            SelectManager selManager = new SelectManager();
            List<int> dateList = Utils.TraverTimeDay(regressRe.dateRangeList_);
            dateList.Reverse();
            App.host_.uiStartProcessBar();
            int nFinishCount = 0;
            int nTotalCount = dateList.Count;

            SelectHint hint = new SelectHint();
            foreach (Stock sk in App.ds_.stockList_)
            {
                hint.nextWantedIndexHintDict_[sk] = -1;
            }
            foreach (int date in dateList)
            {
                SelectResult re = selManager.select(dsh, SelectMode.SM_Regress, date, regressRe.strategyList_, hint);
                regressRe.selItems_.AddRange(re.selItems_);
                App.host_.uiSetProcessBar(String.Format("正在回归{0}-{1}，选择阶段：完成{2}的选择，当前选中记录数：{3}",
                    dateList.Last(), dateList.First(), date, regressRe.selItems_.Count), 
                    nFinishCount * 100 / nTotalCount);
                ++nFinishCount;
            }
            regressRe.selItems_.Sort(delegate(SelectItem lhs, SelectItem rhs)
            {
                var lhsBonus = lhs.getColumnVal("allbonus");
                var rhsBonus = rhs.getColumnVal("allbonus");
                if (lhsBonus == "")
                {
                    return 1;
                }
                if (rhsBonus == "")
                {
                    return -1;
                }
                float lhsBonusValue = Utils.GetBonusValue(lhsBonus);
                float rhsBonusValue = Utils.GetBonusValue(rhsBonus);
                return lhsBonusValue.CompareTo(rhsBonusValue);
            });
            foreach (var item in regressRe.selItems_)
            {
                item.allSelectItems_ = regressRe.selItems_;
            }
            App.host_.uiFinishProcessBar();
            if (regressRe.runMode_ == RunMode.RM_Asset)
            {
                regressRe.buyItems_ = App.grp_.desideToBuy(regressRe);
            }
            else
            {
                regressRe.buyItems_ = App.grp_.buyMostBonusPerDay(regressRe);
            }
        }
    }
}
