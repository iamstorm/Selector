using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SQLite;
using System.Windows.Forms;

namespace SelectImpl
{
    public struct ColumnInfo
    {
        public String name_;
        public int width_;
    }
    public class Utils
    {
        public static int Date(DateTime dateTime)
        {
            return Utils.ToType<int>(dateTime.ToString("yyyyMMdd"));
        }
        public static int NowDate()
        {
            return Date(DateTime.Now);
        }
        public static int NowTime()
        {
            return Utils.ToType<int>(DateTime.Now.ToString("HHmmss")); 
        }
        public static int NowMinute()
        {
            var dependNow = DateTime.Now;
            int nowTime = -1;
            if (dependNow.Hour >= 15)
            {
                dependNow = new DateTime(dependNow.Year, dependNow.Month, dependNow.Day, 15, 0, 0);
                nowTime = 60 * 4;
            }
            else if ((dependNow.Hour == 12 || (dependNow.Hour == 11 && dependNow.Minute > 30)))
            {
                dependNow = new DateTime(dependNow.Year, dependNow.Month, dependNow.Day, 11, 30, 0);
                nowTime = 60 * 2;
            }
            else if (dependNow.Hour < 9 || (dependNow.Hour == 9 && dependNow.Minute < 30))
            {
                dependNow = new DateTime(dependNow.Year, dependNow.Month, dependNow.Day, 9, 30, 0);
                nowTime = 0;
            }
            if (nowTime < 0)
            {
                if (dependNow.Hour >= 13)
                {
                    var deltaDate = dependNow - new DateTime(dependNow.Year, dependNow.Month, dependNow.Day, 13, 0, 0);
                    nowTime = (int)deltaDate.TotalMinutes + 60 * 2;
                }
                else
                {
                    var deltaDate = dependNow - new DateTime(dependNow.Year, dependNow.Month, dependNow.Day, 9, 30, 0);
                    nowTime = (int)deltaDate.TotalMinutes;
                }
            }
            return nowTime;
        }
        public static int DateSpanInt(int startDate, int endDate)
        {
            DateTime start = new DateTime(Year(startDate), Month(startDate), Day(startDate));
            DateTime end = new DateTime(Year(endDate), Month(endDate), Day(endDate));
            TimeSpan span = end - start;
            return (int)span.TotalDays;
        }
        public static String DateSpan(int startDate, int endDate)
        {
            return DateSpanInt(startDate, endDate).ToString() + "days";
        }
        public static float GetBonusValue(String bonus)
        {
            return Utils.ToType<float>(bonus.TrimEnd('%'));
        }
        public static String ToBonus(float zf)
        {
            return (zf * 100).ToString("F2") + "%";
        }
        public static float ToPrice(int normlizedVal)
        {
            return normlizedVal * 1.0f / Setting.NormalizeRate;
        }
        public static bool IsPriceType(Info info)
        {
            if (info == Info.C || info == Info.O || info == Info.H || info == Info.L)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsTradeDay(int date)
        {
            return App.ds_.tradeDateDict_.ContainsKey(date);
        }
        public static bool NowIsTradeDay()
        {
            return IsTradeDay(NowDate());
        }
        public static int LastTradeDay()
        {
            return DB.Global().ExecuteScalar<int>(String.Format("SELECT cal_date from trade_date where cal_date < {0}  order by cal_date desc limit 1", NowDate()));
        }
        public static int NextTradeDay(int date)
        {
            return DB.Global().ExecuteScalar<int>(String.Format("SELECT cal_date from trade_date where cal_date > {0} order by cal_date limit 1", date));
        }
        public static bool IsTradeTime(int hour, int minute)
        {
            if (hour == 9)
            {
                return minute >= 14;
            }
            if (hour == 15)
            {
                return minute <= 1;
            }
            if (hour == 11)
            {
                return minute <= 35;
            }
            if (hour == 12)
            {
                return false;
            }
            return hour > 9 && hour < 15;
        }
        public static bool IsOpenTime(int hour, int minute)
        {
            if (hour == 9 && minute >= 15)
            {
                return true;
            } else {
                return false;
            }
        }
        public static bool IsCloseTime(int hour, int minute)
        {
            if (hour == 14 && minute >= 54)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool IsInDayTime(int hour, int minute)
        {
            if (!IsTradeTime(hour, minute))
            {
                return false;
            }
            if (IsOpenTime(hour, minute) || IsCloseTime(hour, minute))
            {
                return false;
            }
            else if (hour == 15)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static int Year(int date)
        {
            return date / 10000;
        }
        public static int Month(int date)
        {
            return (date - Year(date) * 10000)/100;
        }
        public static int Day(int date)
        {
            return date - Year(date) * 10000 - Month(date) * 100;
        }
        public static String ToTimeDesc(DateTime time)
        {
            return time.ToString("yyyy-MM-dd HH:mm:ss");
        }
        public static DateTime ToDateTime(int date)
        {
            return new DateTime(Year(date), Month(date), Day(date));
        }
        public static bool IsOverlap(DateTime start0, DateTime end0, DateTime start1, DateTime end1)
        {
            int latest_start = Math.Max(Date(start0), Date(start1));
            int earliest_end = Math.Min(Date(end0), Date(end1));
            int overlap = earliest_end - latest_start;
            if (overlap < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static List<int> TraverTimeDay(int startDate, int endDate)
        {
            List<int> list = new List<int>();
            DateTime start = new DateTime(Year(startDate), Month(startDate), Day(startDate));
            DateTime end = new DateTime(Year(endDate), Month(endDate), Day(endDate));
            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                list.Add(Date(date));
            }
            return list;
        }
        public static List<int> TraverTimeDay(List<DateRange> dateRangeList)
        {
            List<int> list = new List<int>();
            foreach (var range in dateRangeList)
            {
                list.AddRange(TraverTimeDay(range.startDate_, range.endDate_));
            }
            return list;
        }
        public static T ToType<T>(object obj)
        {
            return (T)Convert.ChangeType(obj.ToString(), typeof(T));
        }

        public static void SetSysInfo(SQLiteHelper sh, String sKey, String sVal, bool bInsertIfNotExist = true)
        {
            int ret = sh.Execute(String.Format("Update SysInfo Set Val = '{0}' Where Key = '{1}'", sVal, sKey));
            if (bInsertIfNotExist && ret == 0)
            {
                sh.Execute(String.Format("Insert Into SysInfo(Key, Val) Values ('{0}', '{1}')", sKey, sVal));
            }
        }
        public static String GetSysInfo(SQLiteHelper sh, String sKey, String sDefVal = "")
        {
            object obj = sh.ExecuteScalar(String.Format("Select Val From SysInfo Where Key = '{0}'", sKey));
            if (obj == null)
            {
                return sDefVal;
            }
            else
            {
                return obj.ToString();
            }
        }
        public static String ReportWatch(Stopwatch watch)
        {
            Int64 elapseM = watch.ElapsedMilliseconds / 1000 / 60;
            Int64 elapseS = watch.ElapsedMilliseconds / 1000 % 60;
            if (elapseM == 0)
            {
                return String.Format("{0}s", elapseS);
            }
            else
            {
                return String.Format("{0}m{1}s", elapseM, elapseS);
            }
        }
        public static bool IsUpStop(float zf)
        {
            return zf > 0.095;
        }
        public static bool IsDownStop(float zf)
        {
            return zf < -0.095;
        }
        public static String StrategyName(String sRateItemKey)
        {
            return sRateItemKey.Split('/')[0];
        }
        public static float F(float c, float lastC)
        {
            return (c - lastC) / lastC;
        }
        public static float DeltaF(float dc, float lastC)
        {
            return dc / lastC;
        }
    }
}
