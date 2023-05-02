using System;
using System.Collections.Generic;


namespace gdm5._0.Models
{
    public class ProductTypeHistory : BaseObject
    {
        public string NameType { get; set; }

        public int DeletedProductTypeId { get; set; }
        public ICollection<ParameterHistory> ParameterHistory { get; set; } = new List<ParameterHistory>();
        public ICollection<ProductHistory> ProductHistory { get; set; } = new List<ProductHistory>();
    }
}
