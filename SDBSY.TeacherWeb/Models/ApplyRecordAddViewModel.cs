using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDBSY.TeacherWeb.Models
{
    public class ApplyRecordAddViewModel
    {
        public long GoodsId { get; set; }
        public string Goods { get; set; }
        public long ClassId { get; set; }
        public long TeacherId { get; set; }
        public string Classes { get; set; }

    }
}