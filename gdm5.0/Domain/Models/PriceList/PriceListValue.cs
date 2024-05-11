using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.DTO;
using gdm5._0.Models;

namespace gdm5._0.Requests.PriceList
{
    public class PriceListValueDTO : BaseObject
    {
        public double Version { get; set; }
        public int PercentOfMarkup { get; set; }
        public double Price { get; set; }
        public double PriceEUR { get; set; }
        public double PriceNDS { get; set; }
        public double PriceEURNDS { get; set; }
        public double Quantity { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public int ProductParameterUniqCode { get; set; }

        public int ProductId { get; set; }
        public int? PriceListId { get; set; }


        // product
        public string ProductName { get; set; }
        public string Manufacturer { get; set; }
        public string Supplier { get; set; }
        public double? PrimeCost { get; set; }
        public ICollection<ParameterDTO> Parameters { get; set; } = new List<ParameterDTO>();


        public dynamic getSortField(SortOptionsDTO sortOption, List<Parameter> parametrs)
        {
            if (sortOption.IsParameter)
            {
                var listParamID = this.Parameters.Select(el => el.ParameterId).ToList();
                int parmId = 0;
                foreach (var param in listParamID)
                {
                    var pId = parametrs.Where(el => (el.Name.ToLower().Equals("диаметр") || 
                                                     el.Name.ToLower().Equals("размер") ||
                                                     el.Name.ToLower().Equals("внутренний диаметр") ) && el.Id == param).FirstOrDefault();
                    if (pId != null)
                    {
                        parmId = pId.Id;
                        break;
                    }

                }

                if (parmId == 0) return this.Id;

                var paramValue = this.Parameters.Where(el => el.ParameterId == parmId).FirstOrDefault()?.Value;




                double parsedValue = 0;
                if (!string.IsNullOrWhiteSpace(paramValue))
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

    }
}
