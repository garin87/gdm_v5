using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.DTO
{
    public class ProductParametrDTO
    {   
        public int Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public double ValueDouble { get; set; }
        public bool isRequired { get; set; }
        public string NameType { get; set; }
        public int Priority { get; set; }
    }
}
