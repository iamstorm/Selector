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
    public partial class RegressBuyForm : Form
    {
        public RegressResult re_;
        public RegressBuyForm()
        {
            InitializeComponent();

            LUtils.InitListView(mainListView_);
            LUtils.InitListView(subListView_);
        }
        public ListView buyItemListView()
        {
            return mainListView_;
        }
        public ListView daySelectListView()
        {
            return subListView_;
        }

        private void mainListView__RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = LUtils.RetrieveVirtualItem(mainListView_, e.ItemIndex, re_.buyItems_);
        }
    }
}
