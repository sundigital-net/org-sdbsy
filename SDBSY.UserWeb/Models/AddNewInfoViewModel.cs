using SDBSY.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDBSY.UserWeb.Models
{
    public class AddNewInfoViewModel
    {

        public long UserId { get; set; }
        /// <summary>
        /// 幼儿身份证件类型
        /// </summary>
        public string IdCardTypes { get; set; }
        public string BloodTypes { get; set; }//血型
        public string Countries { get; set; }
        public string Nations { get; set; }
        /// <summary>
        /// 港澳台侨外
        /// </summary>
        public string Identities { get; set; }
        public NewPlaceDTO[] Places { get; set; }
        public string HuKouXingZhi { get; set; }
        public string FeiNongHuKouTypes { get; set; }
        public string StudyTypes { get; set; }
        public string IsStayAtHome { get; set; }
        public string HealthyTypes { get; set; }
        public string DisabilityTypes { get; set; }
        public string AdultIdCardTypes { get; set; }
        /// <summary>
        /// 是否显示选择班级的功能
        /// </summary>
        public bool ShowChooseClass { get; set; }
        public string Classes { get; set; }
    }
}