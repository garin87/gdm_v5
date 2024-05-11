using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Models;

namespace gdm5._0.Services
{
    public abstract class BaseService<T> : IBaseServices<T> where T: BaseObject
    {
        protected DataContext _context { get; set; }

        public BaseService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<T> GetAll()
        {
            return  _context.Set<T>().AsNoTracking().ToList();
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
        protected int GetProductTypeID(string nameProductType)
        {
            if (string.IsNullOrEmpty(nameProductType))
                throw new ApplicationException("Enter valid name product");

            var productType = _context.ProductTypes.FirstOrDefault(type => type.NameType == nameProductType.Trim().ToLower());

            if (productType == null)
                throw new ApplicationException("Product name " + nameProductType + " does not exist");

            return productType.Id;
        }
        protected int GetParameterID(int productTypeId, string paramName)
        {
            if (string.IsNullOrWhiteSpace(paramName))
                throw new ApplicationException("Product has no " + paramName);


            var parameter = _context.Parameters.FirstOrDefault(p => p.ProductTypeId == productTypeId &&
                                                                    p.Name == paramName.Trim().ToLower());

            if (parameter == null)
                throw new ApplicationException("Product has no " + paramName);

            return parameter.Id;
        }
        protected IQueryable<Parameter> GetParametersOfProduct(int productTypeID)
        {
            return _context.Parameters.Where(param => param.ProductTypeId == productTypeID);
        }
        protected Product GetProductById(int produtId)
        {
            if (produtId == 0)
                throw new ApplicationException("Enter valid ProductId");

            var product = _context.Products.FirstOrDefault(product => (product.Id == produtId));
            if (product == null)
                throw new ApplicationException("Product Id does not exist");
           
            return product;
        }
    }
}
