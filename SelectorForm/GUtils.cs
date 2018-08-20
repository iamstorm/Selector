using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SelectImpl;
using System.Drawing;

namespace SelectorForm
{
    public static class GUtils
    {
        public static void UpdateGridRowNum(DataGridView grid)
        {
            for (int i = 0; i < grid.Rows.Count; i++)
            {
                grid.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
            grid.TopLeftHeaderCell.Value = grid.Rows.Count.ToString() + "Rows";
        }
        public static void RemoveAllGridRow(DataGridView grid)
        {
            if (grid.VirtualMode == true)
            {
                grid.RowCount = 0;
            }
            else
            {
                grid.Rows.Clear();
            }
        }
    
    }
}
