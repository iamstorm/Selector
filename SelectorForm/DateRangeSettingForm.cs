using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SelectImpl;

namespace SelectorForm
{
    public partial class DateRangeSettingForm : Form
    {
        DateTimePicker dtp_ = new DateTimePicker();
        public DateRangeSettingForm()
        {
            InitializeComponent();

            dateRangeGrid_.Controls.Add(dtp_);  //将时间控件加入DataGridView
            dtp_.Visible = false;  //先不显示
            dtp_.Format = DateTimePickerFormat.Custom;  //设置日期格式，2017-11-11
            dtp_.CustomFormat = "yyyy/MM/dd";
        //    dtp_.TextChanged += new EventHandler(dtp_TextChange); 
            dtp_.ValueChanged += new EventHandler(dtp_ValueChange); 
            dtp_.Leave += new EventHandler(dtp_ValueChange);

            DataTable dt = new DataTable();
            dt.Columns.Add("StartDate");
            dt.Columns.Add("EndDate");
            dateRangeGrid_.DataSource = dt;
            dateRangeGrid_.Columns[0].Width = 80;
            dateRangeGrid_.Columns[1].Width = 80;


            BindingSource bs = new BindingSource();
            bs.DataSource = App.dateRangeSettingList_;

            nameListBox_.DataSource = bs;
            nameListBox_.DisplayMember = "name_";

            string sDateRange = Utils.GetSysInfo(DB.Global(), "DateRangeSettingForm.daterange");
            if (sDateRange != "")
            {
                nameListBox_.SelectedItem = App.DateRange(sDateRange);
            }
        }
        private void dtp_Leave(object sender, EventArgs e)
        {
            dtp_.Visible = false;
        }
        private void dtp_ValueChange(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dateRangeGrid_.DataSource;
            List<Tuple<DateTime, DateTime>> dateTimeList = toDateTimeList(dt, false);

            int iIndex = dateRangeGrid_.CurrentRow.Index;
            if (dateTimeList.Count == dt.Rows.Count)
            {
                dt.Rows.Add(dt.NewRow());
            }
            DataRow row = dt.Rows[iIndex];
            row[dateRangeGrid_.CurrentCell.ColumnIndex] = dtp_.Text.ToString();
        }
        private void dateRangeGrid__CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                Rectangle rect = dateRangeGrid_.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true); //得到所在单元格位置和大小
                dtp_.Size = new Size(rect.Width, rect.Height); //把单元格大小赋给时间控件
                dtp_.Location = new Point(rect.X, rect.Y); //把单元格位置赋给时间控件
                dtp_.Visible = true;  //显示控件
                DateTime date;
                if (DateTime.TryParse(dateRangeGrid_.CurrentCell.Value.ToString(), out date))
                {
                      dtp_.Value = date.AddMinutes(1);
                }
                else
                {
                      dtp_.Value = dtp_.Value.AddMinutes(1);
                }
            }
            else
                dtp_.Visible = false;
        }

        private void dateRangeGrid__ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            dtp_.Visible = false;
        }

        private void dateRangeGrid__Scroll(object sender, ScrollEventArgs e)
        {
            dtp_.Visible = false;
        }
        List<Tuple<DateTime, DateTime>> toDateTimeList(DataTable dt, bool bSortIt = true)
        {
            List<Tuple<DateTime, DateTime>> dateTimeList = new List<Tuple<DateTime, DateTime>>();
            foreach (DataRow row in dt.Rows)
            {
                DateTime startDate, endDate;
                if (!DateTime.TryParse(row["StartDate"].ToString(), out startDate))
                {
                    continue;
                }

                if (!DateTime.TryParse(row["EndDate"].ToString(), out endDate))
                {
                    continue;
                }
                dateTimeList.Add(Tuple.Create(startDate, endDate));
            }
            if (bSortIt)
            {
                dateTimeList.Sort(delegate(Tuple<DateTime, DateTime> lhs, Tuple<DateTime, DateTime> rhs)
                {
                    return lhs.Item1.CompareTo(rhs.Item1);
                });
            }
            return dateTimeList;
        }
        List<DateRange> checkDateTimeRangeValidate()
        {
            DataTable dt = (DataTable)dateRangeGrid_.DataSource;
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("No range is provided!", "Selector");
                return null;
            }
            List<Tuple<DateTime, DateTime>> dateTimeList = toDateTimeList(dt, false);
            if (dateTimeList.Count == 0)
            {
                MessageBox.Show("No valid range is provided!", "Selector");
                return null;  
            }
            for (int i = 0; i < dateTimeList.Count; i++)
            {
                var item = dateTimeList[i];
                if (item.Item1 > item.Item2)
                {
                    MessageBox.Show(String.Format("Range {0} Error:  StartDate must before or equal endDate!", i + 1));
                    return null;
                }
            }
            for (int i =0; i < dateTimeList.Count; i++)
            {
                var startDateI = dateTimeList[i].Item1;
                var endDateI = dateTimeList[i].Item2;
                for (int j = i + 1; j < dateTimeList.Count; j++)
                {
                    var startDateJ = dateTimeList[j].Item1;
                    var endDateJ = dateTimeList[j].Item2;
                    if (Utils.IsOverlap(startDateI, endDateI, startDateJ, endDateJ))
                    {
                        MessageBox.Show(String.Format("Range {0} is overlay with Range {1}", i + 1, j + 1));
                        return null;
                    }
                }
            }
            List<DateRange> validDateRangeList = new List<DateRange>();
            foreach (var item in dateTimeList)
            {
                var startDate = item.Item1;
                var endDate = item.Item2;
                validDateRangeList.Add(new DateRange()
                {
                    startDate_ = Utils.Date(startDate),
                    endDate_ = Utils.Date(endDate)
                });
            }
            List<int> dateList = Utils.TraverTimeDay(validDateRangeList);
            bool bHasTradeDay = false;
            foreach (var date in dateList)
            {
                if (Utils.IsTradeDay(date))
                {
                    bHasTradeDay = true;
                    break;
                }
            }
            if (!bHasTradeDay)
            {
                MessageBox.Show("The date range don't contain any vaid trade day!", "Selector");
                return null;
            }
            return validDateRangeList;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            List<DateRange> dateRangeList = checkDateTimeRangeValidate();
            if (dateRangeList == null)
            {
                return;
            }
            ForNameForm form = new ForNameForm();
            foreach (var item in App.dateRangeSettingList_)
            {
                form.allreadyExistNameList_.Add(item.name_);
            }
            if (DialogResult.OK != form.ShowDialog())
            {
                return;
            }
            DateRangeSetting setting = new DateRangeSetting();
            setting.name_ = form.name_;
            setting.rangeList_ = dateRangeList;
            foreach (var item in dateRangeList)
            {
                Dictionary<String, Object> row = new Dictionary<string, Object>();
                row["name"] = setting.name_;
                row["start"] = item.startDate_;
                row["end"] = item.endDate_;
                DB.Global().Insert("DateRangeSetting", row);
            }
            BindingSource bs = (BindingSource)(nameListBox_.DataSource);
            bs.Add(setting);
            Utils.SetSysInfo(DB.Global(), "DateRangeSettingForm.daterange", nameListBox_.SelectedItem.ToString());

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            List<DateRange> dateRangeList = checkDateTimeRangeValidate();
            if (dateRangeList == null)
            {
                return;
            }
            DateRangeSetting setting = (DateRangeSetting)nameListBox_.SelectedItem;
            setting.rangeList_ = dateRangeList;
            DB.Global().Execute(String.Format("Delete From DateRangeSetting Where name = '{0}'", setting.name_));
            foreach (var item in dateRangeList)
            {
                Dictionary<String, Object> row = new Dictionary<string, Object>();
                row["name"] = setting.name_;
                row["start"] = item.startDate_;
                row["end"] = item.endDate_;
                DB.Global().Insert("DateRangeSetting", row);
            }
            Utils.SetSysInfo(DB.Global(), "DateRangeSettingForm.daterange", nameListBox_.SelectedItem.ToString());

            MessageBox.Show("Save success", "Selector");
        }

        private void nameListBox__SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dateRangeGrid_.DataSource;
            dt.Rows.Clear();
            dtp_.Visible = false;
            DateRangeSetting setting = (DateRangeSetting)nameListBox_.SelectedItem;
            if (setting == null)
            {
                return;
            }
            foreach (var item in setting.rangeList_)
            {
                var row = dt.NewRow();
                row["StartDate"] = Utils.ToDateTime(item.startDate_);
                row["EndDate"] = Utils.ToDateTime(item.endDate_);
                dt.Rows.Add(row);
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)dateRangeGrid_.DataSource;
            dt.Rows.Clear();
            dtp_.Visible = false;
        }

        private void btRemove_Click(object sender, EventArgs e)
        {
            dtp_.Visible = false;
            DateRangeSetting setting = (DateRangeSetting)nameListBox_.SelectedItem;
            if (setting == null)
            {
                return;
            }
            if (DialogResult.Yes != MessageBox.Show("Are you sure to remove ?", "Selector", MessageBoxButtons.YesNo))
            {
                return;
            }
            DB.Global().Execute(String.Format("Delete From DateRangeSetting Where name = '{0}'", setting.name_));
            BindingSource bs = (BindingSource)(nameListBox_.DataSource);
            bs.Remove(setting);
        }
    }
}
