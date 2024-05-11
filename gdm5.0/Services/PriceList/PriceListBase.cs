using System;
using System.Collections.Generic;
using System.Linq;
using gdm5._0.Models;

namespace gdm5._0.Services.PriceListS
{
    public class PriceListBase<T> : BaseService<T> where T: BaseObject
    {
        public PriceListBase(DataContext context) : base(context)
        {
        }
        protected PriceList getPriceListByName(string namePriceList)
        {
            ValidateName(namePriceList);

            var priceList = _context.PriceLists.FirstOrDefault(p => p.Name == namePriceList.Trim().ToLower());

            if (priceList == null)
                throw new ArgumentException("Invalid price list name");

            return priceList;
        }
        protected List<PriceListValue> getPriceListValues(int IdPriceList)
        {
            return _context.PriceListValues.Where(p => p.PriceListId == IdPriceList).ToList();
        }
        protected void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ApplicationException("Enter a valid name for the price list");
        }
        protected void ValidateExistsPriceList(string name)
        {
            if (_context.PriceLists.Any(pl => pl.Name == name.Trim().ToLower()))
                throw new ApplicationException("Entered price list name is duplicate.");
        }
        protected void ValidatePriceListId(int priceListId)
        {
            if (priceListId == 0)
                throw new ArgumentException("No price list ID provided.");
        }
        protected PriceList GetPriceListById(int priceListId)
        {
            ValidatePriceListId(priceListId);

            var priceList = _context.PriceLists.FirstOrDefault(p => p.Id == priceListId);

            if (priceList == null)
                throw new ApplicationException("No price list found for the provided ID.");

            return priceList;
        }
    }
}
