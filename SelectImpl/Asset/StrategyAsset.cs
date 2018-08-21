using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class StrategyAsset
    {
        public Dictionary<String, HistoryData> straDataDict_;
        public Dictionary<String, HistoryData> straRaItemData_;

        public StrategyAsset()
        {
            refreshAsset();
        }
        public void refreshAsset()
        {
            straDataDict_ = new Dictionary<String, HistoryData>();
            straRaItemData_ = new Dictionary<String, HistoryData>();
            foreach(IStrategy stra in App.grp_.strategyList_)
            {
                straDataDict_.Add(stra.name(), null);
            }
            straDataDict_.Add("dontBuy", null);
            straDataDict_.Add("miss", null);
        }

        public HistoryData straData(String straName)
        {
            return straDataDict_[straName];
        }
        public HistoryData straRateItemData(String straName, String rateItemKey)
        {
            string key = straName + "/" + rateItemKey;
            if (straRaItemData_.ContainsKey(key))
            {
                return straRaItemData_[key];
            }
            else
            {
                return null;
            }
        }
    }
}
