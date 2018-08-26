using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectImpl
{
    public class RankBuyDesider : IBuyDesider
    {
        public static RankBuyDesider buyer_ = new RankBuyDesider();
        public String name()
        {
            return "Rank";
        }
        Dictionary<String, List<SelectItem>> descomposeByStrategy(List<SelectItem> selItems)
        {
            var straDict = new Dictionary<String, List<SelectItem>>();
            foreach (var item in selItems)
            {
                List<SelectItem> items;
                if (straDict.TryGetValue(item.strategyName_, out items))
                {
                    items.Add(item);
                }
                else
                {
                    items = new List<SelectItem>();
                    items.Add(item);
                    straDict[item.strategyName_] = items;
                }
            }
            return straDict;
        }
        SelectItem priCompete(List<SelectItem> selItems, out float maxRank)
        {
            SelectItem maxRankItem = null;
            maxRank = 0;
            foreach (var item in selItems)
            {
                int rank = Utils.ToType<int>(item.getColumnVal("prirank"));
                if (rank > maxRank)
                {
                    maxRank = rank;
                    maxRankItem = item;
                }
            }
            return maxRankItem;
        }
        public List<SelectItem> getAllPriCompeteSucList(List<SelectItem> selItems)
        {
            var straDict = descomposeByStrategy(selItems);
            List<SelectItem> priCompeteSucList = new List<SelectItem>();
            foreach (var kv in straDict)
            {
                float maxPriRate;
                priCompeteSucList.Add(priCompete(kv.Value, out maxPriRate));
            }
            return priCompeteSucList;
        }
        SelectItem IBuyDesider.makeDeside(List<SelectItem> selItems)
        {
            var straDict = descomposeByStrategy(selItems);
            List<SelectItem> priCompeteSucList = new List<SelectItem>();
            foreach (var kv in straDict)
            {
                float maxPriRate;
                SelectItem item = priCompete(kv.Value, out maxPriRate);
                if (maxPriRate > Setting.MyBuyRateLimit)
                {
                    priCompeteSucList.Add(item);
                }
            }
            SelectItem maxRankItem = null;
            int maxRank = 0;
            foreach (var item in priCompeteSucList)
            {
                int rank = Utils.ToType<int>(item.getColumnVal("pubrank"));
                if (rank > maxRank)
                {
                    maxRank = rank;
                    maxRankItem = item;
                }
            }
            if (maxRank > Setting.MyBuyRateLimit)
            {
                return maxRankItem;
            }
            else
            {
                return null;
            }
        }
    }
}
