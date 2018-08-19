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
using SelectImpl;

namespace SelectorForm
{
    public partial class MainForm : Form, IHost
    {
        public static MainForm Me;
        public SelectResult reSelect_;
        public SelectItem buyItem_;
        public RegressResult regressingRe_;
        public bool isBusy_;
        public List<TabPage> hideTabPages = new List<TabPage>(); 
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

            App.host_ = this;
            GUtils.InitGrid(selectGrid_);

            Dist.Setup();
        }
        void IHost.uiStartProcessBar()
        {
            Action action;
            Invoke(action = () =>
                {
                    progressBar.Value = 0;
                    progressBar.Visible = true;
                });
        }
        void IHost.uiSetProcessBar(String msgIn, float percentIn)
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
        void IHost.uiFinishProcessBar()
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
        void IHost.uiSetMsg(string msgIn)
        {
            Action<string> action;
            Invoke(action = (msg) =>
            {
                msgText.Text = msg;
                msgText.Update();
            }, msgIn);
        }
        void IHost.uiSetTradeDay()
        {
            Action action;
            Invoke(action = () =>
                {
                    tradedayText.Text = Utils.IsTradeDay() ? "is tradeday" : "not tradeday";
                    tradedayText.Update();
                });
        }
        public Form createTabPage(string name, Form form)
        {
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
        public void removeForm(string name)
        {
            if (name == "TabSelect")
            {
                return;
            }
            for (int i = 0; i < mainTab.TabPages.Count; i++)
            {
                if (mainTab.TabPages[i].Name == name)
                {
                    mainTab.TabPages.RemoveAt(i);
                    return;
                }
            }
        }
        public void showForm(string name, bool bShow=true)
        {
            if (bShow)
            {
                for (int i = 0; i < hideTabPages.Count; i++)
                {
                    if (hideTabPages[i].Name == name)
                    {
                        hideTabPages[i].Parent = mainTab;
                        mainTab.SelectedTab = hideTabPages[i];
                        hideTabPages.RemoveAt(i);
                        break;
                    }
                }
                for (int i = 0; i < mainTab.TabPages.Count; i++)
                {
                    if (mainTab.TabPages[i].Name == name)
                    {
                        mainTab.SelectedTab = mainTab.TabPages[i];
                        return;
                    }
                }
            }
            else
            {
                for (int i = 0; i < mainTab.TabPages.Count; i++)
                {
                    if (mainTab.TabPages[i].Name == name)
                    {
                        hideTabPages.Add(mainTab.TabPages[i]);
                        mainTab.TabPages[i].Parent = null;
                        return;
                    }
                }
            }
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
            GUtils.UpdateGridRowNum(selectGrid_);
        }

        private void selectGrid__RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            GUtils.UpdateGridRowNum(selectGrid_);
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
            try
            {
                if (!App.ds_.prepareForSelect())
                {
                    MessageBox.Show("准备数据工作失败，无法继续执行！");
                    isBusy_ = false;
                    return;
                }
                SelectManager manager = new SelectManager();
                reSelect_ = manager.selectNow();
                buyItem_ = App.grp_.makeDeside(reSelect_.selItems_);
            }
            catch (Exception ex)
            {
                reSelect_ = null;
                buyItem_ = null;
                MessageBox.Show(String.Format("执行发生异常：{0}", ex.Message));
                isBusy_ = false;
                throw;
            }
            isBusy_ = false;
        }

        private void selectWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (reSelect_ == null)
            {
                GUtils.RemoveAllGridRow(selectGrid_);
            }
            else
            {
                GUtils.FillGridData(selectGrid_, reSelect_.selItems_);
            }
            showForm("TabSelect");
        }

        private void regressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isBusy_)
            {
                MessageBox.Show("正忙，请稍微。");
                return;
            }
            NewRegressForm form = new NewRegressForm();
            if (form.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            regressingRe_ = new RegressResult();
            regressingRe_.name_ = form.name.Text;
            regressingRe_.startDate_ = Utils.Date(form.startDate.Value);
            regressingRe_.endDate_ = Utils.Date(form.endDate.Value);

            regressWorker.RunWorkerAsync();
        }

        private void regressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            isBusy_ = true;
            try
            {
                RegressManager regressManager = new RegressManager();
                RegressResult re = regressManager.regress(regressingRe_.name_,
                    regressingRe_.startDate_, regressingRe_.endDate_);
                e.Result = re;
            }
            catch (Exception ex)
            {
                e.Result = null;
                MessageBox.Show(String.Format("执行发生异常：{0}", ex.Message));
                isBusy_ = false;
                throw;
            }
            isBusy_ = false;
        }

        private void regressWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result == null)
            {
                regressingRe_ = null;
                return;
            } else
            {
                RegressResult re = (RegressResult)e.Result;
                re.name_ = regressingRe_.name_;
                RegressSelectForm selectform = (RegressSelectForm)createTabPage(re.name_+"_Select", new RegressSelectForm());
                RegressBuyForm buyform = (RegressBuyForm)createTabPage(re.name_ + "_Buy", new RegressBuyForm());
                selectform.re_ = buyform.re_ = re;
                GUtils.FillGridData(selectform.selectItemGrid(), re.selItems_);
                GUtils.FillGridData(buyform.buyItemGrid(), re.buyItems_);
                regressingRe_ = null;
                App.regressList_.Add(re);
            }

        }

        private void selectGrid__CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            GUtils.GridCellValueNeeded(selectGrid_, reSelect_.selItems_, e);
        }

        private void regressLisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegressListForm form = new RegressListForm();
            form.ShowDialog();
        }



    }
}
