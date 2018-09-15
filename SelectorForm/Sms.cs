using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SelectorForm
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Net;
    using System.IO;
    using SelectImpl;

    public class Sms
    {
        private string THE_UID = "psistorm"; //用户名
        private string THE_KEY = "d41d8cd98f00b204e980"; //接口秘钥

        /// <summary>返回UTF-8编码发送接口地址</summary>
        /// <param name="receivePhoneNumber">目的手机号码（多个手机号请用半角逗号隔开）</param>
        /// <param name="receiveSms">短信内容，最多支持400个字，普通短信70个字/条，长短信64个字/条计费</param>
        /// <returns></returns>
        public string GetPostUrl(string smsMob, string smsText)
        {
            smsText += " 【selector】";
            smsText = System.Web.HttpUtility.HtmlEncode(smsText);
            string postUrl = "http://utf8.api.smschinese.cn/?Uid=" + THE_UID + "&key=" + THE_KEY + "&smsMob=" + smsMob + "&smsText=" + smsText;
            return postUrl;
        }
        /// <summary>
        /// 发送短信，得到返回值
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string PostSmsInfo(string url)
        {
            //调用时只需要把拼成的URL传给该函数即可。判断返回值即可
            string strRet = null;

            if (url == null || url.Trim().ToString() == "")
            {
                return strRet;
            }
            string targeturl = url.Trim().ToString();
            try
            {
                HttpWebRequest hr = (HttpWebRequest)WebRequest.Create(targeturl);
                hr.UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.1)";
                hr.Method = "GET";
                hr.Timeout = 30 * 60 * 1000;
                WebResponse hs = hr.GetResponse();
                Stream sr = hs.GetResponseStream();
                StreamReader ser = new StreamReader(sr, Encoding.Default);
                strRet = ser.ReadToEnd();
            }
            catch (Exception ex)
            {
                strRet = "Exception: " + ex.Message;
            }
            return strRet;
        }
        /// <summary>
        /// 确认返回信息 
        /// </summary>
        /// <param name="strRet"></param>
        /// <returns></returns>
        public string GetResult(string strRet)
        {
            int result = 0;
            try
            {
                result = int.Parse(strRet);
                switch (result)
                {
                    case -1:
                        strRet = "没有该用户账户";
                        break;
                    case -2:
                        strRet = "接口密钥不正确,不是账户登陆密码";
                        break;
                    case -21:
                        strRet = "MD5接口密钥加密不正确";
                        break;
                    case -3:
                        strRet = "短信数量不足";
                        break;
                    case -11:
                        strRet = "该用户被禁用";
                        break;
                    case -14:
                        strRet = "短信内容出现非法字符";
                        break;
                    case -4:
                        strRet = "手机号格式不正确";
                        break;
                    case -41:
                        strRet = "手机号码为空";
                        break;
                    case -42:
                        strRet = "短信内容为空";
                        break;
                    case -51:
                        strRet = "短信签名格式不正确,接口签名格式为：【签名内容】";
                        break;
                    case -6:
                        strRet = "IP限制";
                        break;
                    default:
                        strRet = "发送短信数量：" + result;
                        break;
                }
            }
            catch (Exception ex)
            {
                strRet = ex.Message;
            }
            return strRet;
        }
        public static String SendMsg(string sMsg)
        {
            Sms sms = new Sms();
            return sms.PostSmsInfo(sms.GetPostUrl("13570427002", sMsg));
        }
        public static void SendMsgIfTodayNotSend(string sMsg)
        {
            int date = Utils.NowDate();
            bool bAlreadySend = DB.Global().ExecuteScalar<int>(
                String.Format("Select Count(*) From already_send_sms Where date = '{0}' and msg = '{1}'", date, sMsg)) > 0;
            if (!bAlreadySend)
            {
                var ret = Sms.SendMsg(sMsg);
                Dictionary<String, Object> row = new Dictionary<String, Object>();
                row["date"] = date;
                row["msg"] = sMsg;
                row["sendTime"] = DateTime.Now;
                row["sendRet"] = ret;
                DB.Global().Insert("already_send_sms", row);
            }
        }
    }

}
