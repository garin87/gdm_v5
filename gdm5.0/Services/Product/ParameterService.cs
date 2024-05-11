using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using gdm5._0.DTO;
using gdm5._0.Models;
using gdm5._0.Services.Interfaces;

namespace gdm5._0.Services
{
    public class ParameterService : BaseService<Parameter>, IParameterService
    {
        public ParameterService(DataContext context) : base(context)
        {
        }

        public List<Parameter> getUniqueNameParameters()
        {
            return _context.Parameters.Select(p => p).ToList()
                                      .GroupBy(parameter => parameter.Name)
                                      .Select(p => p.FirstOrDefault())
                                      .ToList();
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
                        existingParam.NameType = parameter.Type;
                        existingParam.Priority = parameter.NavPriority.Value;
                        existingParam.isRequired = (bool)parameter.Required;
                    }
                    else
                    {
                        var inctanceParameter = new Parameter()
                        {
                            ProductTypeId = productTypeId,
                            Name = parameterName,
                            NameType = parameter.Type,
                            Priority = parameter.NavPriority.Value,
                            isRequired = (bool)parameter.Required
                    };

                        _context.Parameters.Add(inctanceParameter);
                    }


                }

                _context.SaveChanges();

            }

            return productNewDTO;
        }
    }
}
