using System;
using System.Collections.Generic;


namespace gdm5._0.Models
{
    public class ProductType : BaseObject
    {
        public string NameType { get; set; }
        public ICollection<Parameter> Parameters { get; set; } = new List<Parameter>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
