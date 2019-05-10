using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelectImpl;
using System.Threading;

namespace SelectorForm
{
    public class SelectTask
    {
        public String sSelectingID_;
        bool bHasReportAutoSelectModeError_ = false;
        public bool bHasStart_ = false;
        static DateTime s_LastFinishTime_;
        public static void Init()
        {
            DB.Global().Execute(String.Format("Delete From autoselect"));
            DB.Global().Execute(String.Format("Delete From select_task"));
            DB.Global().Execute("update sqlite_sequence set seq ='0' where name in ('select_task','autoselect')");
            Utils.SetSysInfo(DB.Global(), "Select.msg", "Not yet");
            Utils.SetSysInfo(DB.Global(), "Select.starttime", "");

            DateTime.TryParse(Utils.GetSysInfo(DB.Global(), "Select.finishTime"), out s_LastFinishTime_);
            if (s_LastFinishTime_ == null)
            {
                s_LastFinishTime_ = new DateTime(2005, 1, 1);
            }
        }
        public static void AutoSelect(bool bAutoAddTaskIfNull)
        {
            if (MainForm.Me.selectTask_ != null)
            {
                if (MainForm.Me.selectTask_.canStart())
                {
                    MainForm.Me.selectTask_.startSelecting();
                }
                return;
            }
            MainForm.Me.selectTask_ = QueryTask();
            if (MainForm.Me.selectTask_ == null && bAutoAddTaskIfNull)
            {
                AddTask();
                MainForm.Me.selectTask_ = QueryTask();
            }

            if (MainForm.Me.selectTask_ != null && MainForm.Me.selectTask_.canStart())
            {
                MainForm.Me.selectTask_.startSelecting();
            }
        }
        public static void AddTask()
        {
            var row = new Dictionary<String, Object>();
            row["addTime"] = Utils.ToTimeDesc(DateTime.Now);
            DB.Global().Insert("select_task", row);
        }
        public static SelectTask QueryTask()
        {
            String  sSelectingID = DB.Global().ExecuteScalar<String>("Select ID From select_task Where startTime is null Order by ID limit 1");
            if (sSelectingID == null)
            {
                return null;
            }
            SelectTask ret = new SelectTask();
            ret.sSelectingID_ = sSelectingID;
            return ret;
        }
        public bool canStart()
        {
            if (bHasStart_)
            {
                return false;
            }
            DateTime canStartTime = s_LastFinishTime_.AddMinutes(1);
            DateTime now = DateTime.Now;
            return now > canStartTime || (now.Hour == 14 && now.Minute == 56 && (now.Second == 10 || now.Second == 11 || now.Second == 20 || now.Second == 21 || now.Second == 30 || now.Second == 31));
        }
        public void startSelecting()
        {
            String sMsg = "Selecting";
            String sStartTime = Utils.ToTimeDesc(DateTime.Now);
            Utils.SetSysInfo(DB.Global(), "Select.msg", sMsg);
            Utils.SetSysInfo(DB.Global(), "Select.starttime", sStartTime);
            DB.Global().Execute(String.Format("Update select_task Set msg = '{0}', startTime = '{1}' Where id = '{2}'", sMsg, sStartTime, sSelectingID_));
            bHasStart_ = true;
            new Thread(MainForm.Me.doSelectWork).Start();
        }
        public void reportError(String sError)
        {
            String sFinishTime = Utils.ToTimeDesc(DateTime.Now);
            sError = "Select error: " + sError;
            Utils.SetSysInfo(DB.Global(), "Select.msg", sError);
            Utils.SetSysInfo(DB.Global(), "Select.finishTime", sFinishTime);
            DB.Global().Execute(String.Format("Update select_task Set msg = '{0}', finishTime = '{1}', hasError = 1 Where id = '{2}'", sError, sFinishTime, sSelectingID_));
            bHasReportAutoSelectModeError_ = true;
        }
        public void end()
        {
            s_LastFinishTime_ = DateTime.Now;
            if (bHasReportAutoSelectModeError_)
            {
                return;
            }
            String sFinishTime = Utils.ToTimeDesc(s_LastFinishTime_);
            String sMsg = "Select completed";
            Utils.SetSysInfo(DB.Global(), "Select.msg", sMsg);
            Utils.SetSysInfo(DB.Global(), "Select.finishTime", sFinishTime);
            DB.Global().Execute(String.Format("Update select_task Set msg = '{0}', finishTime = '{1}', hasError = 0 Where id = '{2}'", sMsg, sFinishTime, sSelectingID_));
        }
    }
}
