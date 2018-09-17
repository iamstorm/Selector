using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Code;
using SelectorWeb.Utils;

namespace SelectorWeb.Controllers
{
    public class SelectController : Controller
    {
        struct SelectItem
        {
            public String Code;
            public String Name;
            public String ZF;
            public String Close;
            public String Strategy;
            public String Pubrank;
            public String Prirank;
        }
        struct TaskItem
        {
            public String ID;
            public String AddTime;
            public String StartAfter;
            public String Message;
        }
        struct ErrTaskItem
        {
            public String ID;
            public String AddTime;
            public String FinishTime;
            public String Message;
        }
        List<SelectItem> querySelectItems(SQLiteHelper sh, out String time)
        {
            time = "";
            var selItems = new List<SelectItem>();
            var dt = sh.Select(String.Format("Select * From autoselect Where Date = '{0}' Order by id", U.NowDate()));
            foreach (DataRow row in dt.Rows)
            {
                SelectItem item = new SelectItem();
                item.Code = row["code"].ToString();
                item.Name = row["name"].ToString();
                item.ZF = U.ToType<float>(row["zf"]).ToString("F2") + "%";
                item.Close = U.ToType<float>(row["close"]).ToString("F2");
                item.Strategy = row["straname"].ToString();
                item.Pubrank = U.ToType<float>(row["pubrank"]).ToString("F0");
                item.Prirank = U.ToType<float>(row["prirank"]).ToString("F0");
                if (time == "")
                {
                    time = row["selecttime"].ToString();
                }
                selItems.Add(item);
            }
            return selItems;
        }
        List<TaskItem> queryTaskItems(SQLiteHelper sh)
        {
            DateTime lastFinishTime;
            DateTime.TryParse(U.GetSysInfo(sh, "Select.finishTime"), out lastFinishTime);
            if (lastFinishTime == null)
            {
                lastFinishTime = new DateTime(2005, 1, 1);
            }
            var taskItems = new List<TaskItem>();
            var dt = sh.Select(String.Format("Select * From select_task Where finishTime is null Order by id"));
            bool bIsTopTask = true;
            foreach (DataRow row in dt.Rows)
            {
                TaskItem item = new TaskItem();
                item.ID = row["id"].ToString();
                item.AddTime = row["addTime"].ToString();
                item.Message = row["msg"].ToString();

                if (bIsTopTask)
                {
                    DateTime willStartTime = lastFinishTime.AddMinutes(1);
                    if (DateTime.Now > willStartTime)
                    {
                        var span = DateTime.Now - willStartTime;

                        if (item.Message == "Selecting")
                        {
                            item.StartAfter = "Already Start " + span.TotalSeconds.ToString("F0") + "s";
                        }
                        else
                        {
                            item.StartAfter = "Will Start " + span.TotalSeconds.ToString("F0") + "s";
                        }
                    }
                    else
                    {
                        var span = willStartTime - DateTime.Now;
                        item.StartAfter = span.TotalSeconds.ToString("F0") + "s";
                    }
                    bIsTopTask = false;
                }


                taskItems.Add(item);
            }
            return taskItems;
        }
        List<ErrTaskItem> queryErrTaskItems(SQLiteHelper sh)
        {
            var taskItems = new List<ErrTaskItem>();
            var dt = sh.Select(String.Format("Select * From select_task Where finishTime is not null And hasError = 1 Order by id desc"));
            foreach (DataRow row in dt.Rows)
            {
                ErrTaskItem item = new ErrTaskItem();
                item.ID = row["id"].ToString();
                item.AddTime = row["addTime"].ToString();
                item.FinishTime = row["finishTime"].ToString();
                item.Message = row["msg"].ToString();

                taskItems.Add(item);
            }
            return taskItems;
        }
        //
        // GET: /Select/
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSelectInfo()
        {
            using (SH sh = new SH())
            {
                String time;
                var selItems = querySelectItems(sh, out time);
                var taskItems = queryTaskItems(sh);
                var errTaskItems = queryErrTaskItems(sh);
                DateTime curTime = DateTime.Now;
                if (time == "")
                {
                    time = U.ToTimeDesc(curTime);
                }
                if (U.IsOpenTime(curTime.Hour, curTime.Minute))
                {
                    time = "O " + time;
                }
                else if (U.IsCloseTime(curTime.Hour, curTime.Minute))
                {
                    time = "C " + time;
                } 
                else if (U.IsTradeTime(curTime.Hour, curTime.Minute))
                {
                    time = "T " + time;
                }
                else
                {
                    time = "N " + time;
                }
                String sSelectMsg = U.GetSysInfo(sh, "Select.msg");
                if (sSelectMsg == "Selecting")
                {
                    String sStartTime = U.GetSysInfo(sh, "Select.starttime");
                    DateTime startTime;
                    if (DateTime.TryParse(sStartTime, out startTime))
                    {
                        var span = DateTime.Now - startTime;
                        sSelectMsg += " Elapse: " + span.TotalSeconds.ToString("F0") + "s";
                    }
                }
                String sMsg = String.Format("{0}: {1} items, {2}", time, selItems.Count, sSelectMsg);
                if (!U.NowIsTradeDay(sh))
                {
                    sMsg = "Holiday " + sMsg;
                }

                var data = new
                {
                    isComplete = sSelectMsg == "Select completed",
                    isError = sSelectMsg.StartsWith("Select error:"),
                    msg = sMsg,
                    selItems = selItems,
                    taskItems = taskItems,
                    errTaskItems = errTaskItems,
                    finshTaskCount = sh.ExecuteScalar<int>("Select Count(*) From select_task Where finishTime is not null")
                };
                return Content(data.ToJson());
            }
      
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult AddTask()
        {
            using (SH sh = new SH())
            {
                var row = new Dictionary<String, Object>();
                row["addTime"] = U.ToTimeDesc(DateTime.Now);
                sh.Insert("select_task", row);
                return Content(new { suc = "1" }.ToJson());
            }
        }
    }
}
