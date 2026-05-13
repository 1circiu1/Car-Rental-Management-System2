using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Linq;
using CarRental.Backend.Data;
using CarRental.Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Project.Views.Dashboard
{
    public sealed partial class OverviewPage : Page
    {
        public OverviewPage()
        {
            InitializeComponent();

            var user = SessionManager.CurrentUser;

            if (user != null)
            {
                LoadWelcomeMessage(user);
                LoadDashboardStats(user);
                LoadUpcomingReservation(user);
                LoadRecentActivity(user);
            }
            else
            {
                WelcomeText.Text = "Welcome back!";
                WelcomeSubtitle.Text = "Welcome to DriveEase.";
            }
        }

        private void LoadWelcomeMessage(User user)
        {
            switch (user.Role)
            {
                case "Admin":
                    WelcomeText.Text = $"Welcome back, Admin {user.FirstName}!";
                    WelcomeSubtitle.Text = "Monitor platform activity, manage users, and oversee reservations across DriveEase.";
                    break;

                case "CarRenter":
                    WelcomeText.Text = $"Welcome back, {user.FirstName}! Ready to manage your fleet?";
                    WelcomeSubtitle.Text = "Manage your listed vehicles, track reservations, and grow your rental business with DriveEase.";
                    break;

                default:
                    WelcomeText.Text = $"Welcome back, {user.FirstName} {user.LastName}!";
                    WelcomeSubtitle.Text = "Manage your reservations, explore available cars, save your favorite vehicles, and get support anytime with DriveEase.";
                    break;
            }
        }

        private CarRental.Backend.Models.Customer GetCurrentCustomer(AppDbContext db, User user)
        {
            return db.Customers.FirstOrDefault(c => c.Email == user.Email);
        }

        private void LoadDashboardStats(User user)
        {
            using var db = new AppDbContext();

            var customer = GetCurrentCustomer(db, user);

            if (customer == null)
            {
                ReservationsCountText.Text = "0";
                FavoritesCountText.Text = "0";
                SupportTicketsText.Text = "0";
                return;
            }

            ReservationsCountText.Text = db.Reservations
                .Count(r =>
                    r.CustomerId == customer.CustomerId &&
                    r.Status != ReservationStatus.Cancelled &&
                    r.EndDate >= DateTime.Now)
                .ToString();

            FavoritesCountText.Text = db.FavoriteCars
                .Count(f => f.UserId == user.UserId)
                .ToString();

            SupportTicketsText.Text = db.SupportTickets
                .Count(t => t.UserId == user.UserId && t.Status == "Open")
                .ToString();
        }

        private void LoadUpcomingReservation(User user)
        {
            using var db = new AppDbContext();

            var customer = GetCurrentCustomer(db, user);

            if (customer == null)
            {
                ShowNoUpcomingReservation();
                return;
            }

            var upcomingReservation = db.Reservations
                .Include(r => r.Car)
                .Where(r =>
                    r.CustomerId == customer.CustomerId &&
                    r.Status != ReservationStatus.Cancelled &&
                    r.EndDate >= DateTime.Now)
                .OrderBy(r => r.StartDate)
                .FirstOrDefault();

            if (upcomingReservation == null)
            {
                ShowNoUpcomingReservation();
                return;
            }

            UpcomingCarNameText.Text = $"{upcomingReservation.Car.Brand} {upcomingReservation.Car.Model}";
            UpcomingLocationText.Text = upcomingReservation.PickupLocation;
            UpcomingDatesText.Text =
                $"{upcomingReservation.StartDate:dd MMM yyyy, HH:mm} → {upcomingReservation.EndDate:dd MMM yyyy, HH:mm}";
            UpcomingStatusText.Text = $"Status: {upcomingReservation.Status}";

            if (!string.IsNullOrEmpty(upcomingReservation.Car.ImagePath))
            {
                UpcomingCarImage.Source = new BitmapImage(new Uri(upcomingReservation.Car.ImagePath));
            }
        }

        private void ShowNoUpcomingReservation()
        {
            UpcomingCarNameText.Text = "No upcoming reservations";
            UpcomingLocationText.Text = "Explore available cars and reserve your next ride.";
            UpcomingDatesText.Text = "";
            UpcomingStatusText.Text = "";
            UpcomingCarImage.Source = null;
        }

        private void LoadRecentActivity(User user)
        {
            using var db = new AppDbContext();

            var latestFavorite = db.FavoriteCars
                .Include(f => f.Car)
                .Where(f => f.UserId == user.UserId)
                .OrderByDescending(f => f.Id)
                .FirstOrDefault();

            var customer = GetCurrentCustomer(db, user);

            var latestReservation = customer == null
                ? null
                : db.Reservations
                    .Include(r => r.Car)
                    .Where(r => r.CustomerId == customer.CustomerId)
                    .OrderByDescending(r => r.ReservationId)
                    .FirstOrDefault();

            string activity = "";

            if (latestFavorite?.Car != null)
            {
                activity += $"• Added {latestFavorite.Car.Brand} {latestFavorite.Car.Model} to favorites\n";
            }

            if (latestReservation?.Car != null)
            {
                activity += $"• Reserved {latestReservation.Car.Brand} {latestReservation.Car.Model}\n";
            }

            RecentActivityText.Text = string.IsNullOrWhiteSpace(activity)
                ? "No recent activity yet."
                : activity.Trim();
        }
    }
}