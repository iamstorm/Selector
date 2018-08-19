using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace SelectorForm
{
    public class RegressResult
    {
        public String name_;
        public int startDate_;
        public int endDate_;
        public List<SelectItem> selItems_ = new List<SelectItem>();
        public List<BuyItem> buyItems_;
        public static RegressResult ReadFromDB(SQLiteHelper sh, String name)
        {
            RegressResult ret = new RegressResult();
            ret.name_ = name;
            return ret;
        }
        public void writeToDB(SQLiteHelper sh)
        {

        }
        public List<SelectItem> ofDate(int date)
        {
            List<SelectItem> retList = new List<SelectItem>();
            foreach (SelectItem item in selItems_)
            {
                if (item.date_ == date)
                {
                    retList.Add(item);
                }
            }
            return retList;
        }
    }
}
