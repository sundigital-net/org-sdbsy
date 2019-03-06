using SDBSY.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDBSY.Web.Models
{
    public class GoodsApplyRecordsListView
    {
        public long GoodsId { get; set; }
        public GoodsDTO[] Goods { get; set; }
        public GoodsAllApplyRecordDTO[] Records { get; set; }
        public string ApplyTime { get; set; }
        public string ReturnTime { get; set; }
    }
}