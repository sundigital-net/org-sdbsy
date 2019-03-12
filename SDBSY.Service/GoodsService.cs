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
        public long AddNew(string name, string unit, string seller, string maker,string format)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                GoodsEntity entity = new GoodsEntity()
                {
                    Name = name,
                    Unit = unit,
                    Seller = seller,
                    Maker = maker,
                    Format=format
                };
                mc.Goods.Add(entity);
                mc.SaveChanges();
                return entity.Id;
            }
        }

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
                var goods = bs.GetAll();
                List<GoodsDTO> list = new List<GoodsDTO>();
                foreach (var item in goods)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }
        public GoodsAllApplyRecordDTO[] GetAllApplyRrcord(DateTime applytime, DateTime returntime)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsApplyRecordEntity> bs = new BaseService<GoodsApplyRecordEntity>(mc);
                var end = returntime.AddDays(1);
                var records = bs.GetAll().Include(t => t.Goods).Where(t => t.ApplyTime >= applytime && t.ReturnTime < end);
                List<GoodsAllApplyRecordDTO> list = new List<GoodsAllApplyRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        private GoodsAllApplyRecordDTO ToDTO(GoodsApplyRecordEntity entity)
        {
            GoodsAllApplyRecordDTO dto = new GoodsAllApplyRecordDTO()
            {
                CreateDateTime = entity.CreateDateTime,
                Id = entity.Id,
                GoodsId = entity.GoodsId,
                ClassId = entity.ClassId,
                TeacherId = entity.TeacherId,
                Amount = entity.Amount,
                Status = entity.Status,
                ApplyTime = entity.ApplyTime,
                ReturnTime = entity.ReturnTime,
            };
            return dto;
        }

        public GoodsAllApplyRecordDTO[] GetAllApplyRrcord()
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsApplyRecordEntity> bs = new BaseService<GoodsApplyRecordEntity>(mc);
                var records = bs.GetAll().Include(t => t.Goods);
                List<GoodsAllApplyRecordDTO> list = new List<GoodsAllApplyRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        public GoodsAllApplyRecordDTO[] GetAllApplyRrcord(long[] ids)
        {
            if (ids == null || ids.Length < 0)
            {
                throw new ArgumentException("没有选择任何信息");
            }
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsApplyRecordEntity> bs = new BaseService<GoodsApplyRecordEntity>(mc);
                var records = bs.GetAll().Include(t => t.Goods).AsNoTracking().Where(t => ids.Contains(t.Id)).OrderBy(t => t.ApplyTime).ToArray();
                List<GoodsAllApplyRecordDTO> list = new List<GoodsAllApplyRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        public GoodsAllApplyRecordDTO[] GetAllApplyRrcord(long goodsId)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsApplyRecordEntity> bs = new BaseService<GoodsApplyRecordEntity>(mc);
                var records = bs.GetAll().Include(t => t.Goods).Where(t => t.GoodsId == goodsId);
                List<GoodsAllApplyRecordDTO> list = new List<GoodsAllApplyRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        public GoodsAllRecordDTO[] GetAllRecords(DateTime startTime, DateTime endTime)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsBuyRecordEntity> bs = new BaseService<GoodsBuyRecordEntity>(mc);
                var end = endTime.AddDays(1);
                var records = bs.GetAll().Include(t => t.Goods).Where(t => t.BuyTime >= startTime && t.BuyTime < end);
                List<GoodsAllRecordDTO> list = new List<GoodsAllRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        private GoodsAllRecordDTO ToDTO(GoodsBuyRecordEntity entity)
        {
            GoodsAllRecordDTO dto = new GoodsAllRecordDTO()
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

        public GoodsAllRecordDTO[] GetAllRecords(long[] ids)
        {
            if (ids == null || ids.Length < 0)
            {
                throw new ArgumentException("没有选择任何信息");
            }
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsBuyRecordEntity> bs = new BaseService<GoodsBuyRecordEntity>(mc);
                var records = bs.GetAll().Include(t => t.Goods).AsNoTracking().Where(t => ids.Contains(t.Id)).OrderBy(t => t.BuyTime).ToArray();
                List<GoodsAllRecordDTO> list = new List<GoodsAllRecordDTO>();
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

        public GoodsAllRecordDTO[] GetAllRecords(long goodsId)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsBuyRecordEntity> bs = new BaseService<GoodsBuyRecordEntity>(mc);
                var records = bs.GetAll().Include(t => t.Goods).Where(t => t.GoodsId == goodsId);
                List<GoodsAllRecordDTO> list = new List<GoodsAllRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        public GoodsAllRecordDTO[] GetAllRecords()
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsBuyRecordEntity> bs = new BaseService<GoodsBuyRecordEntity>(mc);
                var records = bs.GetAll().Include(t => t.Goods);
                List<GoodsAllRecordDTO> list = new List<GoodsAllRecordDTO>();
                foreach (var item in records)
                {
                    list.Add(ToDTO(item));
                }
                return list.ToArray();
            }
        }

        public void RecordDelete(long id)
        {
            using (MyDBContext mc = new MyDBContext())
            {
                BaseService<GoodsBuyRecordEntity> bs = new BaseService<GoodsBuyRecordEntity>(mc);
                bs.MarkDeleted(id);
            }
        }

        public long AddNewApplyRecord(GoodsAllApplyRecordDTO goodsAllApplyRecord)
        {
            throw new NotImplementedException();
        }
    }
}
