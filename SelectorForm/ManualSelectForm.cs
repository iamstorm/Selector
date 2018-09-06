using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SelectImpl;
using System.Data.SQLite;

namespace SelectorForm
{
    public partial class ManualSelectForm : Form
    {
        Dictionary<String, String> straRateItemKeyDict_ = new Dictionary<string, string>();
        public ManualSelectForm()
        {
            InitializeComponent();

            String sSQL = String.Format("Select * From manual_stra Order by straname");
            var dt = DB.Global().Select(sSQL);
            foreach (DataRow row in dt.Rows)
            {
                String sName = row["straname"].ToString();
                comboStra_.Items.Add(sName);
                straRateItemKeyDict_[sName] = row["rateItemKeys"].ToString();

                DB.RegConn(Dist.manualStraPath_+sName, sName);
            }
            comboStra_.SelectedItem = Utils.GetSysInfo(DB.Global(), "ManualSelectForm.SelectStra");
            DataTable rateItemDt = new DataTable();
            rateItemDt.Columns.Add("Key");
            rateItemDt.Columns.Add("Value");
            dataGridRateItem_.DataSource = rateItemDt;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddManualStraForm form = new AddManualStraForm();
            if (DialogResult.OK != form.ShowDialog())
            {
                return;
            }
            comboStra_.Items.Add(form.sName_);
            straRateItemKeyDict_[form.sName_] = form.sRateItemKeys_;
            comboStra_.SelectedItem = form.sName_;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string sStock = textStock_.Text.Trim();
            if (String.IsNullOrEmpty(sStock))
            {
                MessageBox.Show("Must provide stock!", "Selector");
                return;
            }
            Stock sk = null;
            foreach (var stock in App.ds_.stockList_)
            {
                if (stock.name_ == sStock || stock.code_ == sStock)
                {
                    sk = stock;
                    break;
                }
            }
            if (sk == null)
            {
                MessageBox.Show("Stock not found!", "Selector");
                return;
            }
            List<String> rateItemList = new List<string>();
            DataTable dt = (DataTable)dataGridRateItem_.DataSource;
            foreach (DataRow row in dt.Rows)
            {
                string sValue = row["Value"].ToString().Trim();
                if (String.IsNullOrEmpty(sValue))
                {
                    continue;
                }
                rateItemList.Add(row["Key"].ToString().Trim() + "/" + sValue);
            }
            SQLiteHelper sh = DB.connDict_[comboStra_.SelectedItem.ToString()];
            String sSQL = String.Format("Select Count(1) From selectitems Where code = '{0}' And date = {1}", sk.code_, Utils.NowDate());
            if (sh.ExecuteScalar<int>(sSQL) > 0)
            {
                MessageBox.Show("Already select this stock!", "Selector");
                return;
            }
            sSQL = String.Format("Insert Into selectitems (code, name, date, rateItems) Values ('{0}', '{1}', {2}, '{3}')", 
                sk.code_, sk.name_, Utils.NowDate(), String.Join(",", rateItemList));
            sh.Execute(sSQL);
            Utils.SetSysInfo(DB.Global(), "ManualSelectForm.SelectStra", comboStra_.SelectedItem.ToString());
            MessageBox.Show("Select Success!", "Selector");
        }

        private void comboStra__SelectedIndexChanged(object sender, EventArgs e)
        {
            String sName = comboStra_.SelectedItem.ToString();
            if (String.IsNullOrEmpty(sName))
            {
                return;
            }
            String sRateItemKeys = straRateItemKeyDict_[sName];
            var keyList = sRateItemKeys.Split(',');
            DataTable dt = (DataTable)dataGridRateItem_.DataSource;
            if (dt == null)
            {
                return;
            }
            dt.Rows.Clear();
            foreach (var item in keyList)
            {
                DataRow row = dt.NewRow();
                row["Key"] = item;
                dt.Rows.Add(row);
            }
        }
    }
}
