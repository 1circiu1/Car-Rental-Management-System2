using CarRental.Backend.Data;
using CarRental.Backend.Models;
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

            _car = db.Cars.FirstOrDefault(c => c.CarId == carId);

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

            List<string> unavailablePeriods = db.Reservations
                .Where(r =>
                    r.CarId == _car.CarId &&
                    r.Status != ReservationStatus.Cancelled &&
                    r.EndDate >= DateTime.Now)
                .OrderBy(r => r.StartDate)
                .Select(r =>
                    $"{r.StartDate:dd MMM yyyy, HH:mm} → {r.EndDate:dd MMM yyyy, HH:mm}")
                .ToList();

            if (unavailablePeriods.Count == 0)
            {
                unavailablePeriods.Add("No unavailable periods.");
            }

            UnavailablePeriodsList.ItemsSource = unavailablePeriods;
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

            TimeSpan duration = end.Value - start.Value;
            double totalHours = duration.TotalHours;

            decimal totalCost;

            if (totalHours < 24)
            {
                decimal hourlyRate = _car.PricePerDay / 10;
                totalCost = Math.Ceiling((decimal)totalHours) * hourlyRate;

                DurationText.Text = $"Duration: {Math.Ceiling(totalHours)} hours";
            }
            else
            {
                decimal totalDays = Math.Ceiling((decimal)duration.TotalDays);
                totalCost = totalDays * _car.PricePerDay;

                DurationText.Text = $"Duration: {totalDays} days";
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

            bool isBusy = db.Reservations.Any(r =>
                r.CarId == _car.CarId &&
                r.Status != ReservationStatus.Cancelled &&
                start.Value < r.EndDate &&
                end.Value > r.StartDate);

            if (isBusy)
            {
                ErrorText.Text = "This car is already reserved during the selected period. Please choose another time.";
                return;
            }

            var customer = db.Customers.FirstOrDefault(c =>
                c.Email == SessionManager.CurrentUser.Email);

            if (customer == null)
            {
                ErrorText.Text = "Customer profile was not found.";
                return;
            }

            TimeSpan duration = end.Value - start.Value;
            double totalHours = duration.TotalHours;

            decimal totalCost;

            if (totalHours < 24)
            {
                decimal hourlyRate = _car.PricePerDay / 10;
                totalCost = Math.Ceiling((decimal)totalHours) * hourlyRate;
            }
            else
            {
                decimal totalDays = Math.Ceiling((decimal)duration.TotalDays);
                totalCost = totalDays * _car.PricePerDay;
            }

            var reservation = new Reservation
            {
                CarId = _car.CarId,
                CustomerId = customer.CustomerId,
                StartDate = start.Value,
                EndDate = end.Value,
                PickupLocation = pickupLocation,
                TotalCost = totalCost,
                Status = ReservationStatus.Pending
            };

            db.Reservations.Add(reservation);
            db.SaveChanges();

            Frame.Navigate(typeof(Project.Views.Dashboard.ReservationsPage));
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Frame.GoBack();
        }
    }
}