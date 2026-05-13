using CarRental.Backend.Data;
using CarRental.Backend.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Linq;

namespace Project.Views.Dashboard.Customer
{
    public sealed partial class CarDetailsPage : Page
    {
        private Car _car;
        private bool _isFavorite;

        public CarDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int carId = (int)e.Parameter;

            using var db = new AppDbContext();

            _car = db.Cars.FirstOrDefault(c => c.CarId == carId);

            if (_car == null)
                return;

            CarImage.Source = new BitmapImage(new Uri(_car.ImagePath));

            CarTitle.Text = _car.Brand;
            CarModel.Text = _car.Model;
            CarYear.Text = $"Year: {_car.Year}";
            CarPlate.Text = $"Plate number: {_car.PlateNumber}";
            CarStatus.Text = $"Status: {(_car.Status == 0 ? "Available" : "Unavailable")}";
            CarPrice.Text = $"€ {_car.PricePerDay} / day";

            LoadFavoriteStatus();
        }

        private void LoadFavoriteStatus()
        {
            if (SessionManager.CurrentUser == null || _car == null)
                return;

            using var db = new AppDbContext();

            int userId = SessionManager.CurrentUser.UserId;

            _isFavorite = db.FavoriteCars.Any(f =>
                f.UserId == userId &&
                f.CarId == _car.CarId);

            UpdateFavoriteButton();
        }

        private void Favorite_Click(object sender, RoutedEventArgs e)
        {
            if (SessionManager.CurrentUser == null || _car == null)
                return;

            using var db = new AppDbContext();

            int userId = SessionManager.CurrentUser.UserId;

            var existingFavorite = db.FavoriteCars.FirstOrDefault(f =>
                f.UserId == userId &&
                f.CarId == _car.CarId);

            if (existingFavorite == null)
            {
                db.FavoriteCars.Add(new FavoriteCars
                {
                    UserId = userId,
                    CarId = _car.CarId
                });

                _isFavorite = true;
            }
            else
            {
                db.FavoriteCars.Remove(existingFavorite);
                _isFavorite = false;
            }

            db.SaveChanges();
            UpdateFavoriteButton();
        }

        private void UpdateFavoriteButton()
        {
            BtnFavorite.Content = _isFavorite ? "♥ Favorite" : "♡ Favorite";
        }

        private void Reserve_Click(object sender, RoutedEventArgs e)
        {
            if (_car == null)
                return;

            Frame.Navigate(typeof(NewReservationPage), _car.CarId);
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}