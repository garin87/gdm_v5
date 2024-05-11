using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public double PrimeCostUSD { get; set; }
        public double PrimeCostEUR { get; set; }
        public double StandartCost { get; set; }
        public string Manufacturer { get; set; }
        public string Description { get; set; }
        public string Supplier { get; set; }
        public string LastEditedByUser { get; set; }
        public string Units { get; set; }
        public DateTime DateOfReceipt { get; set; }
        public DateTime DateOfLastChanged { get; set; }

        [ForeignKey("ProductType")]
        public int  ProductTypeId {get; set;}

        [ForeignKey("Currency")]
        public int? CurrencyId { get; set; }

        [ForeignKey("WareHouse")]
        public int? WareHouseId { get; set; }
        public virtual ProductType ProductType { get; set; }
        public virtual Currency  Currency { get; set; }
        public virtual WareHouse WareHouse { get; set; }
        public ICollection<ProductParameter> ProductParameters { get; set; } = new List<ProductParameter>();
        public ICollection<OrderProduct> OrderProduct { get; set; } = new List<OrderProduct>();
        public ICollection<PriceListValue> PriceListValue { get; set; } = new List<PriceListValue>();

        public dynamic getSortField(SortOptionsDTO sortOption, List<Parameter> parametrs)
        {
            if (sortOption.Name == "quantity") return this.Quantity;
            if (sortOption.Name == "productNumber") return this.ProductNumber;
            if (sortOption.Name == "manufacturer") return this.Manufacturer;
            if (sortOption.Name == "standartCost") return this.StandartCost;
            if (sortOption.Name == "primeCost") return this.PrimeCost;
            if (sortOption.Name == "primeCostUSD") return this.PrimeCostUSD;
            if (sortOption.Name == "primeCostEUR") return this.PrimeCostEUR;
            if (sortOption.Name == "dateOfReceipt") return this.DateOfReceipt;
            if (sortOption.Name == "warehousename") return this.Manufacturer;
            if (sortOption.IsParameter)
            {
                var parmId = parametrs.Where(el => el.Name == sortOption.Name).FirstOrDefault().Id;
                var paramValue = this.ProductParameters
                            .Where(el => el.ParameterId == parmId).FirstOrDefault()?.Value;
                double parsedValue = 0;
                if (!string.IsNullOrWhiteSpace(paramValue) && (
                    (sortOption.Name.ToLower()).Equals(("диаметр"))||
                    (sortOption.Name.ToLower()).Equals(("размер")) ||
                    (sortOption.Name.ToLower()).Equals(("внутренний диаметр"))
                    ))
                {
                    if (double.TryParse(paramValue, out parsedValue))
                    {
                        return parsedValue;
                    }
                    else
                    {
                        string[] parts = paramValue.Split('*');
                        if (parts.Length > 0)
                        {
                            if (double.TryParse(parts[0], out double number))
                            {
                                return number;
                            }
                        }

                        parts = paramValue.Split('/');
                        if (parts.Length > 0)
                        {
                            if (double.TryParse(parts[0], out double number))
                            {
                                return number;
                            }
                        }
                    }
                }
               
                return paramValue;
            }

            return this.Id;
        }

        public string getSortFieldAsString(SortOptionsDTO sortOption, List<Parameter> parameters)
        {
            if (SortFieldMappings.TryGetValue(sortOption.Name, out Func<Product, string> selector))
            {
                return selector(this);
            }
            if (sortOption.IsParameter)
            {
                return GetParameterSortField(sortOption.Name, parameters);
            }
            return "";
        }

        private string GetParameterSortField(string paramName, List<Parameter> parameters)
        {
            var parameter = parameters.FirstOrDefault(param => param.Name == paramName);
            if (parameter != null)
            {
                var paramId = parameter.Id;
                var paramValue = this.ProductParameters
                    .Where(param => param.ParameterId == paramId)
                    .FirstOrDefault()?.Value;

                return paramValue;
            }
            return "";
        }

        private static readonly Dictionary<string, Func<Product, string>> SortFieldMappings = new Dictionary<string, Func<Product, string>>
    {
        { "quantity", p => p.Quantity.ToString() },
        { "productnumber", p => p.ProductNumber },
        { "manufacturer", p => p.Manufacturer },
        { "standartcost", p => p.StandartCost.ToString() },
        { "primecost", p => p.PrimeCost.ToString() },
        { "primecostusd", p => p.PrimeCostUSD.ToString() },
        { "primecosteur", p => p.PrimeCostEUR.ToString() },
        { "dateofreceipt", p => p.DateOfReceipt.ToString() },
        { "warehousename", p => p.Manufacturer }
    };
    }
}
