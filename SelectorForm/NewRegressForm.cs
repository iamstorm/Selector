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
        public NewRegressForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterParent;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (startDate.Value > endDate.Value)
            {
                MessageBox.Show("StartDate must before or equal endDate!");
                return;
            }
            name.Text = name.Text.Trim();
            if (String.IsNullOrEmpty(name.Text))
            {
                MessageBox.Show("name must be provided!");
                return;
            }
            foreach (var re in App.regressList_)
            {
                if (String.Equals(re.name_,name.Text ,StringComparison.CurrentCultureIgnoreCase))
                {
                    MessageBox.Show("name already exists!");
                    return;   
                }
            }
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
            name.Text = String.Join("-", Utils.Date(startDate.Value), Utils.Date(endDate.Value));
        }

        private void startDate_ValueChanged(object sender, EventArgs e)
        {
            autoSetName();
        }

        private void endDate_ValueChanged(object sender, EventArgs e)
        {
            autoSetName();
        }
    }
}
