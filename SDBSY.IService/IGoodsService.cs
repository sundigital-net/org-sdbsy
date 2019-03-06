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
        long AddNew(string name, string unit, string seller, string maker,string format);
        GoodsDTO[] GetAll();
        GoodsDTO GetById(long id);
        void Delete(long id);
        long AddNewGoodsBuyRecord(long goodsId, DateTime buytime, decimal amount, decimal unitprice, decimal totalprice, string remark);
        GoodsAllRecordDTO[] GetAllRecords(long goodsId);
        GoodsAllRecordDTO[] GetAllRecords(DateTime startTime, DateTime endTime);
        GoodsAllRecordDTO[] GetAllRecords();
        GoodsAllRecordDTO[] GetAllRecords(long[] ids);
        long AddNewGoodsApplyRecord(long goodsId, long classId, long teacherId, DateTime applytime, DateTime returntime);
        GoodsAllApplyRecordDTO[] GetAllApplyRrcord(long goodsId);
        GoodsAllApplyRecordDTO[] GetAllApplyRrcord(DateTime applytime, DateTime returntime);
        GoodsAllApplyRecordDTO[] GetAllApplyRrcord();
        GoodsAllApplyRecordDTO[] GetAllApplyRrcord(long[] ids);
        void RecordDelete(long id);

    }
}
