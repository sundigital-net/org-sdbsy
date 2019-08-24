#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：SDBSY.Service.Configs
* 项目描述 ：
* 类 名 称 ：GoodsTypeConfig
* 类 描 述 ：
* 所在的域 ：HANYZHANG
* 命名空间 ：SDBSY.Service.Configs
* 机器名称 ：HANYZHANG 
* CLR 版本 ：4.0.30319.42000
* 作    者 ：Hany
* 创建时间 ：2019-04-15 10:14:25
* 更新时间 ：2019-04-15 10:14:25
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ Hany 2019. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion

using SDBSY.Service.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSY.Service.Configs
{
    class GoodsTypeConfig: EntityTypeConfiguration<GoodsTypeEntity>
    {
        public GoodsTypeConfig()
        {
            ToTable("T_GoodsTypes");
        }
    }
}
