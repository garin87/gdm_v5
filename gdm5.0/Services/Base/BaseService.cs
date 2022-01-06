using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Models;
using gdm5._0.Services;

namespace gdm5._0.Services
{
    public class BaseService<T> : IBaseServices<T> where T: BaseObject
    {
        private readonly DataContext _context;

        public BaseService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<T> GetAll()
        {
            return  _context.Set<T>().ToList();
        }

        public async Task<T> GetItem(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<T> DeleteItem(int id)
        {
            var temp = await _context.Set<T>().FindAsync(id);
            _context.Set<T>().Remove(temp);
            await _context.SaveChangesAsync();

            return temp;
        }

        public async Task<T> AddItem(T t)
        {
            _context.Set<T>().Add(t);
            await _context.SaveChangesAsync();
            return t;
        }

        public async Task<T> UpdateItem(T t)
        {
            _context.Set<T>().Update(t);
            await _context.SaveChangesAsync();

            return t;
        }


    }
}
