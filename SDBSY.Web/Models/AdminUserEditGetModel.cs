using SDBSY.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDBSY.Web.Models
{
    public class AdminUserEditGetModel
    {
        public RoleDTO[] Roles { get; set; }
        public RoleDTO[] AllRoles { get; set; }
        public AdminUserDTO AdminUser { get; set; }
        //public PermissionDTO[] RolePerms { get; set; }

    }
}