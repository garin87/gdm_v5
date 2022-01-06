using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.DTO
{
    public class OrderPDTO
    {
        public int Id { get; set; }
        public string NameCompany { get; set; }
        public double TotalPrice { get; set; }
        public DateTime DateTime { get; set; }

        public ICollection<OrderProductDTO> OrderProducts { get; set; } = new List<OrderProductDTO>();
        public ICollection<ProductOrderDTO> ProductOrders { get; set; } = new List<ProductOrderDTO>();
    }
}
