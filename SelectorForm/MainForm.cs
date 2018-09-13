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
using System.Reflection;

namespace SelectorForm
{
    public partial class MainForm : Form, IHost
    {
        public Sunisoft.IrisSkin.SkinEngine skinEngine_;
        public static MainForm Me;
        public SelectResult reSelect_;
        public RegressResult regressingRe_;
        public bool isBusy_;
        public List<TabPage> hideTabPages_ = new List<TabPage>();
        public Stopwatch runWatch_ = new Stopwatch();
        public bool isClosing_;
        public int sortColumn_;
        DateTime startupTime_;
        bool bHasAutoSelect_ = false;
        bool bAutoSelectMode_ = false;
        bool bHasSendSms_ = false;
        bool bHasReportAutoSelectModeError_ = false;
        public MainForm()
        {
            Me = this;
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized; 

            var skinName = Utils.GetSysInfo(DB.Global(), "SkinName", "");
            if (skinName != "")
            {
                skinEngine_ = new Sunisoft.IrisSkin.SkinEngine();
                MainForm.Me.skinEngine_.SkinFile = Path.Combine(Dist.binPath_, "Skins", skinName);
            }

            msgText.Text = "";
            toolStripStatusLabel1_.Text = Setting.DebugMode ? "Debug" : "Release";
            toolStripStatusLabel2_.Text = "";

            App.host_ = this;
            LUtils.InitItemListView(selectListView_);

            startupTime_ = DateTime.Now;
            timer_.Start();
            DB.Global().Execute(String.Format("Delete From autoselect"));
            Utils.SetSysInfo(DB.Global(), "Select.msg", "Not yet");
            Utils.SetSysInfo(DB.Global(), "Select.starttime", "");
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
                    toolStripProgressBar_.Value = 0;
                    toolStripProgressBar_.Visible = true;
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
                toolStripProgressBar_.Value = (int)percent;
                msgText.Text = String.Format("{0}...{1}%", msg, percent.ToString("F2"));
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
                toolStripProgressBar_.Value = 100;
                toolStripProgressBar_.Visible = false;
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
                    if (Utils.NowIsTradeDay())
	                {
                        toolStripStatusLabel2_.Text = "tradeday " + DateTime.Now.ToLongDateString();
	                } else {
                        toolStripStatusLabel2_.Text = "holiday " + DateTime.Now.ToLongDateString();
                    }
                });
        }
        void IHost.uiReportSelectMsg(String msgIn, bool bImportantIn)
        {
            if (isClosing_)
            {
                return;
            }
            Action<string, bool> action;
            Invoke(action = (msg, bImportant) =>
            {
                reportSelectMsg(msg, bImportant);
            }, msgIn, bImportantIn);
        }
        bool IHost.uiAutoSelectMode()
        {
            return bAutoSelectMode_;
        }
        public Form createTabPage(string name, Form form)
        {
            TabPage page = new TabPage(name);
            page.Name = name;
            form.TopLevel = false;
            form.FormBorderStyle = FormBorderStyle.None;
            form.Size = mainTab.ClientSize;
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

        private void startWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (Setting.DebugMode)
	        {
                toolStripStatusLabel1_.Text = String.Format("Debug: {0} Stocks", App.ds_.stockList_.Count);
	        } else 
            {
                toolStripStatusLabel1_.Text = String.Format("Release: {0} Stocks", App.ds_.stockList_.Count);
            }
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
        void doSelectWork()
        {
            DB.Global().Execute(String.Format("Delete From autoselect"));
            LUtils.RemoveAllListRow(selectListView_);
            selectWorker.RunWorkerAsync();
        }
        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isBusy_)
            {
                MessageBox.Show("正忙，请稍微。", "Selector");
                return;
            }
            if (DialogResult.Yes != MessageBox.Show("Are you want to select now?", "Selector", MessageBoxButtons.YesNo))
            {
                return;
            }
            doSelectWork();
        }
        public void reportSelectMsg(string sMsg, bool bImportant)
        {
            if (bAutoSelectMode_)
            {
                if (bImportant && !bHasSendSms_)
                {
     //               Sms.SendMsg(sMsg);
                    bHasSendSms_ = true;
                    App.host_.uiSetMsg(sMsg);
                }
            }
            else
            {
                MessageBox.Show(sMsg);
            }
        }
        private void selectWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            isBusy_ = true;
            try
            {
                reSelect_ = null;
                LUtils.RemoveAllListRow(selectListView_);
                if (!App.ds_.prepareForSelect())
                {
                    reportSelectMsg("准备数据工作失败，无法继续执行！", true);
                    if (bAutoSelectMode_)
                    {
                        Utils.SetSysInfo(DB.Global(), "Select.msg", "Select error: prepare work fail.");
                        bHasReportAutoSelectModeError_ = true;
                    }
                    isBusy_ = false;
                    return;
                }
                SelectManager manager = new SelectManager();
                reSelect_ = manager.selectNow();
            }
            catch (Exception ex)
            {
                reSelect_ = null;
                reportSelectMsg(String.Format("执行发生异常：{0}", ex.Message), true);
                if (bAutoSelectMode_)
                {
                    Utils.SetSysInfo(DB.Global(), "Select.msg", "Select error: raise exception: " + ex.Message);
                    bHasReportAutoSelectModeError_ = true;
                }
                isBusy_ = false;
                throw;
            }
            isBusy_ = false;
        }
        void showRuntimeInfo()
        {
                int nUpCount = 0, nDownCount = 0, nZeroCount = 0;
                int now = Utils.NowDate();
                foreach (var sk in App.ds_.stockList_)
                {
                    if (sk.dataList_.Count < 2)
                    {
                        continue;
                    }
                    float zf = sk.zf(now);
                    if (zf > 0)
                    {
                        nUpCount++;
                    }
                    else if (zf < 0)
                    {
                        nDownCount++;
                    }
                    else
                    {
                        nZeroCount++;
                    }
                }
                String envBonus = App.ds_.envBonus(Utils.NowDate());
                if (Setting.DebugMode)
                {
                    toolStripStatusLabel1_.Text = String.Format("Debug: {0} Stocks, {1}, {2} Up, {3} Down, {4} Zero {5} ",
                        App.ds_.stockList_.Count, envBonus, nUpCount, nDownCount, nZeroCount, DateTime.Now.ToShortTimeString());
                }
                else
                {
                    toolStripStatusLabel1_.Text = String.Format("Release: {0} Stocks, {1}, ,{2} Up, {3} Down, {4} Zero {5} ",
                        App.ds_.stockList_.Count, envBonus, nUpCount, nDownCount, nZeroCount, DateTime.Now.ToShortTimeString());
                }
                if (Utils.GetBonusValue(envBonus) > 0)
                {
                    toolStripStatusLabel1_.ForeColor = Color.Red;
                    toolStripStatusLabel2_.ForeColor = Color.Red;
                }
                else if (Utils.GetBonusValue(envBonus) < 0)
                {
                    toolStripStatusLabel1_.ForeColor = Color.Green;
                    toolStripStatusLabel2_.ForeColor = Color.Green;
                }
                else
                {
                    toolStripStatusLabel1_.ForeColor = Color.Black;
                    toolStripStatusLabel2_.ForeColor = Color.Black;
                }
        }
        private void selectWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (reSelect_ != null && reSelect_.selItems_.Count > 0)
            {
                if (bAutoSelectMode_)
                {
                    var item = reSelect_.selItems_[0];
                    String sSelectMsg = String.Format("{0} {1}", item.code_, item.getColumnVal("name"));
                    reportSelectMsg(sSelectMsg, true);
                }
                LUtils.FillListViewData(selectListView_, reSelect_.selItems_);

                DateTime curTime = DateTime.Now;
                foreach (var item in reSelect_.selItems_)
                {
                    Dictionary<String, Object> selectItem = new Dictionary<string, object>();
                    selectItem["code"] = item.code_;
                    selectItem["name"] = item.getColumnVal("name");
                    selectItem["date"] = item.date_;
                    selectItem["straname"] = item.strategyName_;
                    selectItem["zf"] = item.getColumnVal("zf");
                    selectItem["close"] = item.getColumnVal("close");
                    selectItem["pubrank"] = item.getColumnVal("pubrank");
                    selectItem["prirank"] = item.getColumnVal("prirank");
                    selectItem["selecttime"] = Utils.ToTimeDesc(curTime);

                    DB.Global().Insert("autoselect", selectItem);
                }
            }
            else
            {
                if (bAutoSelectMode_)
                {
                    reportSelectMsg("No candidate", true);
                }
            }
            if (reSelect_ != null)
            {
                if (Utils.NowIsTradeDay())
	            {
                    showRuntimeInfo();
	            }
            }
            showForm("TabSelect");
            reportSelectMsg("Select complete.", false);
            if (bAutoSelectMode_ && !bHasReportAutoSelectModeError_)
            {
                Utils.SetSysInfo(DB.Global(), "Select.msg", "Select completed.");
            }
            bAutoSelectMode_ = false;
        }

        private void regressToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (isBusy_)
            {
                MessageBox.Show("正忙，请稍微。", "Selector");
                return;
            }
            NewRegressForm form = new NewRegressForm();
            if (form.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            regressingRe_ = form.re_;

            regressWorker.RunWorkerAsync();
        }

        private void regressWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            isBusy_ = true;
            try
            {
                if (regressingRe_.EndDate >= Utils.LastTradeDay())
                {
                    if (!App.ds_.prepareForSelect())
                    {
                        MessageBox.Show("准备数据工作失败，无法继续执行！");
                        isBusy_ = false;
                        return;
                    }
                }
                RegressManager regressManager = new RegressManager();
                regressManager.regress(regressingRe_);
                e.Result = true;
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
                App.regressList_.Add(regressingRe_);
                RegressSelectForm selectform = (RegressSelectForm)createTabPage(regressingRe_.SelectFormName, new RegressSelectForm());
                RegressBuyForm buyform = (RegressBuyForm)createTabPage(regressingRe_.BuyFormName, new RegressBuyForm());
                if (regressingRe_.runMode_ == RunMode.RM_Asset)
                {
                    createTabPage(regressingRe_.StatisRawHistoryFormName, new RegressStatisticsForm(regressingRe_, WantDataType.WD_RawHistory));
                    createTabPage(regressingRe_.StatisBonusFormName, new RegressStatisticsForm(regressingRe_, WantDataType.WD_BonusData));
                }
                else
                {
                    createTabPage(regressingRe_.StatisBonusFormName, new RegressStatisticsForm(regressingRe_, WantDataType.WD_BonusData));
                    createTabPage(regressingRe_.StatisRateItemFormName, new RegressStatisticsForm(regressingRe_, WantDataType.WD_RateItemData));
                }
                createTabPage(regressingRe_.StatisHistoryFormName, new RegressStatisticsForm(regressingRe_, WantDataType.WD_HistoryData));
                selectform.re_ = buyform.re_ = regressingRe_;
                LUtils.FillListViewData(selectform.selectItemListView(), regressingRe_.selItems_);
                LUtils.FillListViewData(buyform.buyItemListView(), regressingRe_.buyItems_);
                regressingRe_ = null;
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

        private void selectListView__ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LUtils.OnColumnClick(selectListView_, e.Column, ref sortColumn_);
        }

        private void MainForm_SizeChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < mainTab.TabPages.Count; i++)
            {
                if (mainTab.TabPages[i].Name == "TabSelect")
                {
                    continue;
                }
                Form form = (Form)mainTab.TabPages[i].Controls[0];
                form.Size = mainTab.ClientSize;

            }
        }

        private void swichmodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Setting.DebugMode)
            {
                Utils.SetSysInfo(DB.Global(), "DebugMode", "0");
            }
            else
            {
                Utils.SetSysInfo(DB.Global(), "DebugMode", "1");
            }
            Process.Start(Assembly.GetExecutingAssembly().Location, "reset");
            Close();
        }

        private void skinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SkinForm form = new SkinForm();
            form.ShowDialog();
        }

        private void solutionSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SolutionSettingForm form = new SolutionSettingForm();
            form.ShowDialog();
        }

        private void dateRangeSettingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DateRangeSettingForm form = new DateRangeSettingForm();
            form.ShowDialog();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ManualSelectForm form = new ManualSelectForm();
            form.ShowDialog();
        }

        private void timer__Tick(object sender, EventArgs e)
        {
            if (isBusy_)
            {
                return;
            }
            DateTime curTime = DateTime.Now;
            if (!bHasAutoSelect_ && Utils.NowIsTradeDay() &&
                curTime.Hour == 14 && curTime.Minute >= 57)
            {
                bHasAutoSelect_ = true;
                bAutoSelectMode_ = true;
                Utils.SetSysInfo(DB.Global(), "Select.msg", "Selecting");
                Utils.SetSysInfo(DB.Global(), "Select.starttime", Utils.ToTimeDesc(curTime));
                doSelectWork();
            }
            if ((curTime.Year != startupTime_.Year || curTime.Month != startupTime_.Month ||
                                curTime.Day != startupTime_.Day) && curTime.Hour > 9)
            {
                Process.Start(Assembly.GetExecutingAssembly().Location, "reset");
                Close();
            }
        }

        private void addUserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddUserForm form = new AddUserForm();
            form.ShowDialog();
        }

    }
}
