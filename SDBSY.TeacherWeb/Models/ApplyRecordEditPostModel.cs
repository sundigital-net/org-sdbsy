using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDBSY.TeacherWeb.Models
{
    public class ApplyRecordEditPostModel
    {
        public long Id { get; set; }
        [Display(Name = "物品名称")]
        [Required(ErrorMessage = "请输入{0}")]
        public string GoodsName { get; set; }
        [Display(Name = "申请时间")]
        [Required(ErrorMessage = "请输入{0}")]
        public DateTime ApplyRecordTime { get; set; }
        [Display(Name = "申请班级")]
        [Required(ErrorMessage = "请选择{0}")]
        public long ClassId { get; set; }
        [Display(Name = "总金额")]
        [Required(ErrorMessage = "请输入{0}")]
        public decimal Total { get; set; }
        [Display(Name = "存档资料")]
        public HttpPostedFileBase[] UpFiles { get; set; }
    }
}