using System;
using System.Collections.Generic;


namespace gdm5._0.Models
{
    public class ComplexPriceList : BaseObject
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public double Version { get; set; }
  
        public ICollection<PriceList> PriceList { get; set; } = new List<PriceList>();

    }
}
