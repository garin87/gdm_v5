using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace gdm5._0.Models
{
    public class OrderStatus : BaseObject
    {
        public string OrderStatusName { get; set; }
        public double Color { get; set; }
        public double Rank { get; set; }
        public string SpareStringField { get; set; }
        public string Description { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
