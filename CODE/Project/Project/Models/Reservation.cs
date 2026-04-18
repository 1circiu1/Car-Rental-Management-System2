using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        Cancelled,
        Completed
    }

    internal class Reservation
    {
        public int ReservationId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        public int CarId { get; set; }
        public int CustomerId { get; set; }

        public Car Car { get; set; }
        public Customer Customer { get; set; }

        public decimal CalculateTotal()
        {
            if (Car == null)
                return 0;

            int days = (EndDate.Date - StartDate.Date).Days;

            if (days <= 0)
                return 0;

            TotalCost = days * Car.PricePerDay;
            return TotalCost;
        }

        public bool ValidatePeriod()
        {
            return EndDate > StartDate;
        }
    }
}