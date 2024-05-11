using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Models
{
    public class PriceListValue : BaseObject
    {
        public double Version { get; set; }
        public int PercentOfMarkup { get; set; }
        public double Price { get; set; }
        public double PriceEUR { get; set; }
        public double PriceUSD { get; set; }
        public double PriceNDS { get; set; }
        public double PriceEURNDS { get; set; }
        public double PriceUSDNDS { get; set; }
        public double Quantity { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public int ProductParameterUniqCode { get; set; }
        public string ProductParameterValueCode { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int? PriceListId { get; set; }
        public virtual PriceList PriceList { get; set; }

    }
}
