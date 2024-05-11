using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gdm5._0.Models
{
    public class TokenApiDTO
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string UserRole { get; set; }
        public int UserId { get; set; }
    }
}
