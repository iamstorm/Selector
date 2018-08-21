using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.IO;

namespace SelectImpl
{
    public static class App
    {
        public static StrategyGroup grp_ = new StrategyGroup();
        public static DataStore ds_ = new DataStore();
        public static StrategyAsset asset_ = new StrategyAsset();
        public static List<RegressResult> regressList_ = new List<RegressResult>();
        public static IHost host_;

        static App()
        {

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
