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

            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.Visible = false;
            msgText.Text = "";
            tradedayText.Text = "";

            SelectResult.InitGrid(selectGrid_);

            Dist.Setup();
        }
        public void uiStartProcessBar()
        {
            Action action;
            Invoke(action = () =>
                {
                    progressBar.Value = 0;
                    progressBar.Visible = true;
                });
        }
        public void uiSetProcessBar(String msgIn, float percentIn)
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
        public void uiFinishProcessBar()
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
        public void uiSetMsg(string msgIn)
        {
            Action<string> action;
            Invoke(action = (msg) =>
            {
                msgText.Text = msg;
                msgText.Update();
            }, msgIn);
        }
        public void uiSetTradeDay()
        {
            Action action;
            Invoke(action = () =>
                {
                    tradedayText.Text = Utils.IsTradeDay() ? "is tradeday" : "not tradeday";
                    tradedayText.Update();
                });
        }
        public Form showTabPage(string name, Form form)
        {
            for (int i = 0; i < mainTab.TabPages.Count; i++)
            {
                if (mainTab.TabPages[i].Name == name)
                {
                    mainTab.SelectedTab = mainTab.TabPages[i];
                    if (name == "TabSelect")
                    {
                        return this;
                    }
                    else
                    {
                        return (Form)mainTab.TabPages[i].Controls[0];
                    }
                }
            }
            TabPage page = new TabPage(name);
            page.Name = name;
            form.TopLevel = false;
            page.Controls.Add(form);
            mainTab.TabPages.Add((page));
            mainTab.SelectedTab = page;
            form.Show();
            return form;
        }
        public Form queryForm(string name)
        {
            if (name == "TabSelect")
            {
                return this;
            }
            for (int i = 0; i < mainTab.TabPages.Count; i++)
            {
                if (mainTab.TabPages[i].Name == name)
                {
                    return (Form)mainTab.TabPages[i].Controls[0];
                }
            }
            return null;
        }
        private void MainForm_Shown(object sender, EventArgs e)
        {
            startWorker.RunWorkerAsync();
        }

        private void startWorker_DoWork(object sender, DoWorkEventArgs e)
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

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            endWorker.RunWorkerAsync();
        }
        private void endWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            isBusy_ = true;
            if (!App.ds_.end())
            {
                MessageBox.Show("结束时做整理工作失败！");
            }
            isBusy_ = false;
        }
        private void selectGrid__RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            Utils.UpdateGridRowNum(selectGrid_);
        }

        private void selectGrid__RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            Utils.UpdateGridRowNum(selectGrid_);
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
            isBusy_ = false;
        }

        private void selectWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            selectGrid_.RowCount = reSelect_.selItems_.Count ;
            showTabPage("TabSelect", null);
        }
        private void newRegressToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void regressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isBusy_)
            {
                MessageBox.Show("正忙，请稍微。");
                return;
            }
            regressWorker.RunWorkerAsync();
        }

        private void regressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                isBusy_ = true;
                RegressManager regressManager = new RegressManager();
                RegressResult re = regressManager.regress("test", 20050101, 20180817);
                e.Result = re;
                isBusy_ = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("执行发生异常：{0}", ex.Message));
                throw;
            }
        }

        private void regressWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            RegressSelectForm selectform = (RegressSelectForm)showTabPage("RegressSelect", new RegressSelectForm());
            RegressBuyForm buyform = (RegressBuyForm)showTabPage("RegressBuy", new RegressBuyForm());
            RegressResult re = (RegressResult)e.Result;
            selectform.re_ = buyform.re_ = re;
            selectform.selectItemGrid().RowCount = re.selItems_.Count;

        }

        private void selectGrid__CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            SelectResult.GridCellValueNeeded(selectGrid_, reSelect_.selItems_, e);
        }



    }
}
