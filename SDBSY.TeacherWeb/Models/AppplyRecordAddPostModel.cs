using SDBSY.TeacherWeb.App_Start;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDBSY.TeacherWeb.Models
{
    public class AppplyRecordAddPostModel
    {
        public long TeacherId { get; set; }
        [Display(Name = "物品名称")]
        [Required(ErrorMessage = "请输入{0}")]
        public long GoodsId { get; set; }
        [Display(Name = "申领班级")]
        [Required(ErrorMessage = "请选择{0}")]
        public long ClassId { get; set; }
        [Display(Name = "物品数量")]
        [Required(ErrorMessage = "请输入{0}")]
        public int Amount { get; set; }//数量
    }
}