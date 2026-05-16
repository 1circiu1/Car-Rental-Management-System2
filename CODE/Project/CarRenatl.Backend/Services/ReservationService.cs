using CarRental.Backend.Data;
using CarRental.Backend.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarRental.Backend.Services
{
    public class ReservationService
    {
        private readonly AppDbContext _context;
        private const decimal COMPANY_FEE_PERCENT = 0.15m;

        public ReservationService(AppDbContext context)
        {
            _context = context;
        }

        public bool CreateReservation(
            int carId,
            string customerEmail,
            DateTime startDate,
            DateTime endDate,
            string pickupLocation)
        {
            var car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            if (car == null)
                return false;

            var customer = GetCustomerByEmail(customerEmail);

            if (customer == null)
                return false;

            if (!CheckAvailability(carId, startDate, endDate))
                return false;

            decimal totalCost = CalculateCost(carId, startDate, endDate);

            var reservation = new Reservation
            {
                CarId = carId,
                CustomerId = customer.CustomerId,
                StartDate = startDate,
                EndDate = endDate,
                PickupLocation = pickupLocation,
                TotalCost = totalCost,
                Status = ReservationStatus.Pending
            };

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            return true;
        }

        public List<Reservation> GetCustomerReservations(string customerEmail)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Email == customerEmail);

            if (customer == null)
                return new List<Reservation>();

            return _context.Reservations
                .Include(r => r.Car)
                .Where(r => r.CustomerId == customer.CustomerId)
                .OrderByDescending(r => r.StartDate)
                .ToList();
        }

        public Reservation GetUpcomingReservation(string customerEmail)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Email == customerEmail);

            if (customer == null)
                return null;

            return _context.Reservations
                .Include(r => r.Car)
                .Where(r =>
                    r.CustomerId == customer.CustomerId &&
                    r.Status != ReservationStatus.Cancelled &&
                    r.EndDate >= DateTime.Now)
                .OrderBy(r => r.StartDate)
                .FirstOrDefault();
        }

        public List<string> GetUnavailablePeriods(int carId)
        {
            return _context.Reservations
                .Where(r =>
                    r.CarId == carId &&
                    r.Status != ReservationStatus.Cancelled &&
                    r.EndDate >= DateTime.Now)
                .OrderBy(r => r.StartDate)
                .Select(r =>
                    $"{r.StartDate:dd MMM yyyy, HH:mm} → {r.EndDate:dd MMM yyyy, HH:mm}")
                .ToList();
        }

        public int GetActiveReservationsCount(string customerEmail)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Email == customerEmail);

            if (customer == null)
                return 0;

            return _context.Reservations.Count(r =>
                r.CustomerId == customer.CustomerId &&
                r.Status != ReservationStatus.Cancelled &&
                r.EndDate >= DateTime.Now);
        }

        public Reservation GetLatestReservation(string customerEmail)
        {
            var customer = _context.Customers.FirstOrDefault(c => c.Email == customerEmail);

            if (customer == null)
                return null;

            return _context.Reservations
                .Include(r => r.Car)
                .Where(r => r.CustomerId == customer.CustomerId)
                .OrderByDescending(r => r.ReservationId)
                .FirstOrDefault();
        }

        public Customer GetCustomerByEmail(string email)
        {
            return _context.Customers
                .FirstOrDefault(c => c.Email == email);
        }

        public Reservation GetReservationById(int reservationId)
        {
            return _context.Reservations
                .Include(r => r.Car)
                .FirstOrDefault(r => r.ReservationId == reservationId);
        }

        public void CancelReservation(int reservationId)
        {
            var reservation = _context.Reservations
                .FirstOrDefault(r => r.ReservationId == reservationId);

            if (reservation == null)
                return;

            reservation.Status = ReservationStatus.Cancelled;

            _context.SaveChanges();
        }

        public bool CheckAvailability(int carId, DateTime startDate, DateTime endDate)
        {
            bool exists = _context.Reservations.Any(r =>
                r.CarId == carId &&
                r.Status != ReservationStatus.Cancelled &&
                startDate < r.EndDate &&
                endDate > r.StartDate);

            return !exists;
        }

        public bool ValidatePeriod(DateTime startDate, DateTime endDate)
        {
            return startDate >= DateTime.Now && endDate > startDate;
        }

        public bool ValidateWorkingHours(DateTime startDate, DateTime endDate)
        {
            return startDate.Hour >= 8 &&
                   startDate.Hour <= 20 &&
                   endDate.Hour >= 8 &&
                   endDate.Hour <= 20;
        }

        public decimal CalculateCost(int carId, DateTime startDate, DateTime endDate)
        {
            Car car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            if (car == null)
                return 0;

            TimeSpan duration = endDate - startDate;
            double totalHours = duration.TotalHours;

            if (totalHours < 24)
            {
                decimal hourlyRate = car.PricePerDay / 10;
                return Math.Ceiling((decimal)totalHours) * hourlyRate;
            }

            decimal totalDays = Math.Ceiling((decimal)duration.TotalDays);
            return totalDays * car.PricePerDay;
        }






        // OWNER SERVICES
        public List<Reservation> GetOwnerReservations(int ownerUserId)
        {
            return _context.Reservations
                .Include(r => r.Car)
                .Include(r => r.Customer)
                .Where(r => r.Car.UserId == ownerUserId)
                .OrderByDescending(r => r.StartDate)
                .ToList();
        }

        public int CountOwnerReservationsByStatus(int ownerUserId, ReservationStatus status)
        {
            return _context.Reservations
                .Include(r => r.Car)
                .Count(r => r.Car.UserId == ownerUserId && r.Status == status);
        }

        public void ConfirmReservation(int reservationId)
        {
            var reservation = _context.Reservations
                .FirstOrDefault(r => r.ReservationId == reservationId);

            if (reservation == null)
                return;

            reservation.Status = ReservationStatus.Confirmed;
            _context.SaveChanges();
        }

        public void RejectReservation(int reservationId)
        {
            var reservation = _context.Reservations
                .FirstOrDefault(r => r.ReservationId == reservationId);

            if (reservation == null)
                return;

            reservation.Status = ReservationStatus.Cancelled;
            _context.SaveChanges();
        }

        public decimal GetOwnerTotalRevenue(int ownerUserId)
        {
            return _context.Reservations
                .Include(r => r.Car)
                .Where(r => r.Car.UserId == ownerUserId &&
                           (r.Status == ReservationStatus.Confirmed ||
                            r.Status == ReservationStatus.Completed))
                .Sum(r => r.TotalCost * (1 - COMPANY_FEE_PERCENT));
        }

        public decimal GetOwnerMonthlyRevenue(int ownerUserId)
        {
            var now = DateTime.Now;

            return _context.Reservations
                .Include(r => r.Car)
                .Where(r => r.Car.UserId == ownerUserId &&
                           (r.Status == ReservationStatus.Confirmed ||
                            r.Status == ReservationStatus.Completed) &&
                           r.StartDate.Month == now.Month &&
                           r.StartDate.Year == now.Year)
                .Sum(r => r.TotalCost * (1 - COMPANY_FEE_PERCENT));
        }

        public decimal GetOwnerPendingRevenue(int ownerUserId)
        {
            return _context.Reservations
                .Include(r => r.Car)
                .Where(r => r.Car.UserId == ownerUserId &&
                            r.Status == ReservationStatus.Pending)
                .Sum(r => r.TotalCost * (1 - COMPANY_FEE_PERCENT));
        }

        public int GetOwnerCompletedRentalsCount(int ownerUserId)
        {
            return _context.Reservations
                .Include(r => r.Car)
                .Count(r => r.Car.UserId == ownerUserId &&
                           (r.Status == ReservationStatus.Confirmed ||
                            r.Status == ReservationStatus.Completed));
        }

        public List<Reservation> GetOwnerRecentEarnings(int ownerUserId, int count = 5)
        {
            return _context.Reservations
                .Include(r => r.Car)
                .Where(r => r.Car.UserId == ownerUserId &&
                           (r.Status == ReservationStatus.Confirmed ||
                            r.Status == ReservationStatus.Completed))
                .OrderByDescending(r => r.StartDate)
                .Take(count)
                .ToList();
        }

        public Car GetOwnerTopEarningCar(int ownerUserId)
        {
            return _context.Reservations
                .Include(r => r.Car)
                .Where(r => r.Car.UserId == ownerUserId &&
                           (r.Status == ReservationStatus.Confirmed ||
                            r.Status == ReservationStatus.Completed))
                .GroupBy(r => r.Car)
                .OrderByDescending(g => g.Sum(r => r.TotalCost))
                .Select(g => g.Key)
                .FirstOrDefault();
        }

        public decimal GetCarRevenue(int carId)
        {
            return _context.Reservations
                .Where(r => r.CarId == carId &&
                           (r.Status == ReservationStatus.Confirmed ||
                            r.Status == ReservationStatus.Completed))
                .Sum(r => r.TotalCost * (1 - COMPANY_FEE_PERCENT));
        }

        public decimal GetOwnerRevenueForMonth(int ownerUserId, int month, int year)
        {
            return _context.Reservations
                .Include(r => r.Car)
                .Where(r =>
                    r.Car.UserId == ownerUserId &&
                    (r.Status == ReservationStatus.Confirmed ||
                     r.Status == ReservationStatus.Completed) &&
                    r.StartDate.Month == month &&
                    r.StartDate.Year == year)
                .Sum(r => r.TotalCost);
        }
    }
}