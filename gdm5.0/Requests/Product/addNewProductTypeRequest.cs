using System.Collections.Generic;
using gdm5._0.DTO;

namespace gdm5._0.Requests.Product
{
    public class addNewProductTypeRequest 
    {
        public Property Name { get; set; }
        public Property ProductNumber { get; set; }
        public Property Quantity { get; set; }
        public Property StandartCost { get; set; }
        public Property PrimeCost { get; set; }
        public Property PrimeCostUSD { get; set; }
        public Property PrimeCostEUR { get; set; }
        public Property Manufacturer { get; set; }
        public Property Description { get; set; }
        public Property DateOfReceipt { get; set; }
        public Property CurrencyName { get; set; }
        public Property WareHouseName { get; set; }
        public ICollection<Property> Parameters { get; set; } = new List<Property>();
    }
}
