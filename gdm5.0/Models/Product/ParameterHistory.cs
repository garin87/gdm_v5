using gdm5._0.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Models
{
    public class ParameterHistory : BaseObject, IDbSortingModel
    {
        public string Name { get; set; }
        public string NameType { get; set; }
        public int Priority { get; set; }
        public bool isRequired { get; set; }

        public int DeletedParameterId { get; set; }
        public int DeletedProductTypeId { get; set; }

        [ForeignKey("ProductTypeHistory")]
        public int? ProductTypeHistoryId { get; set; }
        public virtual ProductTypeHistory ProductTypeHistory { get; set; }
        public ICollection<ProductParameterHistory> ProductParameterHistory { get; set; } = new List<ProductParameterHistory>();
    }
}
