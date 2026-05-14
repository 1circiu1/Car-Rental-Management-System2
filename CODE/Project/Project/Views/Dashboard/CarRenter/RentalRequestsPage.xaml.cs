using CarRental.Backend.Data;
using CarRental.Backend.Models;
using CarRental.Backend.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Project.Views.Dashboard.CarRenter
{
    public sealed partial class RentalRequestsPage : Page
    {
        private List<Reservation> _reservations = new();

        public RentalRequestsPage()
        {
            InitializeComponent();
            LoadRequests();
        }

        private void LoadRequests()
        {
            if (!SessionManager.IsLoggedIn)
                return;

            using var db = new AppDbContext();
            var reservationService = new ReservationService(db);

            int ownerUserId = SessionManager.CurrentUser.UserId;

            _reservations = reservationService.GetOwnerReservations(ownerUserId);

            PendingRequestsText.Text = reservationService
                .CountOwnerReservationsByStatus(ownerUserId, ReservationStatus.Pending)
                .ToString();

            ApprovedRequestsText.Text = reservationService
                .CountOwnerReservationsByStatus(ownerUserId, ReservationStatus.Confirmed)
                .ToString();

            RejectedRequestsText.Text = reservationService
                .CountOwnerReservationsByStatus(ownerUserId, ReservationStatus.Cancelled)
                .ToString();

            DisplayRequests();
        }

        private void DisplayRequests()
        {
            if (RequestsListPanel == null)
                return;

            RequestsListPanel.Children.Clear();

            string selectedFilter = GetSelectedFilter();

            var filteredRequests = _reservations;

            if (selectedFilter != "All requests")
            {
                filteredRequests = _reservations
                    .Where(r => r.Status.ToString() == selectedFilter)
                    .ToList();
            }

            if (filteredRequests.Count == 0)
            {
                RequestsListPanel.Children.Add(new TextBlock
                {
                    Text = "No rental requests found.",
                    Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"],
                    FontSize = 14
                });

                return;
            }

            foreach (var reservation in filteredRequests)
            {
                RequestsListPanel.Children.Add(CreateRequestCard(reservation));
            }
        }

        private string GetSelectedFilter()
        {
            if (RequestsFilterBox.SelectedItem is ComboBoxItem item &&
                item.Content != null)
            {
                return item.Content.ToString();
            }

            return "All requests";
        }

        private Border CreateRequestCard(Reservation reservation)
        {
            string customerName = reservation.Customer == null
                ? "Unknown customer"
                : $"{reservation.Customer.FirstName} {reservation.Customer.LastName}";

            string initials = GetInitials(customerName);

            var statusBackground = GetStatusBackground(reservation.Status);
            var statusForeground = GetStatusForeground(reservation.Status);

            var card = new Border
            {
                Padding = new Thickness(16),
                CornerRadius = new CornerRadius(10),
                BorderBrush = (Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"],
                BorderThickness = new Thickness(1)
            };

            var grid = new Grid
            {
                ColumnSpacing = 16
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var avatar = new Border
            {
                Width = 48,
                Height = 48,
                CornerRadius = new CornerRadius(24),
                Background = new SolidColorBrush(Windows.UI.Color.FromArgb(34, 110, 168, 254)),
                Child = new TextBlock
                {
                    Text = initials,
                    FontSize = 14,
                    FontWeight = Microsoft.UI.Text.FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 110, 168, 254)),
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                }
            };

            Grid.SetColumn(avatar, 0);

            var infoPanel = new StackPanel
            {
                Spacing = 6
            };

            var titleRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8
            };

            titleRow.Children.Add(new TextBlock
            {
                Text = customerName,
                FontSize = 17,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });

            titleRow.Children.Add(new Border
            {
                Padding = new Thickness(8, 3, 8, 3),
                CornerRadius = new CornerRadius(10),
                Background = statusBackground,
                Child = new TextBlock
                {
                    Text = reservation.Status.ToString(),
                    FontSize = 11,
                    Foreground = statusForeground
                }
            });

            infoPanel.Children.Add(titleRow);

            infoPanel.Children.Add(new TextBlock
            {
                Text = $"{reservation.Car.Brand} {reservation.Car.Model} : {reservation.StartDate:dd MMM yyyy, HH:mm} - {reservation.EndDate:dd MMM yyyy, HH:mm}",
                FontSize = 13,
                Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            });

            infoPanel.Children.Add(new TextBlock
            {
                Text = $"Estimated total: € {reservation.TotalCost:0.00}",
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });

            Grid.SetColumn(infoPanel, 1);

            var actionsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                VerticalAlignment = VerticalAlignment.Center
            };

            if (reservation.Status == ReservationStatus.Pending)
            {
                var approveButton = new Button
                {
                    Content = "Approve",
                    Tag = reservation.ReservationId
                };
                approveButton.Click += Approve_Click;

                var rejectButton = new Button
                {
                    Content = "Reject",
                    Tag = reservation.ReservationId
                };
                rejectButton.Click += Reject_Click;

                actionsPanel.Children.Add(approveButton);
                actionsPanel.Children.Add(rejectButton);
            }

            var detailsButton = new Button
            {
                Content = "Details",
                Tag = reservation.ReservationId
            };
            detailsButton.Click += Details_Click;

            actionsPanel.Children.Add(detailsButton);

            Grid.SetColumn(actionsPanel, 2);

            grid.Children.Add(avatar);
            grid.Children.Add(infoPanel);
            grid.Children.Add(actionsPanel);

            card.Child = grid;

            return card;
        }

        private string GetInitials(string name)
        {
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return "?";

            if (parts.Length == 1)
                return parts[0][0].ToString().ToUpper();

            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
        }

        private SolidColorBrush GetStatusBackground(ReservationStatus status)
        {
            return status switch
            {
                ReservationStatus.Pending => new SolidColorBrush(Windows.UI.Color.FromArgb(34, 255, 149, 0)),
                ReservationStatus.Confirmed => new SolidColorBrush(Windows.UI.Color.FromArgb(34, 52, 199, 89)),
                ReservationStatus.Cancelled => new SolidColorBrush(Windows.UI.Color.FromArgb(34, 255, 59, 48)),
                ReservationStatus.Completed => new SolidColorBrush(Windows.UI.Color.FromArgb(34, 120, 120, 120)),
                _ => new SolidColorBrush(Windows.UI.Color.FromArgb(34, 120, 120, 120))
            };
        }

        private SolidColorBrush GetStatusForeground(ReservationStatus status)
        {
            return status switch
            {
                ReservationStatus.Pending => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 149, 0)),
                ReservationStatus.Confirmed => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 52, 199, 89)),
                ReservationStatus.Cancelled => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 59, 48)),
                ReservationStatus.Completed => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 120, 120, 120)),
                _ => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 120, 120, 120))
            };
        }

        private async void Details_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            int reservationId = (int)button.Tag;

            var reservation = _reservations
                .FirstOrDefault(r => r.ReservationId == reservationId);

            if (reservation == null)
                return;

            string customerName = reservation.Customer == null
                ? "Unknown customer"
                : $"{reservation.Customer.FirstName} {reservation.Customer.LastName}";

            var content = new StackPanel
            {
                Spacing = 12
            };

            content.Children.Add(new TextBlock
            {
                Text = "Customer Information",
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });

            content.Children.Add(new TextBlock { Text = $"Name: {customerName}" });
            content.Children.Add(new TextBlock { Text = $"Email: {reservation.Customer?.Email ?? "-"}" });
            content.Children.Add(new TextBlock { Text = $"Phone: {reservation.Customer?.Phone ?? "-"}" });
            content.Children.Add(new TextBlock { Text = $"Requested Vehicle: {reservation.Car.Brand} {reservation.Car.Model}" });
            content.Children.Add(new TextBlock { Text = $"Rental Period: {reservation.StartDate:dd MMM yyyy, HH:mm} - {reservation.EndDate:dd MMM yyyy, HH:mm}" });
            content.Children.Add(new TextBlock { Text = $"Estimated Total: € {reservation.TotalCost:0.00}" });
            content.Children.Add(new TextBlock { Text = $"Pickup Location: {reservation.PickupLocation}" });
            content.Children.Add(new TextBlock { Text = $"Status: {reservation.Status}" });

            var dialog = new ContentDialog
            {
                Title = "Rental Request Details",
                Content = content,
                CloseButtonText = "Close",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }

        private void Approve_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            int reservationId = (int)button.Tag;

            using var db = new AppDbContext();
            var reservationService = new ReservationService(db);

            reservationService.ConfirmReservation(reservationId);

            LoadRequests();
        }

        private void Reject_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            int reservationId = (int)button.Tag;

            using var db = new AppDbContext();
            var reservationService = new ReservationService(db);

            reservationService.RejectReservation(reservationId);

            LoadRequests();
        }

        private void RequestsFilterBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DisplayRequests();
        }
    }
}