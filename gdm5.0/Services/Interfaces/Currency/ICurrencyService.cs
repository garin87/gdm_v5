using System.Threading.Tasks;
using gdm5._0.Models;
using gdm5._0.Requests.Customer;

namespace gdm5._0.Services.Interfaces
{
    public interface ICurrencyService : IBaseServices<Currency>
    {
        string[] GetNamesCurrencies();
        Currency GetCurrencyByName(string nameCurrency);
        Task<Currency> AddNewCurrency(addCurrencyRequest newcurrency);
        Task<Currency> UpdateCurrency(UpdateCurrencyRequest updatedCurrency);
    }
}
