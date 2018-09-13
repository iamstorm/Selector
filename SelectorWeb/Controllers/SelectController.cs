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
            public String code;
            public String name;
            public String zf;
            public String close;
            public String strategy;
            public String pubrank;
            public String prirank;
        }
        //
        // GET: /Select/
        [HttpGet]
        [HandlerAjaxOnly]
        public ActionResult GetSelectItems()
        {
            if (!U.NowIsTradeDay())
            {
                var invalidData = new
                {
                    time = "Not available",
                    msg = "Holiday",
                    items = new List<SelectItem>()
                };
                return Content(invalidData.ToJson());
            }
            var selItems = new List<SelectItem>();
            String time = "";
            var dt = DB.Global().Select(String.Format("Select * From autoselect Where Date = '{0}' Order by id", U.NowDate()));
            foreach (DataRow row in dt.Rows)
            {
                SelectItem item = new SelectItem();
                item.code = row["code"].ToString();
                item.name = row["name"].ToString();
                item.zf = U.ToType<float>(row["zf"]).ToString("F2") + "%";
                item.close = U.ToType<float>(row["close"]).ToString("F2");
                item.strategy = row["straname"].ToString();
                item.pubrank = U.ToType<float>(row["pubrank"]).ToString("F0");
                item.prirank = U.ToType<float>(row["prirank"]).ToString("F0");
                if (time == "")
                {
                    time = row["selecttime"].ToString();
                }
                selItems.Add(item);
            }
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
                       isComplete = sSelectMsg == "Select completed.",
                       isError = sSelectMsg.StartsWith("Select error:"),
                       msg = sMsg,
                       items = selItems
                       };
            return Content(data.ToJson());
        }

    }
}
