using gdm5._0.DTO;

namespace gdm5._0.Requests.Customer
{
    public class UpdateCurrencyRequest
    {
        public int CurrencyId { get; set; }
        public Property CurrencyName { get; set; }
    }
}
