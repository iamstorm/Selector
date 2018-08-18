using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Text.RegularExpressions;

namespace SelectorForm
{
    public partial class MainForm : Form
    {
        public static MainForm Me;
        public SelectResult reSelect_;
        public BuyItem buyItem_;
        public bool isBusy_;
        public MainForm()
        {
            Me = this;
            InitializeComponent();

//             string sTest = "\r正在创建000001的表总进度[                    ] 0.00%";
//             Match m = Regex.Match(sTest, "^\\r(.*)总进度\\[.*\\](.*)$");
//             if (m.Length > 0)
//             {
//                 string sMsg1 = m.Groups[1].Value;
//                 string sMsg2 = m.Groups[2].Value;
//             }
//             return;

            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.Visible = false;
            msgText.Text = "";
            tradedayText.Text = "";

            selectGrid_.Columns.Add("code", "code");
            selectGrid_.Columns.Add("name", "name");
            selectGrid_.Columns.Add("zf", "zf");
            selectGrid_.Columns.Add("close", "close");
            selectGrid_.Columns.Add("strategy", "strategy");
            selectGrid_.Columns.Add("rate", "rate");
            selectGrid_.Columns.Add("hscount", "hscount");
            selectGrid_.Columns.Add("rateKey", "rateKey");

            foreach (DataGridViewColumn column in this.selectGrid_.Columns)
            {
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
            selectGrid_.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            Dist.Setup();
        }
        public void startProcessBar()
        {
            Action action;
            Invoke(action = () =>
                {
                    progressBar.Value = 0;
                    progressBar.Visible = true;
                });
        }
        public void setProcessBar(String msgIn, float percentIn)
        {
            Action<String, float> action;
            Invoke(action = (msg, percent) =>
            {
                progressBar.Value = 0;
                progressBar.Visible = true;
                msgText.Text = String.Format("{0}...{1}%", msg, percent.ToString("F2"));
                progressBar.Value = (int)percent;
                msgText.Update();
            }, msgIn, percentIn);
        }
        public void finishProcessBar()
        {
            Action action;
            Invoke(action = () =>
            {
                msgText.Text = "";
                progressBar.Value = 100;
                progressBar.Visible = false;
                msgText.Update();
            });

        }
        public void setMsg(string msgIn)
        {
            Action<string> action;
            Invoke(action = (msg) =>
            {
                msgText.Text = msg;
                msgText.Update();
            }, msgIn);
        }
        public void setTradeDay()
        {
            Action action;
            Invoke(action = () =>
                {
                    tradedayText.Text = Utils.IsTradeDay() ? "is tradeday" : "not tradeday";
                    tradedayText.Update();
                });
        }
        private void newRegressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isBusy_)
            {
                MessageBox.Show("正忙，请稍微。");
                return;
            }
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isBusy_)
            {
                MessageBox.Show("正忙，请稍微。");
                return;
            }
            selectWorker.RunWorkerAsync();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            initWorker.RunWorkerAsync();
        }

        private void initWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            isBusy_ = true;
            if (!App.ds_.start())
            {
                MessageBox.Show("初始化失败！");
                Close();
                return;
            }
            isBusy_ = false;
        }

        private void selectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            isBusy_ = true;
            if (!App.ds_.prepareForSelect())
            {
                MessageBox.Show("准备数据工作失败，无法继续执行！");
                isBusy_ = false;
                return;
            }
            SelectManager manager = new SelectManager();
            reSelect_ = manager.selectNow();
            buyItem_ = App.grp_.makeDeside(reSelect_.selItems_);
            Action action = () =>
            {
                selectGrid_.Rows.Clear();
                foreach (SelectItem item in reSelect_.selItems_)
                {
                    List<Data> listData = App.ds_.listData(item.code_);
                    Stock stock = App.ds_.sk(item.code_);
                    StrategyData straData = App.asset_.straData(item.strategyName_);
                    DataGridViewRow row = new DataGridViewRow();
                    int rowIndex = selectGrid_.Rows.Add();
                    selectGrid_.Rows[rowIndex].Cells[0].Value = item.code_;
                    selectGrid_.Rows[rowIndex].Cells[1].Value = stock.name_;
                    selectGrid_.Rows[rowIndex].Cells[2].Value = stock.zfSee(item.date_);
                    selectGrid_.Rows[rowIndex].Cells[3].Value = App.ds_.realVal(Info.C, item.code_, item.date_);
                    selectGrid_.Rows[rowIndex].Cells[4].Value = item.strategyName_;
                    selectGrid_.Rows[rowIndex].Cells[5].Value = straData == null ? 0 : straData.selectCount_;
                    selectGrid_.Rows[rowIndex].Cells[6].Value = item.rateItemKey_;
                    if (stock.zf(item.date_) > 0)
                    {
                        selectGrid_.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Red;
                    }
                    else
                    {
                        selectGrid_.Rows[rowIndex].DefaultCellStyle.ForeColor = Color.Green;
                    }
                }
            };
            Invoke(action);
            isBusy_ = false;
        }

        void updateSelectGridRowNum()
        {
            for (int i = 0; i < selectGrid_.Rows.Count; i++)
            {
                selectGrid_.Rows[i].HeaderCell.Value = (i+1).ToString();
            }
        }

        private void selectGrid__RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            updateSelectGridRowNum();
        }

        private void selectGrid__RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            updateSelectGridRowNum();
        }
    }
}
