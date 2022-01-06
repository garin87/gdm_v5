using gdm5._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Services
{
    public interface IBaseServices<T> where T : BaseObject
    {
        IEnumerable<T> GetAll();
        Task<T> GetItem(int id);
        Task<T> DeleteItem(int id);
        Task<T> AddItem(T t);
    }
}
