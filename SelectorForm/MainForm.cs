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
using System.Diagnostics;

namespace SelectorForm
{
    public partial class MainForm : Form, IHost
    {
        public static MainForm Me;
        public SelectResult reSelect_;
        public SelectItem buyItem_;
        public RegressResult regressingRe_;
        public bool isBusy_;
        public List<TabPage> hideTabPages_ = new List<TabPage>();
        public Stopwatch runWatch_ = new Stopwatch();
        public bool isClosing_;
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
            LUtils.InitListView(selectListView_);

            Dist.Setup();
        }
        void IHost.uiStartProcessBar()
        {
            if (isClosing_)
            {
                return;
            }
            Action action;
            Invoke(action = () =>
                {
                    progressBar.Value = 0;
                    progressBar.Visible = true;
                });
        }
        void IHost.uiSetProcessBar(String msgIn, float percentIn)
        {
            if (isClosing_)
            {
                return;
            }
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
            if (isClosing_)
            {
                return;
            }
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
            if (isClosing_)
            {
                return;
            }
            Action<string> action;
            Invoke(action = (msg) =>
            {
                msgText.Text = msg;
                msgText.Update();
            }, msgIn);
        }
        void IHost.uiSetTradeDay()
        {
            if (isClosing_)
            {
                return;
            }
            Action action;
            Invoke(action = () =>
                {
                    tradedayText.Text = Utils.NowIsTradeDay() ? "is tradeday" : "not tradeday";
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
                for (int i = 0; i < hideTabPages_.Count; i++)
                {
                    if (hideTabPages_[i].Name == name)
                    {
                        hideTabPages_[i].Parent = mainTab;
                        mainTab.SelectedTab = hideTabPages_[i];
                        hideTabPages_.RemoveAt(i);
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
                        hideTabPages_.Add(mainTab.TabPages[i]);
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
            runWatch_.Start();
            if (!App.ds_.start())
            {
                MessageBox.Show("初始化失败！");
                Close();
                return;
            }
            ((IHost)this).uiSetMsg("UseTime: " + Utils.ReportWatch(runWatch_));
            isBusy_ = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosing_ = true;
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
        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isBusy_)
            {
                MessageBox.Show("正忙，请稍微。");
                return;
            }
            LUtils.RemoveAllListRow(selectListView_);
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
                buyItem_ = App.grp_.makeDeside(reSelect_.selItems_, Utils.NowDate());
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
                LUtils.RemoveAllListRow(selectListView_);
            }
            else
            {
                LUtils.FillListViewData(selectListView_, reSelect_.selItems_);
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
                if (!App.ds_.prepareForSelect())
                {
                    MessageBox.Show("准备数据工作失败，无法继续执行！");
                    isBusy_ = false;
                    return;
                }
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
                LUtils.FillListViewData(selectform.selectItemListView(), re.selItems_);
                LUtils.FillListViewData(buyform.buyItemListView(), re.buyItems_);
                regressingRe_ = null;
                App.regressList_.Add(re);
            }

        }

        private void regressLisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RegressListForm form = new RegressListForm();
            form.ShowDialog();
        }

        private void selectList__RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            e.Item = LUtils.RetrieveVirtualItem(selectListView_, e.ItemIndex, reSelect_.selItems_);
        }
    }
}
