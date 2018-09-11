﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Data.SQLite;

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
        public static String DateSpan(int startDate, int endDate)
        {
            DateTime start = new DateTime(Year(startDate), Month(startDate), Day(startDate));
            DateTime end = new DateTime(Year(endDate), Month(endDate), Day(endDate));
            TimeSpan span =  end - start;
            return span.TotalDays.ToString() + "days";
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
            return time.ToString("yyyy-MM-dd hh:mm:ss");
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
    }
}
