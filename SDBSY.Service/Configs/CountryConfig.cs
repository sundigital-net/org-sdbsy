﻿using SDBSY.Service.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDBSY.Service.Configs
{
    class CountryConfig:EntityTypeConfiguration<CountryEntity>
    {
        public CountryConfig()
        {
            ToTable("T_Countries");
            Property(t => t.Name).HasMaxLength(30).IsRequired();
        }
    }
}
