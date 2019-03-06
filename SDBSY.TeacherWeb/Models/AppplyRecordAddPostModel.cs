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
        public string GoodsName { get; set; }
        [Display(Name = "申领时间")]
        [Required(ErrorMessage = "请输入{0}")]
        public DateTime ApplyTime { get; set; }
        [Display(Name = "归还时间")]
        [Required(ErrorMessage = "请输入{0}")]
        public DateTime ReturnTime { get; set; }
        [Display(Name = "申领班级")]
        [Required(ErrorMessage = "请选择{0}")]
        public long ClassId { get; set; }
        [Display(Name = "存档资料")]
        [Required(ErrorMessage = "请选择{0}")]
        [ValidateFile]
        public HttpPostedFileBase[] UpFiles { get; set; }
    }
}