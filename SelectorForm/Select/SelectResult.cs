using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace SelectorForm
{
    public class SelectItem
    {
        public int date_;
        public String code_;
        public String strategyName_;
        public String[] rateItems_;
        public String rateItemKey_;
        public int rate_;
        public String bonus_;
    }
    public class BuyItem
    {
        public int date_;
        public String code_;
        public SelectItem bySelectItem_;
    }
    public class SelectResult
    {
        public List<SelectItem> selItems_ =  new List<SelectItem>();
        public static void InitGrid(DataGridView grid)
        {
            grid.Columns.Clear();
            String[] colNameArr = new String[] {
                "code", "name", "zf", "close", "strategy", "rate", "hscount", "rateKey"
            };
            for (int i = 0; i < colNameArr.Length; i++)
            {
                DataGridViewColumn col = new DataGridViewTextBoxColumn();
                col.Name = colNameArr[i];
                col.HeaderText = colNameArr[i];
                grid.Columns.Add(col);
            }

            //采用虚模式来填充数据   
            grid.VirtualMode = true;

            foreach (DataGridViewColumn column in grid.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
        public static void GridCellValueNeeded(DataGridView grid, List<SelectItem> selItems, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex == grid.RowCount)
                return;
            string colName = grid.Columns[e.ColumnIndex].Name;
            SelectItem item = selItems[e.RowIndex];
            Stock stock = App.ds_.sk(item.code_);
            StrategyData straData = App.asset_.straData(item.strategyName_);
            if (colName == "code")
            {
                e.Value = item.code_;
            }
            else if (colName == "name")
            {
                e.Value = stock.name_;
            }
            else if (colName == "zf")
            {
                e.Value = stock.zfSee(item.date_);
                if (stock.zf(item.date_) > 0)
                {
                    grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Red;
                }
                else
                {
                    grid.Rows[e.RowIndex].DefaultCellStyle.ForeColor = Color.Green;
                }
            }
            else if (colName == "close")
            {
                e.Value = App.ds_.realVal(Info.C, item.code_, item.date_);
            }
            else if (colName == "strategy")
            {
                e.Value = item.strategyName_;
            }
            else if (colName == "rate")
            {
                e.Value = item.rate_;
            }
            else if (colName == "hscount")
            {
                e.Value = straData == null ? 0 : straData.selectCount_;
            }
            else if (colName == "rateKey")
            {
                e.Value = item.rateItemKey_;
            }
        }
    }
}
