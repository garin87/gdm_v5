using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.Models;
using gdm5._0.DTO;
using gdm5._0.Controllers;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Services.Interfaces;

namespace gdm5._0.Services
{
    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly DataContext _context;

        public OrderService(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Order> DeleteOrder(int id)
        {
            Order order = _context.Orders
              .Where(o => o.Id == id)
              .FirstOrDefault();

            var product = _context.OrderProducts
                                  .Include(k => k.Product)
                                  .Where(f => f.OrderId == id).Select(dd => dd.Product).FirstOrDefault();
            if (product != null)
            {
                await UpdateOrderQuantity(id);
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> UpdateOrder(int id, Order order)
        {
            Order p = await GetItem(order.Id);
            p.NameCompany = order.NameCompany;
            p.TotalPrice = order.TotalPrice;
            p.DateTime = order.DateTime;

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<OrderDTO> AddOrders(OrderDTO orderDTO)
        {
            var EntryOrder = _context.Orders.Find(orderDTO.Id);
            if (EntryOrder != null) return orderDTO;

            var order = new Order
            {
                Id = orderDTO.Id,
                NameCompany = orderDTO.NameCompany,
                TotalPrice = orderDTO.TotalPrice,
                DateTime = DateTime.Now

            };

            _context.Orders.Add(order);

            var orderProduct = new OrderProduct
            {  
                OrderId = order.Id,
                ProductId = orderDTO.ProductId,
                Quantity = orderDTO.Quantity
            };

            _context.OrderProducts.Add(orderProduct);

         
            Product product = _context.Products.Find(orderDTO.ProductId);
            if(product.Quantity >= orderDTO.Quantity)
            {
                var newAmout = product.Quantity - orderDTO.Quantity;
                product.Quantity = Math.Round(newAmout, 2);
                await _context.SaveChangesAsync();
            }
            else
            {
               return null;
            }
            
            return orderDTO;
        }

        public async Task<Order> DeleteOrders(int id)
        {
            var order =  _context.Orders
                   .Include(p => p.OrderProduct)
                   .Where(i => i.Id == id)
                   .FirstOrDefault();

            await _context.SaveChangesAsync();
            return order;
        }

        public async Task<Product> UpdateOrderQuantity(int id)
        {
            Product pr = _context.OrderProducts
                           .Include(aa => aa.Product)
                           .Where(k => k.OrderId == id).Select(dd => dd.Product).FirstOrDefault();

            var valueQuantity = _context.OrderProducts
                            .Include(op => op.Product)
                            .Include(prod => prod.Order)
                            .Where(i => i.Order.Id == id)
                            .FirstOrDefault().Product.Quantity;

            var valueOPQuantity = _context.OrderProducts
                             .Include(op => op.Product)
                             .Include(prod => prod.Order)
                             .Where(i => i.Order.Id == id)
                             .FirstOrDefault().Quantity;

            pr.Quantity = valueQuantity + valueOPQuantity;
            await _context.SaveChangesAsync();

            return pr;
        }



        public async Task<IEnumerable<Order>> GetOrders()
        {
            var order = await _context.Orders
                                      .Include(o => o.OrderProduct)
                                      .Select(t => new Order
                                      {  
                                          Id = t.Id,
                                          NameCompany = t.NameCompany,
                                          TotalPrice = t.TotalPrice,
                                          DateTime = t.DateTime,
                                          OrderProduct = t.OrderProduct.Select( h => new OrderProduct
                                          {   
                                              Id = h.Id,
                                              Quantity = h.Quantity,
                                              OrderId = h.Order.Id,
                                              ProductId = h.Product.Id
       
                                          }).ToList(),
                                          
                                      })
                                      .ToListAsync();

            return order;

        }

        public async Task<IEnumerable<OrderPDTO>> GetOrderProduct()
        {
            var order = await _context.Orders
                                      .Include(o => o.OrderProduct)
                                        .ThenInclude(op => op.Product)
                                      .Select(t => new OrderPDTO
                                      {
                                          Id = t.Id,
                                          NameCompany = t.NameCompany,
                                          TotalPrice = t.TotalPrice,
                                          DateTime = t.DateTime,
                                          OrderProducts = t.OrderProduct.Select(jj => new OrderProductDTO {
                                              Quantity = jj.Quantity,
                                              ProductId = jj.ProductId,
                                              OrderId = jj.OrderId
                                          }).ToList(),
                                          ProductOrders = t.OrderProduct.Select(jj => new ProductOrderDTO
                                          {
                                              ProductNumber = jj.Product.ProductNumber,
                                              Manufacturer = jj.Product.Manufacturer,
                                              NameType = jj.Product.ProductType.NameType,
                                              ProductStandartCost = jj.Product.StandartCost
                                          }).ToList(),
                                          
                                      }).OrderByDescending(or => or.DateTime)
                                      .ToListAsync();

            return order;

        }

        public async Task<IEnumerable<OrderPDTO>> GetOrderByNameCompany(string nameCompany)
        {
            var order = await _context.Orders
                                      .Include(o => o.OrderProduct)
                                        .ThenInclude(op => op.Product)
                                      .Where(s => s.NameCompany == nameCompany)
                                      .Select(t => new OrderPDTO
                                      {
                                          Id = t.Id,
                                          NameCompany = t.NameCompany,
                                          TotalPrice = t.TotalPrice,
                                          DateTime = t.DateTime,
                                          OrderProducts = t.OrderProduct.Select(jj => new OrderProductDTO
                                          {
                                              Quantity = jj.Quantity,
                                              ProductId = jj.ProductId,
                                              OrderId = jj.OrderId
                                          }).ToList(),
                                          ProductOrders = t.OrderProduct.Select(jj => new ProductOrderDTO
                                          {
                                              ProductNumber = jj.Product.ProductNumber,
                                              Manufacturer = jj.Product.Manufacturer,
                                              NameType = jj.Product.ProductType.NameType,
                                              ProductStandartCost = jj.Product.StandartCost
                                          }).ToList(),

                                      }).OrderByDescending(or => or.DateTime)
                                      .ToListAsync();

            return order;

        }


    }
}
