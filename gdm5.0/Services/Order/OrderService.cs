using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.Models;
using gdm5._0.DTO;
using gdm5._0.Controllers;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Services.Interfaces;
using gdm5._0.Requests.Product;
using gdm5._0.Extensions;
using Microsoft.AspNetCore.Http;
using gdm5._0.Domain.Models.Filters;
using gdm5._0.Filters;
using gdm5._0.Helpers;
using gdm5._0.Domain.Models.Order;
using gdm5._0.Shared.Constants;
using gdm5._0.Domain.Models;

namespace gdm5._0.Services
{
    public class OrderService : BaseService<Order>, IOrderService
    {
        private readonly DataContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;
        private readonly IUriService _uriService;
        public OrderService(DataContext context, IHttpContextAccessor httpContextAccessor,
            IProductService ProductService, IUriService uriService) : base(context)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _productService = ProductService;
            this._uriService = uriService;
        }

        public string[] getOrderNameCompanies()
        {
            return _context.Orders.Select(order => order.NameCompany).Distinct().ToArray();
        }

        public string[] getOrderNameCompanies(string NameCompany)
        {
            if (string.IsNullOrEmpty(NameCompany))
                throw new ApplicationException("Enter valid name company");

            return _context.Orders.Where(order => order.NameCompany == NameCompany)
                                  .Select(order => order.NameCompany)
                                  .Distinct().ToArray();
        }

        public string[] getNamesProduct()
        {
            return _context.Orders.SelectMany(order => order.OrderProduct
                                                      .Select(pp => pp.Product.ProductType.NameType))
                                                      .Distinct().ToArray();
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
                //   await UpdateOrderQuantity(id);
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
            p.OrderCreatedTime = order.OrderCreatedTime;

            await _context.SaveChangesAsync();

            return order;
        }

        public async Task AddOrder(AddOrderRequest orderData)
        {
            if (orderData.ProductId == 0)
                throw new ApplicationException("Enter valid ProductId");

            var product = _context.Products.FirstOrDefault(product => (product.Id == orderData.ProductId));
            if (product == null)
                throw new ApplicationException("Product Id does not exist");

            if (string.IsNullOrEmpty(orderData.company?.Value))
                throw new ApplicationException("Entered name of company does not exist");

            var company = _context.Customer.FirstOrDefault(Customer => (Customer.NameCompany == orderData.company.Value));
            var companyId = 0;
            if (company != null)
            {
                companyId = company.Id;
            }
            else throw new ApplicationException("Entered name of company does not exist"); ;

            var roundedQ = Math.Round(product.Quantity, 2);
            if (roundedQ >= orderData.quantityorder?.Value.ParseDouble())
            {
                var newQuantity = roundedQ - orderData.quantityorder?.Value.ParseDouble();
                product.Quantity = newQuantity ?? product.Quantity;
                product.DateOfLastChanged = DateTime.Now;
                product.LastEditedByUser = this._httpContextAccessor.HttpContext.User.Identity.Name;
                await _context.SaveChangesAsync();
            }
            else
                throw new ApplicationException("Entered quantity of product invalid");

            var orderStatusCompleted = _context.OrderStatus.FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Completed);

            int currencyId = GetOrCreateCurrency("BYN"); //BYN; 
            double orderQ = orderData.quantityorder.Value.ParseDouble();
            double priceForQ = (orderData.totalprice?.Value.ParseDouble() ?? 0) * orderQ;

            var order = new Order
            {
                Id = 0,
                NameCompany = orderData.company?.Value,
                TotalPrice = priceForQ,
                OrderCreatedTime = DateTime.Now,
                OrderNumber = orderData.number?.Value.ParseInt() ?? 0,
                OrderCreatedByUser = this._httpContextAccessor.HttpContext.User.Identity.Name,
                Description = orderData.description?.Value,
                CurrencyId = currencyId,
                CustomerId = companyId,
                OrderStatusId = orderStatusCompleted.Id
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            ProductHistory deletedProduct = null;
            if (product.Quantity == 0)
            {
                deletedProduct = await this._productService.DeleteProductInstance(product.Id);
            }
            if (deletedProduct == null)
            {
                var orderProduct = new OrderProduct
                {
                    OrderId = order.Id,
                    ProductId = orderData.ProductId,
                    Quantity = orderData.quantityorder?.Value.ParseDouble() ?? 0,
                    TotalPrice = priceForQ,
                    TaxNDS = orderData.taxnds?.Value.ParseDouble() ?? 0,
                    Markup = orderData.markup?.Value.ParseDouble() ?? 0,

                };
                _context.OrderProducts.Add(orderProduct);
            }
            else
            {
                var orderProduct = new OrderProductHistory
                {
                    OrderId = order.Id,
                    ProductHistoryId = deletedProduct.Id,
                    Quantity = orderData.quantityorder?.Value.ParseFloat() ?? 0,
                    TotalPrice = priceForQ,
                 //   TaxNDS = orderData.taxnds?.Value.ParseDouble() ?? 0,
                 //   Markup = orderData.markup?.Value.ParseDouble() ?? 0,
                };
                _context.OrderProductHistory.Add(orderProduct);
            }


            await _context.SaveChangesAsync();
        }
        public async Task AddToCartOrder(AddOrderRequest orderData)
        {
            if (orderData.ProductId == 0)
                throw new ApplicationException("Enter valid ProductId");

            var product = _context.Products.FirstOrDefault(product => (product.Id == orderData.ProductId));
            if (product == null)
                throw new ApplicationException("Product Id does not exist");

           

            var orderStatusProcessing = _context.OrderStatus.FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Processing);
            var cartOrder = _context.Orders.FirstOrDefault(order => order.OrderStatusId == orderStatusProcessing.Id);

            int orderId;
            double orderQ = orderData.quantityorder.Value.ParseDouble();
            double priceForQ = (orderData.totalprice?.Value.ParseDouble() ?? 0) * orderQ;
            if (cartOrder == null)
            {
                if (string.IsNullOrEmpty(orderData.company?.Value))
                    throw new ApplicationException("Cart is Empty. Enter name of company for order");

                var company = _context.Customer.FirstOrDefault(Customer => (Customer.NameCompany == orderData.company.Value));
                var companyId = 0;
                if (company != null)
                {
                    companyId = company.Id;
                }
                else throw new ApplicationException("Entered name of company does not exist");

                await CheckAndChangeQuantityProduct(product, orderData);

                int currencyId = GetOrCreateCurrency("BYN"); //BYN; 

               
                var newOrder = new Order
                {
                    Id = 0,
                    NameCompany = orderData.company?.Value,
                    TotalPrice = priceForQ,
                    OrderCreatedTime = DateTime.Now,
                    OrderNumber = orderData.number?.Value.ParseInt() ?? 0,
                    OrderCreatedByUser = this._httpContextAccessor.HttpContext.User.Identity.Name,
                    Description = orderData.description?.Value,
                    CurrencyId = currencyId,
                    CustomerId = companyId,
                    OrderStatusId = orderStatusProcessing.Id
                };
                _context.Orders.Add(newOrder);
                await _context.SaveChangesAsync();

                orderId = newOrder.Id;
            }
            else
            {
                orderId = cartOrder.Id;
                await CheckAndChangeQuantityProduct(product, orderData);
            }

                       
            ProductHistory deletedProduct = null;
            if (product.Quantity == 0)
            {
                deletedProduct = await this._productService.DeleteProductInstance(product.Id);
            }
            if (deletedProduct == null)
            {
                var orderProduct = new OrderProduct
                {
                    OrderId = orderId,
                    ProductId = orderData.ProductId,
                    Quantity = orderData.quantityorder?.Value.ParseFloat() ?? 0,
                    TotalPrice = priceForQ,
                    TaxNDS = orderData.taxnds?.Value.ParseDouble() ?? 0,
                    Markup = orderData.markup?.Value.ParseDouble() ?? 0,

                };
                _context.OrderProducts.Add(orderProduct);
            }
            else
            {
                var orderProduct = new OrderProductHistory
                {
                    OrderId = orderId,
                    ProductHistoryId = deletedProduct.Id,
                    Quantity = orderData.quantityorder?.Value.ParseFloat() ?? 0,
                    TotalPrice = priceForQ,
                    Markup = orderData?.markup?.Value.ParseDouble() ?? 0,
                    TaxNDS = orderData?.taxnds?.Value.ParseDouble() ?? 0,
                };
                _context.OrderProductHistory.Add(orderProduct);
            }

            await _context.SaveChangesAsync();
        }
        private async Task CheckAndChangeQuantityProduct(Product product, AddOrderRequest orderData)
        {

            var roundedQ = Math.Round(product.Quantity, 2);
            if (roundedQ >= orderData.quantityorder?.Value.ParseDouble())
            {
                var newQuantity = roundedQ - orderData.quantityorder?.Value.ParseDouble();
                product.Quantity = newQuantity ?? product.Quantity;
                product.DateOfLastChanged = DateTime.Now;
                product.LastEditedByUser = this._httpContextAccessor.HttpContext.User.Identity.Name;
                await _context.SaveChangesAsync();
            }
            else
                throw new ApplicationException("Entered quantity of product invalid");
        }
        public async Task addOrderProductList(addOrderListProductRequest orderData)
        {

            if (orderData.orderProductList.Count() <= 1) throw new ApplicationException("Order ProductList is empty"); ;

            // validate ProductList
            foreach (var productOrder in orderData.orderProductList)
            {
                if (productOrder.ProductId == 0)
                    throw new ApplicationException("Enter valid ProductId");

                var product = _context.Products.FirstOrDefault(product => (product.Id == productOrder.ProductId));
                if (product == null)
                    throw new ApplicationException("Product Id does not exist");


                if (product.Quantity >= productOrder.quantityorder?.Value.ParseDouble())
                {
                }
                else
                    throw new ApplicationException("Entered quantity: " + product.Quantity + 
                       "of product: " + product.Name + "ProductNumber " + product.ProductNumber + " invalid" );
            }
          

            
            var company = _context.Customer.FirstOrDefault(Customer => (Customer.NameCompany == orderData.company.Value));
            var companyId = 0;
            if (company != null)
            {
                companyId = company.Id;
            }
            else throw new ApplicationException("Entered name of company does not exist");

            
            int currencyId = GetOrCreateCurrency("BYN"); //BYN; 

            var totalPriceList = orderData.orderProductList.Select(product => product.totalprice.Value.ParseDouble());
            var totalOrderPrice = totalPriceList.Sum();
            var order = new Order
            {
                Id = 0,
                NameCompany = orderData.company?.Value,
                TotalPrice = totalOrderPrice,
                OrderCreatedTime = DateTime.Now,
                OrderNumber = random.Next(9999999),
                OrderCreatedByUser = this._httpContextAccessor.HttpContext.User.Identity.Name,
                Description = orderData.description?.Value,
                CurrencyId = currencyId,
                CustomerId = companyId,
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();



            foreach (var productOrder in orderData.orderProductList) {

                var product = _context.Products.FirstOrDefault(product => (product.Id == productOrder.ProductId));

                var newQuantity = product.Quantity - productOrder.quantityorder?.Value.ParseFloat();
                product.Quantity = newQuantity ?? product.Quantity;
                product.DateOfLastChanged = DateTime.Now;
                await _context.SaveChangesAsync();

                ProductHistory deletedProduct = null;
                if (product.Quantity == 0)
                {
                    deletedProduct = await this._productService.DeleteProductInstance(product.Id);
                }


                if (deletedProduct == null)
                {
                    var orderProduct = new OrderProduct
                    {
                        OrderId = order.Id,
                        ProductId = productOrder.ProductId,
                        Quantity = productOrder.quantityorder?.Value.ParseFloat() ?? 0,
                        TotalPrice = productOrder.totalprice?.Value.ParseDouble() ?? 0,
                        TaxNDS = productOrder.taxnds?.Value.ParseDouble() ?? 0,
                        Markup = productOrder.markup?.Value.ParseDouble() ?? 0,
                    };
                    _context.OrderProducts.Add(orderProduct);
                }
                else
                {
                    var orderProduct = new OrderProductHistory
                    {
                        OrderId = order.Id,
                        ProductHistoryId = deletedProduct.Id,
                        Quantity = productOrder.quantityorder?.Value.ParseFloat() ?? 0,
                        TotalPrice = productOrder.totalprice?.Value.ParseDouble() ?? 0,
                        TaxNDS = productOrder.taxnds?.Value.ParseDouble() ?? 0,
                        Markup = productOrder.markup?.Value.ParseDouble() ?? 0,
                    };
                    _context.OrderProductHistory.Add(orderProduct);
                }

                await _context.SaveChangesAsync();

            }

           
        }

        public async Task saveOrderCart()
        {

            var orderStatusProcessing = _context.OrderStatus
                .FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Processing);
            var cartOrder = _context.Orders
                .FirstOrDefault(order => order.OrderStatusId == orderStatusProcessing.Id);

            if (cartOrder == null)
                throw new ApplicationException("Cart is empty");

            var orderStatusCompleted = _context.OrderStatus
                .FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Completed);


            cartOrder.OrderStatusId = orderStatusCompleted.Id;
            cartOrder.OrderCreatedTime = DateTime.Now;
            cartOrder.OrderCreatedByUser = this._httpContextAccessor.HttpContext.User.Identity.Name;
            await _context.SaveChangesAsync();
        }

        public PagedResponseDTO<List<OrderDomain>> getOrderProduct(string companyName,
           PaginationFilterDTO pageFilter, string route, SortOptionsDTO sortOption,
           OrderFilter filter)
        {
            var validFilter = new PaginationFilterDTO(pageFilter.PageNumber, pageFilter.PageSize);
            if (!string.IsNullOrEmpty(companyName))
            {
                filter.NameCompany = companyName;
            }
  

            var isSortActive = string.IsNullOrEmpty(sortOption.Name) && string.IsNullOrEmpty(sortOption.Direction);

            filter.StartIndex = validFilter.PageNumber - 1;
            filter.CountInstances = validFilter.PageSize;

            var filterAssigner = new OrderAssigner(filter);
            var instancesOfOrder = _context.Orders.AsNoTracking()
                                                  .Include(order => order.OrderProduct)
                                                     .ThenInclude(orderProduct => orderProduct.Product)
                                                     .ThenInclude(product => product.ProductType)
                                                     .ThenInclude(productType => productType.Parameters)
                                                     .ThenInclude(parameters => parameters.ProductParameters)
                                                  .Include(product => product.Currency)
                                                  .Include(order => order.OrderProductHistory)
                                                     .ThenInclude(parameters => parameters.ProductHistory)
                                                  .ApplyPagingFilter(filterAssigner);

           

            var orders = new List<OrderDomain>();


            foreach (var order in instancesOfOrder)
            {
                var orderP = order.OrderProduct;
                var orderPH = order.OrderProductHistory;

                var OProductNumber = "";
                var OManufacturer = "";
                var OCurrency = "";
                var OProductName = "";
                var OQuantity = 0.0;
                   if (orderP.Count() > 0)
                {
                    OProductNumber = order.OrderProduct.Select(orderProduct => orderProduct.Product.ProductNumber).FirstOrDefault();
                    OManufacturer = order.OrderProduct.Select(orderProduct => orderProduct.Product.Manufacturer).FirstOrDefault();
                    OCurrency = order.Currency.CurrencyName;
                    OProductName = order.OrderProduct.Select(orderProduct => orderProduct.Product.Name).FirstOrDefault();
                    OQuantity = order.OrderProduct.Select(orderProduct => orderProduct.Quantity).FirstOrDefault();
                    OQuantity = Math.Round(OQuantity, 2);
                }
                if (orderPH.Count() > 0)
                {
                    OProductNumber = order.OrderProductHistory.Select(orderProduct => orderProduct.ProductHistory.ProductNumber).FirstOrDefault();
                    OManufacturer = order.OrderProductHistory.Select(orderProduct => orderProduct.ProductHistory.Manufacturer).FirstOrDefault();
                   // OCurrency = product.OrderProductHistory.Select(orderProduct => orderProduct.ProductHistory..CurrencyName).FirstOrDefault();
                    OProductName = order.OrderProductHistory.Select(orderProduct => orderProduct.ProductHistory.Name).FirstOrDefault();
                   
                    OQuantity = order.OrderProductHistory.Select(orderProduct => orderProduct.Quantity).FirstOrDefault();
                    OQuantity = Math.Round(OQuantity, 2);
                }

                var taxNDS = order.OrderProduct.Select(orderPrduct => orderPrduct.TaxNDS).FirstOrDefault();
                var productDTO = new OrderDomain()
                {
                    NameCompany = order.NameCompany,
                    ProductNumber = OProductNumber,
                    Manufacturer = OManufacturer,
                    TotalPrice = order.TotalPrice,
                    TaxNDS = taxNDS,
                    Description = order.Description,
                    OrderCreatedTime = order.OrderCreatedTime.ToString("MM/dd/yyyy HH:mm"),
                    OrderCreatedByUser = order.OrderCreatedByUser,
                    Currency = OCurrency,
                    ProductName = OProductName,
                    Quantity = OQuantity,
                };
                // var parameters = _context.Parameters.Where(param => param.ProductTypeId == productType.Id);
                var parameters = order.OrderProduct.SelectMany(orderProduct =>
                                                             orderProduct.Product.ProductType.Parameters);
                foreach (var typeParam in parameters)
                {

                    var paramDTO = new ParameterDTO();
                    var value = order.OrderProduct.SelectMany(orderProduct => orderProduct.Product.ProductParameters)
                                                    .FirstOrDefault(t => t.ParameterId == typeParam.Id);

                    if (value != null)
                    {
                        paramDTO.Id = value.Id;
                        paramDTO.Value = value.Value;
                        paramDTO.Priority = typeParam.Priority;
                        paramDTO.NameType = typeParam.NameType;
                    }

                    paramDTO.ParameterId = typeParam.Id;
                    paramDTO.Name = typeParam.Name;
                    productDTO.Parameters.Add(paramDTO);

                }

                orders.Add(productDTO);
            }

            var totalRecords = instancesOfOrder.Count();
            var orderReponse = PaginationHelper.CreatePagedReponse<OrderDomain>(orders, 
                validFilter, totalRecords, _uriService, route);

            return orderReponse;
        }

        public PagedResponseDTO<List<OrderProductListDomain>> getOrderProducts(string companyName, PaginationFilterDTO pageFilter,
                                                               string route, SortOptionsDTO sortOption, OrderFilter filter)
        {
            var validFilter = new PaginationFilterDTO(pageFilter.PageNumber, pageFilter.PageSize);
            var isSortActive = string.IsNullOrEmpty(sortOption.Name) && string.IsNullOrEmpty(sortOption.Direction);

            filter.StartIndex = validFilter.PageNumber - 1;
            filter.CountInstances = validFilter.PageSize;

            var filterAssigner = new OrderAssigner(filter);
            var orderStatusProcessing = _context.OrderStatus.FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Processing);

            var instancesOfOrder = _context.Orders.Where(order=> order.OrderStatusId != orderStatusProcessing.Id)
                                                  .OrderByDescending(order => order.OrderCreatedTime)
                                                  .AsNoTracking()
                                                  .Include(order => order.OrderProduct)
                                                     .ThenInclude(orderProduct => orderProduct.Product)
                                                     .ThenInclude(productParameter => productParameter.ProductType.Parameters)
                                                  .Include(product => product.Currency)
                                                  .Include(order => order.OrderProductHistory)
                                                     .ThenInclude(parameters => parameters.ProductHistory)
                                                     .ThenInclude(productHistory => productHistory.ProductParameterHistory)
                                                  .ApplyPagingFilter(filterAssigner);


            var parametersValueOfOrder = _context.Orders.Where(order => order.OrderStatusId != orderStatusProcessing.Id).AsNoTracking()
                                                 .Include(order => order.OrderProduct)
                                                    .ThenInclude(orderProduct => orderProduct.Product)
                                                    .ThenInclude(product => product.ProductParameters).ToList();

            var orders = new List<OrderProductListDomain>();


            foreach (var order in instancesOfOrder)
            {
                var orderP = order.OrderProduct;
                var orderPH = order.OrderProductHistory;
                var taxNDS = order.OrderProduct.Select(orderPrduct => orderPrduct.TaxNDS).FirstOrDefault();

                List<OrderProductDomain> orderProductListDTO = new List<OrderProductDomain>(); ;

                if (orderP.Count() > 0)
                {
                    foreach (var orderProduct in orderP)
                    {
                        var oProduct = new OrderProductDomain()
                        {
                            ProductId = orderProduct.ProductId,
                            TotalPrice = Math.Round(orderProduct.TotalPrice, 2),
                            Quantity = Math.Round(orderProduct.Quantity, 2),
                            TaxNDS = taxNDS,
                            Markup = orderProduct.Markup,
                            ProductNumber = orderProduct.Product.ProductNumber,
                            Manufacturer = orderProduct.Product.Manufacturer,
                            ProductName = orderProduct.Product.Name,

                        };

                        var oProductParameters = orderProduct.Product.ProductType.Parameters;
                        var orderProductParameters = _context.ProductParameters
                           .Where(pParameter => pParameter.ProductId == orderProduct.Product.Id).ToList();


                        oProduct = this.getOrderProductParameter(oProductParameters, orderProductParameters, oProduct);
                        orderProductListDTO.Add(oProduct);
                    }
                }

                if (orderPH.Count() > 0)
                {
                    foreach (var orderProductHistory in orderPH)
                    {
                        var oProduct = new OrderProductDomain()
                        {
                            ProductId = orderProductHistory.ProductHistoryId,
                            TotalPrice = Math.Round(orderProductHistory.TotalPrice, 2),
                            Quantity = Math.Round(orderProductHistory.Quantity, 2),
                            TaxNDS = taxNDS,
                            Markup = orderProductHistory.Markup,
                            ProductNumber = orderProductHistory.ProductHistory.ProductNumber,
                            Manufacturer = orderProductHistory.ProductHistory.Manufacturer,
                            ProductName = orderProductHistory.ProductHistory.Name,

                        };
                        var oProductParameters = _context.ParameterHistory
                            .Where(parameterHistory => parameterHistory.ProductTypeHistoryId == orderProductHistory.ProductHistory.ProductTypeHistoryId)
                            .ToList();

                        var orderProductParameters = _context.ProductParameterHistory
                            .Where(productParameters => productParameters.ProductHistoryId == orderProductHistory.ProductHistory.Id)
                            .ToList();

                        oProduct = this.getOrderProductParameterHistory(oProductParameters, orderProductParameters, oProduct);
                        orderProductListDTO.Add(oProduct);
                    }
                }


                var orderDTO = new OrderProductListDomain()
                {
                    OrderId = order.Id,
                    NameCompany = order.NameCompany,
                    TotalPrice = Math.Round(orderProductListDTO.Sum(product => product.TotalPrice),2),
                    Description = order.Description,
                    OrderCreatedTime = order.OrderCreatedTime.ToString("MM/dd/yyyy HH:mm"),
                    OrderCreatedByUser = order.OrderCreatedByUser,
                    Currency = order.Currency.CurrencyName,
                    Products = orderProductListDTO
                };

                orders.Add(orderDTO);
            }
           // orders.Reverse();
            var totalRecords = _context.Orders.Count();
            var orderReponse = PaginationHelper.CreatePagedReponse<OrderProductListDomain>(orders,
                validFilter, totalRecords, this._uriService, route);

            return orderReponse;
        }

        public PagedResponseDTO<List<OrderProductListDomain>> getCartOrderProducts(string route)
        {
            var validFilter = new PaginationFilterDTO(1, 100);

            var orderStatusProcessing = _context.OrderStatus.FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Processing);
            var cartOrder = _context.Orders.FirstOrDefault(order => order.OrderStatusId == orderStatusProcessing.Id);

            if (cartOrder == null)
            {
                return PaginationHelper.CreatePagedReponse<OrderProductListDomain>(new List<OrderProductListDomain>(),
               validFilter, 1, _uriService, route);
            }

            var OrderProcessing = _context.Orders.Where(order => order.Id == cartOrder.Id)
                                                  .Include(order => order.OrderProduct)
                                                     .ThenInclude(orderProduct => orderProduct.Product)
                                                     .ThenInclude(product => product.ProductType.Parameters)
                                                     .ThenInclude(parameters => parameters.ProductParameters)
                                                  .Include(product => product.Currency)
                                                  .Include(order => order.OrderProductHistory)
                                                     .ThenInclude(parameters => parameters.ProductHistory);




            var orders = new List<OrderProductListDomain>();


            foreach (var order in OrderProcessing)
            {
                var orderP = order.OrderProduct;
                var orderPH = order.OrderProductHistory;
                var taxNDS = order.OrderProduct.Select(orderPrduct => orderPrduct.TaxNDS).FirstOrDefault();

                List<OrderProductDomain> orderProductListDTO = new List<OrderProductDomain>(); ;

                if (orderP.Count() > 0)
                {
                    foreach (var orderProduct in orderP)
                    {
                        var oProduct = new OrderProductDomain()
                        {
                            ProductId = orderProduct.ProductId,
                            TotalPrice = Math.Round(orderProduct.TotalPrice, 2),
                            Quantity = Math.Round(orderProduct.Quantity, 2),
                   
                            TaxNDS = taxNDS,
                            Markup = orderProduct.Markup,
                            ProductNumber = orderProduct.Product.ProductNumber,
                            Manufacturer = orderProduct.Product.Manufacturer,
                            ProductName = orderProduct.Product.Name,

                        };

                        var oProductParameters = orderProduct.Product.ProductType.Parameters;
                        var orderProductParameters = _context.ProductParameters
                            .Where(pParameter => pParameter.ProductId == orderProduct.Product.Id).ToList();


                        oProduct = this.getOrderProductParameter(oProductParameters, orderProductParameters, oProduct);
                        orderProductListDTO.Add(oProduct);
                    }
                }

                if (orderPH.Count() > 0)
                {
                    foreach (var orderProductHistory in orderPH)
                    {
                        var oProduct = new OrderProductDomain()
                        {
                            ProductId = orderProductHistory.ProductHistoryId,
                            TotalPrice = Math.Round(orderProductHistory.TotalPrice,2),
                            Quantity = Math.Round(orderProductHistory.Quantity, 2),
                            TaxNDS = taxNDS,
                            Markup = orderProductHistory.Markup,
                            ProductNumber = orderProductHistory.ProductHistory.ProductNumber,
                            Manufacturer = orderProductHistory.ProductHistory.Manufacturer,
                            ProductName = orderProductHistory.ProductHistory.Name,

                        };

                        var oProductParameters = _context.ParameterHistory
                            .Where(parameterHistory => parameterHistory.ProductTypeHistoryId == orderProductHistory.ProductHistory.ProductTypeHistoryId)
                            .ToList();

                        var orderProductParameters = _context.ProductParameterHistory
                            .Where(productParameter => productParameter.ProductHistoryId == orderProductHistory.ProductHistory.Id)
                            .ToList();

                        oProduct = this.getOrderProductParameterHistory(oProductParameters, orderProductParameters, oProduct);
                        orderProductListDTO.Add(oProduct);
                    }
                }

               //orderProductListDTO.ToList().Reverse();
                var orderDTO = new OrderProductListDomain()
                {
                    OrderId = order.Id,
                    NameCompany = order.NameCompany,
                    TotalPrice = Math.Round(orderProductListDTO.Sum(product=>product.TotalPrice), 2),
                    Description = order.Description,
                    OrderCreatedTime = order.OrderCreatedTime.ToString("MM/dd/yyyy HH:mm"),
                    OrderCreatedByUser = order.OrderCreatedByUser,
                    Currency = order.Currency.CurrencyName,
                    Products = orderProductListDTO
                };

                orders.Add(orderDTO);
            }
           // orders.Reverse();
            var totalRecords = 1;
            var orderReponse = PaginationHelper.CreatePagedReponse<OrderProductListDomain>(orders,
                validFilter, totalRecords, this._uriService, route);

            return orderReponse;
        }
        private OrderProductDomain getOrderProductParameter(ICollection<Parameter> oProductParameters,
          ICollection<ProductParameter> orderProductParameters, OrderProductDomain oProduct)
        {
            foreach (var typeParam in oProductParameters)
            {
                var paramDTO = new ParameterDTO();
                var value = orderProductParameters.FirstOrDefault(t => t.ParameterId == typeParam.Id);

                if (value != null)
                {
                    paramDTO.Id = value.Id;
                    paramDTO.Value = value.Value;
                    paramDTO.Priority = typeParam.Priority;
                    paramDTO.NameType = typeParam.NameType;
                }

                paramDTO.ParameterId = typeParam.Id;
                paramDTO.Name = typeParam.Name;
                oProduct.Parameters.Add(paramDTO);

            }

            return oProduct;
        }
        private OrderProductDomain getOrderProductParameterHistory(ICollection<ParameterHistory> oProductParameters,
        ICollection<ProductParameterHistory> orderProductParameters, OrderProductDomain oProduct)
        {
            foreach (var typeParam in oProductParameters)
            {
                var paramDTO = new ParameterDTO();
                var value = orderProductParameters.FirstOrDefault(t => t.ParameterHistoryId == typeParam.Id);

                if (value != null)
                {
                    paramDTO.Id = value.Id;
                    paramDTO.Value = value.Value;
                    paramDTO.Priority = typeParam.Priority;
                    paramDTO.NameType = typeParam.NameType;
                }

                paramDTO.ParameterId = typeParam.Id;
                paramDTO.Name = typeParam.Name;
                oProduct.Parameters.Add(paramDTO);

            }

            return oProduct;
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
                OrderCreatedTime = DateTime.Now

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
                product.Quantity = newAmout;
                await _context.SaveChangesAsync();
            }
            else
            {
               return null;
            }
            
            return orderDTO;
        }

        public void cancelOrderCart()
        {
            var orderStatusProcessing = _context.OrderStatus
                .FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Processing);
            var cartOrder = _context.Orders
                 .Include(order => order.OrderProduct)
                 .Include(orderH => orderH.OrderProductHistory)
                 .FirstOrDefault(order => order.OrderStatusId == orderStatusProcessing.Id);

            if (cartOrder == null)
                throw new Exception("Cart is empty");


            if (cartOrder.OrderProduct.Any())
            {
               // var tempCollection = cartOrder.OrderProduct.ToList();
                foreach (var product in cartOrder.OrderProduct.ToList())
                {
                     this.DeleteCartProduct2(product.ProductId);
                }
            }
           
            if (cartOrder.OrderProductHistory.Any())
            {
                // var tempCollection = cartOrder.OrderProductHistory;
                foreach (var product in cartOrder.OrderProductHistory.ToList())
                {
                    this.DeleteCartProduct2(product.ProductHistoryId);

                }
            }

            if(cartOrder.OrderProductHistory.Count() > 0 && cartOrder.OrderProduct.Count() > 0)
            {
                throw new Exception("Cart is empty");
            }
               

        }
        public void deleteOrderProduct(int idOrder)
        {

            var cartOrder = _context.Orders
                 .Include(order => order.OrderProduct)
                 .Include(orderH => orderH.OrderProductHistory)
                 .FirstOrDefault(order => order.Id == idOrder);

            if (cartOrder == null)
                throw new ApplicationException("Order does not exist");


            if (cartOrder.OrderProduct.Any())
            {
                foreach (var product in cartOrder.OrderProduct.ToList())
                {
                    this.DeleteOrderProduct(product.ProductId, idOrder);
                }
            }

            if (cartOrder.OrderProductHistory.Any())
            {
                foreach (var product in cartOrder.OrderProductHistory.ToList())
                {
                    this.DeleteOrderProduct(product.ProductHistoryId, idOrder);

                }
            }


        }

        public void DeleteCartProduct(int? id)
        {
            if (!id.HasValue) throw new ApplicationException("idProduct does not exist");

            var orderStatusProcessing = _context.OrderStatus.FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Processing);
            var cartOrder = _context.Orders
                                    .Include(order =>order.OrderProduct)
                                       .ThenInclude(orderProduct => orderProduct.Product)
                                    .Include(order => order.OrderProductHistory)
                                       .ThenInclude(orderProductHistory => orderProductHistory.ProductHistory)
                                       .ThenInclude(productHistory => productHistory.ProductTypeHistory.ParameterHistory)
                                       .ThenInclude(parameterHistory => parameterHistory.ProductParameterHistory)
                                    .FirstOrDefault(order => order.OrderStatusId == orderStatusProcessing.Id);


            if(cartOrder == null)
                throw new ApplicationException("Cart is empty");


            var deletedOrderProduct = cartOrder.OrderProduct.FirstOrDefault(oProduct => oProduct.ProductId == id);
           
            if(deletedOrderProduct != null)
            {
                deletedOrderProduct.Product.Quantity = deletedOrderProduct.Product.Quantity + deletedOrderProduct.Quantity;

                deletedOrderProduct.Product.Quantity = Math.Round(deletedOrderProduct.Product.Quantity, 2);
                _context.OrderProducts.Remove(deletedOrderProduct);
            }
            else
            {
                var deletedOrderProductHistory = cartOrder.OrderProductHistory.FirstOrDefault(oProduct => oProduct.ProductHistoryId == id);
                if (deletedOrderProductHistory != null)
                {
                    //need to rollback productHistory
                    var idProductType = this.GetOrCreateProductType(deletedOrderProductHistory.ProductHistory.ProductTypeHistory.NameType);
                    var idInstanceProduct = 0;
                    var instanceProduct = new Product
                    {
                        Id = idInstanceProduct,
                        ProductTypeId = idProductType,
                        Name = deletedOrderProductHistory.ProductHistory.Name,
                        ProductNumber = deletedOrderProductHistory.ProductHistory.ProductNumber,
                        Quantity = Math.Round(deletedOrderProductHistory.Quantity, 2),
                        StandartCost = deletedOrderProductHistory.ProductHistory.StandartCost,
                        Manufacturer = deletedOrderProductHistory.ProductHistory.Manufacturer,
                        Description = deletedOrderProductHistory.ProductHistory.Description,
                        DateOfReceipt = deletedOrderProductHistory.ProductHistory.DateOfReceipt,
                        WareHouseId = deletedOrderProductHistory.ProductHistory.WareHouseId,
                        PrimeCost = deletedOrderProductHistory.ProductHistory.PrimeCost,
                        PrimeCostEUR = deletedOrderProductHistory.ProductHistory.PrimeCostEUR,
                        PrimeCostUSD = deletedOrderProductHistory.ProductHistory.PrimeCostUSD,
                        CurrencyId = deletedOrderProductHistory.ProductHistory.CurrencyId
                    };

                    _context.Products.Add(instanceProduct);
                    _context.SaveChanges();
                    idInstanceProduct = instanceProduct.Id;

                    foreach (var productParameterH in deletedOrderProductHistory.ProductHistory.ProductParameterHistory)
                    {
                        if (string.IsNullOrEmpty(productParameterH.Value)) continue;

                        var parameterH = _context.ParameterHistory
                            .FirstOrDefault(parameterH => parameterH.Id == productParameterH.ParameterHistoryId);
                        var param = new ProductParameter()
                        {
                            ProductId = idInstanceProduct,
                            ParameterId = parameterH.DeletedParameterId,
                            Value = productParameterH.Value
                        };

                        _context.ProductParameters.Add(param);
                    }
                    _context.SaveChanges();
                }
                _context.OrderProductHistory.Remove(deletedOrderProductHistory);
            }

             _context.SaveChanges();
            
            // remove order
            var countOrderProduct = cartOrder.OrderProduct.Count();
            var countOrderProductHistory = cartOrder.OrderProductHistory.Count();
            if (countOrderProduct == 0 && countOrderProductHistory == 0)
                _context.Orders.Remove(cartOrder);

            _context.SaveChanges();

        }

        public void DeleteCartProduct2(int? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id), "Product ID is null");
            }

            var orderStatusProcessing = _context.OrderStatus.FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Processing);
            var cartOrder = _context.Orders
                                    .Include(order => order.OrderProduct)
                                       .ThenInclude(orderProduct => orderProduct.Product)
                                    .Include(order => order.OrderProductHistory)
                                       .ThenInclude(orderProductHistory => orderProductHistory.ProductHistory)
                                       .ThenInclude(productHistory => productHistory.ProductTypeHistory.ParameterHistory)
                                       .ThenInclude(parameterHistory => parameterHistory.ProductParameterHistory)
                                    .FirstOrDefault(order => order.OrderStatusId == orderStatusProcessing.Id);

            if (cartOrder == null)
            {
                throw new ApplicationException("Cart is empty");
            }

            var orderProduct = cartOrder.OrderProduct.FirstOrDefault(op => op.ProductId == id);

            if (orderProduct != null)
            {
                orderProduct.Product.Quantity += orderProduct.Quantity;
                _context.OrderProducts.Remove(orderProduct);
            }
            else
            {
                var orderProductHistory = cartOrder.OrderProductHistory.FirstOrDefault(oph => oph.ProductHistoryId == id);

                if (orderProductHistory != null)
                {
                    RollbackProductHistory(orderProductHistory);
                    _context.OrderProductHistory.Remove(orderProductHistory);
                }

                
            }
            _context.SaveChanges();

            if (!cartOrder.OrderProduct.Any() && !cartOrder.OrderProductHistory.Any())
            {
                _context.Orders.Remove(cartOrder);
            }

            _context.SaveChanges();
        }

        public void DeleteOrderProduct(int? idProduct, int? idOrder)
        {
            if (idProduct == null)
            {
                throw new ArgumentNullException(nameof(idProduct), "Product ID is null");
            }
            if (idOrder == null)
            {
                throw new ArgumentNullException(nameof(idOrder), "Order ID is null");
            }
            // var orderStatusProcessing = _context.OrderStatus.FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Processing);
            var order = _context.Orders
                                    .Include(order => order.OrderProduct)
                                       .ThenInclude(orderProduct => orderProduct.Product)
                                    .Include(order => order.OrderProductHistory)
                                       .ThenInclude(orderProductHistory => orderProductHistory.ProductHistory)
                                       .ThenInclude(productHistory => productHistory.ProductTypeHistory.ParameterHistory)
                                       .ThenInclude(parameterHistory => parameterHistory.ProductParameterHistory)
                                    .FirstOrDefault(order => order.Id == idOrder);

            if (order == null)
            {
                throw new ApplicationException("Does not exist");
            }

            var orderProduct = order.OrderProduct.FirstOrDefault(op => op.ProductId == idProduct);

            if (orderProduct != null)
            {
                orderProduct.Product.Quantity += orderProduct.Quantity;
                _context.OrderProducts.Remove(orderProduct);
            }
            else
            {
                var orderProductHistory = order.OrderProductHistory.FirstOrDefault(oph => oph.ProductHistoryId == idProduct);

                if (orderProductHistory != null)
                {
                    RollbackProductHistory(orderProductHistory);
                    _context.OrderProductHistory.Remove(orderProductHistory);
                }


            }
            _context.SaveChanges();

            if (!order.OrderProduct.Any() && !order.OrderProductHistory.Any())
            {
                _context.Orders.Remove(order);
            }

            _context.SaveChanges();
        }

        private void RollbackProductHistory(OrderProductHistory orderProductHistory)
        {
            var productType = GetOrCreateProductType(orderProductHistory.ProductHistory.ProductTypeHistory.NameType);

            var product = new Product
            {
                ProductTypeId = productType,
                Name = orderProductHistory.ProductHistory.Name,
                ProductNumber = orderProductHistory.ProductHistory.ProductNumber,
                Quantity = orderProductHistory.Quantity,
                StandartCost = orderProductHistory.ProductHistory.StandartCost,
                Manufacturer = orderProductHistory.ProductHistory.Manufacturer,
                Description = orderProductHistory.ProductHistory.Description,
                DateOfReceipt = orderProductHistory.ProductHistory.DateOfReceipt,
                WareHouseId = orderProductHistory.ProductHistory.WareHouseId,
                PrimeCost = orderProductHistory.ProductHistory.PrimeCost,
                PrimeCostEUR = orderProductHistory.ProductHistory.PrimeCostEUR,
                PrimeCostUSD = orderProductHistory.ProductHistory.PrimeCostUSD,
                CurrencyId = orderProductHistory.ProductHistory.CurrencyId
            };

            foreach (var productParameterHistory in orderProductHistory.ProductHistory.ProductParameterHistory)
            {
                if (string.IsNullOrEmpty(productParameterHistory.Value))
                {
                    continue;
                }

                var parameterHistory = _context.ParameterHistory
                    .FirstOrDefault(ph => ph.Id == productParameterHistory.ParameterHistoryId);

                if (parameterHistory == null)
                {
                    continue;
                }

                product.ProductParameters.Add(new ProductParameter
                {
                    ParameterId = parameterHistory.DeletedParameterId,
                    Value = productParameterHistory.Value
                });
            }

            _context.Products.Add(product);
            _context.SaveChanges();
        }

        public CountCartProducts GetCartOrderCount()
        {
            var orderStatusProcessing = _context.OrderStatus
                .FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Processing);

            var cartOrder = _context.Orders
                .Include(order => order.OrderProduct)
                .Include(order => order.OrderProductHistory)
                .FirstOrDefault(order => order.OrderStatusId == orderStatusProcessing.Id);

            int productCount = 0;
            bool isEmptyCart = false;

            if (cartOrder == null)
            {
                isEmptyCart = true;
            }
            else
            {
                productCount = cartOrder.OrderProduct.Count() + cartOrder.OrderProductHistory.Count();
                isEmptyCart = productCount == 0;
            }

           
            return new CountCartProducts()
            {
                countCartProduct = productCount,
                isEmptyCart = isEmptyCart
            };
        }




        //private ProductType GetOrCreateProductType2(string nameType)
        //{
        //    var productType = _context.ProductTypes.FirstOrDefault(pt => pt.NameType == nameType);

        //    if (productType == null)
        //    {
        //        productType = new ProductType { NameType = nameType };
        //        _context.ProductTypes.Add(productType);
        //        _context.SaveChanges();
        //    }

        //    return productType;
        //}

        private int GetOrCreateProductType(string productTypeName)
        {
            var productType = _context.ProductTypes.FirstOrDefault(pt => pt.NameType == productTypeName);
            var idTypeProduct = 0;

            if (productType == null)
            {
                var newTypeProduct = new ProductType()
                {
                    Id = 0,
                    NameType = productTypeName
                };

                _context.ProductTypes.Add(newTypeProduct);
                _context.SaveChanges();
                idTypeProduct = newTypeProduct.Id;
            }
            else
            {
                idTypeProduct = productType.Id;
            }

            return idTypeProduct;
        }

        private Random random = new Random();

        private string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private int GetOrCreateCurrency(string currencyName)
        {
            var currency = _context.Currencies.FirstOrDefault(c => c.CurrencyName == currencyName);
            var currencyId = 0;

            if (currency == null)
            {
                var newCurrency = new Currency()
                {
                    Id = currencyId,
                    CurrencyName = currencyName
                };

                _context.Currencies.Add(newCurrency);
                _context.SaveChanges();

                currencyId = newCurrency.Id;
            }
            else
            {
                currencyId = currency.Id;
            }

            return currencyId;
        }
    }
}
