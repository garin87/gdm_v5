using gdm5._0.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Models
{
    public class Parameter : BaseObject, IDbSortingModel
    {
        public string Name { get; set; }
        public string NameType { get; set; }
        public int Priority { get; set; }

        [ForeignKey("ProductType")]
        public int ProductTypeId { get; set; }
        public  virtual ProductType ProductType { get; set; }

        public ICollection<ProductParameter> ProductParameters { get; set; } = new List<ProductParameter>();
    }
}
