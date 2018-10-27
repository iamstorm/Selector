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
            maxRank = int.MinValue;
            float maxRankC = 0;
            foreach (var item in selItems)
            {
                int rank = Utils.ToType<int>(item.getColumnVal("prirank"));
                if (rank > maxRank)
                {
                    maxRank = rank;
                    maxRankItem = item;
                    maxRankC = Utils.ToType<float>(item.getColumnVal("close"));
                }
                else if (rank == maxRank)
                {
                    float curC = Utils.ToType<float>(item.getColumnVal("close"));
                    if (curC > maxRankC)
                    {
                        maxRank = rank;
                        maxRankItem = item;
                        maxRankC = curC;
                    }
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
            if (selItems.Count == 0)
            {
                return null;
            }
            selItems.Sort(delegate(SelectItem lhs, SelectItem rhs)
            {
                bool bLhsIsNN = lhs.strategyName_.StartsWith("NN");
                bool bRhsIsNN = rhs.strategyName_.StartsWith("NN");
                if (bLhsIsNN && !bRhsIsNN)
                {
                    return 1;
                }
                if (!bLhsIsNN && bRhsIsNN)
                {
                    return -1;
                }
                var lhsPubrank = Utils.ToType<int>(lhs.getColumnVal("pubrank"));
                var rhsPubrank = Utils.ToType<int>(rhs.getColumnVal("pubrank"));
                if (lhsPubrank != rhsPubrank)
                {
                    return rhsPubrank.CompareTo(lhsPubrank);
                }
                var lhsPrirank = Utils.ToType<int>(lhs.getColumnVal("prirank"));
                var rhsPrirank = Utils.ToType<int>(rhs.getColumnVal("prirank"));
                if (lhsPrirank != rhsPrirank)
                {
                    return rhsPrirank.CompareTo(lhsPrirank);
                }
                var lhsClose = Utils.ToType<float>(lhs.getColumnVal("close"));
                var rhsClose = Utils.ToType<float>(rhs.getColumnVal("close"));
                return rhsClose.CompareTo(lhsClose);
            });
            return selItems[0];
        }
    }
}
