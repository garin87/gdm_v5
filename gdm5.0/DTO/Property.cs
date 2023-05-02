using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.DTO
{
    public class Property
    {
        public string Name { get; set; }
        public string DisplayedName { get; set; }
        public string Type { get; set; }
        public string TypeView { get; set; }
        public string Editor { get; set; }
        public string Provider { get; set; }
        public string DefaultValue { get; set; }
        public string Value { get; set; }
        public string Accessor { get; set; }

        public bool? Required { get; set; }
        public bool? ReadOnly { get; set; }
        public int? Order { get; set; }
        public int? NavPriority { get; set; }
        public bool? Hidden { get; set; }
        public bool isDeleted { get; set; }
        public bool? isSortable { get; set; }
        public string newName { get; set; }
        public string OptionOfProvider { get; set; }
        public string category { get; set; }
        
    }
}
