using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.Domain.Interfaces;
using gdm5._0.Domain.Interfaces.Models;

namespace gdm5._0.Extensions
{
    public static class FilterExtensions
    {
        /// <summary>
        /// Adds the filter to the query (paging ignored).
        /// </summary>
        public static IQueryable<TModel> ApplyFilter<TModel>(this IQueryable<TModel> models, IFilterAssigner<TModel> filterAssigner) where TModel : IDbSortingModel
        {
            return filterAssigner.ApplyFilter(models);
        }


        /// <summary>
        /// Adds the filter to the query (paging included).
        /// </summary>
        public static IQueryable<TModel> ApplyPagingFilter<TModel>(this IQueryable<TModel> models, IFilterAssigner<TModel> filterAssigner) where TModel : IDbSortingModel
        {
            return filterAssigner.ApplyPagingFilter(models);
        }


    }
}
