using System;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using gdm5._0.Requests.Customer;

namespace gdm5._0.Services
{
    public class WareHouseService : BaseService<WareHouse>, IWareHouseService
    {
        public WareHouseService(DataContext context) : base(context)
        {
        }

        public string[] GetNamesWareHouses()
        {
            return _context.WareHouse.Select(customer => customer.Name).ToArray();
        }
        public WareHouse GetWareHouseByName(string nameWareHouse)
        {
            ValidateWareHouseName(nameWareHouse);
            var existWareHouse = _context.WareHouse.FirstOrDefault(customer => customer.Name.ToLower() == nameWareHouse.ToLower());
            if (existWareHouse == null) 
                throw new ApplicationException("Entered name of WareHouse does not exist");

            return existWareHouse;
        }
        public async Task<WareHouse> AddNewWareHouse(addWareHouseRequest newWareHouse)
        {
            ValidateWareHouseName(newWareHouse?.Name?.Value);

            var WareHouse = new WareHouse()
            {
                Name = newWareHouse.Name.Value,
                Address = newWareHouse.Address?.Value ?? "",
                LocationDetails = newWareHouse.LocationDetails?.Value ?? "",
                Sector = newWareHouse.Sector?.Value ?? ""
            };

            _context.WareHouse.Add(WareHouse);
            await _context.SaveChangesAsync();

            return WareHouse;
        }
        public async Task<WareHouse> UpdateWareHouse(UpdateWareHouseRequest newWareHouse)
        {
            ValidateWareHouseName(newWareHouse?.Name?.Value);
            var existWarehouse = GetWareHouseById(newWareHouse.WareHouseId);

            existWarehouse.Name = newWareHouse.Name.Value; // needs check existed name 
            existWarehouse.Address = newWareHouse.Address?.Value ?? "";
            existWarehouse.LocationDetails = newWareHouse.LocationDetails?.Value ?? "";
            existWarehouse.Sector = newWareHouse.Sector?.Value ?? "";

            await _context.SaveChangesAsync();

            return existWarehouse;
        }
        public WareHouse GetWareHouseById(int wareHouseId)
        {
            ValidateWareHouseId(wareHouseId);
            var existWarehouse = _context.WareHouse.FirstOrDefault(warehouse => warehouse.Id == wareHouseId);
            if (existWarehouse == null) 
                throw new ApplicationException("Entered warehouse does not exist");

            return existWarehouse;
        }

        protected void ValidateWareHouseName(string nameWareHouse)
        {
            if (string.IsNullOrEmpty(nameWareHouse))
                throw new ApplicationException("Enter valid name WareHouse");
        }
        protected void ValidateWareHouseId(int wareHouseId)
        {
            if (wareHouseId == 0)
                throw new ApplicationException("Enter valid warehouse id");
        }
    }
}

