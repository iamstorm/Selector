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
            public String Msg;
            public String HasError;
        }
        List<SelectItem> querySelectItems(out String time)
        {
            time = "";
            var selItems = new List<SelectItem>();
            var dt = DB.Global().Select(String.Format("Select * From autoselect Where Date = '{0}' Order by id", U.NowDate()));
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
        List<TaskItem> queryTaskItems()
        {
            DateTime lastFinishTime;
            DateTime.TryParse(U.GetSysInfo(DB.Global(), "Select.finishTime"), out lastFinishTime);
            if (lastFinishTime == null)
            {
                lastFinishTime = new DateTime(2005, 1, 1);
            }
            var taskItems = new List<TaskItem>();
            var dt = DB.Global().Select(String.Format("Select * From select_task Where finishTime is null Order by id"));
            bool bIsTopTask = true;
            foreach (DataRow row in dt.Rows)
            {
                TaskItem item = new TaskItem();
                item.ID = row["id"].ToString();
                item.AddTime = row["addTime"].ToString();
                item.Msg = row["msg"].ToString();
                item.HasError = row["hasError"].ToString();

                if (bIsTopTask)
                {
                    DateTime willStartTime = lastFinishTime.AddMinutes(1);
                    if (DateTime.Now > willStartTime)
                    {
                        item.StartAfter = "Already Start";
                    }
                    else
                    {
                        var span = willStartTime - DateTime.Now;
                        item.StartAfter = span.Seconds.ToString() + "s";
                    }
                    bIsTopTask = false;
                }


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
            if (!U.NowIsTradeDay())
            {
                var invalidData = new
                {
                    time = "Not available",
                    msg = "Holiday",
                    selectItems = new List<SelectItem>(),
                    taskItems = new List<TaskItem>(),
                };
                return Content(invalidData.ToJson());
            }
            String time;
            var selItems = querySelectItems(out time);
            var taskItems = queryTaskItems();
            if (time == "")
            {
                time = U.ToTimeDesc(DateTime.Now);
            }
            String sSelectMsg = U.GetSysInfo(DB.Global(), "Select.msg");
            if (sSelectMsg == "Selecting")
            {
                String sStartTime = U.GetSysInfo(DB.Global(), "Select.starttime");
                DateTime startTime;
                if (DateTime.TryParse(sStartTime, out startTime))
                {
                    var span = DateTime.Now - startTime;
                    sSelectMsg += " Elapse: " + span.Seconds + "s";
                }
            }
            String sMsg = String.Format("{0}: {1} items, {2}", time, selItems.Count, sSelectMsg);

            var data = new {
                       isComplete = sSelectMsg == "Select completed",
                       isError = sSelectMsg.StartsWith("Select error:"),
                       msg = sMsg,
                       selItems = selItems,
                       taskItems = taskItems
                       };
            return Content(data.ToJson());
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult AddTask()
        {
            var row = new Dictionary<String, Object>();
            row["addTime"] = U.ToTimeDesc(DateTime.Now);
            DB.Global().Insert("select_task", row);
            return Content(new { suc = "1" }.ToJson());
        }
    }
}
