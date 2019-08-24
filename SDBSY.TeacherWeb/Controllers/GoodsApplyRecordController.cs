using log4net;
using Newtonsoft.Json;
using SDBSY.Common;
using SDBSY.DTO;
using SDBSY.IService;
using SDBSY.TeacherWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using SDBSY.TeacherWeb.App_Start;

namespace SDBSY.TeacherWeb.Controllers
{
    [CheckLogin]
    public class GoodsApplyRecordController : Controller
    {
        private static ILog log= LogManager.GetLogger(typeof(GoodsApplyRecordController));
        public static readonly string serverUrl = WebConfigurationManager.AppSettings["fileserver"];
        public ITeacherService teacherSvc { get; set; }
        public IGoodsService goodsSvc { get; set; }
        public IDataDictionaryService dataSvc { get; set; }
        // GET: GoodsApplyRecord
        [HttpGet]
        public ActionResult Add()
        {
            var id = (long)AdminHelper.GetUserId(HttpContext);
            var teacher = teacherSvc.GetByAdminId(id);
            if(teacher==null)
            {
                return View("Error", (object)"请先添加教师信息");
            }
            var classes = dataSvc.GetByName("ClassType");
            var goods = goodsSvc.GetAll();
            var goodsList=new List<GoodsSelectModel>();
            foreach (var item in goods)
            {
                var goodsModel=new GoodsSelectModel()
                {
                    Id=item.Id,
                    Value = item.Name
                };
                goodsList.Add(goodsModel);
            }
            var model = new ApplyRecordAddViewModel()
            {
                TeacherId = teacher.Id,
                Classes = JsonConvert.SerializeObject(classes, Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() }),
                Goods = JsonConvert.SerializeObject(goodsList, Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() })
            };
            return View(model);
        }
        [HttpPost]
        public ActionResult Add(AppplyRecordAddPostModel model)
        {

            if (!ModelState.IsValid)
            {
                return Json(new AjaxResult() { Status = "error", ErrorMsg = MVCHelper.GetValidMsg(ModelState) });
            }
            //1.保存信息
            var dto = new GoodsApplyRecordAddNewDTO()
            {
                ClassId = model.ClassId,
                GoodsId= model.GoodsId,
                TeacherId = model.TeacherId,
                Amount = model.Amount
            };
            long id = goodsSvc.AddNewApplyRecord(dto);
            
            return Json(new AjaxResult() { Status = "ok" });
        }

        public ActionResult List()
        {
            var id = (long)AdminHelper.GetUserId(HttpContext);
            var teacher = teacherSvc.GetByAdminId(id);
            if (teacher == null)
            {
                return View("Error", (object)"请先添加教师信息");
            }

            var records = goodsSvc.GetAllApplyRrcordByTeacherId(teacher.Id);
            return View(records);
        }
        [HttpPost]
        public ActionResult Del(long id)
        {
            goodsSvc.ApplyRecordDelete(id);
            return Json(new AjaxResult() { Status = "ok" });
        }
    }
}