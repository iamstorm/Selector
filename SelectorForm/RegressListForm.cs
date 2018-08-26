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
    public partial class RegressListForm : Form
    {
        public RegressListForm()
        {
            InitializeComponent();

            regressGrid.Columns.Clear();
            String[] colNameArr = new String[] {
                "Name", "Solution", "DateRange", "BuyDesider", "TotalBonus"
            };
            for (int i = 0; i < colNameArr.Length; i++)
            {
                DataGridViewColumn col = new DataGridViewTextBoxColumn();
                col.Name = colNameArr[i];
                col.HeaderText = colNameArr[i];
                regressGrid.Columns.Add(col);
            }

            foreach (DataGridViewColumn column in regressGrid.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            regressGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            App.regressList_.Sort(delegate(RegressResult lhs, RegressResult rhs)
            {
                return lhs.name_.CompareTo(rhs.name_);
            });
            foreach (var re in App.regressList_)
            {
                String totalBonus = re.TotalBonus;
                int iRowIndex = regressGrid.Rows.Add();
                regressGrid.Rows[iRowIndex].Cells["Name"].Value = re.name_;
                regressGrid.Rows[iRowIndex].Cells["Solution"].Value = re.solutionName_;
                regressGrid.Rows[iRowIndex].Cells["DateRange"].Value = re.dateRangeName_;
                regressGrid.Rows[iRowIndex].Cells["TotalBonus"].Value = totalBonus;
                if (Utils.GetBonusValue(totalBonus) > 0)
                {
                    regressGrid.Rows[iRowIndex].Cells["TotalBonus"].Style.BackColor = Color.Red;
                    regressGrid.Rows[iRowIndex].Cells["TotalBonus"].Style.ForeColor = Color.White;
                }
                else
                {
                    regressGrid.Rows[iRowIndex].Cells["TotalBonus"].Style.BackColor = Color.Green;
                    regressGrid.Rows[iRowIndex].Cells["TotalBonus"].Style.ForeColor = Color.White;
                }
            }
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            if (regressGrid.SelectedRows.Count == 0)
	        {
		        MessageBox.Show("Please select row first!");
                return;
	        }

            foreach (DataGridViewRow row in regressGrid.SelectedRows)
            {
                var ret = from re in App.regressList_
                          where re.name_ == row.Cells["Name"].Value.ToString()
                          select re;
                var regressRe = ret.First();
                foreach (var formName in regressRe.AllFormNames)
                {
                    MainForm.Me.removeForm(formName);
                }
                App.RemoveRegress(regressRe);
                regressGrid.Rows.Remove(row);
            }
            Close();
        }

        void showSelectRegress(bool bShow)
        {
            if (regressGrid.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select row first!");
                return;
            }

            foreach (DataGridViewRow row in regressGrid.SelectedRows)
            {
                var ret = from re in App.regressList_
                          where re.name_ == row.Cells["Name"].Value.ToString()
                          select re;

                var regressRe = ret.First();
                foreach (var formName in regressRe.AllFormNames)
                {
                    MainForm.Me.showForm(formName, bShow);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            showSelectRegress(false);
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            showSelectRegress(true);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void regressGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            GUtils.UpdateGridRowNum(regressGrid);
        }

        private void regressGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            GUtils.UpdateGridRowNum(regressGrid);
        }
    }
}
