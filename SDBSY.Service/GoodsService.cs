using SDBSY.Common;
using SDBSY.DTO;
using SDBSY.IService;
using SDBSY.Service.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSY.Service
{
    public class GoodsService : IGoodsService
    {
        public long AddNewOrEdit(long id, long goodTypeId, string name, string unit, string seller, string maker,string format)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                if (id <= 0)
                {
                    GoodsEntity entity = new GoodsEntity()
                    {
                        GoodsTypeId=goodTypeId,
                        Name = name,
                        Unit = unit,
                        Seller = seller,
                        Maker = maker,
                        Format = format
                    };
                    mc.Goods.Add(entity);
                    mc.SaveChanges();
                    return entity.Id;
                }
                else
                {
                    var bs=new BaseService<GoodsEntity>(mc);
                    var entity = bs.GetById(id);
                    entity.GoodsTypeId = goodTypeId;
                    entity.Format = format;
                    entity.Name = name;
                    entity.Unit = unit;
                    entity.Seller = seller;
                    entity.Maker = maker;
                    mc.SaveChanges();
                    return id;
                }
            }
        }
        /*
        public long AddNewGoodsApplyRecord(long goodsId, long classId, long teacherId,DateTime applytime, DateTime returntime)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                GoodsApplyRecordEntity entity = new GoodsApplyRecordEntity()
                {
                    GoodsId = goodsId,
                    ClassId = classId,
                    TeacherId = teacherId,
                    ApplyTime = applytime,
                    ReturnTime = returntime
                };
                mc.GoodsApplyRecords.Add(entity);
                mc.SaveChanges();
                return entity.Id;
            }
        }
        */
        public long AddNewGoodsBuyRecord(long goodsId, DateTime buytime, decimal amount, decimal unitprice, decimal totalprice, string remark)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                GoodsBuyRecordEntity entity = new GoodsBuyRecordEntity()
                {
                    GoodsId = goodsId,
                    BuyTime = buytime,
                    Amount = amount,
                    UnitPrice = unitprice,
                    TotalPrice = totalprice,
                    Remark = remark,
                };
                mc.GoodsBuyRecords.Add(entity);
                mc.SaveChanges();
                return entity.Id;
            }
        }

        public void Delete(long id)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsEntity> bs = new BaseService<GoodsEntity>(mc);
                bs.MarkDeleted(id);
            }
        }
        private GoodsDTO ToDTO(GoodsEntity entity)
        {
            GoodsDTO dto = new GoodsDTO()
            {
                GoodsTypeId = entity.GoodsTypeId,
                GoodsTypeName = entity.GoodsType.Name,
                CreateDateTime = entity.CreateDateTime,
                Id = entity.Id,
                Name = entity.Name,
                Unit = entity.Unit,
                Maker = entity.Maker,
                Seller = entity.Seller,
                Format = entity.Format,
                ImgUrl = entity.ImgUrl,
            };
            dto.Maker = string.IsNullOrEmpty(entity.Maker) ? "未知" : entity.Maker;
            return dto;
        }

        public GoodsDTO[] GetAll()
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsEntity> bs = new BaseService<GoodsEntity>(mc);
                var goods = bs.GetAll().Include(t=>t.GoodsType).ToList();
                List<GoodsDTO> list = new List<GoodsDTO>();
                foreach (var item in goods)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }
        public GoodsApplyRecordDTO[] GetAllApplyRrcord(DateTime applytime, DateTime returntime)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsApplyRecordEntity> bs = new BaseService<GoodsApplyRecordEntity>(mc);
                var end = returntime.AddDays(1);
                var records = bs.GetAll().Include(t => t.Goods).Where(t => t.ApplyTime >= applytime && t.ReturnTime < end);
                List<GoodsApplyRecordDTO> list = new List<GoodsApplyRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        private GoodsApplyRecordDTO ToDTO(GoodsApplyRecordEntity entity)
        {
            var dto = new GoodsApplyRecordDTO();
            dto.CreateDateTime = entity.CreateDateTime;
            dto.Id = entity.Id;
            dto.GoodsId = entity.GoodsId;
            dto.GoodsName = entity.Goods.Name;
            dto.ClassId = entity.ClassId;
            dto.ClassName = entity.Class.Value;
            dto.TeacherId = entity.TeacherId;
            dto.TeacherName = entity.Teacher.Name;
            dto.Amount = entity.Amount;
            dto.Status = entity.Status;
            dto.ApplyTime = entity.ApplyTime;
            dto.ReturnTime = entity.ReturnTime;
            dto.NoPassReason = entity.NoPassReason;
            
            switch (entity.Status)
            {
                case ShenHeZhuangTai.MoRen:
                    dto.StatusStr = "审核中";
                    break;
                case ShenHeZhuangTai.BoHui:
                    dto.StatusStr = "未通过";
                    break;
                case ShenHeZhuangTai.TongGuo:
                    dto.StatusStr = "通过";
                    break;
                default:
                    dto.StatusStr = "未知状态";
                    break;
            }
            return dto;
        }

        public GoodsApplyRecordDTO[] GetAllApplyRrcord()
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsApplyRecordEntity> bs = new BaseService<GoodsApplyRecordEntity>(mc);
                var records = bs.GetAll().Include(t => t.Goods).ToList();
                List<GoodsApplyRecordDTO> list = new List<GoodsApplyRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        public GoodsApplyRecordDTO[] GetAllApplyRrcord(long[] ids)
        {
            if (ids == null || ids.Length < 0)
            {
                throw new ArgumentException("没有选择任何信息");
            }
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsApplyRecordEntity> bs = new BaseService<GoodsApplyRecordEntity>(mc);
                var records = bs.GetAll().Include(t => t.Goods).AsNoTracking().Where(t => ids.Contains(t.Id)).OrderBy(t => t.ApplyTime).ToArray();
                List<GoodsApplyRecordDTO> list = new List<GoodsApplyRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        public GoodsApplyRecordDTO[] GetAllApplyRrcord(long goodsId)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsApplyRecordEntity> bs = new BaseService<GoodsApplyRecordEntity>(mc);
                var records = bs.GetAll().Include(t => t.Goods).Where(t => t.GoodsId == goodsId);
                List<GoodsApplyRecordDTO> list = new List<GoodsApplyRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        public GoodsBuyRecordDTO[] GetAllRecords(DateTime startTime, DateTime endTime)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsBuyRecordEntity> bs = new BaseService<GoodsBuyRecordEntity>(mc);
                var end = endTime.AddDays(1);
                var records = bs.GetAll().Include(t => t.Goods).Where(t => t.BuyTime >= startTime && t.BuyTime < end);
                List<GoodsBuyRecordDTO> list = new List<GoodsBuyRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        private GoodsBuyRecordDTO ToDTO(GoodsBuyRecordEntity entity)
        {
            GoodsBuyRecordDTO dto = new GoodsBuyRecordDTO()
            {
                GoodsName =entity.Goods.Name,
                Unit=entity.Goods.Unit,
                Id = entity.Id,
                GoodsId = entity.GoodsId,
                BuyTime = entity.BuyTime,
                Amount = entity.Amount,
                UnitPrice = entity.UnitPrice,
                TotalPrice = entity.TotalPrice,
                Remark = entity.Remark,
            };
            return dto;
        }

        public GoodsBuyRecordDTO[] GetAllRecords(long[] ids)
        {
            if (ids == null || ids.Length < 0)
            {
                throw new ArgumentException("没有选择任何信息");
            }
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsBuyRecordEntity> bs = new BaseService<GoodsBuyRecordEntity>(mc);
                var records = bs.GetAll().Include(t => t.Goods).AsNoTracking().Where(t => ids.Contains(t.Id)).OrderBy(t => t.BuyTime).ToArray();
                List<GoodsBuyRecordDTO> list = new List<GoodsBuyRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        public GoodsDTO GetById(long id)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsEntity> bs = new BaseService<GoodsEntity>(mc);
                var goods = bs.GetById(id);
                return goods == null ? null : ToDTO(goods);
            }
        }

        public GoodsBuyRecordDTO[] GetAllRecords(long goodsId)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsBuyRecordEntity> bs = new BaseService<GoodsBuyRecordEntity>(mc);
                var records = bs.GetAll().Include(t => t.Goods).Where(t => t.GoodsId == goodsId);
                List<GoodsBuyRecordDTO> list = new List<GoodsBuyRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        public GoodsBuyRecordDTO[] GetAllRecords()
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsBuyRecordEntity> bs = new BaseService<GoodsBuyRecordEntity>(mc);
                var records = bs.GetAll().Include(t => t.Goods);
                List<GoodsBuyRecordDTO> list = new List<GoodsBuyRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        public void BuyRecordDelete(long id)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsBuyRecordEntity> bs = new BaseService<GoodsBuyRecordEntity>(mc);
                bs.MarkDeleted(id);
            }
        }

        public long AddNewApplyRecord(GoodsApplyRecordAddNewDTO dto)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                GoodsApplyRecordEntity entity = new GoodsApplyRecordEntity()
                {
                    GoodsId = dto.GoodsId,
                    ClassId = dto.ClassId,
                    TeacherId = dto.TeacherId,
                    Amount = dto.Amount,
                    Status = ShenHeZhuangTai.MoRen
                    //ApplyTime = applytime,
                    //ReturnTime = returntime
                };
                mc.GoodsApplyRecords.Add(entity);
                mc.SaveChanges();
                return entity.Id;
            }
        }

        public GoodsApplyRecordDTO[] GetAllApplyRrcordByTeacherId(long teacherId)
        {
            using (var mc =new MyDBContext())
            {
                var bs = new BaseService<GoodsApplyRecordEntity>(mc);
                var records= bs.GetAll().Include(t => t.Class).Include(t => t.Goods).Include(t => t.Teacher)
                    .Where(t => t.TeacherId == teacherId).ToList();
                var list=new List<GoodsApplyRecordDTO>();
                foreach (var record in records)
                {
                    list.Add(ToDTO(record));
                }

                return list.ToArray();
            }
        }

        public void ApplyRecordDelete(long id)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                var bs = new BaseService<GoodsApplyRecordEntity>(mc);
                bs.MarkDeleted(id);
            }
        }

        public GoodsApplyRecordDTO GetOne(long id)
        {
            using (var mc=new MyDBContext())
            {
                var bs=new BaseService<GoodsApplyRecordEntity>(mc);
                var record= bs.GetAll().Include(t => t.Class).Include(t => t.Goods).Include(t => t.Teacher)
                    .SingleOrDefault(t => t.Id == id);
                return record == null ? null : ToDTO(record);
            }
        }

        public void ApplyRecordShenhe(long id, int status, string msg)
        {
            using (var mc = new MyDBContext())
            {
                var bs = new BaseService<GoodsApplyRecordEntity>(mc);
                var record = bs.GetById(id);
                if (record == null)
                {
                    throw new ArgumentException("不存在申领信息，id=" + id);
                }

                record.Status = status;
                record.NoPassReason = status == ShenHeZhuangTai.TongGuo ? string.Empty : msg;
                mc.SaveChanges();
            }
        }

        private GoodsTypeDTO ToDTO(GoodsTypeEntity entity)
        {
            var dto=new GoodsTypeDTO();
            dto.Name = entity.Name;
            dto.CreateDateTime = entity.CreateDateTime;
            dto.Id = entity.Id;
            return dto;
        }
        public GoodsTypeDTO[] GetAllTypes()
        {
            using (var mc = new MyDBContext())
            {
                var bs=new BaseService<GoodsTypeEntity>(mc);
                var types = bs.GetAll();
                var list=new List<GoodsTypeDTO>();
                foreach (var type in types)
                {
                    list.Add(ToDTO(type));
                }

                return list.ToArray();
            }
        }

        public GoodsTypeDTO GetTypeById(long id)
        {
            using (var mc = new MyDBContext())
            {
                var bs = new BaseService<GoodsTypeEntity>(mc);
                var type = bs.GetById(id);
                return type == null ? null : ToDTO(type);
            }
        }

        public long AddGoodsType(long id, string name)//goodsType 
        {
            if (id <= 0)
            {
                using (var mc = new MyDBContext())
                {
                    var bs = new BaseService<GoodsTypeEntity>(mc);
                    if (bs.GetAll().Any(t => t.Name == name))
                    {
                        return -1;//存在相同的名称
                    }
                    var entity=new GoodsTypeEntity();
                    entity.Name = name;
                    mc.GoodsTypes.Add(entity);
                    mc.SaveChanges();
                    return entity.Id;
                }
            }
            else
            {
                using (var mc=new MyDBContext())
                {
                    var bs=new BaseService<GoodsTypeEntity>(mc);
                    if (bs.GetAll().Any(t => t.Id != id && t.Name == name))
                    {
                        return -1;//存在相同的名称
                    }

                    var entity = bs.GetById(id);
                    entity.Name = name;
                    mc.SaveChanges();
                    return id;
                }
            }
            
        }
    }
}
