using CarRental.Backend.Data;
using CarRental.Backend.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Project.Views.Dashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SupportPage : Page
    {
        public SupportPage()
        {
            InitializeComponent();
        }

        private void SubmitTicket_Click(object sender, RoutedEventArgs e)
        {
            string subject = SubjectTextBox.Text;
            string message = MessageTextBox.Text;

            var selectedCategory = CategoryComboBox.SelectedItem as ComboBoxItem;
            string category = selectedCategory?.Content?.ToString() ?? "Other";

            if (string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            using var db = new AppDbContext();

            var ticket = new SupportTicket
            {
                UserId = 1,
                Subject = subject,
                Category = category,
                Message = message,
                Status = "Open",
                CreatedAt = DateTime.Now
            };

            db.SupportTickets.Add(ticket);
            db.SaveChanges();

            SubjectTextBox.Text = "";
            MessageTextBox.Text = "";
            CategoryComboBox.SelectedIndex = -1;
        }
    }
}
