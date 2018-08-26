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
    public struct StraInfo
    {
        public String Name
        {
            get;
            set;
        }
    }
    public partial class SolutionSettingForm : Form
    {
        ComboBox cmbBox_ = new ComboBox();
        public SolutionSettingForm()
        {
            InitializeComponent();

            straGrid_.Controls.Add(cmbBox_);  //将时间控件加入DataGridView
            cmbBox_.Visible = false;  //先不显示
            cmbBox_.SelectedIndexChanged += new EventHandler(cmbBox_SelectedIndexChanged); //为时间控件加入事件dtp_TextChange
            cmbBox_.DropDownStyle = ComboBoxStyle.DropDownList;

            foreach (var stra in App.grp_.strategyList_)
            {
                cmbBox_.Items.Add(stra.name());
            }
            cmbBox_.Items.Add("");

            var dt = new DataTable();
            dt.Columns.Add("Name");
            straGrid_.DataSource = dt;

            straGrid_.Columns[0].Width = 400;

            var bs = new BindingSource();
            bs.DataSource = App.customSolutionSettingList_;
            nameListBox_.DataSource = bs;
            nameListBox_.DisplayMember = "name_";
        }
        bool bDontRunSelecteIndexChanged = false;
        private void cmbBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bDontRunSelecteIndexChanged)
            {
                bDontRunSelecteIndexChanged = false;
                return;
            }
            DataTable dt = (DataTable)straGrid_.DataSource;
            List<String> straNameList = toStraNameList(dt);
            String straName = cmbBox_.SelectedItem.ToString();
            if (straNameList.Contains(straName))
            {
                MessageBox.Show("The strategy allready exists!", "Selector");
                return;
            }


            int iIndex = straGrid_.CurrentRow.Index;
            if (dt.Rows.Count <= iIndex)
            {
                dt.Rows.Add(dt.NewRow());
            }
            DataRow row = dt.Rows[iIndex];
            row[straGrid_.CurrentCell.ColumnIndex] = straName;
            bDontRunSelecteIndexChanged = true;
            cmbBox_.Visible = false;
            cmbBox_.SelectedIndex = cmbBox_.Items.Count - 1;
        }
        private void straGrid__CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                Rectangle rect = straGrid_.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true); //得到所在单元格位置和大小
                cmbBox_.Size = new Size(rect.Width, rect.Height); //把单元格大小赋给时间控件
                cmbBox_.Location = new Point(rect.X, rect.Y); //把单元格位置赋给时间控件
                cmbBox_.Visible = true;  //显示控件
            }
            else
                cmbBox_.Visible = false;
        }

        private void straGrid__ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            cmbBox_.Visible = false;
        }

        private void straGrid__Scroll(object sender, ScrollEventArgs e)
        {
            cmbBox_.Visible = false;
        }
        List<String> toStraNameList(DataTable dt)
        {
            List<String> straNameList = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                var name = row["Name"].ToString().Trim();
                if (String.IsNullOrEmpty(name))
                {
                    continue;
                }
                straNameList.Add(name);
            }
            return straNameList;
        }
        private void btnAdd_Click(object sender, EventArgs e)
        {
            ForNameForm form = new ForNameForm();
            foreach (var item in App.customSolutionSettingList_)
            {
                form.allreadyExistNameList_.Add(item.name_);
            }
            foreach (var item in App.autoSolutionSettingList_)
            {
                form.allreadyExistNameList_.Add(item.name_);
            }
            if (DialogResult.OK != form.ShowDialog())
            {
                return;
            }
            SolutionSetting setting = new SolutionSetting();
            setting.name_ = form.name_;
            setting.straList_ = new List<IStrategy>();
            DataTable dt = (DataTable)straGrid_.DataSource;
            List<String> straNameList = toStraNameList(dt);
            foreach (var name in straNameList)
            {
                setting.straList_.Add(App.grp_.strategy(name));
                Dictionary<String, Object> row = new Dictionary<string, Object>();
                row["solution"] = setting.name_;
                row["strategy"] = name;
                DB.Global().Insert("SolutionSetting", row);
            }
            BindingSource bs = (BindingSource)(nameListBox_.DataSource);
            bs.Add(setting);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SolutionSetting setting = (SolutionSetting)nameListBox_.SelectedItem;
            setting.straList_.Clear();
            cmbBox_.Visible = false;
            DB.Global().Execute(String.Format("Delete From SolutionSetting Where solution = '{0}'", setting.name_));
            DataTable dt = (DataTable)straGrid_.DataSource;
            List<String> straNameList = toStraNameList(dt);
            foreach (var name in straNameList)
            {
                setting.straList_.Add(App.grp_.strategy(name));
                Dictionary<String, Object> row = new Dictionary<string, Object>();
                row["solution"] = setting.name_;
                row["strategy"] = name;
                DB.Global().Insert("SolutionSetting", row);
            }
            MessageBox.Show("Save success", "Selector");
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)straGrid_.DataSource;
            dt.Rows.Clear();
            cmbBox_.Visible = false;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)straGrid_.DataSource;
            dt.Rows.Clear();
            cmbBox_.Visible = false;
            SolutionSetting setting = (SolutionSetting)nameListBox_.SelectedItem;
            if (setting == null)
            {
                return;
            }
            if (DialogResult.Yes != MessageBox.Show("Are you sure to remove ?", "Selector", MessageBoxButtons.YesNo))
            {
                return;
            }
            DB.Global().Execute(String.Format("Delete From SolutionSetting Where solution = '{0}'", setting.name_));
            BindingSource bs = (BindingSource)(nameListBox_.DataSource);
            bs.Remove(setting);
        }

        private void nameListBox__SelectedIndexChanged(object sender, EventArgs e)
        {
            DataTable dt = (DataTable)straGrid_.DataSource;
            dt.Rows.Clear();
            cmbBox_.Visible = false;
            SolutionSetting setting = (SolutionSetting)nameListBox_.SelectedItem;
            if (setting == null)
            {
                return;
            }
            foreach (var item in setting.straList_)
            {
                var row = dt.NewRow();
                row["Name"] = item.name();
                dt.Rows.Add(row);
            }
        }

    }
}
