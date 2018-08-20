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
        public static void InitGrid(DataGridView grid)
        {
            grid.Columns.Clear();
            String[] colNameArr = SelectItem.ShowColumnList;
            for (int i = 0; i < colNameArr.Length; i++)
            {
                DataGridViewColumn col = new DataGridViewTextBoxColumn();
                col.Name = colNameArr[i];
                col.HeaderText = colNameArr[i];
                grid.Columns.Add(col);
            }

            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
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
        public static void FillGridData(DataGridView grid, List<SelectItem> selItems)
        {
            RemoveAllGridRow(grid);
            if (selItems.Count > 300)
            {
                grid.VirtualMode = true;
                grid.RowCount = selItems.Count;
            }
            else
            {
                grid.VirtualMode = false;
                for (int i = 0; i < selItems.Count; i++)
                {
                    SelectItem item = selItems[i];
                    Stock stock = item.code_ == null ? null : App.ds_.sk(item.code_);
                    StrategyData straData = App.asset_.straData(item.strategyName_);

                    int iRowIndex = grid.Rows.Add();
                    String[] colNameArr = SelectItem.ShowColumnList;
                    foreach (String colName in colNameArr)
                    {
                        grid.Rows[iRowIndex].Cells[colName].Value = item.getCellValue(grid.Rows[iRowIndex], colName, stock, straData);
                    }
                }
            }
        }
        public static void GridCellValueNeeded(DataGridView grid, List<SelectItem> selItems, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex == grid.RowCount)
                return;
            string colName = grid.Columns[e.ColumnIndex].Name;
            SelectItem item = selItems[e.RowIndex];
            Stock stock = item.code_ == null ? null : App.ds_.sk(item.code_);
            StrategyData straData = App.asset_.straData(item.strategyName_);

            e.Value = item.getCellValue(grid.Rows[e.RowIndex], colName, stock, straData);
        }

    }
}
