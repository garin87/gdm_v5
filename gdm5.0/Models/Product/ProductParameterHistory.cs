using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using gdm5._0.Domain.Interfaces.Models;

namespace gdm5._0.Models
{
    public class ProductParameterHistory : BaseObject, IDbSortingModel
    {

        public int? ProductHistoryId { get; set; }
        public virtual ProductHistory ProductHistory { get; set; }

        public int? ParameterHistoryId { get; set; }
        public virtual ParameterHistory Parameter { get; set; }

        public string Value { get; set; }

    }

}
