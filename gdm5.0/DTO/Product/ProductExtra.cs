using gdm5._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.DTO
{
    public class ProductExtra : Product
    {
        public dynamic getSortField(SortOptionsDTO sortOption, List<Parameter> parametrs)
        {
            if (sortOption.Name == "quantity") return this.Quantity;
            if (sortOption.Name == "productNumber") return this.ProductNumber;
            if (sortOption.Name == "manufacturer") return this.Manufacturer;
            if (sortOption.Name == "standartCost") return this.StandartCost;
            if (sortOption.Name == "primeCost") return this.PrimeCost;
            if (sortOption.Name == "dateOfReceipt") return this.DateOfReceipt;
            if (sortOption.Name == "warehousename") return this.Manufacturer;
            if (sortOption.IsParameter)
            {
                var parmId = parametrs.Where(el => el.Name == sortOption.Name).FirstOrDefault().Id;
                return this.ProductParameters
                            .Where(el => el.ParameterId == parmId).FirstOrDefault().Value;
            }

            return this.Id;
        }

    }
}
