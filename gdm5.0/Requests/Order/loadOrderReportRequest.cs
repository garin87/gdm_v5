using System;
using System.Collections.Generic;
using gdm5._0.Domain.Models.Filters;
using gdm5._0.DTO;
using gdm5._0.Requests.Base;

namespace gdm5._0.Requests.Order
{
    public class loadOrderReportRequest : BaseLoadReportRequest
    {
        public Property NameCompany { get; set; }
        public Property FilterStartDate { get; set; }
        public Property FilterEndDate { get; set; }
    }
}
