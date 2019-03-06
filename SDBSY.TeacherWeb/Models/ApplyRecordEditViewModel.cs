using SDBSY.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDBSY.TeacherWeb.Models
{
    public class ApplyRecordEditViewModel
    {
        public GoodsAllApplyRecordDTO[] Applyrecord { get; set; }
        public string Classes { get; set; }
    }
}