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

        public User Login(string email, string password)
        {
            var users = _context.Users.ToList();

            return _context.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password);
        }


        public void Register(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }
    }
}