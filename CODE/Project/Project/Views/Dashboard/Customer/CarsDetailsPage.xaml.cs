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

        public CarDetailsPage()
        {
            this.InitializeComponent();
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
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }

        private void Favorite_Click(object sender, RoutedEventArgs e)
        {
            BtnFavorite.Content = "♥ Favorited";
        }
    }
}