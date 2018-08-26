using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace SelectImpl
{
    public class SolutionSetting
    {
        public String name_
        {
            get;
            set;
        }
        public List<IStrategy> straList_ = new List<IStrategy>();
        public override string ToString()
        {
            return name_;
        }
    }
    public class DateRangeSetting
    {
        public String name_
        {
            get;
            set;
        }
        public List<DateRange> rangeList_ = new List<DateRange>();
        public override string ToString()
        {
            return name_;
        }
    }
    public static class App
    {
        public static StrategyGroup grp_ = new StrategyGroup();
        public static DataStore ds_ = new DataStore();
        public static StrategyAsset asset_ = new StrategyAsset();
        public static List<RegressResult> regressList_ = new List<RegressResult>();
        public static IHost host_;
        public static List<SolutionSetting> autoSolutionSettingList_;
        public static List<SolutionSetting> customSolutionSettingList_;
        public static List<DateRangeSetting> dateRangeSettingList_;
        static App()
        {
            Dist.Setup();
            SyncStrategy();
            InstallAutoSolutionSetting();
            ReadSolutionSetting();
            ReadDateRangeSetting();
            asset_.readAssetFromDB();
        }
        public static void SyncStrategy()
        {
            DataTable dt = DB.Global().Select("Select straName, verTag From stra_his");
            Dictionary<String, String> straVerTagDict = new Dictionary<string, string>();
            foreach (DataRow row in dt.Rows)
            {
                straVerTagDict[row["straName"].ToString()] = row["verTag"].ToString();
            }

            Dictionary<String, String> validStraFileNameDict = new Dictionary<string,string>();
            foreach (var stra in grp_.strategyList_)
            {
                var straDBFile = Dist.straPath_ + stra.name() + ".data";
                String verTag;
                if (straVerTagDict.TryGetValue(stra.name(), out verTag))
                {
                    if (verTag != stra.verTag())
	                {
                        File.Delete(straDBFile);
                        DB.Global().Execute(String.Format("Delete From stra_his Where straname = '{0}'", stra.name()));
	                }
                }

                bool bIsCreateMode = false;
                if (!File.Exists(straDBFile))
                {
                    bIsCreateMode = true;
                }
                SQLiteHelper sh = DB.RegConn(straDBFile, stra.dbKey());
                if (bIsCreateMode)
                {
                    sh.Execute(@"CREATE TABLE rateitem_his ( 
                                rateItemKey               VARCHAR( 200 )   PRIMARY KEY
                                                                        UNIQUE,
                                tradeSucProbility      NUMERIC( 5, 2 )  NOT NULL,
                                selectSucProbility     NUMERIC( 5, 2 )  NOT NULL,
                                bonusValue             NUMERIC( 5, 2 )  NOT NULL,
                                antiRate               NUMERIC( 5, 2 )  NOT NULL,
                                tradeDayRate           NUMERIC( 5, 2 )  NOT NULL,
                                dontBuyRate            NUMERIC( 5, 2 )  NOT NULL,
                                startDate              INT              NOT NULL,
                                endDate                INT              NOT NULL,
                                nDayCount              INT              NOT NULL,
                                nTradeCount            INT              NOT NULL,
                                nGoodSampleSelectCount INT              NOT NULL,
                                nAntiEnvCount          INT              NOT NULL,
                                nAntiEnvCheckCount  INT              NOT NULL,
                                nSelectSucCount        INT              NOT NULL,
                                nTradeSucCount         INT              NOT NULL,
                                nDontBuyAndDown        INT              NOT NULL,
                                nDontBuyButUp          INT              NOT NULL,
                                rank                   INT              NOT NULL 
                            );
                    ");
                }
                validStraFileNameDict[stra.name().ToLower()+".data"] = "";
            }
            FileInfo[] infos = new DirectoryInfo(Dist.straPath_).GetFiles();
            foreach (var info in infos)
            {
                if (!validStraFileNameDict.ContainsKey(info.Name.ToLower()))
	            {
                    if (File.Exists(info.FullName))
                    {
                        File.Delete(info.FullName);
                    }
                    DB.Global().Execute(String.Format("Delete From stra_his Where straname = '{0}'", info.Name));
	            }
            }
        }
        public static void InstallAutoSolutionSetting()
        {
            autoSolutionSettingList_ = new List<SolutionSetting>();
            SolutionSetting allSolution = new SolutionSetting();
            allSolution.name_ = "$All";
            foreach (var stra in grp_.strategyList_)
            {
                SolutionSetting setting = new SolutionSetting();
                setting.name_ = "$" + stra.name();
                setting.straList_.Add(stra);
                autoSolutionSettingList_.Add(setting);
                allSolution.straList_.Add(stra);
            }
            autoSolutionSettingList_.Add(allSolution);
        }
        public static void ReadSolutionSetting()
        {
            customSolutionSettingList_ = new List<SolutionSetting>();
            DataTable dt = DB.Global().Select("Select * From SolutionSetting Order by solution, strategy");
            foreach (DataRow row in dt.Rows)
            {
                var setting = (from item in customSolutionSettingList_
                               where item.name_ == row["solution"].ToString()
                               select item).FirstOrDefault();

                if (setting == null)
                {
                    setting = new SolutionSetting();
                    setting.name_ = row["solution"].ToString();
                    customSolutionSettingList_.Add(setting);
                }

                IStrategy strategy = grp_.strategy(row["strategy"].ToString());
                if (strategy == null)
                {
                    continue;
                }
                setting.straList_.Add(strategy);
            }
        }
        public static void ReadDateRangeSetting()
        {
            dateRangeSettingList_ = new List<DateRangeSetting>();
            DataTable dt = DB.Global().Select("Select * From DateRangeSetting Order by name, start");
            foreach (DataRow row in dt.Rows)
            {
                var setting = (from item in dateRangeSettingList_
                              where item.name_ == row["name"].ToString()
                              select item).FirstOrDefault();
                if (setting == null)
                {
                    setting = new DateRangeSetting();
                    setting.name_ = row["name"].ToString();
                    dateRangeSettingList_.Add(setting);
                }
                setting.rangeList_.Add(new DateRange()
                {
                    startDate_ = Utils.ToType<int>(row["start"]),
                    endDate_ = Utils.ToType<int>(row["end"]),
                });
            }
        }
        public static SolutionSetting Solution(String sSolutionName)
        {
            foreach (var item in customSolutionSettingList_)
            {
                if (item.name_ == sSolutionName)
                {
                    return item;
                }
            }
            foreach (var item in autoSolutionSettingList_)
            {
                if (item.name_ == sSolutionName)
                {
                    return item;
                }
            }
            return null;
        }
        public static DateRangeSetting DateRange(String sDateRangeName)
        {
            foreach (var item in dateRangeSettingList_)
            {
                if (item.name_ == sDateRangeName)
                {
                    return item;
                }
            }
            return null;
        }
        public static RegressResult RegressResult(string sName)
        {
            var ret = from re in regressList_
                      where re.name_ == sName
                      select re;
            return ret.ToList().FirstOrDefault();
        }

        public static void RemoveRegress(RegressResult re)
        {
            App.regressList_.Remove(re);
        }

        public static bool RunScript(string mode)
        {
            App.host_.uiSetMsg("执行脚本:"+mode);
            Utils.SetSysInfo(DB.Global(), "pyrun", "0");
            string scriptFilePath = Dist.scriptPath_ + "PromiseData\\PromiseData.py";
            string arguments = String.Format("\"{0}\" \"{1}\" -m {2}", scriptFilePath.Replace("\\", "\\\\"), Dist.binPath_.Replace("\\", "\\\\"), mode);

            string cmd = String.Format("python.exe {0}", arguments);

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//接受来自调用程序的输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序

            //向cmd窗口发送输入信息
            p.StandardInput.WriteLine(cmd + "&exit");

            p.StandardInput.AutoFlush = true;

            //获取cmd窗口的输出信息
            string output;
            output =p.StandardOutput.ReadLine();
            bool bScriptStart = false;
            bool bProgressStart = false;
            string sFailBecause = null;
            int iReadChar = -1;
            String msg = "";
            bool bRuntimeGettingData = false;
            while (true)
            {
                if (!bScriptStart)
                {
                    output = p.StandardOutput.ReadLine();
                    if (output == "exe.start")
                    {
                        bScriptStart = true;
                        iReadChar = p.StandardOutput.Read();
                    }
                    else if (output == null)
                    {
                        break;
                    }
                    continue;
                }
                iReadChar = p.StandardOutput.Read();
                if (iReadChar == -1)
                {
                    break;
                }
                char c = (char)iReadChar;
                string msgLine = null;
                if (c == '\n' || c == '\r')
                {
                    msgLine = msg;
                    msg = "";
                    bRuntimeGettingData = false;
                }
                else
                {
                    msg += c;
                }
                if (msg == "[Getting data:]")
                {
                    bRuntimeGettingData = true;
                }
                if (bRuntimeGettingData)
                {
                    App.host_.uiSetMsg(msg);
                    continue;
                }
                if (msgLine == null)
                {
                    continue;   
                }

                if (msgLine.IndexOf("HTTPError") > -1)
                {
                    sFailBecause = "实时数据暂时无法获取，请稍后再试...";
                }

                Match m = Regex.Match(msgLine, "^(.*)总进度\\[.*\\](.*)%$");
                string sProgressMsg = null;
                string sProgressPercent = null;
                if (m.Length > 0)
                {
                    sProgressMsg = m.Groups[1].Value;
                    sProgressPercent = m.Groups[2].Value;
                    if (!bProgressStart)
                    {
                        bProgressStart = true;
                        App.host_.uiStartProcessBar();
                    }
                }
                if (sProgressMsg != null)
                {
                    float percent = Utils.ToType<float>(sProgressPercent);
                    App.host_.uiSetProcessBar(sProgressMsg, percent);
                    if (percent == 100)
                    {
                        App.host_.uiFinishProcessBar();
                        bProgressStart = false;
                    }
                }
                else
                {
                    if (msgLine == "")
                    {
                        App.host_.uiSetMsg("正在执行脚本...");
                    }
                    else
                    {
                        App.host_.uiSetMsg(msgLine);
                    }
                 }
            }

            p.WaitForExit();//等待程序执行完退出进程
            p.Close();

            string sPyRun = Utils.GetSysInfo(DB.Global(), "pyrun");
            if (sPyRun != "1")
            {
                if (sFailBecause != null)
                {
                    App.host_.uiSetMsg("脚本执行失败：" + sFailBecause);
                }
                else
                {
                    App.host_.uiSetMsg("脚本执行失败：" + cmd);
                }
                return false;
            }
            else
            {
                App.host_.uiSetMsg("Script success completed");
                return true;
            }
        }
    }
}
