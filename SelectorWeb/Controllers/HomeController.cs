using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SelectorWeb.Controllers
{
    [HandlerLogin]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult US30()
        {
            return View();
        }
        public ActionResult TodayLF()
        {
            return View();
        }
    }
}
