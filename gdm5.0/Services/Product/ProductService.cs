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
using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Drawing;
using System.Linq.Expressions;
using gdm5._0.Domain.Models.ProductParameter;
using gdm5._0.Helpers;
using gdm5._0.Extensions;
using gdm5._0.Services.ProductS;

namespace gdm5._0.Services
{
    public class ProductService : ProductBase<Product>, IProductService
    {
        public ProductService(DataContext context) : base(context)
        {
        }
        public string[] GetProductManufacturers()
        {
            return _context.Products.Select(product => product.Manufacturer).Distinct().ToArray();
        }
        public string[] GetProductManufacturers(string productName)
        {
            if (string.IsNullOrEmpty(productName))
                throw new ApplicationException("Enter valid name product");


            return _context.Products.Include(product=> product.ProductType)
                                    .Where(product=> product.ProductType.NameType == productName)
                                    .Select(product => product.Manufacturer)
                                    .Distinct().ToArray();
        }
        public string[] GetProductSuppliers()
        {
            return _context.Products.Select(customer => customer.Supplier).Distinct().ToArray();
        }
        public string[] GetProductSuppliers(string productName)
        {
            if (string.IsNullOrEmpty(productName))
                throw new ApplicationException("Enter valid name product");

            return _context.Products.Include(product => product.ProductType)
                                    .Where(product => product.ProductType.NameType == productName)
                                    .Select(customer => customer.Supplier).Distinct().ToArray();
        }
        public addNewProductTypeRequest AddOtherProducts(addNewProductTypeRequest product, string currentUserName)
        {
            ValidateAddOtherProducts(product);

            var idWarehouse = GetOrCreateWarehouse(product.WareHouseName.Value);
            var currencyId = GetOrCreateCurrency(product.CurrencyName.Value);
            var idTypeProduct = GetOrCreateProductType(product.Name.Value);

            var idInstanceProduct = 0;

            double Quantity;
            double StandartCost;
            double PrimeCost;
            DateTime DateOfReceipt;

            MathHelper.CallParseDouble(product.Quantity?.Value, out Quantity);
            MathHelper.CallParseDouble(product.StandartCost?.Value, out StandartCost);
            MathHelper.CallParseDouble(product.PrimeCost?.Value, out PrimeCost);
            if (!String.IsNullOrEmpty(product.DateOfReceipt?.Value)) DateTime.TryParse(product.DateOfReceipt?.Value, out DateOfReceipt);
            else DateOfReceipt = new DateTime();

            var instanceProduct = new Product
            {
                Id = idInstanceProduct,
                ProductTypeId = idTypeProduct,
                Name = product.Name?.Value,
                ProductNumber = product.ProductNumber?.Value,
                Quantity = Quantity,
                StandartCost = StandartCost,
                Manufacturer = product.Manufacturer?.Value,
                Description = product.Description?.Value,
                DateOfReceipt = DateOfReceipt,
                WareHouseId = idWarehouse,
                PrimeCost = PrimeCost,
                CurrencyId = currencyId,
                LastEditedByUser = currentUserName
            };

            _context.Products.Add(instanceProduct);
            _context.SaveChanges();
            idInstanceProduct = instanceProduct.Id;

            foreach (var parameter in product.Parameters)
            {
                if (string.IsNullOrEmpty(parameter.Name)) continue;

                var existingParam = _context.Parameters?
                    .Where(item => item.Name == parameter.Name && item.ProductTypeId == idTypeProduct)
                    .FirstOrDefault();

                var idParam = 0;
                if (existingParam != null)
                {
                    idParam = existingParam.Id;
                }
                else
                {
                    var inctanceParameter = new Parameter()
                    {
                        ProductTypeId = idTypeProduct,
                        Name = parameter.Name,
                        Priority = parameter.NavPriority.Value,
                        NameType = parameter.Type,
                        isRequired = (bool)parameter.Required
                    };

                    _context.Parameters.Add(inctanceParameter);
                    _context.SaveChanges();

                    idParam = inctanceParameter.Id;
                }

                var param = new ProductParameter()
                {
                    ProductId = idInstanceProduct,
                    ParameterId = idParam,
                    Value = parameter.Value
                };

                _context.ProductParameters.Add(param);
            }
            _context.SaveChanges();

            return product;
        }
        public ProductNewDTO AddInstanceProduct(ProductNewDTO productNewDTO, string currentUserName)
        {

            if (string.IsNullOrEmpty(productNewDTO.Name?.Value))
                throw new ApplicationException("Enter valid name product");

            if (string.IsNullOrEmpty(productNewDTO.WareHouseName?.Value))
                throw new ApplicationException("Enter valid name warehouse");

            var warehouse = _context.WareHouse.FirstOrDefault(warehouse => (warehouse.Name == productNewDTO.WareHouseName.Value));
            var idWarehouse = 0;
            if (warehouse != null)
            {
                idWarehouse = warehouse.Id;
            }
            else throw new ApplicationException("Entered name of warehouse does not exist"); ;


            var currency = _context.Currencies.FirstOrDefault(currency =>
                                   currency.CurrencyName == productNewDTO.CurrencyName.Value);
            var currencyId = 0;

            if (currency != null)
            {
                currencyId = currency.Id;
            }
            else throw new ApplicationException("Entered name of currency does not exist");

            var productTypeName = _context.ProductTypes.FirstOrDefault(productType => productType.NameType == productNewDTO.Name.Value);
            var idTypeProduct = 0;
            if (productTypeName != null)
            {
                idTypeProduct = productTypeName.Id;
            }
            else throw new ApplicationException("Entered name of product does not exist");

            double Quantity;
            double StandartCost;
            double PrimeCost;
            double PrimeCostEUR;
            double PrimeCostUSD;
            DateTime DateOfReceipt;

            MathHelper.CallParseDouble(productNewDTO.Quantity?.Value, out Quantity);
            MathHelper.CallParseDouble(productNewDTO.StandartCost?.Value, out StandartCost);
            MathHelper.CallParseDouble(productNewDTO.PrimeCost?.Value, out PrimeCost);
            MathHelper.CallParseDouble(productNewDTO.PrimeCostEUR?.Value, out PrimeCostEUR);
            MathHelper.CallParseDouble(productNewDTO.PrimeCostUSD?.Value, out PrimeCostUSD);
            if (!String.IsNullOrEmpty(productNewDTO.DateOfReceipt?.Value)) DateTime.TryParse(productNewDTO.DateOfReceipt?.Value, out DateOfReceipt);
            else DateOfReceipt = new DateTime();
            var idInstanceProduct = 0;
       
            var instanceProduct = new Product
            {
                Id = idInstanceProduct,
                ProductTypeId = idTypeProduct,
                Name = productNewDTO.Name?.Value,
                ProductNumber = productNewDTO.ProductNumber?.Value,
                Quantity = Quantity,
                StandartCost = StandartCost,
                Manufacturer = productNewDTO.Manufacturer?.Value,
                Description = productNewDTO.Description?.Value,
                DateOfReceipt = DateOfReceipt,
                WareHouseId = idWarehouse,
                PrimeCost = PrimeCost,
                PrimeCostEUR = PrimeCostEUR,
                PrimeCostUSD = PrimeCostUSD,
                CurrencyId = currencyId,
                LastEditedByUser = currentUserName
            };

            _context.Products.Add(instanceProduct);
            _context.SaveChanges();
            idInstanceProduct = instanceProduct.Id;

            var produtParameterValueCode = "";
            foreach (var parameter in productNewDTO.Parameters)
            {
                if (string.IsNullOrEmpty(parameter.Name)) continue;

                var existingParam = _context.Parameters?
                    .Where(item => item.Name == parameter.Name && item.ProductTypeId == idTypeProduct)
                    .FirstOrDefault();

                var idParam = 0;
                if (existingParam != null)
                {
                    idParam = existingParam.Id;
                }
                else continue;

                var param = new ProductParameter()
                {
                    ProductId = idInstanceProduct,
                    ParameterId = idParam,
                    Value = parameter.Value
                };

                if(!existingParam.Name.ToLower().Equals("номер"))
                    produtParameterValueCode = produtParameterValueCode + param.Value + ", ";
                
                _context.ProductParameters.Add(param);
            }
            produtParameterValueCode = produtParameterValueCode.Substring(0, (produtParameterValueCode.Length - 2));
            var PriceListValues = _context.PriceListValues.FirstOrDefault(pl => pl.ProductParameterValueCode.Equals(produtParameterValueCode));
            if (PriceListValues != null)
            {
                var percentPriceEUR = PriceListValues.PercentOfMarkup > 0 ? (PriceListValues.PercentOfMarkup / 100 * instanceProduct.PrimeCostEUR) + instanceProduct.PrimeCostEUR : 0;

                var instancePriceListValue = new PriceListValue
                {
                    Version = 1,
                    PercentOfMarkup = PriceListValues.PercentOfMarkup,
                    Price = PriceListValues.Price,
                    PriceNDS = (0.2 * PriceListValues.Price) + PriceListValues.Price, // need to improve NDS 20
                    PriceEUR = percentPriceEUR, 
                    PriceEURNDS = (0.2 * percentPriceEUR) + percentPriceEUR, // need to improve NDS 20
                    Quantity = 1,
                    ProductId = idInstanceProduct,
                    PriceListId = PriceListValues.PriceListId,
                    ProductParameterUniqCode = PriceListValues.ProductParameterUniqCode,
                    ProductParameterValueCode = produtParameterValueCode
                };

                _context.PriceListValues.Add(instancePriceListValue);
            }



            _context.SaveChanges();

            return productNewDTO;
        }
        public List<ProductParametrDTO> GetInstancesOfProductParameterUpdated(getInstancesOfProductParameterRequest requestParameters)
        {
            var productType = getProductTypeByName(requestParameters.NameType);
        
            List<ProductParametrDTO> productParameters = GetProductParameters(productType, requestParameters);

            return OrderProductParameters(productParameters);
        }
        public async Task<updateProductInstancesRequest> UpdateProduct(updateProductInstancesRequest productDTO, string currentUserName)
        {

            if (productDTO.ProductId == 0)
                throw new ApplicationException("Enter valid ProductId");

            var warehouse = _context.WareHouse.FirstOrDefault(warehouse => (warehouse.Name == productDTO.warehouse.Value));
            var idWarehouse = 0;

            if (warehouse != null)
            {
                idWarehouse = warehouse.Id;
            }
            else throw new ApplicationException("Entered name of warehouse does not exist"); ;


            //var currency = _context.Currencies.FirstOrDefault(currency => currency.CurrencyName == productDTO.currency.Value);
            //var currencyId = 0;

            //if (currency != null)
            //{
            //    currencyId = currency.Id;
            //}
            //else throw new ApplicationException("Entered name of currency does not exist");

            var product = _context.Products.Include(product => product.ProductType)
                                           .Include(product => product.ProductParameters)
                                           .Include(product => product.WareHouse)
                                           .Include(product => product.Currency)
                                           .Where(t => t.Id == productDTO.ProductId)
                                           .FirstOrDefault();

            double Quantity;
            double StandartCost;
            double PrimeCost;
            double PrimeCostUSD;
            double PrimeCostEUR;
            DateTime DateOfReceipt = product.DateOfReceipt;

            MathHelper.CallParseDouble(productDTO.quantity?.Value, out Quantity);
            MathHelper.CallParseDouble(productDTO.standartCost?.Value, out StandartCost);
            MathHelper.CallParseDouble(productDTO.primeCost?.Value, out PrimeCost);
            MathHelper.CallParseDouble(productDTO.PrimeCostUSD?.Value, out PrimeCostUSD);
            MathHelper.CallParseDouble(productDTO.PrimeCostEUR?.Value, out PrimeCostEUR);
            if (!String.IsNullOrEmpty(productDTO.dateOfReceipt?.Value)) DateTime.TryParse(productDTO.dateOfReceipt?.Value, out DateOfReceipt);

            product.ProductNumber = productDTO.productNumber?.Value;
            product.Manufacturer = productDTO.manufacturer?.Value;
            product.Quantity = Quantity;
            product.StandartCost = StandartCost;
            product.PrimeCost = PrimeCost;

            if (PrimeCostUSD != 0)
            {
                product.PrimeCostUSD = PrimeCostUSD;
            }
            if (PrimeCostEUR != 0)
            {
                product.PrimeCostEUR = PrimeCostEUR;
            }
            product.Description = productDTO.description?.Value;
            product.DateOfReceipt = DateOfReceipt;
            product.WareHouseId = idWarehouse;
            //  product.CurrencyId = currencyId;
            product.DateOfLastChanged = DateTimeHelper.DateTimeNowWithOffset();
            product.LastEditedByUser = currentUserName;
            await _context.SaveChangesAsync();
            foreach (var paramDTO in productDTO.parameters)
            {
                if (!string.IsNullOrEmpty(paramDTO.Name))
                {
                    var existingParam = await _context.Parameters.Where(item => item.ProductTypeId == product.ProductTypeId)
                                                           .Where(item => item.Name == paramDTO.Name).FirstOrDefaultAsync();

                    if (existingParam != null)
                    {
                        var productParameters = product.ProductParameters.Where(p => p.ParameterId == existingParam.Id).FirstOrDefault();

                        if (productParameters != null & !string.IsNullOrWhiteSpace(paramDTO?.Value))
                        {
                            productParameters.Value = paramDTO.Value;
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();


            return productDTO;
        }
        public async Task<ProductHistory> DeleteProductInstance(int? id, string currentUserName)
        {
            if (id is null) throw new ApplicationException("ProductId does not exist");
            if (!id.HasValue) throw new ApplicationException("ProductId does not exist");

            var product = _context.Products
                   .Include(p => p.ProductType)
                    .ThenInclude(parameters => parameters.Parameters)
                    .ThenInclude(productParameters => productParameters.ProductParameters)
                   .Where(i => i.Id == id).FirstOrDefault();


            if (product == null) throw new ApplicationException("ProductId does not exist");

            var productTypeHistoryId = GetOrCreateProductTypeHistory(product.ProductType.NameType, product.ProductType.Id);
           
            var deletedProduct = new ProductHistory()
            {
                Name = product.Name,
                ProductNumber = product.ProductNumber,
                Quantity = product.Quantity,
                PrimeCost = product.PrimeCost,
                PrimeCostEUR = product.PrimeCostEUR,
                PrimeCostUSD = product.PrimeCostUSD,
                StandartCost = product.StandartCost,
                Supplier = product.Supplier,
                ProductStandartCost = product.StandartCost,
                Manufacturer = product.Manufacturer,
                Description = product.Description,
                ProductDeleted = true,
                DateOfChange = DateTimeHelper.DateTimeNowWithOffset(),
                DateOfReceipt = product.DateOfReceipt,
                ProductTypeHistoryId = productTypeHistoryId,
                DeletedProductTypeId = product.ProductTypeId,
                CurrencyId = product.CurrencyId ?? 0,
                WareHouseId = product.WareHouseId,
                DeletedProductId = product.Id,
                UserName = currentUserName
            };

            _context.ProductHistory.Add(deletedProduct);

            await _context.SaveChangesAsync();

            foreach (var productValue in product.ProductParameters)
            {
                var idPameterHistory = this.GetOrCreateParameterHistory(productValue.ParameterId, productTypeHistoryId);
                var deletedProductParameters = new ProductParameterHistory
                {
                    ProductHistoryId = deletedProduct.Id,
                    ParameterHistoryId = idPameterHistory,
                    Value = productValue.Value
                };
                _context.ProductParameterHistory.Add(deletedProductParameters);
            }
            await _context.SaveChangesAsync();

            _context.Products.Remove(product);


            await this.AddOrderProductHistoryAsync(id, deletedProduct.Id);

            await _context.SaveChangesAsync();


            return deletedProduct;
        }

        private ProductType getProductTypeByName(string typeName)
        {
            var productType = _context.ProductTypes.FirstOrDefault(type => type.NameType.Trim() == typeName.Trim());

            if (productType == null)
            {
                throw new ArgumentException("Product name " + typeName + " does not exist");
            }

            return productType;
        }
        private List<ProductParametrDTO> GetProductParameters(ProductType productType, getInstancesOfProductParameterRequest parametrOption)
        {

            var parameterValues = parametrOption.IsParameter
                ? GetParameterValues(productType, parametrOption)
                : GetProductPropertyValues(productType, parametrOption);

            return parameterValues
                .Select(value => new ProductParametrDTO
                {
                    Name = parametrOption.NameParameter,
                    Value = value,
                    ValueDouble = value.ParseDouble()
                })
                .ToList();
        }
        private List<string> GetParameterValues(ProductType productType, getInstancesOfProductParameterRequest parametrOption)
        {
            var param = _context.Parameters
                  .Where(param => param.ProductTypeId == productType.Id && param.Name.Trim().ToLower() == parametrOption.NameParameter)
                 // .Select(param => (int?)param.Id) // need to improve
                  .FirstOrDefault();

            var pProducts = param != null
                 ? _context.ProductParameters
                     .Where(productParam => productParam.ParameterId == param.Id && !string.IsNullOrEmpty(productParam.Value))
                     .ToList() : null;


            if (parametrOption.FilterParameters.Count() > 0)
            {
                // Get parameter info
                List<ParamaterValue> listParameters  = new List<ParamaterValue>();
                foreach (var fParam in parametrOption.FilterParameters)
                {
                    var p = _context.Parameters
                     .Where(parameter => parameter.ProductTypeId == productType.Id && parameter.Name.Trim().ToLower() == fParam.ParameterName)
                     .FirstOrDefault();
                   
                    if (p != null)
                    {
                        listParameters.Add(new ParamaterValue() { ParameterValue = fParam.ParameterValue, SelectedParameter = p });
                    }

                }
               
                if(listParameters != null && listParameters.Count()>0)
                {
                    listParameters.OrderBy(p => p.SelectedParameter.Priority);
                    var firstParameter = listParameters.First();
                    var topPParameterValue = _context.ProductParameters
                       .Where(productParam => productParam.ParameterId == firstParameter.SelectedParameter.Id && !string.IsNullOrEmpty(productParam.Value) 
                             && productParam.Value.Equals(firstParameter.ParameterValue))
                       .ToList();

                    // List<ProductParameter> filtersProduct = null;
                    listParameters.Remove(firstParameter);
                    foreach (var fparam in listParameters)
                    {
                       var secPProducts =_context.ProductParameters
                       .Where(productParam => productParam.ParameterId == fparam.SelectedParameter.Id && !string.IsNullOrEmpty(productParam.Value)
                             && productParam.Value.Equals(fparam.ParameterValue))
                       .ToList();
                      
                        List<ProductParameter> fValues = new List<ProductParameter>();
                        foreach (var tr in topPParameterValue)
                        {
                            foreach (var secPP in secPProducts)
                            {
                                if (tr.ProductId == secPP.ProductId)
                                {
                                    fValues.Add(tr);
                                }

                            }
                        }
                        topPParameterValue = fValues;
                        //     var filterdProducts = ;
                        //   filtersProduct.AddRange(filterdProducts);
                    }

                    if (topPParameterValue.Count() > 0)
                    {
                        List<ProductParameter> fValues = new List<ProductParameter>();
                        foreach (var tr in pProducts)
                        {
                            foreach (var topPP in topPParameterValue)
                            {
                                if (tr.ProductId == topPP.ProductId)
                                {
                                    fValues.Add(tr);
                                }

                            }
                        }

                        var paramValues = fValues.Select(pp => pp.Value).Distinct().ToList();
                        if (param != null && ((param.Name.ToLower()).Equals(("диаметр")) || (param.Name.ToLower()).Equals(("размер"))
                            || param.Name.ToLower().Equals("внутренний диаметр")
                            ))
                        {
                            return sortDiamterParameterValues(paramValues); // sort string with delimiter *
                        }

                        return paramValues;
                    }
                }

            }
            var pValues = pProducts.Count() > 0 ? pProducts.Select(pp => pp.Value).Distinct().ToList() : new List<string>();
            if (param != null && ((param.Name.ToLower()).Equals(("диаметр")) || (param.Name.ToLower()).Equals(("размер"))
                || param.Name.ToLower().Equals("внутренний диаметр")))
            {
                return sortDiamterParameterValues(pValues); // sort string with delimiter *
            }

            return pValues;
        }
        private List<string> sortDiamterParameterValues(List<string> pValues)
        {
            double parsedValue = 0;
            Dictionary<string, double> listDiamters = new Dictionary<string, double>();
            foreach (var item in pValues)
            {
                if (double.TryParse(item, out parsedValue))
                {
                    listDiamters.Add(item, parsedValue);
                }
                else
                {
                    string[] parts = item.Split('*');
                    if (parts.Length > 0)
                    {
                        if (double.TryParse(parts[0], out double number))
                        {
                            listDiamters.Add(item, number);
                        }
                    }


                    parts = item.Split('/');
                    if (parts.Length > 0)
                    {
                        if (double.TryParse(parts[0], out double number))
                        {
                            listDiamters.Add(item, number);
                        }
                    }
                }

            }
            var rr = listDiamters.OrderBy(item => item.Value);
            return rr.Select(item => item.Key).ToList();
        }
        private List<string> GetProductPropertyValues(ProductType productType, getInstancesOfProductParameterRequest parametrOption)
        {

            var x = Expression.Parameter(typeof(Product), parametrOption.NameParameter);
            var body = Expression.PropertyOrField(x, parametrOption.NameParameter);
            var lambda = Expression.Lambda<Func<Product, string>>(body, x);

            var tt = _context.Products
                .Where(param => param.ProductTypeId == productType.Id)
                .Select(el => lambda.Compile())
                .Select(el => el.ToString())
                .Distinct()
                .ToList();

            return tt;
        }
        private List<ProductParametrDTO> OrderProductParameters(List<ProductParametrDTO> productParameters)
        {
            return productParameters
                .OrderBy(el => el.ValueDouble != 0 ? el.ValueDouble : double.MaxValue)
                .ToList();
        }
        private async Task AddOrderProductHistoryAsync(int? productId, int? deletedProductId)
        {
            if (productId == null)
            {
                throw new ArgumentException("ProductId does not exist.");
            }

            if (deletedProductId == null)
            {
                throw new ArgumentException("DeletedProductId does not exist.");
            }

            var orderProducts = await _context.OrderProducts
                                              .Where(op => op.ProductId == productId)
                                              .ToListAsync();

            if (orderProducts.Any())
            {
                var orderProductHistories = orderProducts.Select(op => new OrderProductHistory
                {
                    Quantity = op.Quantity,
                    ProductHistoryId = deletedProductId.Value,
                    OrderId = op.OrderId,
                    TotalPrice = op.TotalPrice,
                    TaxNDS = op.TaxNDS,
                    Markup = op.Markup
                }).ToList();

                _context.OrderProductHistory.AddRange(orderProductHistories);
                await _context.SaveChangesAsync();
            }
        }

        private readonly Dictionary<string, Func<Product, string>> SortFieldMappings = new Dictionary<string, Func<Product, string>>
        {
           { "quantity", p => p.Quantity.ToString() },
           { "productnumber", p => p.ProductNumber },
           { "manufacturer", p => p.Manufacturer },
           { "standartcost", p => p.StandartCost.ToString() },
           { "primecost", p => p.PrimeCost.ToString() },
           { "primecostusd", p => p.PrimeCostUSD.ToString() },
           { "primecosteur", p => p.PrimeCostEUR.ToString() },
           { "dateofreceipt", p => p.DateOfReceipt.ToString() },
           { "warehousename", p => p.Manufacturer }
        };
        public byte[] GeneratePDF()
        {
            // Создание нового документа PDF
            PdfDocument document = new PdfDocument();

            // Создание страницы
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);
                document.Info.Title = "A sample invoice";
                document.Info.Subject = "Demonstrates how to create an invoice.";
                document.Info.Author = "Stefan Lange";
            // Расположение и размеры таблицы
            XRect rect = new XRect(30, 100, page.Width - 80, 200);

            // Создание шрифта и форматирование текста
            XFont font = new XFont("Arial", 12, XFontStyle.Regular);
            XStringFormat format = new XStringFormat();
            format.Alignment = XStringAlignment.Center;
            format.LineAlignment = XLineAlignment.Center;
            // Добавление шапки страницы
            DrawHeader(gfx, "Имя файла: " + "Статистика", 40, 40, page.Width - 80, 20, font, format);
            DrawHeader(gfx, "Время: " + DateTimeHelper.DateTimeNowWithOffset().ToString(), 40, 60, page.Width - 80, 20, font, format);
            DrawHeader(gfx, "Краткое описание: Ваше описание", 40, 80, page.Width - 80, 20, font, format);

            // Добавление заголовков столбцов с границами
            // Добавление заголовков столбцов с границами
            DrawCell(gfx, "Заголовок 1", font, rect.Left, rect.Top, 100, 20, format);
            DrawCell(gfx, "Заголовок 2", font, rect.Left + 100, rect.Top, 100, 20, format);
            DrawCell(gfx, "Заголовок 3", font, rect.Left + 200, rect.Top, 100, 20, format);

            // Добавление данных в таблицу с границами
            for (int i = 0; i < 150; i++)
            {
                int yOffset = (i + 1) * 20;
                DrawCell(gfx, $"Значение {i + 1},1", font, rect.Left, rect.Top + yOffset, 100, 20, format);
                DrawCell(gfx, $"Значение {i + 1},2", font, rect.Left + 100, rect.Top + yOffset, 100, 20, format);
                DrawCell(gfx, $"Значение {i + 1},3", font, rect.Left + 200, rect.Top + yOffset, 100, 20, format);
            }

            // Сохранение документа в файл
            document.Save("FirstPDFDocument.pdf");


            MemoryStream stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }
        public void DrawCell(XGraphics gfx, string text, XFont font, double x, double y, double width, double height, XStringFormat format)
        {
            // Рисование текста в ячейке
            gfx.DrawString(text, font, XBrushes.Black, x + width / 2, y + height / 2, format);

            // Рисование границ ячейки
            gfx.DrawRectangle(XPens.Black, x, y, width, height);
        }
        public void DrawHeader(XGraphics gfx, string text, double x, double y, double width, double height, XFont font, XStringFormat format)
        {
            // Рисование текста в шапке
            gfx.DrawString(text, font, XBrushes.Black, x + width / 2, y + height / 2, format);
        }
    }


}

