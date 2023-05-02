using gdm5._0.DTO;

namespace gdm5._0.Requests.Customer
{
    public class UpdateWareHouseRequest
    {
        public int WareHouseId { get; set; }
        public Property Name { get; set; }
        public Property Address { get; set; }
        public Property LocationDetails { get; set; }
        public Property Sector { get; set; }
        
    };
}
