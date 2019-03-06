using SDBSY.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SDBSY.Common;
using SDBSY.DTO;
using SDBSY.Web.Models;
using SDBSY.Web.App_Start;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;

namespace SDBSY.Web.Controllers
{
    public class InvoiceController : Controller
    {
        public IAdminLogService logSvc { get; set; }
        public IInvoiceService invSvc { get; set; }
        [CheckPermission("Invoice.List")]
        // GET: Invoice
        public ActionResult List()
        {
            var invoices= invSvc.GetAll();
            InvoiceListViewModel model=new InvoiceListViewModel();
            model.Invoices = invoices;
            return View(model);
        }
        [CheckPermission("Invoice.Index")]
        public ActionResult Index(long id)
        {
            var invoice = invSvc.GetById(id);
            var pics = invSvc.GetPics(id);
            var model=new InvoiceIndexViewModel();
            model.Invoice = invoice;
            model.Pics = pics;
            return View(model);
        }

        [HttpPost]
        [CheckPermission("Invoice.ShenHe")]
        public ActionResult Shenhe(long id, int status, string msg)
        {
            invSvc.Shenhe(id,status,msg);
            return Json(new AjaxResult {Status = "ok"});
        }

        [HttpPost]
        public ActionResult ExportExcle(long[] selectedIds)
        {
            var invoices = invSvc.GetAll(selectedIds);
            if (invoices == null || invoices.Length <= 0)
            {
                return Json(new AjaxResult() {Status = "error", ErrorMsg = "没有符合条件的数据"});
            }
            //导出
            string path = "";
            //WriteToExcel(students ,model.DicFields, out path);
            WriteToExcel(invoices, out path);
            path = path.Remove(0, 1);
            long adminId = (long)AdminHelper.GetUserId(HttpContext);
            logSvc.AddNew(adminId, "导出票据信息");
            return Json(new AjaxResult { Status = "ok", Data = path });
        }

        #region 导出

        public void WriteToExcel(InvoiceDTO[] invs, out string path)
        {
            var guid = Guid.NewGuid();

            var filePath = string.Empty;
            var filename = guid.ToString() + ".xls";
            path = "~/Download/File/" + DateTime.Now.ToString("yyyyMMdd") + "/" + filename;
            //临时存放路径
            filePath = Server.MapPath(path);
            new FileInfo(filePath).Directory.Create(); //尝试创建路径，即使存在也不会报错
            

            var hssfworkbook = new HSSFWorkbook();

            //
            ISheet sheet1 = hssfworkbook.CreateSheet();
            for (int i = 0; i < invs.Count(); i++)
            {
                //todo 
            }

            sheet1.ForceFormulaRecalculation = true;

            using (FileStream filess = System.IO.File.OpenWrite(filePath))
            {
                hssfworkbook.Write(filess);
            }
        }

        #endregion

        [HttpPost]
        [CheckPermission("Invoice.Delete")]
        public ActionResult Delete(long id)
        {
            invSvc.Delete(id);
            return Json(new AjaxResult() {Status = "ok"});
        }
    }
}