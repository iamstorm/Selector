using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class StrategyAsset
    {
        public Dictionary<String, StrategyData> straDataDict_;
        public Dictionary<String, RateItemData> straRaItemKeyData;

        public StrategyAsset()
        {
            refreshAsset();
        }
        public void refreshAsset()
        {
            straDataDict_ = new Dictionary<String, StrategyData>();
            straRaItemKeyData = new Dictionary<String, RateItemData>();
            foreach(IStrategy stra in App.grp_.strategyList_)
            {
                straDataDict_.Add(stra.name(), null);
            }
            straDataDict_.Add("dontBuy", null);
            straDataDict_.Add("miss", null);
        }

        public StrategyData straData(String straName)
        {
            return straDataDict_[straName];
        }
        public StrategyData straRateItemData(String straName, String rateItemKey)
        {
            string key = straName + "/" + rateItemKey;
            if (straRaItemKeyData.ContainsKey(key))
            {
                return straRaItemKeyData[key];
            }
            else
            {
                return null;
            }
        }
    }
}
