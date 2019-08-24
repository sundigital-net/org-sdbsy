using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDBSY.Common;
using SDBSY.IService;
using SDBSY.UserWebNew.App_Start;
using SDBSY.UserWebNew.Models;

namespace SDBSY.UserWebNew.Controllers
{
    public class AccountController : Controller
    {
        public IUserService userSvc { get; set; }
        // GET: Account

        #region 创建验证码
        public void CreateCaptcha()
        {
            string captcha = CommonHelper.CreateVerifyCode(4);
            //验证码使用一次就应该销毁，避免恶意
            TempData["Captcha"] = captcha;//验证码
            CommonHelper.CreateImage(captcha);
        }
        #endregion

        #region 登录
        
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CheckSystem]
        public ActionResult Signin()
        {
            Session.Abandon();
            return View();
        }

        [HttpPost]
        [CheckSystem]
        public ActionResult Signin(UserSigninModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = MVCHelper.GetValidMsg(ModelState) });
            }
            string loginChatcha = (string)TempData["Captcha"];
            if (string.IsNullOrEmpty(loginChatcha) || loginChatcha.ToLower() != model.LoginCaptcha.ToLower())
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "验证码错误。" });
            }
            else
            {
                bool b = userSvc.CheckLogin(model.PhoneNum, model.Pwd);
                var user = userSvc.GetByPhoneNum(model.PhoneNum);

                if (!b)//登陆失败
                {
                    if (user == null)//用户名不存在
                    {
                        return Json(new AjaxResult { Status = "error", ErrorMsg = "用户名或密码错误。" });
                    }
                    //用户名存在
                    userSvc.MarkLoginError(user.Id);//标记登录错误
                    return Json(new AjaxResult { Status = "error", ErrorMsg = "用户名或密码错误。" });
                }
                if (userSvc.IsLocked(user.Id))
                {
                    TimeSpan? timeSpan = TimeSpan.FromMinutes(30) - (DateTime.Now - user.LastLoginErrorDateTime);
                    return Json(new AjaxResult { Status = "error", ErrorMsg = "账号已被锁定，请" + (int)timeSpan.Value.TotalMinutes + "分钟后再试。" });
                }
                userSvc.ResetLoginError(user.Id);//清除登录错误
                Session["LoginUserId"] = userSvc.GetByPhoneNum(model.PhoneNum).Id;
                Session["UserRole"] = "user";
                return Json(new AjaxResult { Status = "ok" });
            }
        }
        #endregion

        #region 注册
        
        /// <summary>
        /// 注册
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [CheckSystem]
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Signup(UserSignupModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = MVCHelper.GetValidMsg(ModelState) });
            }
            if (!CheckPhoneNum(model.PhoneNum))//手机号查重
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "手机号已被占用" });
            }
            //密码复杂度验证
            if (!CommonHelper.CheckPwd(model.Pwd))
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "密码长度为8-30，必须包括数字、字母以及字符。" });
            }
            string regChatcha = (string)TempData["Captcha"];
            if (string.IsNullOrEmpty(regChatcha) || regChatcha.ToLower() != model.RegCaptcha.ToLower())
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "验证码错误，请重试。" });
            }
            else
            {
                long id = userSvc.AddNew(model.PhoneNum, model.Pwd);
                return Json(new AjaxResult { Status = "ok" });

            }
        }


        #endregion

        #region 手机号占用查询
        /// <summary>
        /// 注册手机号查重
        /// </summary>
        /// <param name="phoneNum"></param>
        /// <returns></returns>
        private bool CheckPhoneNum(string phoneNum)
        {
            var user = userSvc.GetByPhoneNum(phoneNum);
            return user == null;
        }
        /// <summary>
        /// ui界面ajax查询
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Validate(string account)
        {
            var admin = userSvc.GetByPhoneNum(account);
            return admin == null ? Content("true") : Content("false");
        }


        #endregion

        #region 登出

        public ActionResult Signout()
        {
            Session.Abandon();
            return RedirectToAction("Signin");
        }

        #endregion
    }
}