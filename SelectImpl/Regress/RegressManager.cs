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
        public RegressResult regress(String name, int startDate, int endDate)
        {
            DataStoreHelper dsh = new DataStoreHelper();
            RegressResult regressRe = new RegressResult();
            regressRe.name_ = name;
            regressRe.startDate_ = startDate;
            regressRe.endDate_ = endDate;
            SelectManager selManager = new SelectManager();
            List<int> dateList = Utils.TraverTimeDay(startDate, endDate);
            dateList.Reverse();
            App.host_.uiStartProcessBar();
            int nFinishCount = 0;
            int nTotalCount = dateList.Count;

            SelectHint hint = new SelectHint();
            foreach (IStrategy stra in App.grp_.strategyList_)
            {
                Dictionary<String, String> param = stra.setup();
                hint.straParamList_.Add(param);
            }
            foreach (Stock sk in App.ds_.stockList_)
            {
                hint.nextWantedIndexHintDict_[sk] = -1;
            }
            foreach (int date in dateList)
            {
                SelectResult re = selManager.select(dsh, date, hint);
                regressRe.selItems_.AddRange(re.selItems_);
                App.host_.uiSetProcessBar(String.Format("正在回归{0}-{1}完成{2}的选择, 当前选中记录数:{3}", 
                    startDate, endDate, date, regressRe.selItems_.Count), 
                    nFinishCount * 100 / nTotalCount);
                ++nFinishCount;
            }
            regressRe.buyItems_ = App.grp_.desideToBuy(regressRe);
            regressRe.selItems_ = SelectResult.MergeSelectItem(regressRe.selItems_);
            App.host_.uiFinishProcessBar();
            return regressRe;
        }
    }
}
