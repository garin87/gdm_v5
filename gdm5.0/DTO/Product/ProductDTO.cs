using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.DTO
{
    public class ProductDTO
    {   

        public int ProductId { get; set; }
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public double? Quantity { get; set; }
        public double? ProductStandartCost { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public int ProductTypeId { get; set; }
        public string NameType { get; set; }
        public string Currency { get; set; }
        public string WareHouse { get; set; }
        public string PriceList { get; set; }
        public string Supplier { get; set; }
        public double? PrimeCost { get; set; }
        public double? PrimeCostUSD { get; set; }
        public double? PrimeCostEUR { get; set; }
        public double? StandartCost { get; set; }
        public string DateOfReceipt { get; set; }
        public string DateOfLastChanged { get; set; }
        public string LastEditedByUser { get; set; }
        public ICollection<ParameterDTO> Parameters { get; set; } = new List<ParameterDTO>();

    }
}
