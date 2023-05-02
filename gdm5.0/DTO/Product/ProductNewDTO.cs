using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.DTO
{
    public class ProductNewDTO
    {   

        // public int ProductId { get; set; }
        public Property Name { get; set; }
        public Property ProductNumber { get; set; }
        public Property Quantity { get; set; }
        public Property StandartCost { get; set; }
        public Property PrimeCost { get; set; }
        public Property PrimeCostEUR { get; set; }
        public Property PrimeCostUSD { get; set; }
        //public Property primecosteur { get; set; }
        //public Property primecostusd { get; set; }
        public Property Manufacturer { get; set; }
        public Property Description { get; set; }
        public Property DateOfReceipt { get; set; }
        public Property CurrencyName { get; set; }
        public Property WareHouseName { get; set; }
        //public int ProductTypeId { get; set; }
        //public string NameType { get; set; }

        public ICollection<Property> Parameters { get; set; } = new List<Property>();
    }
}
