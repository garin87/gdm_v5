namespace gdm5._0.Domain.Models.Filters
{
    public sealed class ProductParametersFilter : PagingFilter
    {
        /// <summary>
        /// Gets or sets part of product parameter value for filter.
        /// </summary>
        public string Name { get; set; }
        public int? ProductId { get; set; }
        public int? ParameterId { get; set; }
        public string ParameterName{ get; set; }
        public string Value { get; set; }
    }
}
