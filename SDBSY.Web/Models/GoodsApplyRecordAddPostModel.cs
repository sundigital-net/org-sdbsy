using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDBSY.Web.Models
{
    public class GoodsApplyRecordAddPostModel
    {
        [Required(ErrorMessage = "物品名称必填")]
        public long GoodsId { get; set; }
        [Required(ErrorMessage = "班级必填")]
        public long ClassId { get; set; }
        [Required(ErrorMessage = "教师姓名必填")]
        public long TeacherId { get; set; }
        [Required(ErrorMessage = "数量必填")]
        public int Amount { get; set; }
        public int Status { get; set; }
        public DateTime? ApplyTime { get; set; }
        public DateTime? ReturnTime { get; set; }
    }
}