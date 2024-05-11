
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
        public CurrencyService(DataContext context) : base(context)
        {
        }

        public string[] GetNamesCurrencies()
        {
            return _context.Currencies.Select(customer => customer.CurrencyName).ToArray();
        }

        public Currency GetCurrencyByName(string nameCurrency)
        {

            ValidateCurrencyName(nameCurrency);

            var existCurrency = GetCurrency(nameCurrency);
            if (existCurrency == null) 
                throw new ApplicationException("Entered name of currency does not exist");

            return existCurrency;
        }

        public async Task<Currency> AddNewCurrency(addCurrencyRequest newcurrency)
        {
            ValidateCurrencyName(newcurrency.CurrencyName?.Value);

            var existCurrency = GetCurrency(newcurrency.CurrencyName.Value);
            if (existCurrency != null) 
                throw new ApplicationException("Entered name of currency exists");

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
            ValidateCurrencyId(updatedCurrency.CurrencyId);
            ValidateCurrencyName(updatedCurrency.CurrencyName?.Value);

            var existCurrency = GetCurrencyById(updatedCurrency.CurrencyId);
            existCurrency.CurrencyName = updatedCurrency.CurrencyName.Value;

            await _context.SaveChangesAsync();
            return existCurrency;
        }

        protected Currency GetCurrencyById(int currencyId)
        {
            ValidateCurrencyId(currencyId);

            var existCurrency = _context.Currencies.FirstOrDefault(customer => customer.Id == currencyId);
            if (existCurrency == null) 
                throw new ApplicationException("Entered currency does not exist");

            return existCurrency;
        }
        protected void ValidateCurrencyId(int currencyId)
        {
            if (currencyId == 0)
                throw new ApplicationException("Enter valid currency Id");
        }
        protected void ValidateCurrencyName(string currencyName)
        {
            if (string.IsNullOrEmpty(currencyName))
                throw new ApplicationException("Enter valid name currency");
        }
        protected void ValidateExistsCurrency(Currency? currency)
        {
            if (currency == null)
                throw new ApplicationException("Entered currency does not exist");
        }
        protected Currency GetCurrency(string currencyName)
        {
            return _context.Currencies.FirstOrDefault(customer => customer.CurrencyName.ToLower() == currencyName.Trim().ToLower());
        }
    }


}

