using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Models;
using gdm5._0.DTO;
using gdm5._0.Services.Interfaces;
using gdm5._0.Requests.Product;
using Microsoft.AspNetCore.Http;
using gdm5._0.Requests.Customer;

namespace gdm5._0.Services
{
    public class WareHouseService : BaseService<WareHouse>, IWareHouseService
    {
        private readonly DataContext _context;

        public WareHouseService(DataContext context) : base(context)
        {
            _context = context;
        }

        public string[] GetNamesWareHouses()
        {
            return _context.WareHouse.Select(customer => customer.Name).ToArray();
        }

        public WareHouse GetWareHouseByName(string nameWareHouse)
        {

            if (string.IsNullOrEmpty(nameWareHouse))
                throw new ApplicationException("Enter valid name WareHouse");

            var existWareHouse = _context.WareHouse.FirstOrDefault(customer => customer.Name.ToLower() == nameWareHouse.ToLower());
            if (existWareHouse == null) throw new ApplicationException("Entered name of WareHouse does not exist");

            return existWareHouse;
        }

        public async Task<WareHouse> AddNewWareHouse(addWareHouseRequest newWareHouse)
        {
            if (string.IsNullOrEmpty(newWareHouse.Name.Value))
                throw new ApplicationException("Enter valid name WareHouse");

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
            if (newWareHouse.WareHouseId == 0)
                throw new ApplicationException("Enter valid warehouseId");

            if (string.IsNullOrEmpty(newWareHouse.Name.Value))
                throw new ApplicationException("Enter valid name warehouse");

            var existWarehouse = _context.WareHouse.FirstOrDefault(warehouse => 
                                          warehouse.Id == newWareHouse.WareHouseId);
            if (existWarehouse == null) throw new ApplicationException("Entered warehouse does not exist");


            existWarehouse.Name = newWareHouse.Name.Value; // needs check existed name 
            existWarehouse.Address = newWareHouse.Address?.Value ?? "";
            existWarehouse.LocationDetails = newWareHouse.LocationDetails?.Value ?? "";
            existWarehouse.Sector = newWareHouse.Sector?.Value ?? "";

            await _context.SaveChangesAsync();

            return existWarehouse;
        }
    }
}

