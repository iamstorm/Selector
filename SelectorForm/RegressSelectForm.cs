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
    public partial class RegressSelectForm : Form
    {
        public RegressResult re_;
        public List<SelectItem> daySelectItems_;
        public int mainListViewSortColumn_;
        public int subListViewSortColumn_;
        public RegressSelectForm()
        {
            InitializeComponent();

            LUtils.InitItemListView(mainListView_);
            LUtils.InitItemListView(subListView_);
        }
        public ListView selectItemListView()
        {
            return mainListView_;
        }
        public ListView daySelectListView()
        {
            return subListView_;
        }

        private void mainListView__RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = LUtils.RetrieveVirtualItem(mainListView_, e.ItemIndex, re_.selItems_);
        }

        private void mainListView__ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                int selDate = Utils.ToType<int>(e.Item.SubItems[1].Text);
                if (daySelectItems_ != null && daySelectItems_[0].date_ == selDate)
                {
                    return;
                }
                daySelectItems_ = SelectResult.OfDate(selDate, re_.selItems_);
                daySelectItems_.Sort(delegate(SelectItem lhs, SelectItem rhs)
                {
                    if (re_.buyItems_.Contains(lhs))
                    {
                        return -1;
                    }
                    if (re_.buyItems_.Contains(rhs))
                    {
                        return -1;
                    }
                    return rhs.getColumnVal("bonus").CompareTo(lhs.getColumnVal("bonus"));
                });
                LUtils.FillListViewData(subListView_, daySelectItems_);
            }
            else
            {
                LUtils.RemoveAllListRow(subListView_);
                daySelectItems_ = null;
            }
        }

        private void subListView__RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = LUtils.RetrieveVirtualItem(subListView_, e.ItemIndex, daySelectItems_);
        }

        private void mainListView__ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LUtils.OnColumnClick(mainListView_, e.Column, ref mainListViewSortColumn_);
        }

        private void subListView__ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LUtils.OnColumnClick(subListView_, e.Column, ref subListViewSortColumn_);
        }

    }
}
