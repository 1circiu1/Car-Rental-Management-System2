using CarRental.Backend.Data;
using CarRental.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Backend.Services
{
    public class CarService
    {
        private readonly AppDbContext _context;

        public CarService(AppDbContext context)
        {
            _context = context;
        }

        public List<Car> GetAvailableCars()
        {
            return _context.Cars
                 .Where(c => c.Status == CarStatus.Available)
                 .ToList();
        }

        public void AddCar(Car car)
        {
            _context.Cars.Add(car);
            _context.SaveChanges();
        }

        public Car GetCarById(int carId)
        {
            return _context.Cars.FirstOrDefault(c => c.CarId == carId);
        }

        public List<Car> GetAllCars()
        {
            return _context.Cars.ToList();
        }

        public List<Car> GetCarsByOwner(int ownerUserId)
        {
            return _context.Cars
                .Where(c => c.UserId == ownerUserId)
                .ToList();
        }

        public void ToggleMaintenance(int carId)
        {
            var car = GetCarById(carId);

            if (car == null)
                return;

            car.Status = car.Status == CarStatus.Maintenance
                ? CarStatus.Available
                : CarStatus.Maintenance;

            _context.SaveChanges();
        }

        public int CountOwnerCarsByStatus(int ownerUserId, CarStatus status)
        {
            return _context.Cars
                .Count(c => c.UserId == ownerUserId && c.Status == status);
        }

        public int CountOwnerCarsNeedingReports(int ownerUserId)
        {
            return _context.Cars
                .Count(c => c.UserId == ownerUserId &&
                           (c.Status == CarStatus.Rented ||
                            c.Status == CarStatus.Maintenance));
        }

        public void UpdateCar(Car car)
        {
            _context.Cars.Update(car);
            _context.SaveChanges();
        }

        public void DeleteCar(int id)
        {
            var car = _context.Cars.FirstOrDefault(c => c.CarId == id);

            if (car != null)
            {
                _context.Cars.Remove(car);
                _context.SaveChanges();
            }
        }
    }
}
