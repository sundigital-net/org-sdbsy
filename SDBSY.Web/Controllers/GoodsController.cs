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
        public IAdminUserService adminSvc { get; set; }
        public IAdminLogService logSvc { get; set; }

        #region 物品类型
        [CheckPermission("GoodsType.List")]
        public ActionResult GoodsTypeList()
        {
            var types = goodsSvc.GetAllTypes();
            return View(types);
        }
        [CheckPermission("GoodsType.Add")]
        public ActionResult GoodsTypeAdd()
        {
            return View();
        }

        [HttpPost]
        [CheckPermission("GoodsType.Add")]
        public ActionResult GoodsTypeAdd(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new AjaxResult() {Status = "error",ErrorMsg = "名称必填"});
            }

            var typeId= goodsSvc.AddGoodsType(0,name);
            if (typeId <= 0)
            {
                return Json(new AjaxResult() { Status = "error", ErrorMsg = "已存在相同的名称" });
            }
            return Json(new AjaxResult() { Status ="ok"});
        }

        [HttpGet]
        [CheckPermission("GoodsType.Edit")]
        public ActionResult GoodsTypeEdit(long id)
        {
            var type = goodsSvc.GetTypeById(id);
            return View(type);
        }

        [HttpPost]
        [CheckPermission("GoodsType.Edit")]
        public ActionResult GoodsTypeEdit(long id, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return Json(new AjaxResult() {Status = "error", ErrorMsg = "名称必填"});
            }

            var typeId= goodsSvc.AddGoodsType(id, name);
            if (typeId <= 0)
            {
                return Json(new AjaxResult() { Status = "error", ErrorMsg = "已存在相同的名称" });
            }

            return Json(new AjaxResult() {Status = "ok"});
        }
        #endregion



        // GET: Goods
        [CheckPermission("Goods.Index")]
        public ActionResult Index(long id)
        {
            var types = goodsSvc.GetAllTypes();
            var goods = goodsSvc.GetById(id);
            var model=new GoodsIndexViewModel()
            {
                Goods = goods,
                GoodsTypes = types
            };
            return View(model);
        }

        
        [HttpGet]
        [CheckPermission("Goods.Add")]
        public ActionResult Add()
        {
            var types = goodsSvc.GetAllTypes();
            return View(types);
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
                    long id = goodsSvc.AddNewOrEdit(model.Id,model.GoodsTypeId,model.Name, model.Unit, model.Seller, model.Maker, model.Format);
                    long userId = (long)AdminHelper.GetUserId(HttpContext);
                    logSvc.AddNew(userId, "添加或编辑物品,id=" + id);
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
                    goodsSvc.BuyRecordDelete(id);
                    long userId = (long)AdminHelper.GetUserId(HttpContext);
                    logSvc.AddNew(userId, "删除入库记录" + id);
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
        [CheckPermission("GoodsBuyRecord.Delete")]
        public ActionResult RecordsPatchDel(long[] selectedIds)
        {
            if (selectedIds == null || selectedIds.Length <= 0)
            {
                return Json(new AjaxResult { Status = "error", ErrorMsg = "未选中任何信息" });
            }
            long userId = (long)AdminHelper.GetUserId(HttpContext);
            foreach (long id in selectedIds)
            {
                goodsSvc.BuyRecordDelete(id);
                logSvc.AddNew(userId, "删除入库记录,id=" + id);
            }
            return Json(new AjaxResult { Status = "ok" });
        }
        [CheckPermission("GoodsBuyRecord.List")]
        public ActionResult BuyRecordsList(long id, DateTime? startTime, DateTime? endTime)
        {
            //食材
            var goods = goodsSvc.GetAll().ToList();
            goods.Insert(0, new GoodsDTO { Name = "全部", Id = 0 });
            //食物的入库记录
            GoodsBuyRecordDTO[] records;
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
                Goods = goods.ToArray(),
                Records = records,
                GoodsId = id,
                StartTime = startTime == null ? "" : startTime.Value.ToShortDateString(),
                EndTime = endTime == null ? "" : endTime.Value.ToShortDateString(),
            };
            return View(model);
        }
        [HttpGet]
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
            if (!ModelState.IsValid)
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
        [HttpPost]
        [CheckPermission("GoodsApplyRecord.Delete")]
        public ActionResult ApplyRecordDelete(long id)
        {
            using (var tran = new TransactionScope())
            {
                try
                {
                    goodsSvc.ApplyRecordDelete(id);
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
                goodsSvc.ApplyRecordDelete(id);
                logSvc.AddNew(userId, "删除申领记录,id=" + id);
            }
            return Json(new AjaxResult { Status = "ok" });
        }
        [CheckPermission("GoodsApplyRecord.List")]
        public ActionResult ApplyRecordsList(long id, DateTime? startTime, DateTime? endTime)
        {
            var goods = goodsSvc.GetAll().ToList();
            goods.Insert(0, new GoodsDTO { Name = "全部", Id = 0 });
            GoodsApplyRecordDTO[] records;
            if (id <= 0)
            {//所有记录
                records = goodsSvc.GetAllApplyRrcord();
            }
            else
            {
                records = goodsSvc.GetAllApplyRrcord(id);
            }
            if (startTime != null)
            {
                records = records.Where(t => t.CreateDateTime >= startTime.Value).ToArray();
            }
            if (endTime != null)
            {
                records = records.Where(t => t.CreateDateTime < endTime.Value.AddDays(1)).ToArray();
            }

            GoodsApplyRecordsListViewModel model = new GoodsApplyRecordsListViewModel()
            {
                Goods = goods.ToArray(),
                Records = records,
                GoodsId = id,
                StartTime = startTime == null ? "" : startTime.Value.ToShortDateString(),
                EndTime = endTime == null ? "" : endTime.Value.ToShortDateString(),
            };
            return View(model);
        }
        [HttpGet]
        //[CheckPermission("GoodsApplyRecord.List")]
        public ActionResult ApplyRecordIndex(long id)
        {
            var userId = AdminHelper.GetUserId(HttpContext);
            if (userId == null)
            {
                return RedirectToAction("Signin", "Management");
            }

            var record = goodsSvc.GetOne(id);
            var goods = goodsSvc.GetById(record.GoodsId);
            //物品申领审核分为两类（暂定为两类）
            var hasPermission = adminSvc.HasPermission(userId.Value, "GoodsApplyRecord.ShenHe" + goods.GoodsTypeId.ToString());
            if (hasPermission)
            {
                return View(record);
            }
            else
            {
                return Content("抱歉，您无权限进行此操作");
            }
            
        }
        [HttpPost]
        //[CheckPermission("GoodsApplyRecord.ShenHe")]
        public ActionResult ApplyRecordShenhe(long goodsId, long id, int status, string msg)
        {
            var userId=AdminHelper.GetUserId(HttpContext);
            if (userId == null)
            {
                return RedirectToAction("Signin","Management");
            }

            var goods = goodsSvc.GetById(goodsId);
            //物品申领审核分为两类（暂定为两类）
            var hasPermission = adminSvc.HasPermission(userId.Value, "GoodsApplyRecord.ShenHe" + goods.GoodsTypeId.ToString());
            if (hasPermission)
            {
                goodsSvc.ApplyRecordShenhe(id, status, msg);
                return Json(new AjaxResult { Status = "ok" });
            }
            else
            {
                return Json(new AjaxResult() {Status = "error", ErrorMsg = "抱歉，您无权限进行此操作" });
            }
        }

    }
}