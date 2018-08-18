using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SelectorForm
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
            return (App.ds_.Ref(Info.ZF, dataList_, App.ds_.index(this, date))*100).ToString("F2")+"%";
        }

        public float zf(int date)
        {
            return App.ds_.Ref(Info.ZF, dataList_, App.ds_.index(this, date));
        }
    }
    public class DataStore
    {
        public Dictionary<String, Stock> stockDict_ = new Dictionary<String, Stock>();
        public List<Stock> stockList_ = new List<Stock>();
        public List<Data> szListData_ = new List<Data>();
        void readStocks()
        {
            DataTable stocks = DB.Global().Select("Select * From Stock Order by symbol");
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
        void readDayData(Stock sk)
        {
            string fileName = Dist.dayPath_+ sk.code_+".day";
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    while(true)
                    {
                        Data d = new Data();
                        d.date_ = reader.ReadInt32();
                        d.open_ = reader.ReadInt32();
                        d.high_ = reader.ReadInt32();
                        d.low_ = reader.ReadInt32();
                        d.close_ = reader.ReadInt32();
                        d.vol_ = reader.ReadInt32();
                        d.amount_ = reader.ReadInt32();
                        sk.dataList_.Add(d);
                    }
                }
            }
            catch (System.IO.EndOfStreamException )
            {
            }
            if (Utils.IsTradeDay())
            {
                sk.dataList_.Add(null);//代表今日
            }
            sk.dataList_.Reverse();
        }
        void readSZData()
        {
            DataTable datas = DB.Global().Select("Select * From [000001] Order by trade_date desc");
            if (Utils.IsTradeDay())
            {
                szListData_.Add(null);//代表今日
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
            if (!App.RunScript("start"))
            {
                return false;
            }

            MainForm.Me.setTradeDay();
            readStocks();
            if (stockDict_.Count == 0)
            {
                return false;
            }
            int nFinishCount = 0;
            int totalCount = stockList_.Count + 1;
            MainForm.Me.startProcessBar();
            foreach (Stock sk in stockList_)
            {
                readDayData(sk);
                ++nFinishCount;
                MainForm.Me.setProcessBar(String.Format("已完成读入{0}", sk.code_), nFinishCount * 100 / totalCount);
            }
            readSZData();
            MainForm.Me.finishProcessBar();
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
        public void end()
        {
            if (!App.RunScript("end"))
            {
                MessageBox.Show("结束时运行脚本失败！");
            }
        }
        public bool prepareForSelect()
        {
            if (!Utils.IsTradeDay())
            {
                return true;
            }
            if (!updateRuntime())
                return false;

            DataTable dt =DB.Global().Select("Select * From [runtime]");
            int nowDate = Utils.Date(DateTime.Now);
            foreach (DataRow row in dt.Rows)
            {
                Data d = new Data();
                string code = Utils.ToType<string>(row["code"]);
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

        float F(int z, List<Data> dataList, int iIndex)
        {
            if (iIndex + 1 >= dataList.Count)
            {
                throw new DataException();
            }
            float lastC = dataList[iIndex + 1].close_;
            return (z - lastC) / lastC;
        }
        public Stock sk(String code)
        {
            return stockDict_[code];
        }
        public List<Data> listData(String code)
        {
            return stockDict_[code].dataList_;
        }
        public int index(Stock stock, int date)
        {
            for (int i = 0; i < stock.dataList_.Count; ++i )
            {
                if (stock.dataList_[i].date_ == date)
                {
                    return i;
                }
            }
            return -1;
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
            if (wantedIndex >= dataList.Count)
            {
                throw new DataException();
            }
            Data d = dataList[wantedIndex];
            switch (info)
            {
                case Info.ZF:
                    return F(d.close_, dataList, wantedIndex);
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
                default:
                    throw new ArgumentException(string.Format("invalid info {0}", info));
            }
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
 
        public DataStoreHelper helper(String code, int iIndex)
        {
            return new DataStoreHelper(this, code, iIndex);
        }
    }
    public class DataStoreHelper
    {
        DataStore ds_;
        String code_;
        int iIndex_;
        List<Data> listData_;
        List<Data> szListData_;
        public DataStoreHelper(DataStore ds, String code, int iIndex)
        {
            ds_ = ds;
            code_ = code;
            iIndex_ = iIndex;
            listData_ = ds_.listData(code_);
            szListData_ = ds_.szListData_;
        }
        public float Ref(Info info, int dayCount = 0)
        {
            return ds_.Ref(info, listData_, iIndex_, dayCount);
        }
        public float MA(Info info, int count, int dayCount = 0)
        {
            return ds_.MA(info, count, listData_, iIndex_, dayCount);
        }
        public float SZRef(Info info, int dayCount = 0)
        {
            return ds_.Ref(info, szListData_, iIndex_, dayCount);
        }
        public float SZMA(Info info, int count, int dayCount = 0)
        {
            return ds_.MA(info, count, szListData_, iIndex_, dayCount);
        }
    }
}
