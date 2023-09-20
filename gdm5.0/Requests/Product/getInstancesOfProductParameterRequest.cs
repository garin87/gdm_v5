using System.Collections.Generic;
using gdm5._0.Domain.Models.Product;

namespace gdm5._0.Requests.Product
{
    public class getInstancesOfProductParameterRequest
    {
        public string NameType { get; set; }
        public string NameParameter { get; set; }
        public bool IsParameter { get; set; }
        public ICollection<ProductParametersFilter> FilterParameters { get; set; }
    }
}
