using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace SelectorForm
{
    public static class App
    {
        public static StrategyGroup grp_ = new StrategyGroup();
        public static DataStore ds_ = new DataStore();
        public static StrategyAsset asset_ = new StrategyAsset();


        public static bool RunScript(string mode)
        {
            MainForm.Me.setMsg("执行脚本:"+mode);
            string scriptFilePath = Dist.scriptPath_ + "PromiseData\\PromiseData.py";
            string arguments = String.Format("{0} {1} -m {2}", scriptFilePath, Dist.binPath_, mode);

            string cmd = String.Format("python.exe {0}", arguments);

            Process p = new Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//直接由命令行窗口来显示输出
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
            while (output != null)
            {
                if (output == "exe.start")
                {
                    bScriptStart = true;
                }
                output = p.StandardOutput.ReadLine();
                if (!bScriptStart || output == null)
                    continue;

                Match m = Regex.Match(output, "^(.*)总进度\\[.*\\](.*)%$");
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
                if (bProgressStart)
                {
                    float percent = Utils.ToType<float>(sProgressPercent);
                    MainForm.Me.setProcessBar(sProgressMsg, percent);
                    if (percent == 100)
                    {
                        MainForm.Me.finishProcessBar();
                        bProgressStart = false;
                    }
                }
                else
                {
                    if (output == "")
                    {
                        MainForm.Me.setMsg("正在执行...");
                    }
                    else
                    {
                        MainForm.Me.setMsg(output);
                    }
                }
            }

            p.WaitForExit();//等待程序执行完退出进程
            p.Close();

            return true;
        }
    }
}
