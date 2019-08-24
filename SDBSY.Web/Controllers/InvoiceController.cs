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
using System.Net;
using log4net;

namespace SDBSY.Web.Controllers
{
    public class InvoiceController : Controller
    {
        private static ILog log = LogManager.GetLogger(typeof(InvoiceController));
        public IAdminLogService logSvc { get; set; }
        public IInvoiceService invSvc { get; set; }
        public IDataDictionaryService dataSvc { get; set; }
        [CheckPermission("Invoice.List")]
        // GET: Invoice
        public ActionResult List(long? classId, DateTime? startTime, DateTime? endTime)
        {
            var invoices= invSvc.GetAll(classId,startTime,endTime);
            var classes = dataSvc.GetByName("ClassType").ToList();
            InvoiceListViewModel model=new InvoiceListViewModel();
            model.Invoices = invoices;
            model.Classes = classes.ToArray();
            model.ClassId = classId == null ? 0 : classId.Value;
            model.StartTime = startTime == null ? "" : startTime.Value.ToShortDateString();
            model.EndTime = endTime == null ? "" : endTime.Value.ToShortDateString();
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
            ISheet sheet = hssfworkbook.CreateSheet();
            HSSFPatriarch patriarch = (HSSFPatriarch)sheet.CreateDrawingPatriarch();
            #region 表头

            var rowTitle = sheet.CreateRow(0);
            var cell01 = rowTitle.CreateCell(0);
            cell01.SetCellValue("申请教师");
            var cell02 = rowTitle.CreateCell(1);
            cell02.SetCellValue("申请班级");
            var cell03 = rowTitle.CreateCell(2);
            cell03.SetCellValue("购买物品");
            var cell04 = rowTitle.CreateCell(3);
            cell04.SetCellValue("购买时间");
            var cell05 = rowTitle.CreateCell(4);
            cell05.SetCellValue("总金额(元)");
            var cell06 = rowTitle.CreateCell(5);
            cell06.SetCellValue("明细");
            var cell07 = rowTitle.CreateCell(6);
            cell07.SetCellValue("状态");
            var cell08 = rowTitle.CreateCell(7);
            cell08.SetCellValue("驳回原因");
            var cell09 = rowTitle.CreateCell(8);
            cell09.SetCellValue("存档照片");


            #endregion

            #region 数据

            for (int i = 0; i < invs.Count(); i++)
            {
                var row = sheet.CreateRow(i + 1);//创建数据行
                row.Height = 14 * 256;
                var cell1 = row.CreateCell(0);
                cell1.SetCellValue(invs[i].TeacherName);
                var cell2 = row.CreateCell(1);
                cell2.SetCellValue(invs[i].ClassName);
                var cell3 = row.CreateCell(2);
                cell3.SetCellValue(invs[i].GoodsName);
                var cell4 = row.CreateCell(3);
                cell4.SetCellValue(invs[i].BuyDateTimeStr);
                var cell5 = row.CreateCell(4);
                cell5.SetCellValue(invs[i].TotalStr);
                var cell6 = row.CreateCell(5);
                cell6.SetCellValue(invs[i].Detail);
                var cell7 = row.CreateCell(6);
                cell7.SetCellValue(invs[i].StatusStr);
                var cell8 = row.CreateCell(7);
                cell8.SetCellValue(invs[i].NoPassReason);
                var cell9 = row.CreateCell(8);
                var pic = invSvc.GetPics(invs[i].Id).ToList();//获取图片
                if (pic.Count>0)
                {
                    for (int j = 0; j < pic.Count; j++)
                    {
                        SetPic(hssfworkbook, patriarch, pic[j].ThumbUrl, sheet, i + 1, 8+j);
                    }
                    
                }
            }

            #endregion


            sheet.ForceFormulaRecalculation = true;

            using (FileStream filess = System.IO.File.OpenWrite(filePath))
            {
                hssfworkbook.Write(filess);
            }
        }

        #endregion
        private void SetPic(HSSFWorkbook workbook, HSSFPatriarch patriarch, string path, ISheet sheet, int rowline, int col)
        {
            if (string.IsNullOrEmpty(path)) return;
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    webClient.Credentials = CredentialCache.DefaultCredentials;
                    //byte[] bytes = System.IO.File.ReadAllBytes(Server.MapPath(path));//相对路径
                    byte[] bytes = webClient.DownloadData(path);
                    int pictureIdx = workbook.AddPicture(bytes, PictureType.PNG);
                    /*
                      参数的解析: HSSFClientAnchor（int dx1,int dy1,int dx2,int dy2,int col1,int row1,int col2,int row2)
    
                        dx1:图片左边相对excel格的位置(x偏移) 范围值为:0~1023;即输100 偏移的位置大概是相对于整个单元格的宽度的100除以1023大概是10分之一
    
                        dy1:图片上方相对excel格的位置(y偏移) 范围值为:0~256 原理同上。
    
                        dx2:图片右边相对excel格的位置(x偏移) 范围值为:0~1023; 原理同上。
    
                        dy2:图片下方相对excel格的位置(y偏移) 范围值为:0~256 原理同上。
    
                        col1和row1 :图片左上角的位置，以excel单元格为参考,比喻这两个值为(1,1)，那么图片左上角的位置就是excel表(1,1)单元格的右下角的点(A,1)右下角的点。
    
                        col2和row2:图片右下角的位置，以excel单元格为参考,比喻这两个值为(2,2)，那么图片右下角的位置就是excel表(2,2)单元格的右下角的点(B,2)右下角的点。
                     */
                    HSSFClientAnchor anchor = new HSSFClientAnchor(70, 10, 0, 0, col, rowline, col + 1, rowline + 1);
                    //把图片插到相应的位置
                    HSSFPicture pict = (HSSFPicture) patriarch.CreatePicture(anchor, pictureIdx);
                }
                catch (Exception ex)
                {
                    log.Error("图片生成出错，或没有图片"+ex.Message);
                }
            }
        }



        [HttpPost]
        [CheckPermission("Invoice.Delete")]
        public ActionResult Delete(long id)
        {
            invSvc.Delete(id);
            return Json(new AjaxResult() {Status = "ok"});
        }
    }
}