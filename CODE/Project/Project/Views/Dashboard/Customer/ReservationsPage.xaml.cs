using CarRental.Backend.Data;
using CarRental.Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Linq;

namespace Project.Views.Dashboard
{
    public sealed partial class ReservationsPage : Page
    {
        public ReservationsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadReservations();
        }

        private void LoadReservations()
        {
            if (SessionManager.CurrentUser == null)
                return;

            using var db = new AppDbContext();

            var customer = db.Customers.FirstOrDefault(c =>
                c.Email == SessionManager.CurrentUser.Email);

            if (customer == null)
                return;

            List<Reservation> reservations = db.Reservations
                .Include(r => r.Car)
                .Where(r => r.CustomerId == customer.CustomerId)
                .OrderByDescending(r => r.StartDate)
                .ToList();

            ReservationsList.ItemsSource = reservations;
        }
    }
}