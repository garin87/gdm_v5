using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Models
{
    public class Customer : BaseObject
    {
        public string NameCompany { get; set; }
        public string AddressCompany { get; set; }
        public string City { get; set; }
        public string CustomerName { get; set; }
        public string MobilePhone { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string PriorityColor { get; set; }
        
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}
