using SDBSY.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SDBSY.Web.Models
{
    public class InvoiceListViewModel
    {
        public InvoiceDTO[] Invoices { get; set; }
        public DataDictionaryDTO[] Classes { get; set; }
        public long ClassId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}