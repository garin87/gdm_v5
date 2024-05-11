using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.Models;
using gdm5._0.DTO;
using Microsoft.EntityFrameworkCore;
using gdm5._0.Services.Interfaces;
using gdm5._0.Requests.Product;
using gdm5._0.Extensions;
using gdm5._0.Domain.Models.Filters;
using gdm5._0.Filters;
using gdm5._0.Helpers;
using gdm5._0.Domain.Models.Order;
using gdm5._0.Shared.Constants;
using gdm5._0.Domain.Models;
using gdm5._0.Requests.Order;
using gdm5._0.Services.OrderB;

namespace gdm5._0.Services
{
    public class OrderService : BaseOrderService<Order>, IOrderService
    {
        private readonly IProductService _productService;
        private readonly IUriService _uriService;
        public OrderService(DataContext context, IProductService ProductService, IUriService uriService) : base(context)
        {
            _productService = ProductService;
            _uriService = uriService;
        }

        public string[] getOrderNameCompanies()
        {
            return _context.Orders.Select(order => order.NameCompany)
                                  .Distinct()
                                  .AsNoTracking()
                                  .ToArray();
        }
        public string[] getOrderNameCompanies(string NameCompany)
        {
            ValidateNameCompany(NameCompany);

            return _context.Orders.Where(order => order.NameCompany == NameCompany)
                                  .Select(order => order.NameCompany)
                                  .Distinct()
                                  .AsNoTracking()
                                  .ToArray();
        }
        public string[] getNamesProduct()
        {
            return _context.Orders.SelectMany(order => order.OrderProduct
                                                      .Select(pp => pp.Product.ProductType.NameType))
                                                      .Distinct()
                                                      .AsNoTracking()
                                                      .ToArray();
        }


        //public async Task AddOrder(AddOrderRequest orderData, string currentUserName)
        //{
        //    var product = GetProductById(orderData.ProductId);
        //    var companyId = GetCustomerByName(orderData.company.Value);

        //    var roundedQ = Math.Round(product.Quantity, 2);
        //    if (roundedQ >= orderData.quantityorder?.Value.ParseDouble())
        //    {
        //        var newQuantity = roundedQ - orderData.quantityorder?.Value.ParseDouble();
        //        product.Quantity = newQuantity ?? product.Quantity;
        //        product.DateOfLastChanged = DateTimeHelper.DateTimeNowWithOffset();
        //        product.LastEditedByUser = currentUserName;
        //        await _context.SaveChangesAsync();
        //    }
        //    else
        //        throw new ApplicationException("Entered quantity of product invalid");

        //    var orderStatusCompletedId = GetCompletedOrderStatusId();

        //    int currencyId = GetOrCreateCurrency("BYN"); //BYN; 
        //    double orderQ = orderData.quantityorder.Value.ParseDouble();
        //    double priceForQ = (orderData.totalprice?.Value.ParseDouble() ?? 0) * orderQ;

        //    var order = new Order
        //    {
        //        Id = 0,
        //        NameCompany = orderData.company?.Value,
        //        TotalPrice = priceForQ,
        //        OrderCreatedTime = DateTimeHelper.DateTimeNowWithOffset(), // utcoffset +3
        //        OrderNumber = orderData.number?.Value.ParseInt() ?? 0,
        //        OrderCreatedByUser = currentUserName,
        //        Description = orderData.description?.Value,
        //        CurrencyId = currencyId,
        //        CustomerId = companyId,
        //        OrderStatusId = orderStatusCompletedId
        //    };

        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    ProductHistory deletedProduct = null;
        //    if (product.Quantity == 0)
        //    {
        //        deletedProduct = await _productService.DeleteProductInstance(product.Id, currentUserName);
        //    }
        //    if (deletedProduct == null)
        //    {
        //        var orderProduct = new OrderProduct
        //        {
        //            OrderId = order.Id,
        //            ProductId = orderData.ProductId,
        //            Quantity = orderData.quantityorder?.Value.ParseDouble() ?? 0,
        //            TotalPrice = priceForQ,
        //            TaxNDS = orderData.taxnds?.Value.ParseDouble() ?? 0,
        //            Markup = orderData.markup?.Value.ParseDouble() ?? 0,

        //        };
        //        _context.OrderProducts.Add(orderProduct);
        //    }
        //    else
        //    {
        //        var orderProduct = new OrderProductHistory
        //        {
        //            OrderId = order.Id,
        //            ProductHistoryId = deletedProduct.Id,
        //            Quantity = orderData.quantityorder?.Value.ParseDouble() ?? 0,
        //            TotalPrice = priceForQ,
        //         //   TaxNDS = orderData.taxnds?.Value.ParseDouble() ?? 0,
        //         //   Markup = orderData.markup?.Value.ParseDouble() ?? 0,
        //        };
        //        _context.OrderProductHistory.Add(orderProduct);
        //    }


        //    await _context.SaveChangesAsync();
        //}

        //public async Task AddToCartOrder1(AddOrderRequest orderData, string currentUserName)
        //{
        //    var userId = GetUserIdByContext(currentUserName);
        //    var product = GetProductById(orderData.ProductId);
        //    var orderStatusProcessingId = GetProcessingOrderStatusId();

        //    // need change on multi orders
        //    Order cartOrder = _context.Orders.FirstOrDefault(order => order.OrderStatusId == orderStatusProcessingId && order.UserId == userId);

        //    int orderId;
        //    double orderQ = orderData.quantityorder.Value.ParseDouble();
        //    double priceForQ = (orderData.totalprice?.Value.ParseDouble() ?? 0) * orderQ;
        //    if (cartOrder == null)
        //    {
        //        var companyId = GetCustomerByName(orderData.company.Value);
        //        await CheckAndChangeQuantityProduct(product, orderData, currentUserName);
        //        int currencyId = GetOrCreateCurrency("BYN");

        //        var newOrder = new Order
        //        {
        //            Id = 0,
        //            NameCompany = orderData.company?.Value,
        //            TotalPrice = priceForQ,
        //            OrderCreatedTime = DateTimeHelper.DateTimeNowWithOffset(),
        //            OrderNumber = orderData.number?.Value.ParseInt() ?? 0,
        //            OrderCreatedByUser = currentUserName,
        //            Description = orderData.description?.Value,
        //            CurrencyId = currencyId,
        //            CustomerId = companyId,
        //            OrderStatusId = orderStatusProcessingId,
        //            UserId = userId
        //        };
        //        _context.Orders.Add(newOrder);
        //        await _context.SaveChangesAsync();

        //        orderId = newOrder.Id;
        //    }
        //    else
        //    {
        //        orderId = cartOrder.Id;
        //        await CheckAndChangeQuantityProduct(product, orderData, currentUserName);
        //    }

        //    ProductHistory deletedProduct = null;
        //    if (product.Quantity == 0)
        //    {
        //        deletedProduct = await this._productService.DeleteProductInstance(product.Id, currentUserName);
        //    }
        //    if (deletedProduct == null)
        //    {
        //        var orderProduct = new OrderProduct
        //        {
        //            OrderId = orderId,
        //            ProductId = orderData.ProductId,
        //            Quantity = orderData.quantityorder?.Value.ParseDouble() ?? 0,
        //            TotalPrice = priceForQ,
        //            TaxNDS = orderData.taxnds?.Value.ParseDouble() ?? 0,
        //            Markup = orderData.markup?.Value.ParseDouble() ?? 0,

        //        };
        //        _context.OrderProducts.Add(orderProduct);
        //    }
        //    else
        //    {
        //        var orderProduct = new OrderProductHistory
        //        {
        //            OrderId = orderId,
        //            ProductHistoryId = deletedProduct.Id,
        //            Quantity = orderData.quantityorder?.Value.ParseDouble() ?? 0,
        //            TotalPrice = priceForQ,
        //            Markup = orderData?.markup?.Value.ParseDouble() ?? 0,
        //            TaxNDS = orderData?.taxnds?.Value.ParseDouble() ?? 0,
        //        };
        //        _context.OrderProductHistory.Add(orderProduct);
        //    }

        //    await _context.SaveChangesAsync();
        //}

        public async Task AddOrder(AddOrderRequest orderData, string currentUserName)
        {
            // Input validation
            if (orderData == null)
                throw new ArgumentNullException(nameof(orderData));

            // Validate other input parameters

            // Retrieve product
            var product = GetProductById(orderData.ProductId);
            if (product == null)
                throw new ArgumentException("Invalid ProductId", nameof(orderData.ProductId));

            // Validate quantity
            double orderQuantity = orderData.quantityorder?.Value.ParseDouble() ?? 0;
            var productQuantity = Math.Round(product.Quantity, 2);

            if (productQuantity < orderQuantity)
                throw new ArgumentException("Entered quantity of product is invalid");

            // Update product quantity
            double newQuantity = Math.Max(0, productQuantity - orderQuantity);
            product.Quantity = newQuantity;
            product.DateOfLastChanged = DateTimeHelper.DateTimeNowWithOffset();
            product.LastEditedByUser = currentUserName;

            // Save changes to product
            await _context.SaveChangesAsync();

            // Create order
            var order = new Order
            {
                NameCompany = orderData.company?.Value,
                TotalPrice = (orderData.totalprice?.Value.ParseDouble() ?? 0) * orderQuantity,
                OrderCreatedTime = DateTimeHelper.DateTimeNowWithOffset(),
                OrderNumber = orderData.number?.Value.ParseInt() ?? 0,
                OrderCreatedByUser = currentUserName,
                Description = orderData.description?.Value,
                CurrencyId = GetOrCreateCurrency("BYN"),
                CustomerId = GetCustomerByName(orderData.company.Value),
                OrderStatusId = GetCompletedOrderStatusId()
            };

            // Add order to context
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            // Handle product history
            var deletedProduct = (product.Quantity == 0)
                ? await _productService.DeleteProductInstance(product.Id, currentUserName)
                : null;

            // Add order product or order product history
            if (deletedProduct == null)
            {
                var orderProduct = new OrderProduct
                {
                    OrderId = order.Id,
                    ProductId = orderData.ProductId,
                    Quantity = orderQuantity,
                    TotalPrice = order.TotalPrice,
                    TaxNDS = orderData.taxnds?.Value.ParseDouble() ?? 0,
                    Markup = orderData.markup?.Value.ParseDouble() ?? 0
                };
                _context.OrderProducts.Add(orderProduct);
            }
            else
            {
                var orderProductHistory = new OrderProductHistory
                {
                    OrderId = order.Id,
                    ProductHistoryId = deletedProduct.Id,
                    Quantity = orderQuantity,
                    TotalPrice = order.TotalPrice
                    // Add other properties if needed
                };
                _context.OrderProductHistory.Add(orderProductHistory);
            }

            // Save changes to the context
            await _context.SaveChangesAsync();
        }

        public async Task AddToCartOrder(AddOrderRequest orderData, string currentUserName)
        {
            var userId = GetUserIdByContext(currentUserName);
            var product = GetProductById(orderData.ProductId);
            var orderStatusProcessingId = GetProcessingOrderStatusId();

            Order cartOrder = _context.Orders.FirstOrDefault(order => order.OrderStatusId == orderStatusProcessingId && order.UserId == userId);

            if (cartOrder == null)
            {
                await CreateNewOrder(orderData, currentUserName, userId, product, orderStatusProcessingId);
            }
            else
            {
                await UpdateExistingOrder(cartOrder, orderData, currentUserName, product);
            }
        }

        private async Task CreateNewOrder(AddOrderRequest orderData, string currentUserName, int userId, Product product, int orderStatusProcessingId)
        {
            var companyId = GetCustomerByName(orderData.company.Value);
            await CheckAndChangeQuantityProduct(product, orderData, currentUserName);
            int currencyId = GetOrCreateCurrency("BYN");

            var newOrder = new Order
            {
                Id = 0,
                NameCompany = orderData.company?.Value,
                TotalPrice = CalculateTotalPrice(orderData),
                OrderCreatedTime = DateTimeHelper.DateTimeNowWithOffset(),
                OrderNumber = orderData.number?.Value.ParseInt() ?? 0,
                OrderCreatedByUser = currentUserName,
                Description = orderData.description?.Value,
                CurrencyId = currencyId,
                CustomerId = companyId,
                OrderStatusId = orderStatusProcessingId,
                UserId = userId
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            await CreateOrUpdateProductQ(newOrder.Id, orderData, currentUserName, product);
        }

        private async Task UpdateExistingOrder(Order cartOrder, AddOrderRequest orderData, string currentUserName, Product product)
        {
            await CheckAndChangeQuantityProduct(product, orderData, currentUserName);
            await CreateOrUpdateProductQ(cartOrder.Id, orderData, currentUserName, product);
        }

        private async Task CreateOrUpdateProductQ(int orderId, AddOrderRequest orderData, string currentUserName, Product product)
        {

            if (product.Quantity == 0)
            {
                var deletedProduct = await _productService.DeleteProductInstance(product.Id, currentUserName);

                if (deletedProduct == null)
                {
                    AddOrderProduct(orderId, orderData);
                }
                else
                {
                    AddOrderProductHistory(orderId, deletedProduct, orderData);
                }
            }
            else
            {
                AddOrderProduct(orderId, orderData);
            }
        }

        private void AddOrderProduct(int orderId, AddOrderRequest orderData)
        {
            var priceForQ = CalculateTotalPrice(orderData);

            var orderProduct = new OrderProduct
            {
                OrderId = orderId,
                ProductId = orderData.ProductId,
                Quantity = orderData.quantityorder?.Value.ParseDouble() ?? 0,
                TotalPrice = priceForQ,
                TaxNDS = orderData.taxnds?.Value.ParseDouble() ?? 0,
                Markup = orderData.markup?.Value.ParseDouble() ?? 0,
            };

            _context.OrderProducts.Add(orderProduct);
            _context.SaveChanges();
        }
        private void AddOrderProductHistory(int orderId, ProductHistory deletedProduct, AddOrderRequest orderData)
        {
            var priceForQ = CalculateTotalPrice(orderData);

            var orderProductHistory = new OrderProductHistory
            {
                OrderId = orderId,
                ProductHistoryId = deletedProduct.Id,
                Quantity = orderData.quantityorder?.Value.ParseDouble() ?? 0,
                TotalPrice = priceForQ,
                Markup = orderData?.markup?.Value.ParseDouble() ?? 0,
                TaxNDS = orderData?.taxnds?.Value.ParseDouble() ?? 0,
            };

            _context.OrderProductHistory.Add(orderProductHistory);
            _context.SaveChanges();
        }

        private double CalculateTotalPrice(AddOrderRequest orderData)
        {
            double orderQ = orderData.quantityorder.Value.ParseDouble();
            double priceForQ = (orderData.totalprice?.Value.ParseDouble() ?? 0) * orderQ;
            return priceForQ;
        }

        private async Task CheckAndChangeQuantityProduct(Product product, AddOrderRequest orderData, string currentUserName)
        {

            var roundedQ = Math.Round(product.Quantity, 2);
            if (roundedQ >= orderData.quantityorder?.Value.ParseDouble())
            {
                var newQuantity = roundedQ - orderData.quantityorder?.Value.ParseDouble();
                product.Quantity = newQuantity ?? product.Quantity;
                product.DateOfLastChanged = DateTimeHelper.DateTimeNowWithOffset();
                product.LastEditedByUser = currentUserName;
                await _context.SaveChangesAsync();
            }
            else
                throw new ApplicationException("Entered quantity of product invalid");
        }

        public async Task addOrderProductList(addOrderListProductRequest orderData, int currentUserId, string currentUserName)
        {
            ValidateOrderProductList(orderData);
            var companyId = GetCustomerByName(orderData.company.Value);
            int currencyId = GetOrCreateCurrency("BYN"); //BYN; 

            var totalOrderPrice = orderData.orderProductList.Select(product => product.totalprice.Value.ParseDouble())
                                                            .Sum();

            var order = new Order
            {
                Id = 0,
                NameCompany = orderData.company?.Value,
                TotalPrice = totalOrderPrice,
                OrderCreatedTime = DateTime.UtcNow.AddHours(3),
                OrderNumber = random.Next(9999999),
                OrderCreatedByUser = currentUserName,
                Description = orderData.description?.Value,
                CurrencyId = currencyId,
                CustomerId = companyId,
                UserId = currentUserId
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            foreach (var productOrder in orderData.orderProductList) {

                var product = GetProductById(productOrder);

                var newQuantity = product.Quantity - productOrder.quantityorder?.Value.ParseFloat();
                product.Quantity = newQuantity ?? product.Quantity;
                product.DateOfLastChanged = DateTimeHelper.DateTimeNowWithOffset();
                
                await _context.SaveChangesAsync();

                ProductHistory deletedProduct = null;
                if (product.Quantity == 0)
                    deletedProduct = await this._productService.DeleteProductInstance(product.Id, currentUserName);

                if (deletedProduct == null)
                    AddOrderProduct(order, productOrder);
                else
                    AddOrderProductHistory(order, deletedProduct, productOrder);
                
                await _context.SaveChangesAsync();
            }
        }
        public async Task saveOrderCart(string currentUserName, int CurrentUserId)
        {
            var orderStatusProcessingId = GetProcessingOrderStatusId();
            var cartOrder = _context.Orders.FirstOrDefault(order => order.OrderStatusId == orderStatusProcessingId && order.UserId == CurrentUserId);

            if (cartOrder == null)
                throw new ApplicationException("Cart is empty");

            var orderStatusCompletedId = GetCompletedOrderStatusId();

            cartOrder.OrderStatusId = orderStatusCompletedId;
            cartOrder.OrderCreatedTime = DateTimeHelper.DateTimeNowWithOffset();
            cartOrder.OrderCreatedByUser = currentUserName;
            await _context.SaveChangesAsync();
        }


        public String GetProcessingOrderInfo(int CurrentUserId)
        {
            var orderStatusProcessingId = GetProcessingOrderStatusId();
            var cartOrder = _context.Orders.FirstOrDefault(order => order.OrderStatusId == orderStatusProcessingId && order.UserId == CurrentUserId);

            if (cartOrder == null)
                throw new ApplicationException("Cart is empty");

            return cartOrder.NameCompany;
        }

        

        public PagedResponseDTO<List<OrderDomain>> getOrderProduct(string companyName, PaginationFilterDTO pageFilter, string route, 
            SortOptionsDTO sortOption, OrderFilter filter)
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
            var instancesOfOrder = _context.Orders.Include(order => order.OrderProduct)
                                                     .ThenInclude(orderProduct => orderProduct.Product)
                                                     .ThenInclude(product => product.ProductType)
                                                     .ThenInclude(productType => productType.Parameters)
                                                     .ThenInclude(parameters => parameters.ProductParameters)
                                                  .Include(product => product.Currency)
                                                  .Include(order => order.OrderProductHistory)
                                                     .ThenInclude(parameters => parameters.ProductHistory)
                                                  .AsNoTracking()
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
                                                  .Include(order => order.OrderProduct)
                                                     .ThenInclude(orderProduct => orderProduct.Product)
                                                     .ThenInclude(productParameter => productParameter.ProductType.Parameters)
                                                  .Include(product => product.Currency)
                                                  .Include(order => order.OrderProductHistory)
                                                     .ThenInclude(parameters => parameters.ProductHistory)
                                                     .ThenInclude(productHistory => productHistory.ProductParameterHistory)
                                                  .AsNoTracking()
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
        public PagedResponseDTO<List<OrderProductListDomain>> getCartOrderProducts(string route, int currentUserId)
        {
            var validFilter = new PaginationFilterDTO(1, 100);
            var orderStatusProcessing = _context.OrderStatus.FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Processing);

            Order cartOrder = _context.Orders.FirstOrDefault(order => order.OrderStatusId == orderStatusProcessing.Id && order.UserId == currentUserId);

            if (cartOrder == null)
            {
                return PaginationHelper.CreatePagedReponse<OrderProductListDomain>(new List<OrderProductListDomain>(),
                validFilter, 1, _uriService, route);
            }

            var OrderProcessing = _context.Orders.Where(order => order.Id == cartOrder.Id && order.UserId == currentUserId)
                                    .Include(order => order.OrderProduct)
                                       .ThenInclude(orderProduct => orderProduct.Product)
                                       .ThenInclude(parameters => parameters.ProductParameters)
                                    .Include(order => order.OrderProductHistory)
                                       .ThenInclude(parameters => parameters.ProductHistory)
                                       .AsNoTracking();


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

                        var oProductParameters = _context.Parameters.Where(p => p.ProductTypeId == orderProduct.Product.ProductTypeId).ToList();
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

                orderProductListDTO.ToList().Reverse();
                var orderDTO = new OrderProductListDomain()
                {
                    OrderId = order.Id,
                    NameCompany = order.NameCompany,
                    TotalPrice = Math.Round(orderProductListDTO.Sum(product=>product.TotalPrice), 2),
                    Description = order.Description,
                    OrderCreatedTime = order.OrderCreatedTime.ToString("MM/dd/yyyy HH:mm"),
                    OrderCreatedByUser = order.OrderCreatedByUser,
                    Currency = "BYN",//order.Currency.CurrencyName,
                    Products = orderProductListDTO
                };

                orders.Add(orderDTO);
            }
            orders.Reverse();
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

            var orderStatusProcessingId = GetProcessingOrderStatusId();
            var cartOrder = _context.Orders
                                    .Include(order =>order.OrderProduct)
                                       .ThenInclude(orderProduct => orderProduct.Product)
                                    .Include(order => order.OrderProductHistory)
                                       .ThenInclude(orderProductHistory => orderProductHistory.ProductHistory)
                                       .ThenInclude(productHistory => productHistory.ProductTypeHistory.ParameterHistory)
                                       .ThenInclude(parameterHistory => parameterHistory.ProductParameterHistory)
                                    .FirstOrDefault(order => order.OrderStatusId == orderStatusProcessingId);


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
        public CountCartProducts GetCartOrderCount(int currentUserId)
        {
            var orderStatusProcessing = _context.OrderStatus.AsNoTracking()
                                                            .FirstOrDefault(oStatus => oStatus.OrderStatusName == OrderStatusConstants.Processing);

            var cartOrder = _context.Orders.Include(order => order.OrderProduct)
                                           .Include(order => order.OrderProductHistory)
                                           .AsNoTracking()
                                           .FirstOrDefault(order => order.OrderStatusId == orderStatusProcessing.Id && order.UserId == currentUserId);

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
        private int GetOrCreateProductType(string productTypeName)
        {
            var productType = _context.ProductTypes.AsNoTracking().FirstOrDefault(pt => pt.NameType == productTypeName);
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
        public List<OrderTotalQuantity> LoadOrderReport(loadOrderReportRequest request)
        {
            int productTypeID = GetProductTypeID(request.Name?.Value);
            int productTypeHistoryID = GetProductTypeHistoryID(request.Name?.Value);

            int parameterDiameterID = GetParameterDiameterID(productTypeID);
            int parameterHistoryDiameterID = GetParameterHistoryDiameterID(productTypeHistoryID);
           
            var filterOrderReportAssigner = createOrderReportAssigner(request);
            if (request.Parameters != null && request.Parameters.Length > 0)
            {
                foreach (var param in request.Parameters)
                {
                    if (!string.IsNullOrEmpty(param.Name))
                    {
                        param.ParameterId = _context.Parameters
                            .FirstOrDefault(item => item.Name.Equals(param.Name) && item.ProductTypeId == productTypeID).Id;
                    }
                }
            }

            ProductFilter filter = new ProductFilter() {
               Parameters = request.Parameters,
               Manufacturer = request.Manufacturer?.Value
            };
            var filterAssigner = new ProductAssigner(filter);
            var filterProductHistoryAssigner = new ProductHistoryAssigner(filter);

            var OrderProductTotalQ = getOrderProductQuantityByParameter(productTypeID, parameterDiameterID, filterOrderReportAssigner, filterAssigner);
          //  var test11 = OrderProductTotalQ.ToQueryString();
          //  var test22 = OrderProductTotalQ.ToList();

            var OrderProductHistoryTotalQ = getOrderProductHistoryQuantityByParameter(productTypeHistoryID, parameterHistoryDiameterID,
                filterOrderReportAssigner, filterProductHistoryAssigner);
            //var test1 = OrderProductHistoryTotalQ.ToQueryString();
            //var test = OrderProductHistoryTotalQ.ToList();

            var combinedResults = OrderProductTotalQ.Union(OrderProductHistoryTotalQ);
            var combinedResultsList = combinedResults.ToList();

            var realAmountProduct = getProductQuantityByParameter(productTypeID, parameterDiameterID, filterAssigner);
            var realAmountProductList = realAmountProduct.ToList();


          

            var groupedResults = combinedResults
                .GroupBy(t => t.Diameter)
                .Select(g => new OrderTotalQuantity
                {
                    Name = request.Name.Value,
                    Diameter = g.Key,
                    TotalAmount = (double)Math.Round((double)(g.Sum(t => t.TotalAmount)), 2),
                })
                .OrderByDescending(t => t.TotalAmount)
                .ToList();


            var mergedList = from orderR in groupedResults
                             join realP in realAmountProductList on orderR.Diameter equals realP.Diameter
                             select new OrderTotalQuantity
                             {
                                 Name = orderR.Name,
                                 Diameter = orderR.Diameter,
                                 TotalAmount = orderR.TotalAmount,
                                 RealAmountProduct = (double)Math.Round((double)(realP.TotalAmount), 2),
                             };

            var mergedLZero = from orderR in groupedResults
                              join realP in realAmountProductList on orderR.Diameter equals realP.Diameter into matches
                              from realP in matches.DefaultIfEmpty()
                              where realP == null
                              select new OrderTotalQuantity
                              {
                                  Name = orderR.Name,
                                  Diameter = orderR.Diameter,
                                  TotalAmount = orderR.TotalAmount,
                                  RealAmountProduct = realP != null ? (double)Math.Round((double)realP.TotalAmount, 2) : 0
                              };

         //   var test6 = mergedLZero.ToList();

            var combinedOutcomeLists = mergedList.Union(mergedLZero);


            var groupedOutcomeList = combinedOutcomeLists
                                     .OrderByDescending(t => t.TotalAmount)
                                     .ToList();

            return groupedOutcomeList;
        }

        public IQueryable<OrderTotalQuantity> getOrderProductQuantityByParameter(int productTypeID, int parameterID,
            OrderReportAssigner filterOrderReportAssigner, ProductAssigner filterProductAssigner)
        {
            return from subT in (
                           from o in _context.Orders
                           .ApplyFilter(filterOrderReportAssigner)
                           join op in _context.OrderProducts on o.Id equals op.OrderId
                           join p in _context.Products.ApplyFilter(filterProductAssigner) on op.ProductId equals p.Id
                           join pt in _context.ProductTypes on p.ProductTypeId equals pt.Id
                           join param in _context.Parameters on pt.Id equals param.ProductTypeId
                           join pp in _context.ProductParameters on param.Id equals pp.ParameterId
                           where pt.Id == productTypeID && pp.ParameterId == parameterID
                           select new {
                               Diameter = pp.Value,
                               ProductIDF = p.Id,
                               pp.ProductId,
                               pp.ParameterId,
                               ProductIDP = op.ProductId,
                               OrderId = o.Id,
                               Amount = op.Quantity,
                               o.OrderCreatedTime,
                           })
                      join pp in _context.ProductParameters on subT.ProductIDF equals pp.ProductId
                      where subT.ProductIDF == subT.ProductId && pp.ParameterId == parameterID
                      group subT by subT.Diameter into g
                      select new OrderTotalQuantity
                      {
                          Diameter = g.Key,
                          TotalAmount = g.Sum(subT => subT.Amount)
                      };
        }
        public IQueryable<OrderTotalQuantity> getOrderProductHistoryQuantityByParameter(int productTypeHistoryID, 
            int parameterHistoryID, OrderReportAssigner filterOrderReportAssigner, ProductHistoryAssigner filterProductHistoryAssigner)
        {
            return from subT in (
                             from o in _context.Orders.ApplyFilter(filterOrderReportAssigner)
                             join op in _context.OrderProductHistory on o.Id equals op.OrderId
                             join p in _context.ProductHistory.ApplyFilter(filterProductHistoryAssigner) on op.ProductHistoryId equals p.Id
                             join pt in _context.ProductTypeHistory on p.ProductTypeHistoryId equals pt.Id
                             join param in _context.ParameterHistory on pt.Id equals param.ProductTypeHistoryId
                             join pp in _context.ProductParameterHistory on param.Id equals pp.ParameterHistoryId
                             where pt.Id == productTypeHistoryID && pp.ParameterHistoryId == parameterHistoryID
                             select new {
                                 Diameter = pp.Value,
                                 ProductIDF = p.Id,
                                 pp.ProductHistoryId,
                                 pp.ParameterHistoryId,
                                 ProductIDP = op.ProductHistoryId,
                                 OrderId = o.Id,
                                 Amount = op.Quantity })
                   join pp in _context.ProductParameterHistory on subT.ProductIDF equals pp.ProductHistoryId
                   where subT.ProductIDF == subT.ProductHistoryId && pp.ParameterHistoryId == parameterHistoryID
                   group subT by subT.Diameter into g
                   select new OrderTotalQuantity
                   {
                       Diameter = g.Key,
                       TotalAmount = g.Sum(subT => subT.Amount)
                   };
        }


        private IQueryable<OrderTotalQuantity> getProductQuantityByParameter(int productTypeID, int parameterID,
            ProductAssigner filterProductAssigner)
        {
            return from subT in (
                           from p in _context.Products.ApplyFilter(filterProductAssigner)
                           join pt in _context.ProductTypes on p.ProductTypeId equals pt.Id
                           join param in _context.Parameters on pt.Id equals param.ProductTypeId
                           join pp in _context.ProductParameters on param.Id equals pp.ParameterId
                           where pt.Id == productTypeID && pp.ParameterId == parameterID
                           select new {
                               Diameter = pp.Value,
                               ProductIDF = p.Id,
                               pp.ProductId,
                               pp.ParameterId,
                               Amount = p.Quantity,
                           })
                   join pp in _context.ProductParameters on subT.ProductIDF equals pp.ProductId
                   where subT.ProductIDF == subT.ProductId && pp.ParameterId == parameterID
                   group subT by subT.Diameter into g
                   select new OrderTotalQuantity
                   {
                       Diameter = g.Key,
                       TotalAmount = g.Sum(subT => subT.Amount)
                   };
        }
        private OrderReportAssigner createOrderReportAssigner(loadOrderReportRequest request)
        {
            DateTime dtStartV;
            DateTime dtEndV;

            OrderReportFilter filterOrderReport = new OrderReportFilter()
            {
                NameCompany = request.NameCompany?.Value,
                FilterStartDate = DateTime.TryParse(request.FilterStartDate?.Value, out dtStartV) ? dtStartV : null,
                FilterEndDate = DateTime.TryParse(request.FilterEndDate?.Value, out dtEndV) ? dtEndV : null,
            };

            return new OrderReportAssigner(filterOrderReport);
        }
        public IQueryable<Order> getOrderProductByParameter(int productTypeID, int parameterID)
        {
            return from o in _context.Orders
                   join op in _context.OrderProducts on o.Id equals op.OrderId
                   join p in _context.Products on op.ProductId equals p.Id
                   join pt in _context.ProductTypes on p.ProductTypeId equals pt.Id
                   join param in _context.Parameters on pt.Id equals param.ProductTypeId
                   join pp in _context.ProductParameters on param.Id equals pp.ParameterId
                   where pt.Id == productTypeID && pp.ParameterId == parameterID
                   select o;

        }
        protected int GetUserIdByContext(string identityName)
        {
            if (string.IsNullOrEmpty(identityName))
                throw new ApplicationException("Not found user " + identityName);

            return _context.Users.AsNoTracking().FirstOrDefault(user => user.UserName.ToLower().Equals(identityName.ToLower())).Id;
        }
        protected void ValidateOrderProductList(addOrderListProductRequest orderData)
        {
            if (orderData.orderProductList.Count() < 1)
                throw new ApplicationException("Order ProductList is empty"); ;

            // validate ProductList
            foreach (var productOrder in orderData.orderProductList)
            {
                var product = GetProductById(productOrder.ProductId);

                if (product.Quantity < productOrder.quantityorder?.Value.ParseDouble())
                    throw new ApplicationException("Entered quantity: " + product.Quantity + "of product: " + product.Name + "ProductNumber " + product.ProductNumber + " invalid");
            }
        }

        protected Product GetProductById(OrderProductRequest productOrder)
        {
            var product = _context.Products.FirstOrDefault(product => product.Id == productOrder.ProductId);
            if (product is null)
                new ArgumentNullException(productOrder.ProductId.ToString());

            return product;
        }

        protected void AddOrderProduct(Order order, OrderProductRequest productOrder)
        {
            var orderProduct = new OrderProduct
            {
                OrderId = order.Id,
                ProductId = productOrder.ProductId,
                Quantity = productOrder.quantityorder?.Value.ParseDouble() ?? 0,
                TotalPrice = productOrder.totalprice?.Value.ParseDouble() ?? 0,
                TaxNDS = productOrder.taxnds?.Value.ParseDouble() ?? 0,
                Markup = productOrder.markup?.Value.ParseDouble() ?? 0,
            };
            _context.OrderProducts.Add(orderProduct);
        }

        protected void AddOrderProductHistory(Order order, ProductHistory deletedProduct, OrderProductRequest productOrder)
        {
            var orderProduct = new OrderProductHistory
            {
                OrderId = order.Id,
                ProductHistoryId = deletedProduct.Id,
                Quantity = productOrder.quantityorder?.Value.ParseDouble() ?? 0,
                TotalPrice = productOrder.totalprice?.Value.ParseDouble() ?? 0,
                TaxNDS = productOrder.taxnds?.Value.ParseDouble() ?? 0,
                Markup = productOrder.markup?.Value.ParseDouble() ?? 0,
            };
            _context.OrderProductHistory.Add(orderProduct);
        }

        private int GetParameterDiameterID(int productTypeId)
        {
            var parameterDiameter = _context.Parameters.AsNoTracking().FirstOrDefault(p => p.ProductTypeId == productTypeId && 
                                                                      (p.Name.ToLower().Equals("диаметр") ||
                                                                       p.Name.ToLower().Equals("размер") ||
                                                                       p.Name.ToLower().Equals("внутренний диаметр")
                                                                       ));

            if (parameterDiameter == null)
                throw new ApplicationException("Product has no 'диаметр' or 'размер'");

            return parameterDiameter.Id;
        }
        private int GetProductTypeHistoryID(string nameProductType)
        {
            if (string.IsNullOrEmpty(nameProductType))
                throw new ApplicationException("Enter valid name product");

            var productTypeHistory = _context.ProductTypeHistory.AsNoTracking().Where(type => type.NameType == nameProductType).FirstOrDefault();

            if (productTypeHistory == null)
                return 0;

            return productTypeHistory.Id;
        }
        private int GetParameterHistoryDiameterID(int productTypeHistoryId)
        {
            var parameterDiameter = _context.ParameterHistory.AsNoTracking().FirstOrDefault(p => p.ProductTypeHistoryId == productTypeHistoryId &&
                                                                      (p.Name.ToLower().Equals("диаметр") ||
                                                                       p.Name.ToLower().Equals("размер") ||
                                                                       p.Name.ToLower().Equals("внутренний диаметр")
                                                                       ));

            if (parameterDiameter == null)
                return 0;

            return parameterDiameter.Id;
        }
    }
}
