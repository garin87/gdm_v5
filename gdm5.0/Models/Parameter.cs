using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Models
{
    public class Parameter : BaseObject
    {
       // public int Id { get; set; }
        public string Name { get; set; }

        [ForeignKey("ProductType")]
        public int ProductTypeId { get; set; }
        public  virtual ProductType ProductType { get; set; }

        public ICollection<ProductParameter> ProductParameters { get; set; } = new List<ProductParameter>();
    }
}
