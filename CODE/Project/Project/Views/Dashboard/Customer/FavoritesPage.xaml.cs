using CarRental.Backend.Data;
using CarRental.Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using System.Collections.Generic;
using System.Linq;

namespace Project.Views.Dashboard
{
    public sealed partial class FavoritesPage : Page
    {
        public FavoritesPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            LoadFavorites();
        }

        private void LoadFavorites()
        {
            if (SessionManager.CurrentUser == null)
                return;

            using var db = new AppDbContext();

            int userId = SessionManager.CurrentUser.UserId;

            List<Car> favoriteCars = db.FavoriteCars
                .Include(f => f.Car)
                .Where(f => f.UserId == userId)
                .Select(f => f.Car)
                .Where(c => c != null)
                .ToList();

            FavoritesGridView.ItemsSource = favoriteCars;
        }

        private void FavoritesGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is Car car)
            {
                Frame.Navigate(typeof(Customer.CarDetailsPage), car.CarId);
            }
        }
    }
}