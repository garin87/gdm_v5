using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Models
{
    public class Currency : BaseObject
    {
        public string CurrencyName { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<CurrencyRate> CurrencyRates { get; set; } = new List<CurrencyRate>();
    }


}
