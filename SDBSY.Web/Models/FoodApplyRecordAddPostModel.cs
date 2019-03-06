using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDBSY.Web.Models
{
    public class FoodApplyRecordAddPostModel
    {
        [Required(ErrorMessageResourceName ="物品id必填")]
        public long GoodsId { get; set; }
        [Required(ErrorMessageResourceName ="班级名称必填")]
        public long ClassId { get; set; }
        [Required(ErrorMessageResourceName ="教师姓名必填")]
        public long TeacherId { get; set; }
        [Required(ErrorMessageResourceName ="数量必填")]
        public int Amount { get; set; }
    }
}