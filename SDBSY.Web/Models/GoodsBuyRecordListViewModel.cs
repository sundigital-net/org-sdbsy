using SDBSY.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDBSY.Web.Models
{
    public class GoodsBuyRecordListViewModel
    {
        public long GoodsId { get; set; }
        public GoodsDTO[] Goods { get; set; }
        public GoodsAllRecordDTO[] Records { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}