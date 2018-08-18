using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectorForm
{
    public class SelectItem
    {
        public int date_;
        public String code_;
        public String strategyName_;
        public String[] rateItems_;
        public String rateItemKey_;
        public int rate_;
        public String bonus_;
    }
    public class BuyItem
    {
        public int date_;
        public String code_;
        public SelectItem bySelectItem_;
    }
    public class SelectResult
    {
        public List<SelectItem> selItems_ =  new List<SelectItem>();
    }
}
