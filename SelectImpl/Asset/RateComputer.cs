using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public static class RateComputer
    {
        public static int NoDataRate = 0;
        public static int ComputerRank(String straName, Dictionary<String, String> rateItemDict)
        {
            List<HistoryData> dataList =  new List<HistoryData>();
            foreach (var kv in rateItemDict)
            {
                HistoryData data = App.asset_.straRateItemData(kv.Key);
                if (data != null)
                {
                    dataList.Add(data);
                }
            }
            if (dataList.Count == 0)
            {
                return App.asset_.straData(straName).rank_;
            }
            int nTotalRank = 0;
            foreach (var data in dataList)
            {
                nTotalRank += data.rank_;
            }
            return nTotalRank / dataList.Count;
        }
    }
}
