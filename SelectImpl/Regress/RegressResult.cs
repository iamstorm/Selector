using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace SelectImpl
{
    public class RegressResult
    {
        public String name_;
        public int startDate_;
        public int endDate_;
        public List<SelectItem> selItems_ = new List<SelectItem>();
        public List<SelectItem> buyItems_;
        public String TotalBonus
        {
            get 
            {
                float totalBous = 0;
                foreach (var item in buyItems_)
                {
                    var bonus = item.getColumnVal("bonus");
                    if (bonus == "")
                    {
                        continue;
                    }
                    totalBous += Utils.GetBonusValue(bonus);
                }
                return totalBous.ToString("F2") + "%";
            }
        }
        public String DBFilePath
        {
            get
            {
                return Dist.regressPath_+ name_+".data";
            }
        }
        public String ConnKey
        {
            get
            {
                return "re$" + name_;
            }
        }
        public String SelectFormName
        {
            get
            {
                return name_ + "_Select";
            }
        }
        public String BuyFormName
        {
            get
            {
                return name_ + "_Buy";
            }
        }
        public String[] AllFormNames
        {
            get
            {
                return new String[] { SelectFormName, BuyFormName };
            }
        }
    }
}
