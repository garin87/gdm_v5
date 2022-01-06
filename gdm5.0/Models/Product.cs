using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using gdm5._0.DTO;

namespace gdm5._0.Models
{
    public class Product : BaseObject
    {
        public string Name { get; set; }
        public string ProductNumber { get; set; }
        public double Quantity { get; set; }
        public double PrimeCost { get; set; }
        public double StandartCost { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public DateTime DateOfReceipt { get; set; }
       
        [ForeignKey("ProductType")]
        public int  ProductTypeId {get; set;}

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }

        [ForeignKey("WareHouse")]
        public int WareHouseId { get; set; }
        public virtual ProductType ProductType { get; set; }
        public virtual Currency Currency { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        public ICollection<ProductParameter> ProductParameters { get; set; } = new List<ProductParameter>();
        public ICollection<OrderProduct> OrderProduct { get; set; } = new List<OrderProduct>();
        public ICollection<PriceListValue> PriceListValue { get; set; } = new List<PriceListValue>();


        public dynamic getSortField(SortOptionsDTO sortOption, IQueryable<Parameter> parametrs)
        {
            if (sortOption.Name == "quantity") return this.Quantity;
            if (sortOption.Name == "productNumber") return this.ProductNumber;
            if (sortOption.Name == "manufacturer") return this.Manufacturer;
            if (sortOption.Name == "standartCost") return this.StandartCost;
            if (sortOption.Name == "primeCost") return this.PrimeCost;
            if (sortOption.Name == "dateOfReceipt") return this.DateOfReceipt;
            if (sortOption.IsParameter) {
                var parmId = parametrs.Where(el => el.Name == sortOption.Name).FirstOrDefault().Id;
                return this.ProductParameters
                            .Where(el => el.ParameterId == parmId).FirstOrDefault().Value;
            } 
            
            return this.Id;
        }

    }
}
