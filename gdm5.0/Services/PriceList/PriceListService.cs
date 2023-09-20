
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using gdm5._0.Helpers;
using gdm5._0.Models;
using gdm5._0.Requests.PriceList;
using gdm5._0.Requests.Product;
using gdm5._0.Services.Interfaces;


namespace gdm5._0.Services
{
    public class PriceListService : BaseService<PriceList>, IPriceListService
    {
        private readonly DataContext _context;

        public PriceListService(DataContext context) : base(context)
        {
            _context = context;
        }

        public void AddPriceList(addPriceListRequest addPriceList)
        {
            ValidateName(addPriceList.Name?.Value);

            if (_context.PriceLists.Any(pl => pl.Name == addPriceList.Name.Value))
                throw new ApplicationException("Entered price list name is duplicate."); ;

            double version = MathHelper.ParseDouble(addPriceList.Version?.Value);

            var instancePriceList = new PriceList
            {
                Name = addPriceList.Name?.Value,
                Version = version,
                Description = addPriceList.Description?.Value,
                CreationDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                ComplexPriceListId = 4
            };

            _context.PriceLists.Add(instancePriceList);
            _context.SaveChanges();
        }

        public void UpdatePriceList(addPriceListRequest addPriceList)
        {
            if (addPriceList.idPriceList == 0)
                throw new ApplicationException("Enter valid idPriceList");

            ValidateName(addPriceList.Name?.Value);

            var priceList = _context.PriceLists.FirstOrDefault(PriceLists => PriceLists.Id == addPriceList.idPriceList);
            if (priceList == null)
                throw new ApplicationException("Entered price list does not exist.");

            double version = MathHelper.ParseDouble(addPriceList.Version?.Value);

            priceList.Name = addPriceList.Name?.Value;
            priceList.Version = version;
            priceList.Description = addPriceList.Description?.Value;
            priceList.LastModifiedDate = DateTime.Now;

            _context.SaveChanges();
        }
        public PriceListWithValues GetPriceListWithValues(string namePriceList)
        {
            if (string.IsNullOrWhiteSpace(namePriceList))
                throw new ArgumentException("Invalid price list name");

            var priceList = _context.PriceLists.Include(priceList => priceList.PriceListValue)
                                                     .Where(p => p.Name == namePriceList);

            foreach(var priceL in priceList)
            {
                foreach (var priceListValue in priceL.PriceListValue)
                {
                    var productID = priceListValue.Product;

                }
               
            }

            var priceListV = priceList.Select(item => new PriceListWithValues()
            {
                Name = item.Name,
                Description = item.Description,
                Version = item.Version,
                CreationDate = item.CreationDate.ToString("MM/dd/yyyy HH:mm"),
                LastModifiedDate = item.LastModifiedDate.ToString("MM/dd/yyyy HH:mm"),
                PriceListValue = item.PriceListValue,

            }).FirstOrDefault();


            return priceListV;
        }

        public void DeletePriceList(int priceListId)
        {
            if (priceListId == 0)
                throw new ArgumentException("No price list ID provided.");

            var priceListValuesToDelete = _context.PriceLists
                .Where(p => p.Id == priceListId);

            if (priceListValuesToDelete == null)
                throw new ApplicationException("No price list found for the provided ID.");

            _context.PriceLists.Remove(priceListValuesToDelete as PriceList);
            _context.SaveChanges();
        }

        private void ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ApplicationException("Enter a valid name for the price list");
        }
    }

}

