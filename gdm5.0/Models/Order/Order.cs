using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;


namespace gdm5._0.Models
{
    public class Order : BaseObject
    {
        public string NameCompany { get; set; }
        public double TotalPrice { get; set; }
        public int OrderNumber { get; set; }
        public DateTime OrderCreatedTime { get; set; }
        public string OrderCreatedByUser { get; set; }
        public string Description { get; set; }

        [ForeignKey("Currency")]
        public int CurrencyId { get; set; }
        public virtual Customer Customer { get; set; }
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public virtual Currency Currency { get; set; }
        [ForeignKey("OrderStatusId")]
        public int? OrderStatusId { get; set; }
        public virtual OrderStatus OrderStatus { get; set; }
        public ICollection<OrderProduct> OrderProduct { get; set; } = new List<OrderProduct>();
        public ICollection<OrderProductHistory> OrderProductHistory { get; set; } = new List<OrderProductHistory>();
        
    }
}
