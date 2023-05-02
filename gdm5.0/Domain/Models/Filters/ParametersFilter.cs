namespace gdm5._0.Domain.Models.Filters
{
    public sealed class ParametersFilter : PagingFilter
    {

        public string Name { get; set; }

        public string NameType { get; set; }

        public int? NavigationOrder { get; set; }

        public int? ProductTypeId { get; set; }

    }
}
