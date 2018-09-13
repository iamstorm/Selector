/*******************************************************************************
 * Copyright © 2016 NFine.Framework 版权所有
 * Author: NFine
 * Description: NFine快速开发平台
 * Website：http://www.nfine.cn
*********************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NFine.Code;
using SelectorWeb;
using SelectorWeb.Utils;
using System.Data;

namespace NFine.Web.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public virtual ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult GetAuthCode()
        {
            return File(new VerifyCode().GetVerifyCode(), @"image/Gif");
        }
        [HttpGet]
        public ActionResult OutLogin()
        {
            Session.Abandon();
            Session.Clear();
            OperatorProvider.Provider.RemoveCurrent();
            return RedirectToAction("Index", "Login");
        }
        [HttpPost]
        [HandlerAjaxOnly]
        public ActionResult CheckLogin(string username, string password, string code)
        {
            try
            {
                if (Session["nfine_session_verifycode"].IsEmpty() || Md5.md5(code.ToLower(), 16) != Session["nfine_session_verifycode"].ToString())
                {
                    throw new Exception("验证码错误，请重新输入");
                }
                DataTable dt = DB.Global().Select(String.Format("Select * From User Where name = '{0}'", username));
                if (dt.Rows.Count == 0)
                {
                    throw new Exception("用户不存在");
                }
                String sKey = dt.Rows[0]["key"].ToString();
                string dbPassword = Md5.md5(DESEncrypt.Encrypt(password.ToLower(), sKey).ToLower(), 32).ToLower();
                if (dbPassword != dt.Rows[0]["psw"].ToString())
                {
                    throw new Exception("密码不正确");
                }

                OperatorModel operatorModel = new OperatorModel();
                operatorModel.UserId = dt.Rows[0]["id"].ToString();
                operatorModel.UserCode = username;
                operatorModel.UserName = username;
                operatorModel.CompanyId = "";
                operatorModel.DepartmentId = "";
                operatorModel.RoleId = "";
                operatorModel.LoginIPAddress = Net.Ip;
                operatorModel.LoginIPAddressName = Net.GetLocation(operatorModel.LoginIPAddress);
                operatorModel.LoginTime = DateTime.Now;
                operatorModel.LoginToken = DESEncrypt.Encrypt(Guid.NewGuid().ToString());
                operatorModel.SecretKey = "Selector";
                if (username == "admin")
                {
                    operatorModel.IsSystem = true;
                }
                else
                {
                    operatorModel.IsSystem = false;
                }
                OperatorProvider.Provider.AddCurrent(operatorModel);
                return Content(new AjaxResult { state = ResultType.success.ToString(), message = "登录成功。" }.ToJson());
            }
            catch (Exception ex)
            {
                return Content(new AjaxResult { state = ResultType.error.ToString(), message = ex.Message }.ToJson());
            }
        }
    }
}
