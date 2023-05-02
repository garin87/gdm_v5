using gdm5._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Services.Interfaces
{
    public interface IParameterService : IBaseServices<Parameter>
    {
        Task<Parameter> UpdateParameter(long id, Parameter parameter);
        List<Parameter> getUniqueNameParameters();
    }
}
