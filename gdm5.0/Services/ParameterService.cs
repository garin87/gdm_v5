using System;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.DTO;
using gdm5._0.Models;
using gdm5._0.Services.Interfaces;

namespace gdm5._0.Services
{
    public class ParameterService : BaseService<Parameter>, IParameterService
    {
        private readonly DataContext _context;

        public ParameterService(DataContext context) : base(context)
        {
            _context = context;
        }


        public async Task<Parameter> UpdateParameter(long id, Parameter parameter)
        {
            Parameter p = await GetItem(parameter.Id);
            p.Name = parameter.Name;
            await _context.SaveChangesAsync();
            return parameter;
        }

        public ProductNewDTO UpdateProductTypeParameters(ProductNewDTO productNewDTO)
        {

            if (string.IsNullOrEmpty(productNewDTO.Name?.Value))
                throw new ApplicationException("Enter valid name product");
           
            var productTypeName = _context.ProductTypes.FirstOrDefault(productType => 
                                           productType.NameType == productNewDTO.Name.Value);

            if (productTypeName == null)
                throw new ApplicationException("Product " + productNewDTO.Name.Value + " does not exist");

            var productTypeId = productTypeName.Id;
            foreach (var parameter in productNewDTO.Parameters)
            {
                if (string.IsNullOrEmpty(parameter.Name)) break;

                var parameterName =  !string.IsNullOrEmpty(parameter.newName) ? parameter.newName : parameter.Name;
                var parameterIsDeleted = parameter.isDeleted;
             
                var existingParam = _context.Parameters?
                    .Where(item => item.Name == parameter.Name && item.ProductTypeId == productTypeId)
                    .FirstOrDefault();

                if (parameterIsDeleted)
                {
                    if(existingParam != null) _context.Parameters.Remove(existingParam);
                    else continue;

                }
                else
                {
                    if (existingParam != null)
                    {
                        existingParam.Name = parameterName;
                    }
                    else
                    {
                        var inctanceParameter = new Parameter()
                        {
                            ProductTypeId = productTypeId,
                            Name = parameterName,
                        };

                        _context.Parameters.Add(inctanceParameter);
                    }


                }

                _context.SaveChanges();

            }
            // _context.SaveChanges();

            return productNewDTO;
        }
    }
}
