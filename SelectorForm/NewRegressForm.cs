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
    public partial class NewRegressForm : Form
    {
        public RegressResult re_;
        public NewRegressForm()
        {
            InitializeComponent();


            AcceptButton = btnOk;

            List<  SolutionSetting> allSolutionSetting = new List<SolutionSetting>();
            allSolutionSetting.AddRange(App.customSolutionSettingList_);
            allSolutionSetting.AddRange(App.autoSolutionSettingList_);

            slnComboBox_.DataSource = allSolutionSetting;
            slnComboBox_.DisplayMember = "name_";

            dateRangeComboBox_.DataSource = App.dateRangeSettingList_;
            dateRangeComboBox_.DisplayMember = "name_";

            string sSolution = Utils.GetSysInfo(DB.Global(), "NewRegressForm.solution");
            if (sSolution != "")
            {
                slnComboBox_.SelectedItem = App.Solution(sSolution);
            }
            string sDateRange = Utils.GetSysInfo(DB.Global(), "NewRegressForm.daterange");
            if (sDateRange != "")
            {
                dateRangeComboBox_.SelectedItem = App.DateRange(sDateRange);
            }
            string sMode = Utils.GetSysInfo(DB.Global(), "NewRegressForm.Mode");
            if (sMode == "Asset")
            {
                radioAsset.Checked = true;
            }
            else
            {
                radioRaw.Checked = true;
            }
        }
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (slnComboBox_.SelectedIndex == -1)
            {
                MessageBox.Show("Must select Solution!", "Selector");
                this.DialogResult = DialogResult.None;
                return;
            }
            if (dateRangeComboBox_.SelectedIndex == -1)
            {
                MessageBox.Show("Must select DateRange!", "Selector");
                this.DialogResult = DialogResult.None;
                return;
            }
            name_.Text = name_.Text.Trim();
            if (String.IsNullOrEmpty(name_.Text))
            {
                MessageBox.Show("name must be provided!");
                this.DialogResult = DialogResult.None;
                return;
            }
            foreach (var re in App.regressList_)
            {
                if (String.Equals(re.name_,name_.Text ,StringComparison.CurrentCultureIgnoreCase))
                {
//                if (DialogResult.Yes == MessageBox.Show("Same name regress already exists, remove it and regress?", "Selector", MessageBoxButtons.YesNo))
                    {
                        foreach (var formName in re.AllFormNames)
                        {
                            MainForm.Me.removeForm(formName);
                        }
                        App.RemoveRegress(re);
                        break;
                    }
//                else
//                {
//                    this.DialogResult = DialogResult.None;
//                    return;   
//                }
                }
            }
            List<IStrategy> strategyList = App.Solution(slnComboBox_.SelectedItem.ToString()).straList_;
            String sMode = radioAsset.Checked ? "Asset" : "Raw";
            if (sMode == "Asset")
            {
                foreach (var stra in strategyList)
                {
                    if (App.asset_.straData(stra.name()) == null)
                    {
                        MessageBox.Show(String.Format("Strategy: {0} has no asset, can't run in asset mode!", stra.name()), "Selector");
                        this.DialogResult = DialogResult.None;
                        return;         
                    }
                }
            }
            else
            {
                if (strategyList.Count != 1)
                {
                    MessageBox.Show("Raw mode only support single strategy solution!", "Selector");
                    this.DialogResult = DialogResult.None;
                    return;
                }
            }

            re_ = new RegressResult();
            re_.runMode_ = sMode == "Asset" ? RunMode.RM_Asset : RunMode.RM_Raw;
            re_.name_ = name_.Text;
            re_.solutionName_ = slnComboBox_.SelectedItem.ToString();
            re_.dateRangeName_ = dateRangeComboBox_.SelectedItem.ToString();
            re_.dateRangeList_ = App.DateRange(re_.dateRangeName_).rangeList_;
            re_.strategyList_ = App.Solution(re_.solutionName_).straList_;
            Utils.SetSysInfo(DB.Global(), "NewRegressForm.solution", slnComboBox_.SelectedItem.ToString());
            Utils.SetSysInfo(DB.Global(), "NewRegressForm.daterange", dateRangeComboBox_.SelectedItem.ToString());
            Utils.SetSysInfo(DB.Global(), "NewRegressForm.Mode", sMode);
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            Close();
        }

        void autoSetName()
        {
            if (slnComboBox_.SelectedItem == null ||
                dateRangeComboBox_.SelectedItem == null)
            {
                return;
            }
            SolutionSetting slnSetting = (SolutionSetting)slnComboBox_.SelectedItem;
            DateRangeSetting dateRangeSetting = (DateRangeSetting)dateRangeComboBox_.SelectedItem;
            name_.Text = String.Join("-",  slnSetting.name_, dateRangeSetting.name_);
        }

        private void NewRegressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.None)
                e.Cancel = true;
            base.OnClosing(e);
        }

        private void slnComboBox__SelectedIndexChanged(object sender, EventArgs e)
        {
            autoSetName();
        }

        private void dateRangeComboBox__SelectedIndexChanged(object sender, EventArgs e)
        {
            autoSetName();
        }

        private void buyerComboBox__SelectedIndexChanged(object sender, EventArgs e)
        {
            autoSetName();
        }

    }
}
