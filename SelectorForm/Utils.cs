using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;

namespace SelectorForm
{
    class Utils
    {
        public static int Date(DateTime dateTime)
        {
            int date = 0;
            int.TryParse(dateTime.ToString("yyyyMMdd"), out date);
            return date;
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
        static bool isTradeDayIsInit_;
        static bool isTradeDay_;
        public static bool IsTradeDay()
        {
            if (!isTradeDayIsInit_)
            {
                isTradeDay_ = DB.Global().ExecuteScalar<int>(
                            String.Format("Select Count(*) From trade_date Where cal_date = '{0}'", Date(DateTime.Now))) > 0;
                isTradeDayIsInit_ = true;
            }
            return isTradeDay_;
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
        public static String FormatRateItemKey(String[] rateItems)
        {
            return String.Join("|", rateItems);
        }
        public static T ToType<T>(object obj)
        {
            return (T)Convert.ChangeType(obj.ToString(), typeof(T));
        }
        public static void UpdateGridRowNum(DataGridView grid)
        {
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                grid.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
        }
    }
}
