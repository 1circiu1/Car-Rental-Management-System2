using Microsoft.UI.Xaml.Controls;
using System.Linq;
using CarRental.Backend.Services;
using CarRental.Backend.Data;

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
                LoadDashboardStats(user.UserId);
            }
            else
            {
                WelcomeText.Text = "Welcome back!";
                WelcomeSubtitle.Text = "Welcome to DriveEase.";
            }
        }

        private void LoadWelcomeMessage(CarRental.Backend.Models.User user)
        {
            switch (user.Role)
            {
                case "Admin":
                    WelcomeText.Text = $"Welcome back, Admin {user.FirstName}!";
                    WelcomeSubtitle.Text = "Monitor platform activity, manage users, and oversee reservations across DriveEase.";
                    break;

                case "Renter":
                    WelcomeText.Text = $"Welcome back, {user.FirstName}! Ready to manage your fleet?";
                    WelcomeSubtitle.Text = "Manage your listed vehicles, track reservations, and grow your rental business with DriveEase.";
                    break;

                default:
                    WelcomeText.Text = $"Welcome back, {user.FirstName} {user.LastName}!";
                    WelcomeSubtitle.Text = "Manage your reservations, explore available cars, save your favorite vehicles, and get support anytime with DriveEase.";
                    break;
            }
        }

        private void LoadDashboardStats(int userId)
        {
            using var db = new AppDbContext();

            var userReservations = db.Reservations
                .Where(r => r.CustomerId == userId);

            ReservationsCountText.Text = userReservations
                .Count()
                .ToString();

            FavoritesCountText.Text = db.FavoriteCars
                .Count(f => f.UserId == userId)
                .ToString();

            SupportTicketsText.Text = db.SupportTickets
                .Count(t => t.UserId == userId && t.Status == "Open")
                .ToString();

            var totalSpent = userReservations
                .Sum(r => r.TotalCost);

            TotalSpentText.Text = $"${totalSpent:0.00}";
        }
    }
}