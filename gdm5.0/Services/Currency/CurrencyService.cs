
using System;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.Models;
using gdm5._0.Requests.Customer;
using gdm5._0.Services.Interfaces;


namespace gdm5._0.Services
{
    public class CurrencyService : BaseService<Currency>, ICurrencyService
    {
        private readonly DataContext _context;

        public CurrencyService(DataContext context) : base(context)
        {
            _context = context;
        }

        public string[] GetNamesCurrencies()
        {
            return _context.Currencies.Select(customer => customer.CurrencyName).ToArray();
        }

        public Currency GetCurrencyByName(string nameCurrency)
        {

            if (string.IsNullOrEmpty(nameCurrency))
                throw new ApplicationException("Enter valid name currency");

            var existCurrency = _context.Currencies.FirstOrDefault(customer => customer.CurrencyName.ToLower() == nameCurrency.ToLower());
            if (existCurrency == null) throw new ApplicationException("Entered name of currency does not exist");

            return existCurrency;
        }

        public async Task<Currency> AddNewCurrency(addCurrencyRequest newcurrency)
        {

            if (string.IsNullOrEmpty(newcurrency.CurrencyName?.Value))
                throw new ApplicationException("Enter valid name currency");


            var existCurrency = _context.Currencies.FirstOrDefault(currency =>
            currency.CurrencyName == newcurrency.CurrencyName.Value);
            if (existCurrency != null) throw new ApplicationException("Entered name of currency exists");


            var currency = new Currency()
            {
                CurrencyName = newcurrency.CurrencyName?.Value,
            };

            _context.Currencies.Add(currency);
            await _context.SaveChangesAsync();

            return currency;
        }


        public async Task<Currency> UpdateCurrency(UpdateCurrencyRequest updatedCurrency)
        {
            if (updatedCurrency.CurrencyId == 0)
                throw new ApplicationException("Enter valid currency Id");

            if (string.IsNullOrEmpty(updatedCurrency.CurrencyName?.Value))
                throw new ApplicationException("Enter valid name currency");


            var existCurrency = _context.Currencies.FirstOrDefault(customer => customer.Id == updatedCurrency.CurrencyId);
            if (existCurrency == null) throw new ApplicationException("Entered currency does not exist");


            existCurrency.CurrencyName = updatedCurrency.CurrencyName.Value;

            await _context.SaveChangesAsync();
            return existCurrency;
        }
    }


}

