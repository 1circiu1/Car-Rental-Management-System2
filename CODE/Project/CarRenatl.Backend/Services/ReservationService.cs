using CarRental.Backend.Data;
using CarRental.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CarRental.Backend.Services
{
    public class ReservationService
    {
        private readonly AppDbContext _context;

        public ReservationService(AppDbContext context)
        {
            _context = context;
        }

        public void CreateReservation(Reservation reservation)
        {
            Car car = _context.Cars.FirstOrDefault(c => c.CarId == reservation.CarId);

            if (car == null)
                return;

            reservation.Car = car;

            if (!reservation.ValidatePeriod())
                return;

            if (!CheckAvailability(reservation.CarId, reservation.StartDate, reservation.EndDate))
                return;

            reservation.TotalCost = reservation.CalculateTotal();
            reservation.Status = ReservationStatus.Confirmed;

            car.Status = CarStatus.Rented;

            _context.Reservations.Add(reservation);
            _context.SaveChanges();
        }

        public void CancelReservation(int reservationId)
        {
            Reservation reservation = _context.Reservations
                .FirstOrDefault(r => r.ReservationId == reservationId);

            if (reservation != null)
            {
                reservation.Status = ReservationStatus.Cancelled;

                Car car = _context.Cars.FirstOrDefault(c => c.CarId == reservation.CarId);

                if (car != null)
                {
                    car.Status = CarStatus.Available;
                }

                _context.SaveChanges();
            }
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

        public decimal CalculateCost(int carId, DateTime startDate, DateTime endDate)
        {
            Car car = _context.Cars.FirstOrDefault(c => c.CarId == carId);

            if (car == null)
                return 0;

            int days = (endDate.Date - startDate.Date).Days;

            if (days <= 0)
                return 0;

            return days * car.PricePerDay;
        }
    }
}
