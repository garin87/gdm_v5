using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public double StandartCost { get; set; }
        public double ProductStandartCost { get; set; }
        public double PrimeCostUSD { get; set; }
        public double PrimeCostEUR { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public bool ProductDeleted { get; set; }
        public string Supplier { get; set; }
        public DateTime DateOfChange { get; set; }
        public DateTime DateOfReceipt { get; set; }
        public int WareHouseId { get; set; }
        public int DeletedProductId { get; set; }
        public int DeletedProductTypeId { get; set; }

        public string UserName { get; set; }
        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        [ForeignKey("ProductTypeHistory")]
        public int? ProductTypeHistoryId { get; set; }
        public virtual ProductTypeHistory ProductTypeHistory { get; set; }
        public ICollection<OrderProductHistory> OrderProductHistory { get; set; } = new List<OrderProductHistory>();
        public ICollection<ProductParameterHistory> ProductParameterHistory { get; set; } = new List<ProductParameterHistory>();
    }
}
