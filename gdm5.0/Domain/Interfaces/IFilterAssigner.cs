using gdm5._0.Domain.Interfaces.Models;
using System.Linq;

namespace gdm5._0.Domain.Interfaces
{
    public interface IFilterAssigner<TModel> where TModel : IDbSortingModel
    {
        /// <summary>
        /// Adds the filter to the query (paging ignored).
        /// </summary>
        public IQueryable<TModel> ApplyFilter(IQueryable<TModel> models);

        /// <summary>
        /// Adds the filter to the query (paging included).
        /// </summary>
        public IQueryable<TModel> ApplyPagingFilter(IQueryable<TModel> models);
    }
}
