using CarRental.Backend.Data;
using CarRental.Backend.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Storage.Pickers;

namespace Project.Views.Dashboard
{
    public sealed partial class SupportPage : Page
    {
        private string _selectedCategory = "Other";
        private string _attachedFileName = "";

        public SupportPage()
        {
            InitializeComponent();
            LoadSupportStats();
            LoadRecentTickets();
        }

        private void CategoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button || button.Tag == null)
                return;

            _selectedCategory = button.Tag.ToString();

            SelectedCategoryText.Text = $"Selected category: {_selectedCategory}";

            ResetCategoryButtons();
            button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 139, 92, 246));
            button.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 196, 181, 253));
        }

        private void ResetCategoryButtons()
        {
            List<Button> buttons = new()
            {
                BtnReservation,
                BtnPayment,
                BtnVehicle,
                BtnLateReturn,
                BtnAccount,
                BtnOther
            };

            foreach (var button in buttons)
            {
                button.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 30, 41, 59));
                button.BorderBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 59, 59, 92));
            }
        }

        private async void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker picker = new FileOpenPicker();

            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.Current);
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hwnd);

            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");

            var file = await picker.PickSingleFileAsync();

            if (file != null)
            {
                _attachedFileName = file.Name;
                AttachedFileText.Text = $"Attached: {_attachedFileName}";
            }
        }

        private void SubmitTicket_Click(object sender, RoutedEventArgs e)
        {
            StatusText.Text = "";

            string subject = SubjectTextBox.Text.Trim();
            string message = MessageTextBox.Text.Trim();

            if (SessionManager.CurrentUser == null)
            {
                StatusText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 248, 113, 113));
                StatusText.Text = "You must be signed in to submit a ticket.";
                return;
            }

            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
            {
                StatusText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 248, 113, 113));
                StatusText.Text = "Please complete the subject and message.";
                return;
            }

            if (!string.IsNullOrEmpty(_attachedFileName))
            {
                message += $"\n\nAttached file: {_attachedFileName}";
            }

            using var db = new AppDbContext();

            var ticket = new SupportTicket
            {
                UserId = SessionManager.CurrentUser.UserId,
                Subject = subject,
                Category = _selectedCategory,
                Message = message,
                Status = "Open",
                CreatedAt = DateTime.Now
            };

            db.SupportTickets.Add(ticket);
            db.SaveChanges();

            SubjectTextBox.Text = "";
            MessageTextBox.Text = "";
            _selectedCategory = "Other";
            SelectedCategoryText.Text = "Selected category: Other";
            _attachedFileName = "";
            AttachedFileText.Text = "No file attached";

            ResetCategoryButtons();

            StatusText.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 52, 211, 153));
            StatusText.Text = "Ticket submitted successfully.";

            LoadSupportStats();
            LoadRecentTickets();
        }

        private void LoadSupportStats()
        {
            if (SessionManager.CurrentUser == null)
            {
                ActiveTicketsText.Text = "0";
                return;
            }

            using var db = new AppDbContext();

            int activeTickets = db.SupportTickets.Count(t =>
                t.UserId == SessionManager.CurrentUser.UserId &&
                t.Status == "Open");

            ActiveTicketsText.Text = activeTickets.ToString();
        }

        private void LoadRecentTickets()
        {
            if (SessionManager.CurrentUser == null)
            {
                RecentTicketsList.ItemsSource = new List<SupportTicket>();
                return;
            }

            using var db = new AppDbContext();

            var tickets = db.SupportTickets
                .Where(t => t.UserId == SessionManager.CurrentUser.UserId)
                .OrderByDescending(t => t.CreatedAt)
                .Take(5)
                .ToList();

            RecentTicketsList.ItemsSource = tickets;
        }
    }
}