using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDBSY.TeacherWeb.Models
{
    public class ApplyRecordAddViewModel
    {
        public long GoodsId { get; set; }
        public long ClassId { get; set; }
        public long TeacherId { get; set; }
        public int Amount { get; set; }
        public string Classes { get; set; }
        public DateTime? ApplyTime { get; set; }//领取时间
        public DateTime? ReturnTime { get; set; }//归还时间

    }
}