using System.Collections.Generic;

namespace gdm5._0.Models
{
    public class LabelCategory : BaseObject
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<Label> Labels { get; set; } = new List<Label>();
    }
}
