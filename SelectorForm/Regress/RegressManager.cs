using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectorForm
{
    public class RegressManager
    {
        public DataStore ds_;
        public RegressResult regress(String name, int startDate, int endDate)
        {
            RegressResult regressRe = new RegressResult();
            regressRe.name_ = name;
            regressRe.startDate_ = startDate;
            regressRe.endDate_ = endDate;
            SelectManager selManager = new SelectManager();
            IList<int> dateList = Utils.TraverTimeDay(startDate, endDate);
            foreach (int date in dateList)
            {
                SelectResult re = selManager.select(date);
                regressRe.selectItems_.AddRange(re.selItems_);
            }
            return regressRe;
        }
    }
}
