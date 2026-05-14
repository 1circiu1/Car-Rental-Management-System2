using CarRental.Backend.Data;
using CarRental.Backend.Models;
using System.Linq;

namespace CarRental.Backend.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService()
        {
            _context = new AppDbContext();
        }

        public User Login(string email, string password, string role)
        {
            return _context.Users
                .FirstOrDefault(u =>
                    u.Email == email &&
                    u.Password == password &&
                    u.Role == role);
        }


        public User Register(User user, string phone)
        {
            _context.Users.Add(user);
            _context.SaveChanges();

            if (user.Role == "Customer")
            {
                Customer customer = new Customer
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = phone
                };

                _context.Customers.Add(customer);
            }
            else if (user.Role == "CarRenter")
            {
                CarRenter carRenter = new CarRenter
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Phone = phone
                };

                _context.CarRenters.Add(carRenter);
            }

            _context.SaveChanges();

            return user;
        }
    }
}