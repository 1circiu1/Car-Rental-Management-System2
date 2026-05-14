using CarRental.Backend.Data;
using CarRental.Backend.Models;
using CarRental.Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Views.Dashboard.Customer
{
    public sealed partial class NewReservationPage : Page
    {
        private Car _car;

        public NewReservationPage()
        {
            InitializeComponent();

            var today = DateTimeOffset.Now.Date;

            StartDatePicker.MinDate = today;
            EndDatePicker.MinDate = today;

            StartDatePicker.Date = today;
            EndDatePicker.Date = today;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            int carId = (int)e.Parameter;

            using var db = new AppDbContext();
            var carService = new CarService(db);

            _car = carService.GetCarById(carId);

            if (_car == null)
                return;

            CarImage.Source = new BitmapImage(new Uri(_car.ImagePath));
            CarTitleText.Text = $"{_car.Brand} {_car.Model}";
            CarPriceText.Text = $"€ {_car.PricePerDay:0.00} / day";

            LoadUnavailablePeriods();
            UpdateReservationSummary();
        }

        private void LoadUnavailablePeriods()
        {
            if (_car == null)
                return;

            using var db = new AppDbContext();

            var reservationService = new ReservationService(db);

            var periods = reservationService.GetUnavailablePeriods(_car.CarId);

            if (periods.Count == 0)
            {
                periods.Add("No unavailable periods.");
            }

            UnavailablePeriodsList.ItemsSource = periods;
        }

        private void ReservationDateTime_Changed(object sender, object e)
        {
            if (StartDatePicker == null || EndDatePicker == null)
                return;

            var today = DateTimeOffset.Now.Date;

            if (StartDatePicker.Date < today)
                StartDatePicker.Date = today;

            if (EndDatePicker.Date < StartDatePicker.Date)
                EndDatePicker.Date = StartDatePicker.Date;

            EndDatePicker.MinDate = StartDatePicker.Date ?? today;

            UpdateReservationSummary();
        }

        private DateTime? GetStartDateTime()
        {
            if (StartDatePicker.Date == null)
                return null;

            if (StartTimeBox.SelectedItem is not ComboBoxItem selectedTime)
                return null;

            TimeSpan time = TimeSpan.Parse(selectedTime.Content.ToString());

            return StartDatePicker.Date.Value.Date + time;
        }

        private DateTime? GetEndDateTime()
        {
            if (EndDatePicker.Date == null)
                return null;

            if (EndTimeBox.SelectedItem is not ComboBoxItem selectedTime)
                return null;

            TimeSpan time = TimeSpan.Parse(selectedTime.Content.ToString());

            return EndDatePicker.Date.Value.Date + time;
        }

        private void UpdateReservationSummary()
        {
            if (_car == null)
                return;

            var start = GetStartDateTime();
            var end = GetEndDateTime();

            if (start == null || end == null || end <= start)
            {
                DurationText.Text = "Duration: -";
                TotalPriceText.Text = "€ 0.00";
                return;
            }

            using var db = new AppDbContext();

            var reservationService = new ReservationService(db);

            decimal totalCost = reservationService.CalculateCost(
                _car.CarId,
                start.Value,
                end.Value);

            TimeSpan duration = end.Value - start.Value;

            if (duration.TotalHours < 24)
            {
                DurationText.Text =
                    $"Duration: {Math.Ceiling(duration.TotalHours)} hours";
            }
            else
            {
                DurationText.Text =
                    $"Duration: {Math.Ceiling(duration.TotalDays)} days";
            }

            TotalPriceText.Text = $"€ {totalCost:0.00}";
        }

        private void ConfirmReservation_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Text = "";

            if (_car == null || SessionManager.CurrentUser == null)
                return;

            var start = GetStartDateTime();
            var end = GetEndDateTime();

            if (start == null || end == null)
            {
                ErrorText.Text = "Please select both pick-up and return date.";
                return;
            }

            if (start.Value < DateTime.Now)
            {
                ErrorText.Text = "Pick-up date and time cannot be in the past.";
                return;
            }

            if (end.Value <= start.Value)
            {
                ErrorText.Text = "Return date and time must be after pick-up date and time.";
                return;
            }

            if (PickupLocationBox.SelectedItem is not ComboBoxItem selectedLocation)
            {
                ErrorText.Text = "Please choose a pick-up location.";
                return;
            }

            string pickupLocation = selectedLocation.Content.ToString();

            using var db = new AppDbContext();

            var reservationService = new ReservationService(db);

            bool success = reservationService.CreateReservation(
                _car.CarId,
                SessionManager.CurrentUser.Email,
                start.Value,
                end.Value,
                pickupLocation);

            if (!success)
            {
                ErrorText.Text = "This car is already reserved during the selected period.";
                return;
            }

            Frame.Navigate(typeof(Project.Views.Dashboard.ReservationsPage));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}