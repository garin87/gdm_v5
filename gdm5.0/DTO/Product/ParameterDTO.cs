using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.DTO
{
    public class ParameterDTO
    {
        public int Id { get; set; }
        public int ParameterId { get; set; }
        public string ProductTypeName { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public bool isDeleted { get; set; }
        public string newName { get; set; }
        public bool isParameter { get; set; }
        public string NameType { get; set; }
        public int Priority { get; set; }
        //public string Value? { get; set; }
        //public string Value? { get; set; }

    }
}
