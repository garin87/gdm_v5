using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace gdm5._0.Models
{
    public class Order : BaseObject
    {
        public string NameCompany { get; set; }
        public double TotalPrice { get; set; }
        public double TaxNDS { get; set; }
        public double Markup { get; set; }
        public DateTime DateTime { get; set; }
        
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual Currency Currency { get; set; }
        public ICollection<OrderProduct> OrderProduct { get; set; } = new List<OrderProduct>();
    }
}
