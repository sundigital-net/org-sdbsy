using SDBSY.UserWebNew.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDBSY.IService;

namespace SDBSY.UserWebNew.Controllers
{
    public class StudentController : Controller
    {
        public IStudentService stuSvc { get; set; }
        // GET: Student
        [CheckPermission("user")]
        [CheckSystem]
        public ActionResult Index()
        {
            return View();
        }
        [CheckPermission("user")]
        [CheckSystem]
        public ActionResult List(long id)
        {
            long userId = (long)UserHelper.GetUserId(HttpContext);
            if (userId != id)
            {
                return View("BaseView", (object)"您无权查看其他用户信息");
            }
            var children = stuSvc.GetByUserId(id);
            if (!children.Any())
            {
                return View("BaseView", (object)"暂未添加幼儿信息");
            }
            return View(children);
        }
    }
}