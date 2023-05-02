using gdm5._0.Domain.Interfaces;
using gdm5._0.Domain.Interfaces.Models;
using gdm5._0.Domain.Models.Filters;
using System.Linq;

namespace gdm5._0.Filters
{
    public abstract class FilterAssigner<TModel, TFilter> : IFilterAssigner<TModel>
        where TModel : IDbSortingModel
        where TFilter : PagingFilter
    {
        public FilterAssigner(TFilter filter)
        {
            Filter = filter;
        }

        /// <summary>
        /// Gets filter.
        /// </summary>
        protected TFilter Filter { get; }

        /// <summary>
        /// Adds the filter to the query (paging ignored).
        /// </summary>
        public abstract IQueryable<TModel> ApplyFilter(IQueryable<TModel> models);

        /// <summary>
        /// Adds the filter to the query (paging included).
        /// </summary>
        public IQueryable<TModel> ApplyPagingFilter(IQueryable<TModel> models)
        {
            return Filter.CountInstances > 0 ? ApplyFilter(models).Skip(Filter.StartIndex).Take(Filter.CountInstances)
                                    : ApplyFilter(models);
        }
    }
}