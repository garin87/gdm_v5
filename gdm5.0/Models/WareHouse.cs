using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Models
{
    public class WareHouse : BaseObject
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public string LocationDetails { get; set; }
        public string Sector { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
