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
        public RegressSelectForm()
        {
            InitializeComponent();

            GUtils.InitGrid(mainGrid);
            GUtils.InitGrid(subGrid);
        }
        public DataGridView selectItemGrid()
        {
            return mainGrid;
        }
        public DataGridView daySelectGrid()
        {
            return subGrid;
        }

        private void mainGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            GUtils.UpdateGridRowNum(mainGrid);
        }

        private void mainGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            GUtils.UpdateGridRowNum(mainGrid);
        }

        private void subGrid_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            GUtils.UpdateGridRowNum(subGrid);
        }

        private void subGrid_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            GUtils.UpdateGridRowNum(subGrid);
        }

        private void mainGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            GUtils.GridCellValueNeeded(mainGrid, re_.selItems_, e);
        }

        private void subGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {

        }
    }
}
