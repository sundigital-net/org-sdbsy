﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDBSY.TeacherWeb
{
    public class AdminHelper
    {
        public static long? GetUserId(HttpContextBase ctx)
        {
            return (long?)ctx.Session["LoginTeacherId"];
        }
    }
}