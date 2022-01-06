using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.Models;
using gdm5._0.Services;
using gdm5._0.DTO;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Services.Interfaces;

namespace gdm5._0.Services
{
    public class OrderProductService : BaseService<OrderProduct>, IOrderProductService
    {
        private readonly DataContext _context;

        private readonly OrderService _orderServise;

        public OrderProductService(DataContext context) : base(context)
        {
            _context = context;
        }
   
        public async Task<OrderProduct> UpdateOrderProduct(int id, OrderProduct orderProduct)
        {
            OrderProduct p = await GetItem(orderProduct.Id);
            p.Quantity = orderProduct.Quantity;
            p.Order = orderProduct.Order;
            p.Product = orderProduct.Product;

            await _context.SaveChangesAsync();

            return orderProduct;
        }

        public async Task<OrderProduct> UpdateOrderP(OrderDTO orderDTO)
        {
            OrderProduct op = await GetItem(orderDTO.OrderProductId);
            Order o = _context.Orders.Find(orderDTO.OrderId);
            Product product = _context.Products.Find(orderDTO.ProductId);
            await _orderServise.DeleteOrder(o.Id);

            op.Quantity = orderDTO.Quantity;
            o.NameCompany = orderDTO.NameCompany;
            o.TotalPrice = orderDTO.TotalPrice;

           
            if (product.Quantity >= orderDTO.Quantity)
            {
                var newAmout = product.Quantity - orderDTO.Quantity;
                product.Quantity = Math.Round(newAmout, 2);
                await _context.SaveChangesAsync();
            }
            else
            {
                return null;
            }


            return op;
        }


        public async Task<IEnumerable<OrderPDTO>> GetOrderByPruductId(int id)
        {
            var order = await _context.OrderProducts
                                      .Include(o => o.Order)
                                      .Include(op => op.Product)
                                      .Where(or => or.Product.Id == id)
                                      .Select(t => new OrderPDTO
                                      {
                                          Id = t.Id,
                                          NameCompany = t.Order.NameCompany,
                                          TotalPrice = t.Order.TotalPrice,
                                          DateTime = t.Order.DateTime,
                                          OrderProducts = t.Order.OrderProduct.Select(jj => new OrderProductDTO
                                          {   OrderProductId = jj.Id,
                                              Quantity = jj.Quantity,
                                              ProductId = jj.ProductId,
                                              OrderId = jj.OrderId
                                          }).ToList(),
                                          ProductOrders = t.Order.OrderProduct.Select(jj => new ProductOrderDTO
                                          {
                                              ProductNumber = jj.Product.ProductNumber,
                                              Manufacturer = jj.Product.Manufacturer,
                                              NameType = jj.Product.ProductType.NameType,
                                              ProductStandartCost = jj.Product.StandartCost
                                          }).ToList(),

                                      })
                                      .ToListAsync();

            return order;

        }

        public async Task<IEnumerable<OrderPDTO>> GetOrderByOrderId(int id)
        {
            var order = await _context.OrderProducts
                                      .Include(o => o.Order)
                                      .Include(op => op.Product)
                                      .Where(o => o.Order.Id == id)
                                      .Select(t => new OrderPDTO
                                      {
                                          Id = t.Id,
                                          NameCompany = t.Order.NameCompany,
                                          TotalPrice = t.Order.TotalPrice,
                                          DateTime = t.Order.DateTime,
                                          OrderProducts = t.Order.OrderProduct.Select(jj => new OrderProductDTO
                                          {
                                              OrderProductId = jj.Id,
                                              Quantity = jj.Quantity,
                                              ProductId = jj.ProductId,
                                              OrderId = jj.OrderId
                                          }).ToList(),
                                          ProductOrders = t.Order.OrderProduct.Select(jj => new ProductOrderDTO
                                          {
                                              ProductNumber = jj.Product.ProductNumber,
                                              Manufacturer = jj.Product.Manufacturer,
                                              NameType = jj.Product.ProductType.NameType,
                                              ProductStandartCost = jj.Product.StandartCost
                                          }).ToList(),

                                      })
                                      .ToListAsync();

            return order;

        }

    }
}
