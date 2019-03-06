using SDBSY.Common;
using SDBSY.DTO;
using SDBSY.IService;
using SDBSY.Web.App_Start;
using SDBSY.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace SDBSY.Web.Controllers
{
    public class GoodsController : Controller
    {
        public IGoodsService goodsSvc { get; set; }
        public IAdminLogService logSvc { get; set; }
        // GET: Goods
        [CheckPermission("Goods.Index")]
        public ActionResult Index(long id)
        {
            return View();
        }
        [HttpGet]
        [CheckPermission("Goods.Add")]
        public ActionResult Add()
        {
            return View();
        }
        [HttpPost]
        [CheckPermission("Goods.Add")]
        public ActionResult Add(GoodsAddPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = MVCHelper.GetValidMsg(ModelState) });
            }
            using (var tran = new TransactionScope())
            {
                try
                {
                    long id = goodsSvc.AddNew(model.Name, model.Unit, model.Seller, model.Maker,model.Format);
                    long userId = (long)AdminHelper.GetUserId(HttpContext);
                    logSvc.AddNew(userId, "添加物品,id=" + id);
                    tran.Complete();
                    return Json(new AjaxResult { Status = "ok" });
                }
                catch (Exception ex)
                {
                    return Json(new AjaxResult { Status = "error", ErrorMsg = ex.Message });
                }
            }
        }
        [CheckPermission("Goods.List")]
        public ActionResult List()
        {
            var goods = goodsSvc.GetAll();
            return View(goods);
        }
        [HttpPost]
        [CheckPermission("Goods.Delete")]
        public ActionResult Delete(long id)
        {
            using (var tran = new TransactionScope())
            {
                try
                {
                    goodsSvc.Delete(id);
                    long userId = (long)AdminHelper.GetUserId(HttpContext);
                    logSvc.AddNew(userId, "删除物品" + id);
                    tran.Complete();
                    return Json(new AjaxResult { Status = "ok" });
                }
                catch (Exception ex)
                {
                    return Json(new AjaxResult { Status = "error", ErrorMsg = ex.Message });
                }
            }
        }
        [HttpPost]
        [CheckPermission("Goods.Delete")]
        public ActionResult PatchDel(long[] selectedIds)
        {
            if (selectedIds == null || selectedIds.Length <= 0)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "未选中任何信息" });
            }
            long userId = (long)AdminHelper.GetUserId(HttpContext);
            foreach (long id in selectedIds)
            {
                goodsSvc.Delete(id);
                logSvc.AddNew(userId, "删除物品,id=" + id);
            }
            return Json(new AjaxResult { Status = "ok" });
        }
        [HttpPost]
        [CheckPermission("GoodsBuyRecord.Delete")]
        public ActionResult BuyRecordDelete(long id)
        {
            using (var tran = new TransactionScope())
            {
                try
                {
                    goodsSvc.RecordDelete(id);
                    long userId = (long)AdminHelper.GetUserId(HttpContext);
                    logSvc.AddNew(userId, "删除入库记录" + id);
                    tran.Complete();
                    return Json(new AjaxResult { Status = "ok" });
                }
                catch(Exception ex)
                {
                    return Json(new AjaxResult { Status = "error", ErrorMsg = ex.Message });
                }
            }
        }
        [HttpPost]
        [CheckPermission("GoodsBuyRecord.Delete")]
        public ActionResult RecordsPatchDel(long[] selectedIds)
        {
            if(selectedIds==null||selectedIds.Length<=0)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "未选中任何信息" });
            }
            long userId = (long)AdminHelper.GetUserId(HttpContext);
            foreach(long id in selectedIds)
            {
                goodsSvc.RecordDelete(id);
                logSvc.AddNew(userId, "删除入库记录,id=" + id);
            }
            return Json(new AjaxResult { Status = "ok" });
        }
        [CheckPermission("GoodsAllRecord.List")]
        public ActionResult BuyRecordsList(long id, DateTime? startTime, DateTime? endTime)
        {
            //食材
            var foods = goodsSvc.GetAll().ToList();
            foods.Insert(0, new GoodsDTO { Name = "全部", Id = 0 });
            //食物的入库记录
            GoodsAllRecordDTO[] records;
            if (id <= 0)
            {//所有记录
                records = goodsSvc.GetAllRecords();
            }
            else
            {
                records = goodsSvc.GetAllRecords(id);
            }
            if (startTime != null)
            {
                records = records.Where(t => t.BuyTime >= startTime.Value).ToArray();
            }
            if (endTime != null)
            {
                records = records.Where(t => t.BuyTime < endTime.Value.AddDays(1)).ToArray();
            }

            GoodsBuyRecordListViewModel model = new GoodsBuyRecordListViewModel()
            {
                Goods = foods.ToArray(),
                Records = records,
                GoodsId = id,
                StartTime = startTime == null ? "" : startTime.Value.ToShortDateString(),
                EndTime = endTime == null ? "" : endTime.Value.ToShortDateString(),
            };
            return View(model);
        }
        [HttpPost]
        [CheckPermission("GoodsBuyRecord.Add")]
        public ActionResult BuyRecordAdd()
        {
            var goods = goodsSvc.GetAll();
            return View(goods);
        }
        [HttpPost]
        [CheckPermission("GoodsBuyRecord.Add")]
        public ActionResult BuyRecordAdd(GoodsBuyRecordAddPostModel model)
        {
            if(!ModelState.IsValid)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = MVCHelper.GetValidMsg(ModelState) });
            }
            using (var tran = new TransactionScope())
            {
                try
                {
                    long id = goodsSvc.AddNewGoodsBuyRecord(model.GoodsId, model.BuyTime, model.Amount, model.UnitPrice, model.TotalPrice, model.Remark);
                    long userId = (long)AdminHelper.GetUserId(HttpContext);
                    logSvc.AddNew(userId, "添加入库记录,id" + id);
                    tran.Complete();
                    return Json(new AjaxResult { Status = "ok" });
                }
                catch (Exception ex)
                {
                    return Json(new AjaxResult
                    {
                        Status = "error",
                        ErrorMsg = ex.Message
                    });
                }
            }
        }
        public ActionResult ApplyRecordDelete(long id)
        {
            using (var tran = new TransactionScope())
            {
                try
                {
                    goodsSvc.RecordDelete(id);
                    long userId = (long)AdminHelper.GetUserId(HttpContext);
                    logSvc.AddNew(userId, "删除申领记录" + id);
                    tran.Complete();
                    return Json(new AjaxResult { Status = "ok" });
                }
                catch (Exception ex)
                {
                    return Json(new AjaxResult { Status = "error", ErrorMsg = ex.Message });
                }
            }
        }
        [HttpPost]
        [CheckPermission("GoodsApplyRecord.Delete")]
        public ActionResult ApplyRecordsPatchDel(long[] selectedIds)
        {
            if (selectedIds == null || selectedIds.Length <= 0)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "未选中任何信息" });
            }
            long userId = (long)AdminHelper.GetUserId(HttpContext);
            foreach (long id in selectedIds)
            {
                goodsSvc.RecordDelete(id);
                logSvc.AddNew(userId, "删除申领记录,id=" + id);
            }
            return Json(new AjaxResult { Status = "ok" });
        }
        [CheckPermission("GoodsAllApplyRecord.List")]
        public ActionResult ApplyRecordsList(long id, DateTime? applyTime, DateTime? returnTime)
        {
            //食材
            var goods = goodsSvc.GetAll().ToList();
            goods.Insert(0, new GoodsDTO { Name = "全部", Id = 0 });
            //食物的入库记录
            GoodsAllApplyRecordDTO[] records;
            if (id <= 0)
            {//所有记录
                records = goodsSvc.GetAllApplyRrcord();
            }
            else
            {
                records = goodsSvc.GetAllApplyRrcord(id);
            }
            if (applyTime != null)
            {
                records = records.Where(t => t.ApplyTime >= applyTime.Value).ToArray();
            }
            if (returnTime != null)
            {
                records = records.Where(t => t.ReturnTime < returnTime.Value.AddDays(1)).ToArray();
            }

            GoodsApplyRecordsListView model = new GoodsApplyRecordsListView()
            {
                Goods = goods.ToArray(),
                Records = records,
                GoodsId = id,
                ApplyTime = applyTime == null ? "" : applyTime.Value.ToShortDateString(),
                ReturnTime = returnTime == null ? "" : returnTime.Value.ToShortDateString(),
            };
            return View(model);
        }
        [HttpPost]
        [CheckPermission("GoodsAllApplyRecord.Add")]
        public ActionResult ApplyRecordAdd()
        {
            var goods = goodsSvc.GetAll();
            return View(goods);
        }
        [HttpPost]
        [CheckPermission("GoodsBuyRecord.Add")]
        public ActionResult ApplyRecordAdd(GoodsApplyRecordAddPostModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = MVCHelper.GetValidMsg(ModelState) });
            }
            using (var tran = new TransactionScope())
            {
                try
                {
                    long id = goodsSvc.AddNewGoodsApplyRecord(model.GoodsId,model.ClassId,model.TeacherId,(DateTime)model.ApplyTime, (DateTime)model.ReturnTime);
                    long userId = (long)AdminHelper.GetUserId(HttpContext);
                    logSvc.AddNew(userId, "添加申领记录,id" + id);
                    tran.Complete();
                    return Json(new AjaxResult { Status = "ok" });
                }
                catch (Exception ex)
                {
                    return Json(new AjaxResult
                    {
                        Status = "error",
                        ErrorMsg = ex.Message
                    });
                }
            }
        }


        }
}