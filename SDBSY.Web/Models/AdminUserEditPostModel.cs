using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SDBSY.Web.Models
{
    public class AdminUserEditPostModel
    {
        public long Id { get; set; }
        
        public long[] RoleIds { get; set; }
    }
}