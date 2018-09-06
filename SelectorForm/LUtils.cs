using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SelectImpl;
using System.Drawing;
using System.Collections;

namespace SelectorForm
{
    class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder order;
        public ListViewItemComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }
        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            int returnVal = -1;
            float a = 0, b = 0;
            ListViewItem xlvi = (ListViewItem)x;
            ListViewItem ylvi = (ListViewItem)y;
            string xStr = xlvi.SubItems[col].Text.TrimEnd('%');
            string yStr = ylvi.SubItems[col].Text.TrimEnd('%');
            if (float.TryParse(xStr, out a) && float.TryParse(yStr, out b))
            {
                returnVal = a >= b ? (a == b ? 0 : 1) : -1;
                if (order == SortOrder.Descending)
                {
                    returnVal *= -1;
                }
            }
            else
            {
                returnVal = String.Compare(xStr, yStr);
                // Determine whether the sort order is descending.
                if (order == SortOrder.Descending)
                {
                    // Invert the value returned by String.Compare.
                    returnVal *= -1;
                }
            }
            return returnVal;
        }
    }
    public static class LUtils
    {
        public static void InitListView(ListView lv, ColumnInfo[] infos)
        {
            // 设置行高
            ImageList imgList = new ImageList();
            // 分别是宽和高
            imgList.ImageSize = new Size(1, 30);
            // 这里设置listView的SmallImageList ,用imgList将其撑大
            lv.SmallImageList = imgList;
            foreach (var info in infos)
            {
                lv.Columns.Add(info.name_, info.width_).TextAlign = HorizontalAlignment.Center;
            }
            lv.FullRowSelect = true;
            lv.GridLines = true;
            lv.AllowColumnReorder = true;
            lv.View = View.Details;
            lv.HeaderStyle = ColumnHeaderStyle.Clickable;
            lv.MultiSelect = false;
            lv.HideSelection = false;
        }
        public static void InitItemListView(ListView lv)
        {
            lv.Columns.Add("num", 50);
            InitListView(lv, SelectItem.ShowColumnInfos);
        }
        public static void RemoveAllListRow(ListView lv)
        {
            if (lv.VirtualMode == true)
            {
                lv.VirtualListSize = 0;
            }
            else
            {
                lv.Items.Clear();
            }
        }
        public static void FillListViewData(ListView lv, List<SelectItem> selItems)
        {
            RemoveAllListRow(lv);
            if (selItems.Count > 1000)
            {
                lv.VirtualMode = true;
                lv.VirtualListSize = selItems.Count;
            }
            else
            {
                lv.VirtualMode = false;
                for (var i = 0; i < selItems.Count; i++)
                {
                    lv.Items.Add(selItems[i].toListViewItem(lv, i, selItems.Count));
                }
            }
        }
        public static ListViewItem RetrieveVirtualItem(ListView lv, int iItemIndex, List<SelectItem> selItems)
        {
            return selItems[iItemIndex].toListViewItem(lv, iItemIndex, selItems.Count);
        }
        public static void OnColumnClick(ListView lv, int clickColumn, ref int sortColumn)
        {
            if (lv.VirtualMode)
            {
                return;
            }
            if (clickColumn != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = clickColumn;
                // Set the sort order to ascending by default.
                lv.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (lv.Sorting == SortOrder.Ascending)
                {
                    lv.Sorting = SortOrder.Descending;
                }
                else
                {
                    lv.Sorting = SortOrder.Ascending;
                }
            }
            // Set the ListViewItemSorter property to a new ListViewItemComparer
            // object.
            lv.ListViewItemSorter = new ListViewItemComparer(clickColumn, lv.Sorting);
            // Call the sort method to manually sort.
            lv.Sort();
        }
    }
}
