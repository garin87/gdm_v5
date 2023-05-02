
namespace gdm5._0.Models
{
    public class Label : BaseObject
    {
        public string Name { get; set; }
        public string LabelValue { get; set; }

        public int LabelCategoryId { get; set; }
        public virtual LabelCategory LabelCategory { get; set; }
        public int DictionaryId { get; set; }
        public virtual Dictionary Dictionary { get; set; }
    }
}
