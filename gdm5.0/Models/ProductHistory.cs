using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Models
{
    public class ProductHistory : BaseObject
    {
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public double Quantity { get; set; }
        public double PrimeCost { get; set; }
        public double ProductStandartCost { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public bool ProductDeleted { get; set; }
        public DateTime DateOfChange { get; set; }
    }
}
