using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SelectImpl
{
    public enum Info
    {
        O,
        C,
        H,
        L,
        OF,
        ZF,
        HF,
        LF,
        V,
        A,
        CO,
        HL,
    }
    public class Data
    {
        public int date_;
        public int open_;
        public int close_;
        public int high_;
        public int low_;
        public int vol_;
        public int amount_;

        public static Data NowInvalidData = new Data() { date_ = Utils.NowDate() };
    }
    public class Stock
    {
        public String code_;
        public String name_;
        public String is_hs_;
        public String industry_;
        public String area_;
        public bool sme_;//中小板
        public bool gem_;//创业板
        public bool st_;//ST
        public bool hs300_;//沪深300
        public bool sz50_;//上证50
        public bool zz500_;//中证500
        public List<Data> dataList_ = new List<Data>();


        public String zfSee(int date)
        {
            return Utils.ToBonus(App.ds_.Ref(Info.ZF, dataList_, App.ds_.index(this, date)));
        }

        public float of(int date, int iDateIndexHint = -1)
        {
            return App.ds_.Ref(Info.OF, dataList_, App.ds_.index(this, date, iDateIndexHint));
        }
        public float zf(int date, int iDateIndexHint = -1)
        {
            return App.ds_.Ref(Info.ZF, dataList_, App.ds_.index(this, date, iDateIndexHint));
        }
        public float hf(int date, int iDateIndexHint = -1)
        {
            return App.ds_.Ref(Info.HF, dataList_, App.ds_.index(this, date, iDateIndexHint));
        }
        public float lf(int date, int iDateIndexHint = -1)
        {
            return App.ds_.Ref(Info.LF, dataList_, App.ds_.index(this, date, iDateIndexHint));
        }
        public int nextTradeDate(int date)
        {
            int iIndex = App.ds_.index(this, date);
            if (iIndex == 0)
            {
                return -1;
            }
            return dataList_[iIndex-1].date_;
        }
        public int preDate(int date)
        {
            int iIndex = App.ds_.index(this, date);
            if (iIndex == dataList_.Count - 1)
            {
                return -1;
            }
            return dataList_[iIndex + 1].date_;
        }
    }
    public class DataStore
    {
        public Dictionary<String, Stock> stockDict_ = new Dictionary<String, Stock>();
        public List<Stock> stockList_ = new List<Stock>();
        public List<Data> szListData_ = new List<Data>();
        public Dictionary<int, int> tradeDateDict_ = new Dictionary<int,int>();
        void readTradeDate()
        {
            DataTable  tradedays = DB.Global().Select("Select cal_date From trade_date");
            foreach (DataRow row in tradedays.Rows)
            {
                tradeDateDict_[(int)row["cal_date"]] = 0;
            }
            App.host_.uiSetTradeDay();
        }
        void readStocks()
        {
            DataTable stocks;
            if (Setting.DataMode)
            {
                stocks = DB.Global().Select("Select * From Stock Order by symbol limit 50");
            }
            else
            {
                stocks = DB.Global().Select("Select * From Stock Order by symbol");
            }
            foreach (DataRow row in stocks.Rows)
            {
                Stock sk = new Stock();
                sk.code_ = row["symbol"].ToString();
                sk.name_ = row["name"].ToString();
                sk.is_hs_ = row["is_hs"].ToString();
                sk.industry_ = row["industry"].ToString();
                sk.area_ = row["area"].ToString();
                sk.sme_ = row["sme"].ToString() == "1";
                sk.gem_ = row["gem"].ToString() == "1";
                sk.st_ = row["st"].ToString() == "1";
                sk.hs300_ = row["hs300"].ToString() == "1";
                sk.sz50_ = row["sz50"].ToString() == "1";
                sk.zz500_ = row["zz500"].ToString() == "1";
                stockDict_.Add(sk.code_, sk);
                stockList_.Add(sk);
            }
        }
        class AdjFactor
        {
            public int date_;
            public float factor_;
        }
        void readDayData(Stock sk, List<AdjFactor> factors)
        {
            string fileName = Dist.dayPath_+ sk.code_+".day";
            try
            {
                int iCurFactorIndex = 0;
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    while(true)
                    {
                        Data d = new Data();
                        d.date_ = reader.ReadInt32();
                        
                        float factor = 1;
                        if (factors != null)
	                    {
                            do
                            {
                                if (d.date_ >= factors[iCurFactorIndex+1].date_)
                                    ++iCurFactorIndex;
                                else
                                    break;
                            } while (true);

                            factor = factors[iCurFactorIndex].factor_;
	                    }

                        d.open_ = (int)(reader.ReadInt32() * factor);
                        d.high_ = (int)(reader.ReadInt32() * factor);
                        d.low_ = (int)(reader.ReadInt32() * factor);
                        d.close_ = (int)(reader.ReadInt32() * factor);
                        d.vol_ = reader.ReadInt32();
                        d.amount_ = reader.ReadInt32();
                        sk.dataList_.Add(d);
                    }
                }
            }
            catch (System.IO.EndOfStreamException )
            {
            }
            if (Utils.NowIsTradeDay())
            {
                sk.dataList_.Add(Data.NowInvalidData);//代表今日
            }
            sk.dataList_.Reverse();
        }
        void readSZZSData()
        {
            DataTable datas;
            if (Utils.NowIsTradeDay())
            {
                datas = DB.Global().Select(String.Format("Select * From [000001] Where trade_date < {0} Order by trade_date desc", Utils.NowDate()));
            }
            else
            {
                datas = DB.Global().Select(String.Format("Select * From [000001] Order by trade_date desc"));
            }
            if (Utils.NowIsTradeDay())
            {
                szListData_.Add(Data.NowInvalidData);//代表今日
            }
            foreach (DataRow row in datas.Rows)
            {
                Data d = new Data();
                d.date_ = Utils.ToType<int>(row["trade_date"]);
                d.open_ = Utils.ToType<int>(row["open"]);
                d.high_ = Utils.ToType<int>(row["high"]);
                d.low_ = Utils.ToType<int>(row["low"]);
                d.close_ = Utils.ToType<int>(row["close"]);
                d.vol_ = Utils.ToType<int>(row["vol"]);
                szListData_.Add(d);
            }
        }
        public bool start()
        {
            if (Utils.GetSysInfo(DB.Global(), "SucRunStartScriptDate", "0") != Utils.NowDate().ToString())
            {
                if (!App.RunScript("start"))
                {
                    return false;
                }
            }

            readTradeDate();
            readStocks();
            if (stockDict_.Count == 0)
            {
                return false;
            }
            int nFinishCount = 0;
            int nTotalCount = stockList_.Count + 1;
            App.host_.uiStartProcessBar();
            Dictionary<String, List<AdjFactor>> stockFactorDict = new Dictionary<string, List<AdjFactor>>();
            DataTable dtFactor = DB.Global().Select("Select * From AdjFactor Order by trade_date");

            foreach (DataRow row in dtFactor.Rows)
            {
                String code = row["code"].ToString();
                AdjFactor adjFactor = new AdjFactor();
                adjFactor.date_ = Utils.ToType<int>(row["trade_date"]);
                adjFactor.factor_ = Utils.ToType<float>(row["adj_factor"]);
                List<AdjFactor> factors;
                if (stockFactorDict.TryGetValue(code, out factors))
                {
                    factors.Add(adjFactor);
                }
                else
                {
                    factors = new List<AdjFactor>();
                    factors.Add(adjFactor);
                    stockFactorDict[code] = factors;
                }
            }
            AdjFactor dummyFactor = new AdjFactor();
            dummyFactor.date_ = Utils.Date(DateTime.Now.AddYears(2));
            dummyFactor.factor_ = 1;
            foreach (var kv in stockFactorDict)
            {
                float newestFactor = kv.Value[kv.Value.Count - 1].factor_;
                kv.Value[kv.Value.Count - 1].factor_ = 1;
                for (int i = kv.Value.Count - 2; i >= 0; i--)
                {
                    kv.Value[i].factor_ = kv.Value[i].factor_ / newestFactor;                    
                }
                kv.Value.Add(dummyFactor);
            }

            foreach (Stock sk in stockList_)
            {
                List<AdjFactor> factors = null;
                stockFactorDict.TryGetValue(sk.code_, out factors);
                readDayData(sk, factors);
                ++nFinishCount;
                App.host_.uiSetProcessBar(String.Format("已完成读入{0}", sk.code_), nFinishCount * 100 / nTotalCount);
            }
            readSZZSData();
            int lastTrayDay = Utils.LastTradeDay();
            int iNewestLastTrayDayIndex;
            if (Utils.NowIsTradeDay())
            {
                iNewestLastTrayDayIndex = 1;
            }
            else
            {
                iNewestLastTrayDayIndex = 0;
            }
            bool bSZZSHasFullHistory = szListData_[iNewestLastTrayDayIndex].date_ == lastTrayDay;
            bool bStockHasFullHistory = false;
            foreach (var sk in stockList_)
            {
                if (sk.dataList_[iNewestLastTrayDayIndex].date_ == lastTrayDay)
                {
                    bStockHasFullHistory = true;
                    break;
                }
            }
            if (bSZZSHasFullHistory && bStockHasFullHistory)
            {
                Utils.SetSysInfo(DB.Global(), "SucRunStartScriptDate", Utils.NowDate().ToString());
            }
            else
            {
                if (Utils.NowIsTradeDay())
                {
                    MessageBox.Show("The history is not full!", "Selector");
                }
            }

            App.host_.uiFinishProcessBar();
            return true;
        }
        public bool updateRuntime()
        {
            if (!App.RunScript("runtime"))
            {
                return false;
            }
            return true;
        }
        public bool end()
        {
            if (!App.RunScript("end"))
            {
                return false;
            }
            return true;
        }
        bool runRunTimeScript()
        {
            if (!App.host_.uiAutoSelectMode() )
            {
                String sLastUpdateTime = Utils.GetSysInfo(DB.Global(), "SucRunRuntimeScriptTime", "2005-01-01");
                DateTime lastUpdateTime;
                if (DateTime.TryParse(sLastUpdateTime, out lastUpdateTime))
                {
                    var now = DateTime.Now;
                    if (lastUpdateTime.Year == now.Year && lastUpdateTime.Day == now.Day &&
                        (lastUpdateTime.Hour > 15 || (lastUpdateTime.Hour == 15 && lastUpdateTime.Minute > 15)))
                    {
                        return true;
                    }
                    var span = DateTime.Now - lastUpdateTime;
                    if (span.TotalMinutes < 2)
                    {
                        if (DialogResult.No == MessageBox.Show("Update runtime Now?", "Selector", MessageBoxButtons.YesNo))
                        {
                            return true;
                        }
                    }
                }
            }
       
            if (!updateRuntime())
                return false;

            Utils.SetSysInfo(DB.Global(), "SucRunRuntimeScriptTime", DateTime.Now.ToString());
            return true;
        }
        public bool prepareForSelect()
        {
            if (!Utils.NowIsTradeDay())
            {
                return true;
            }

            if (!runRunTimeScript())
            {
                return false;
            }

            DataTable dt = DB.Global().Select("Select * From [runtime]");
            int nowDate = Utils.NowDate();
            foreach (DataRow row in dt.Rows)
            {
                string code = row["code"].ToString();
                if (!stockDict_.ContainsKey(code))
                {
                    if (Setting.DataMode)
                    {
                        continue;
                    }
                    if (Setting.IsAcceptableRuntimeCode(code))
                    {
                        App.host_.uiReportSelectMsg(String.Format("{0} is not in history database!", code), false);
                    }
                    continue;
                }
                Data d = new Data();
                d.date_ = nowDate;
                d.open_ = Utils.ToType<int>(row["open"]);
                d.high_ = Utils.ToType<int>(row["high"]);
                d.low_ = Utils.ToType<int>(row["low"]);
                d.close_ = Utils.ToType<int>(row["trade"]);
                d.vol_ = Utils.ToType<int>(row["volume"]);
                d.amount_ = Utils.ToType<int>(row["amount"]);
                listData(code)[0] = d;
            }
            dt = DB.Global().Select("Select * From [000001runtime]");
            foreach (DataRow row in dt.Rows)
            {
                Data d = new Data();
                d.date_ = nowDate;
                d.open_ = Utils.ToType<int>(row["open"]);
                d.high_ = Utils.ToType<int>(row["high"]);
                d.low_ = Utils.ToType<int>(row["low"]);
                d.close_ = Utils.ToType<int>(row["close"]);
                d.vol_ = Utils.ToType<int>(row["vol"]);
                szListData_[0] = d;
            }
            return true;
        }

        public float F(int z, List<Data> dataList, int iIndex)
        {
            if (iIndex + 1 >= dataList.Count)
            {
                throw new DataException();
            }
            float lastC = dataList[iIndex + 1].close_;
            return (z - lastC) / lastC;
        }
        public String envBonus(int date, int dayCount = 0)
        {
            int iIndex = index(szListData_, date + dayCount);
            if (iIndex == -1 || iIndex == szListData_.Count - 1)
            {
                return "";
            }
            float envZf = App.ds_.F(
                szListData_[iIndex].close_, 
                szListData_,
                iIndex) * 100;
            return envZf.ToString("F2") + "%";
        }
        public Stock sk(String code)
        {
            return stockDict_[code];
        }
        public List<Data> listData(String code)
        {
            return stockDict_[code].dataList_;
        }
        public int index(List<Data> dataList, int date, int iDateIndexHint = -1)
        {
            int iStartIndex = iDateIndexHint == -1 ? 0 : iDateIndexHint;
            for (int i = iStartIndex; i < dataList.Count; ++i)
            {
                if (dataList[i].date_ == date)
                {
                    return i;
                }
                if (dataList[i].date_ < date)
                {
                    return -1;
                }
            }
            return -1;
        }
        public int index(Stock stock, int date, int iDateIndexHint = -1)
        {
            return index(stock.dataList_, date, iDateIndexHint);
        }
        public float realVal(Info info, String code, int date)
        {
            if (!Utils.IsPriceType(info))
            {
                throw new ArgumentException("input info is not price type!");
            }
            Stock stock = sk(code);
            return Utils.ToPrice((int)Ref(info, stock.dataList_, index(stock, date), 0));
        }
        public float Ref(Info info, List<Data> dataList, int iIndex, int dayCount = 0)
        {
            int wantedIndex = iIndex + dayCount;
            if (wantedIndex >= dataList.Count || wantedIndex < 0)
            {
                throw new DataException();
            }
            Data d = dataList[wantedIndex];
            switch (info)
            {
                case Info.ZF:
                    return F(d.close_, dataList, wantedIndex);
                case Info.OF:
                    return F(d.open_, dataList, wantedIndex);
                case Info.HF:
                    return F(d.high_, dataList, wantedIndex);
                case Info.LF:
                    return F(d.low_, dataList, wantedIndex);
                case Info.C:
                    return d.close_;
                case Info.O:
                    return d.open_;
                case Info.H:
                    return d.high_;
                case Info.L:
                    return d.low_;
                case Info.V:
                    return d.vol_;
                case Info.A:
                    return d.amount_;
                case Info.CO:
                    return Math.Abs(d.close_ - d.open_);
                case Info.HL:
                    return d.high_ - d.low_;
                default:
                    throw new ArgumentException(string.Format("invalid info {0}", info));
            }
        }
        public int Date(List<Data> dataList, int iIndex, int dayCount = 0)
        {
            int wantedIndex = iIndex + dayCount;
            if (wantedIndex >= dataList.Count || wantedIndex < 0)
            {
                throw new DataException();
            }
            return dataList[wantedIndex].date_;
        }
        public float MA(Info info, int count, List<Data> dataList, int iIndex, int dayCount = 0)
        {
            int wantedIndex = iIndex + dayCount;
            float sum = 0;
            for (int i = 0; i < count;  ++i )
            {
                sum += Ref(info, dataList, wantedIndex+i, 0);
            }
            return sum / count;
        }
    }
    public class DataStoreHelper
    {
        public DataStore ds_;
        public Stock stock_;
        public int iIndex_;
        public List<Data> dataList_;
        List<Data> szListData_;
        public int iSZIndex_;
        public DataStoreHelper()
        {
            ds_ = App.ds_;
            szListData_ = App.ds_.szListData_;
        }
        public DataStoreHelper newDsh(int dayCount)
        {
            var ret = new DataStoreHelper();
            ret.ds_ = ds_;
            ret.stock_ = stock_;
            ret.iIndex_ = iIndex_ + dayCount;
            ret.dataList_ = dataList_;
            ret.szListData_ = szListData_;
            ret.iSZIndex_ = iSZIndex_;
            return ret;
        }
        public void setStock(Stock stock)
        {
            stock_ = stock;
            dataList_ = stock_.dataList_;
        }
        public float Ref(Info info, int dayCount = 0)
        {
            return ds_.Ref(info, dataList_, iIndex_, dayCount);
        }
        public float MA(Info info, int count, int dayCount = 0)
        {
            return ds_.MA(info, count, dataList_, iIndex_, dayCount);
        }
        public float SZRef(Info info, int dayCount = 0)
        {
            return ds_.Ref(info, szListData_, iSZIndex_, dayCount);
        }
        public float SZMA(Info info, int count, int dayCount = 0)
        {
            return ds_.MA(info, count, szListData_, iSZIndex_, dayCount);
        }
        public int Date(int dayCount = 0)
        {
            return ds_.Date(dataList_, iIndex_, dayCount);
        }
        public float UpShadow(int dayCount = 0)
        {
            float maxCO = Math.Max(Ref(Info.C, dayCount), Ref(Info.O, dayCount));
            return (Ref(Info.H, dayCount) - maxCO) / Ref(Info.C, dayCount + 1);
        }
        public float DownShadow(int dayCount = 0)
        {
            float minCO = Math.Min(Ref(Info.C, dayCount), Ref(Info.O, dayCount));
            return (Ref(Info.L, dayCount) - minCO) / Ref(Info.C, dayCount + 1);
        }
        public bool CrossMA(Info info, int count, int beCrossCount, int dayCount = 0)
        {
            float curMA = ds_.MA(info, count, dataList_, iIndex_, dayCount);
            float curBeCrossMA = ds_.MA(info, beCrossCount, dataList_, iIndex_, dayCount);
            float preMA = ds_.MA(info, count, dataList_, iIndex_, dayCount+1);
            float preBeCrossMA = ds_.MA(info, beCrossCount, dataList_, iIndex_, dayCount+1);
            return preBeCrossMA > preMA && curBeCrossMA < curMA;
        }
        public bool IsUpStopEveryDay(int count, int dayCount=0)
        {
            for (int i = 0; i < count; i++)
            {
                if (Ref(Info.ZF, i + dayCount) < 0.095)
                    return false;
            }
            return true;
        }
        public float AbsCO(int dayCount=0)
        {
            return Math.Abs(Ref(Info.C, dayCount) - Ref(Info.O, dayCount));
        }

        public int SZLL(Info info, int count, int dayCount = 0, int upOrDown = 0)
        {
            float minVal = float.MaxValue;
            int iLastDay = dayCount + count;
            int iMinIndex = -1;
            for (int i = dayCount; i < iLastDay; i++)
            {
                var curZF = SZRef(Info.ZF, i);
                if (upOrDown == 1 && curZF <= 0)
                {
                    continue;
                }
                else if (upOrDown == -1 && curZF >= 0)
                {
                    continue;
                }
                float val = SZRef(info, i);
                if (val < minVal)
                {
                    minVal = val;
                    iMinIndex = i;
                }
            }
            return iMinIndex;
        }
        public int SZHH(Info info, int count, int dayCount = 0, int upOrDown = 0)
        {
            float maxVal = float.MinValue;
            int iLastDay = dayCount + count;
            int iMaxIndex = -1;
            for (int i = dayCount; i < iLastDay; i++)
            {
                var curZF = SZRef(Info.ZF, i);
                if (upOrDown == 1 && curZF <= 0)
                {
                    continue;
                }
                else if (upOrDown == -1 && curZF >= 0)
                {
                    continue;
                }
                float val = SZRef(info, i);
                if (val > maxVal)
                {
                    maxVal = val;
                    iMaxIndex = i;
                }
            }
            return iMaxIndex;
        }
        public int LL(Info info, int count, int dayCount = 0, int upOrDown = 0)
        {
            float minVal = float.MaxValue;
            int iLastDay = dayCount + count;
            int iMinIndex = -1;
            for (int i = dayCount; i < iLastDay; i++)
            {
                var curZF = Ref(Info.ZF, i);
                if (upOrDown == 1 && curZF <= 0)
                {
                    continue;
                }
                else if (upOrDown == -1 && curZF >= 0)
                {
                    continue;
                }
                float val = Ref(info, i);
                if (val < minVal)
	            {
		            minVal = val;
                    iMinIndex = i;
	            }
            }
            return iMinIndex;
        }
        public int HH(Info info, int count, int dayCount = 0, int upOrDown = 0)
        {
            float maxVal = float.MinValue;
            int iLastDay = dayCount + count;
            int iMaxIndex = -1;
            for (int i = dayCount; i < iLastDay; i++)
            {
                var curZF = Ref(Info.ZF, i);
                if (upOrDown == 1 && curZF <= 0)
                {
                    continue;
                }
                else if (upOrDown == -1 && curZF >= 0)
                {
                    continue;
                }
                float val = Ref(info, i);
                if (val > maxVal)
                {
                    maxVal = val;
                    iMaxIndex = i;
                }
            }
            return iMaxIndex;
        }
        public bool IsLowPeak(Info info, int iIndex, int count)
        {
            float indexVal = Ref(info, iIndex);
            int iMinIndex = iIndex - count;
            int iMaxIndex = iIndex + count;
            for (int i = iMinIndex; i < iMaxIndex; i++)
            {
                if (i == iIndex)
                {
                    continue;
                }
                float val = Ref(info, i);
                if (val < indexVal)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsHighPeak(Info info, int iIndex, int count)
        {
            float indexVal = Ref(info, iIndex);
            int iMinIndex = iIndex - count;
            int iMaxIndex = iIndex + count;
            for (int i = iMinIndex; i < iMaxIndex; i++)
            {
                if (i == iIndex)
                {
                    continue;
                }
                float val = Ref(info, i);
                if (val > indexVal)
                {
                    return false;
                }
            }
            return true;
        }
        public bool IsRealUp(int dayCount = 0)
        {
            if (Ref(Info.C, dayCount) <= Ref(Info.O, dayCount))
            {
                return false;
            }
            if (Ref(Info.ZF, dayCount) <= 0)
            {
                return false;
            }
            return true;
        }
        public bool IsRealDown(int dayCount = 0)
        {
            if (Ref(Info.C, dayCount) >= Ref(Info.O, dayCount))
            {
                return false;
            }
            if (Ref(Info.ZF, dayCount) >= 0)
            {
                return false;
            }
            return true;
        }
        public bool IsReal(int dayCount = 0)
        {
            float zf = Ref(Info.ZF, dayCount);
            if (zf > 0)
            {
                if (Ref(Info.C, dayCount) <= Ref(Info.O, dayCount))
                {
                    return false;
                }
            }
            else if (zf < 0)
            {
                if (Ref(Info.C, dayCount) >= Ref(Info.O, dayCount))
                {
                    return false;
                }
            }
            else
            {
                if (Ref(Info.C, dayCount) != Ref(Info.O, dayCount))
                {
                    return false;
                }
            }
            return true;
        }
        public int birthCount(int dayCount = 0)
        {
            return dataList_.Count - iIndex_ - dayCount;
        }
        public float MinCO(int dayCount = 0)
        {
            return Math.Min(Ref(Info.C, dayCount), Ref(Info.O, dayCount));
        }
        public float MaxCO(int dayCount = 0)
        {
            return Math.Max(Ref(Info.C, dayCount), Ref(Info.O, dayCount));
        }
        public bool IsLikeSTStop(int dayCount = 0)
        {
            if (Ref(Info.ZF, dayCount) < 0.06 && Ref(Info.ZF, dayCount) > 0.04 && Ref(Info.C, dayCount) == Ref(Info.H, dayCount))
            {
                return true;
            }

            if (Ref(Info.ZF, dayCount) < -0.04 && Ref(Info.ZF, dayCount) > -0.06 && Ref(Info.C, dayCount) == Ref(Info.L, dayCount))
            {
                return true;
            }
            return false;
        }
        public float AccZF(int count, int dayCount = 0)
        {
            float accZF = 0;
            for (int i = 0; i < count; ++i )
            {
                accZF += Ref(Info.ZF, i + dayCount);
            }
            return accZF;
        }
        public float Acc(Info info, int count, int dayCount = 0, int upOrDown = 0)
        {
            float acc = 0;
            for (int i = 0; i < count; ++i)
            {
                var curZF = Ref(Info.ZF, i + dayCount);
                if (upOrDown == 1 && curZF <= 0)
                {
                    continue;
                }
                else if (upOrDown == -1 && curZF >= 0)
                {
                    continue;
                }
                acc += Ref(info, i + dayCount);
            }
            return acc;
        }
        public float SZAcc(Info info, int count, int dayCount = 0, int upOrDown = 0)
        {
            float acc = 0;
            for (int i = 0; i < count; ++i)
            {
                var curZF = SZRef(Info.ZF, i + dayCount);
                if (upOrDown == 1 && curZF <= 0)
                {
                    continue;
                }
                else if (upOrDown == -1 && curZF >= 0)
                {
                    continue;
                }
                acc += SZRef(info, i + dayCount);
            }
            return acc;
        }
        public int EveryDown(int dayCount = 0)
        {
            int nCount = 0;
            int i = 0;
            do 
            {
                if (Ref(Info.ZF, dayCount+i) >= 0)
                {
                    break;
                }
                ++nCount;
                ++i;
            } while (true);
            return nCount;
        }
        public int EveryUp(int dayCount = 0)
        {
            int nCount = 0;
            int i = 0;
            do
            {
                if (Ref(Info.ZF, dayCount + i) <= 0)
                {
                    break;
                }
                ++nCount;
                ++i;
            } while (true);
            return nCount;
        }
        public bool IsRed(int dayCount = 0)
        {
            return Ref(Info.O, dayCount) < Ref(Info.C, dayCount);
        }
        public bool IsGreen(int dayCount = 0)
        {
            return Ref(Info.O, dayCount) > Ref(Info.C, dayCount);
        }
        public int RedCount(int count, int dayCount = 0)
        {
            int nRedCount = 0;
            for (int i = 0; i < count; ++i)
            {
                if (IsRed(i+dayCount))
                {
                    ++nRedCount;
                }
            }
            return nRedCount;
        }
        public int GreenCount(int count, int dayCount = 0)
        {
            int nGreenCount = 0;
            for (int i = 0; i < count; ++i)
            {
                if (IsGreen(i + dayCount))
                {
                    ++nGreenCount;
                }
            }
            return nGreenCount;
        }

        public bool IsSZRed(int dayCount = 0)
        {
            return SZRef(Info.O, dayCount) < SZRef(Info.C, dayCount);
        }
        public bool IsSZGreen(int dayCount = 0)
        {
            return SZRef(Info.O, dayCount) > SZRef(Info.C, dayCount);
        }
        public int SZRedCount(int count, int dayCount = 0)
        {
            int nRedCount = 0;
            for (int i = 0; i < count; ++i)
            {
                if (IsSZRed(i + dayCount))
                {
                    ++nRedCount;
                }
            }
            return nRedCount;
        }
        public int SZGreenCount(int count, int dayCount = 0)
        {
            int nGreenCount = 0;
            for (int i = 0; i < count; ++i)
            {
                if (IsSZGreen(i + dayCount))
                {
                    ++nGreenCount;
                }
            }
            return nGreenCount;
        }
    }
}
