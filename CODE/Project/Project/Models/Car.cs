using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Models
{
    public enum CarStatus
    {
        Available,
        Rented,
        Maintenance
    }

    public class Car
    {
        public int CarId { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string PlateNumber { get; set; }
        public decimal PricePerDay { get; set; }

        public CarStatus Status { get; set; } = CarStatus.Available;

        public List<Reservation> Reservations { get; set; } = new List<Reservation>();

     
        public bool IsAvailable() => Status == CarStatus.Available;
    }
}