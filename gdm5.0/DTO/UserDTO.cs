using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.DTO
{
    public class UserDTO
    {
        public string Email { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string RefreshToken { get; set; }
        public string UserRole { get; set; }
    }
}
