using CodeCarvings.Piczard;
using FileUploadServer.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FileUploadServer.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return Content("home/index");
        }
        [HttpPost]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            MyJsonResult result = new MyJsonResult();
            if(file==null|| file.ContentLength<=0)
            {
                result.Status = "error";
                result.Data = "no file";
                return Json(result);
            }

            if (!CheckFileSizeAndType(file))
            {
                result.Status = "error";
                result.Data = "文件类型错误或大小超过5M";
                return Json(result);
            }
            string uid = Request.Headers["uid"];
            string pwd = Request.Headers["pwd"];
            string path = Request.Headers["path"];
            if (uid=="sdbsy"&&pwd=="ysbds")
            {
                Guid guid = Guid.NewGuid();
                //源文件
                string filePath = "/upload/"+path+"/" + DateTime.Now.ToString("yyyyMMdd") + "/" + guid.ToString() + Path.GetExtension(file.FileName);
                string fullPath = HttpContext.Server.MapPath("~" + filePath);//获取全路径
                new FileInfo(fullPath).Directory.Create();
                file.SaveAs(fullPath);
                file.InputStream.Position = 0;
                //缩略图
                var thumbPath="/upload/"+path+"Thumb/"+ DateTime.Now.ToString("yyyyMMdd") + "/" + guid.ToString() + Path.GetExtension(file.FileName);
                string thumbFullPath = HttpContext.Server.MapPath("~" + thumbPath);//获取全路径
                new FileInfo(thumbFullPath).Directory.Create();
                ImageProcessingJob jobThumb = new ImageProcessingJob();
                jobThumb.Filters.Add(new FixedResizeConstraint(200, 200));//缩略图尺寸 200*200
                jobThumb.SaveProcessedImageToFileSystem(file.InputStream, thumbFullPath);



                result.Status = "ok";
                result.Data = filePath+"|"+ thumbPath;
                return Json(result);
            }
            else
            {
                result.Status = "error";
                result.Data = "uid or pwd is wrong";
                return Json(result);
            }
        }

        private bool CheckFileSizeAndType(HttpPostedFileBase file)
        {
            var result = false;
            var extList = new List<string>(){".jpeg",".jpg",".gif",".png",".bmp"};
            var ext = Path.GetExtension(file.FileName);
            if (extList.Contains(ext))
            {
                result = true;
            }
            else
            {
                result = false;
            }

            if (result)
            {
                var size = file.ContentLength;
                if (size < 1024 * 5 * 1024)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }
    }
}