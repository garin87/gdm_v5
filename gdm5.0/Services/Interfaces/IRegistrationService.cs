using gdm5._0.DTO;
using gdm5._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Services.Interfaces
{
    public interface IRegistrationService
    {
        UserDTO CreateUser(UserDTO user);
    }
}
