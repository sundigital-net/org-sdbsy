using SDBSY.UserWebNew.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDBSY.IService;

namespace SDBSY.UserWebNew.Controllers
{
    public class HomeController : Controller
    {
        public IUserService userSvc { get; set; }

        [CheckPermission("user")]
        [CheckSystem]
        public ActionResult Index()
        {
            //增加了权限检查，走到这里肯定已经登录
            long userId = (long)UserHelper.GetUserId(HttpContext);
            var user = userSvc.GetById(userId);
            var phone = user.PhoneNum;
            var substring = phone.Substring(3, 4);
            user.PhoneNum = phone.Replace(substring, "****");
            return View(user);
        }
        
    }
}