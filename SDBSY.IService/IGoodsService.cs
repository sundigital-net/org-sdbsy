using SDBSY.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSY.IService
{
    public interface IGoodsService : IServiceSupport
    {
        long AddGoodsType(long id, string name);//goodsType 
        long AddNewOrEdit(long id,long goodTypeId,string name, string unit, string seller, string maker,string format);
        GoodsDTO[] GetAll();
        GoodsDTO GetById(long id);
        GoodsTypeDTO[] GetAllTypes();
        GoodsTypeDTO GetTypeById(long id);
        void Delete(long id);
        long AddNewGoodsBuyRecord(long goodsId, DateTime buytime, decimal amount, decimal unitprice, decimal totalprice, string remark);
        GoodsBuyRecordDTO[] GetAllRecords(long goodsId);
        GoodsBuyRecordDTO[] GetAllRecords(DateTime startTime, DateTime endTime);
        GoodsBuyRecordDTO[] GetAllRecords();
        GoodsBuyRecordDTO[] GetAllRecords(long[] ids);
        /*long AddNewGoodsApplyRecord(long goodsId, long classId, long teacherId, DateTime applytime, DateTime returntime);*/
        GoodsApplyRecordDTO[] GetAllApplyRrcord(long goodsId);
        GoodsApplyRecordDTO[] GetAllApplyRrcord(DateTime applytime, DateTime returntime);
        GoodsApplyRecordDTO[] GetAllApplyRrcord();
        GoodsApplyRecordDTO[] GetAllApplyRrcordByTeacherId(long teacherId);
        GoodsApplyRecordDTO[] GetAllApplyRrcord(long[] ids);
        GoodsApplyRecordDTO GetOne(long id);
        long AddNewApplyRecord(GoodsApplyRecordAddNewDTO dto);
        void BuyRecordDelete(long id);
        void ApplyRecordDelete(long id);
        void ApplyRecordShenhe(long id, int status, string msg);

    }
}
