using CarRental.Backend.Data;
using CarRental.Backend.Models;
using CarRental.Backend.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;

namespace Project.Views.Dashboard.Customer
{
    public sealed partial class ReservationDetailsPage : Page
    {
        private Reservation _reservation;

        public ReservationDetailsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int reservationId = (int)e.Parameter;

            using var db = new AppDbContext();
            var reservationService = new ReservationService(db);

            _reservation = reservationService.GetReservationById(reservationId);

            if (_reservation == null)
                return;

            LoadReservationDetails();
        }

        private void LoadReservationDetails()
        {
            CarImage.Source = new BitmapImage(new Uri(_reservation.Car.ImagePath));

            CarNameText.Text = $"{_reservation.Car.Brand} {_reservation.Car.Model}";
            CarPlateText.Text = $"Plate: {_reservation.Car.PlateNumber}";
            CarPriceText.Text = $"€ {_reservation.Car.PricePerDay:0.00} / day";

            ReservationIdText.Text = $"Reservation #{_reservation.ReservationId}";
            StatusText.Text = _reservation.Status.ToString();

            PickupLocationText.Text = _reservation.PickupLocation;
            PickupDateText.Text = $"Pick-up: {_reservation.StartDate:dd MMM yyyy, HH:mm}";
            ReturnDateText.Text = $"Return: {_reservation.EndDate:dd MMM yyyy, HH:mm}";

            TimeSpan duration = _reservation.EndDate - _reservation.StartDate;

            if (duration.TotalHours < 24)
            {
                DurationText.Text = $"{Math.Ceiling(duration.TotalHours)} hours";
            }
            else
            {
                DurationText.Text = $"{Math.Ceiling(duration.TotalDays)} days";
            }

            TotalPriceText.Text = $"€ {_reservation.TotalCost:0.00}";

            if (_reservation.Status == ReservationStatus.Cancelled ||
                _reservation.Status == ReservationStatus.Completed)
            {
                CancelButton.Visibility = Visibility.Collapsed;
            }

            UpdateStatusBadge();
        }

        private void UpdateStatusBadge()
        {
            if (_reservation.Status == ReservationStatus.Pending)
            {
                StatusBadge.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 139, 92, 246));
            }
            else if (_reservation.Status == ReservationStatus.Confirmed)
            {
                StatusBadge.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 34, 197, 94));
            }
            else if (_reservation.Status == ReservationStatus.Cancelled)
            {
                StatusBadge.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 220, 38, 38));
            }
            else
            {
                StatusBadge.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 100, 116, 139));
            }
        }

        private void CancelReservation_Click(object sender, RoutedEventArgs e)
        {
            if (_reservation == null)
                return;

            using var db = new AppDbContext();
            var reservationService = new ReservationService(db);

            reservationService.CancelReservation(_reservation.ReservationId);

            Frame.Navigate(typeof(Project.Views.Dashboard.ReservationsPage));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}