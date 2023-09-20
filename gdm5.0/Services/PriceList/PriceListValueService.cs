
using System;
using System.Collections.Generic;
using System.Linq;
using gdm5._0.Helpers;
using gdm5._0.Models;
using gdm5._0.Requests.PriceList;
using gdm5._0.Services.Interfaces;


namespace gdm5._0.Services
{
    public class PriceListValueService : BaseService<PriceListValue>, IPriceListValueService
    {
        private readonly DataContext _context;

        public PriceListValueService(DataContext context) : base(context)
        {
            _context = context;
        }

        public void AddPriceListValues(addPriceListValuesRequest addPriceListValues)
        {

            if (addPriceListValues == null)
                throw new ArgumentNullException(nameof(addPriceListValues), "Enter valid price list values");

            foreach (var priceListValue in addPriceListValues.PriceListValues)
            {

                ValidatePriceListValue(priceListValue);

                double price = MathHelper.ParseDouble(priceListValue.Price?.Value);
                double quantity = MathHelper.ParseDouble(priceListValue.Quantity?.Value);
                double version = MathHelper.ParseDouble(priceListValue.Version?.Value);
 
             
                var instancePriceListValue = new PriceListValue
                {
                     Version = version,
                     Price = price,
                     Quantity = quantity,
                     Description = priceListValue.Description?.Value,
                     ProductId = priceListValue.ProductId,
                     PriceListId = priceListValue.PriceListId
                };

                _context.PriceListValues.Add(instancePriceListValue);
                _context.SaveChanges();
            }

        }

        public void UpdatePriceListValues(addPriceListValuesRequest addPriceListValues)
        {

            if (addPriceListValues == null)
                throw new ArgumentNullException(nameof(addPriceListValues),"Enter valid price list values");

            foreach (var priceListValue in addPriceListValues.PriceListValues)
            {
                if (priceListValue.idPriceListValue == 0)
                    throw new ApplicationException("Enter valid PriceListValue Id");

                var priceValue = _context.PriceListValues.FirstOrDefault(PriceListValue => (PriceListValue.Id == priceListValue.idPriceListValue));
                if (priceValue == null)
                    throw new ApplicationException("Entered price value id does not exist.");

                ValidatePriceListValue(priceListValue);

                double price = MathHelper.ParseDouble(priceListValue.Price?.Value);
                double quantity = MathHelper.ParseDouble(priceListValue.Quantity?.Value);
                double version = MathHelper.ParseDouble(priceListValue.Version?.Value);

                priceValue.Version = version;
                priceValue.Price = price;
                priceValue.Quantity = quantity;
                priceValue.Description = priceListValue.Description?.Value;
                priceValue.ProductId = priceListValue.ProductId;
                priceValue.PriceListId = priceListValue.PriceListId;

                _context.SaveChanges();
            }

        }

        public List<PriceListValue> GetPriceListValues(int priceListId)
        {
            if (priceListId <= 0)
                throw new ArgumentException("Invalid price list id");

            var priceListValues = _context.PriceListValues
                .Where(p => p.PriceListId == priceListId)
                .ToList();

            return priceListValues;
        }
        public void DeletePriceListValues(List<int> priceListValueIds)
        {
            if (priceListValueIds == null || priceListValueIds.Count == 0)
                throw new ArgumentException("No price list value IDs provided.");

            var priceListValuesToDelete = _context.PriceListValues
                .Where(p => priceListValueIds.Contains(p.Id))
                .ToList();

            if (priceListValuesToDelete.Count == 0)
                throw new ApplicationException("No price list values found for the provided IDs.");

            _context.PriceListValues.RemoveRange(priceListValuesToDelete);
            _context.SaveChanges();
        }
        private void ValidatePriceListValue(addPriceListValueRequest value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value), "Enter valid price list value");

            if (value.ProductId == 0)
                throw new ApplicationException("Enter valid ProductId");

            var products = _context.Products.FirstOrDefault(Products => (Products.Id == value.ProductId));
            if (products == null)
                throw new ApplicationException("Entered product id does not exist.");

            if (value.PriceListId == 0)
                throw new ApplicationException("Enter valid PriceListId");

            var priceList = _context.Products.FirstOrDefault(priceList => (priceList.Id == value.PriceListId));
            if (priceList == null)
                throw new ApplicationException("Entered price list id does not exist.");
        }

    }


}

