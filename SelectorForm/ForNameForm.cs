using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SelectorForm
{
    public partial class ForNameForm : Form
    {
        public String name_;
        public List<String> allreadyExistNameList_ = new List<String>();
        public ForNameForm()
        {
            InitializeComponent();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            nameTextBox_.Text = nameTextBox_.Text.Trim();
            if (String.IsNullOrEmpty(nameTextBox_.Text))
            {
                MessageBox.Show("Name is Empty!", "Selector");
                this.DialogResult = DialogResult.None;
                return;     
            }
            if (allreadyExistNameList_.Contains(nameTextBox_.Text))
            {
                MessageBox.Show("Name allready exists!", "Selector");
                this.DialogResult = DialogResult.None;
                return;
            }
            name_ = nameTextBox_.Text;
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void ForNameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.None)
                e.Cancel = true;
            base.OnClosing(e);
        }
    }
}
