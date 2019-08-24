using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSY.DTO
{
    public class GoodsApplyRecordAddNewDTO
    {
        public long GoodsId { get; set; }
        public long ClassId { get; set; }
        public long TeacherId { get; set; }
        public int Amount { get; set; }//数量
    }
}
