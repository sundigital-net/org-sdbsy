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

namespace SDBSY.TeacherWeb.Controllers
{
    public class GoodsApplyRecordController : Controller
    {
        private static ILog log= LogManager.GetLogger(typeof(GoodsApplyRecordController));
        public static readonly string serverUrl = WebConfigurationManager.AppSettings["fileserver"];
        public ITeacherService teacherSvc { get; set; }
        public IGoodsService applyrecordSvc { get; set; }
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
            var model = new ApplyRecordAddViewModel()
            {
                TeacherId = teacher.Id,
                Classes = JsonConvert.SerializeObject(classes, Formatting.Indented, new JsonSerializerSettings() { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() }),
            };
            return View(model);
        }
        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> Add(AppplyRecordAddPostModel model)
        {

            if (!ModelState.IsValid)
            {
                return Json(new AjaxResult() { Status = "error", ErrorMsg = MVCHelper.GetValidMsg(ModelState) });
            }
            //1.保存信息
            var dto = new GoodsAllApplyRecordDTO()
            {
                ClassId = model.ClassId,
                GoodsName = model.GoodsName,
                TeacherId = model.TeacherId,
                ApplyTime=model.ApplyTime,
                ReturnTime=model.ReturnTime
            };
            long id = applyrecordSvc.AddNewApplyRecord(dto);
            //2.保存图片
            if (model.UpFiles.Length > 0)
            {
                for (int i = 0; i < model.UpFiles.Length; i++)
                {
                    HttpPostedFileBase file = model.UpFiles[i];//这个对象是用来接收文件,利用这个对象可以获取文件的name path等
                    try
                    {
                        //log.Debug("保存图片开始-------");
                        var result = await SaveImgAsync(file);
                        if (result.Status == "ok")
                        {
                            var dto1 = new ApplyRecordPicAddNewDTO()
                            {
                                ApplyRecordId = id,
                                Url = result.Data.ToString()
                            };
                            long picId = applyrecordSvc.AddNewApplyRecord(dto1);
                            //log.Debug("保存图片成功-------");
                        }
                        else
                        {
                            //log.Debug("保存图片失败-------");
                            return Json(new AjaxResult() { Status = "error", ErrorMsg = result.Data.ToString() });
                        }
                    }
                    catch (Exception ex)
                    {

                        log.Debug("图片保存出错：" + ex.Message);
                        return Json(new AjaxResult() { Status = "error", ErrorMsg = ex.Message });
                    }
                }

            }
            return Json(new AjaxResult() { Status = "ok" });
        }

        private async System.Threading.Tasks.Task<AjaxResult> SaveImgAsync(HttpPostedFileBase file)
        {
            var uri = serverUrl + "/Home/FileUpload/";
            Guid guid = Guid.NewGuid();
            using (var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.None })
            using (HttpClient httpClient = new HttpClient(handler))
            {

                log.Debug("old-URI:" + httpClient.BaseAddress);
                log.Debug("serverUrl:" + serverUrl);
                log.Debug("string-URI:" + uri);
                httpClient.BaseAddress = new Uri(uri);
                //日志
                log.Debug("new-URI:" + httpClient.BaseAddress);
                //Bitmap bitmap = new Bitmap(memoryStream);
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Headers.Add("uid", "sdbsy");//增加账号密码，防止恶意上传
                content.Headers.Add("pwd", "ysbds");
                content.Headers.Add("path", "InvoicePics");
                content.Add(new StreamContent(file.InputStream), "file", guid.ToString() + ".png");
                var respMsg = httpClient.PostAsync(httpClient.BaseAddress, content).Result;
                string msgBody = await respMsg.Content.ReadAsStringAsync();
                //日志
                log.Debug("开始获取服务器保存的结果-------");
                if (respMsg.IsSuccessStatusCode)
                {
                    log.Debug("开始获取服务器结果成功！！！！！");
                    //请求成功
                    var result = (AjaxResult)Newtonsoft.Json.JsonConvert.DeserializeObject(msgBody, typeof(AjaxResult));
                    result.Data = serverUrl + result.Data;
                    result.Status = "ok";
                    return result;
                }
                else
                {
                    log.Debug("开始获取服务器结果失败！！！！！");
                    return new AjaxResult { Status = "error", ErrorMsg = msgBody };
                }

            }
        }
        public ActionResult List()
        {
            var id = (long)AdminHelper.GetUserId(HttpContext);
            var teacher = teacherSvc.GetByAdminId(id);
            if(teacher==null)
            {
                return View("Error", (object)"请先添加教师信息");
            }
            

        }
    }
}