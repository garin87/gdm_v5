using gdm5._0.DTO;
using gdm5._0.Models;
using gdm5._0.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace gdm5._0.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly DataContext _context;

        public RegistrationService(DataContext context)
        {
            _context = context;
        }

        public UserDTO CreateUser(UserDTO user)
        {
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Password))
                return null;

            if (_context.Users.Any(x => x.UserName == user.UserName))
                throw new ApplicationException("Username \"" + user.UserName + "\" is already exists");


            var userRole = _context.Roles.FirstOrDefault(item => item.Name == user.UserRole);
            var newUser = new User
            {
                Email = user.Email,
                UserName = user.UserName,
                Password = user.Password,
                RoleId = userRole.Id // 1 Admin 2 User
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();

            //byte[] passwordHash, passwordSalt;
            //CreatePasswordHash(user.Password, out passwordHash, out passwordSalt);

            return user;
        }

        //private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        //{
        //    if (password == null) throw new ArgumentNullException("password");
        //    if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

        //    using (var hmac = new System.Security.Cryptography.HMACSHA512())
        //    {
        //        passwordSalt = hmac.Key;
        //        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //    }
        //}
    }
}
