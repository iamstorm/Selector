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
    public partial class WriteAssetForm : Form
    {
        public String sDatarangeName_;
        public WriteAssetForm()
        {
            InitializeComponent();


            AcceptButton = btnOk;


            dateRangeComboBox_.DataSource = App.dateRangeSettingList_;
            dateRangeComboBox_.DisplayMember = "name_";

            string sDateRange = Utils.GetSysInfo(DB.Global(), "WriteAssetForm.daterange");
            if (sDateRange != "")
            {
                dateRangeComboBox_.SelectedItem = App.DateRange(sDateRange);
            }
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (dateRangeComboBox_.SelectedIndex == -1)
            {
                MessageBox.Show("Must select DateRange!", "Selector");
                this.DialogResult = DialogResult.None;
                return;
            }
            sDatarangeName_ = dateRangeComboBox_.SelectedItem.ToString();
            Utils.SetSysInfo(DB.Global(), "WriteAssetForm.daterange", sDatarangeName_);
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
