using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Project.Models;

namespace Project.Views
{
    public sealed partial class SignInPage : Page
    {
        private string _selectedRole = "Customer";

        public SignInPage()
        {
            this.InitializeComponent();
        }

        private void RoleToggle_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            _selectedRole = btn.Tag.ToString();

            ResetRoleButton(BtnCustomerRole);
            ResetRoleButton(BtnRenterRole);
            ActivateRoleButton(btn);
        }

        private void ResetRoleButton(Button btn)
        {
            btn.Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
            btn.BorderThickness = new Thickness(0);
        }

        private void ActivateRoleButton(Button btn)
        {
            btn.Background = (Brush)Application.Current.Resources["CardBackgroundFillColorDefaultBrush"];
            btn.BorderThickness = new Thickness(0.5);
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            TxtError.Visibility = Visibility.Collapsed;

            var email = TxtEmail.Text.Trim();
            var password = TxtPassword.Password;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TxtError.Text = "Please fill in all fields.";
                TxtError.Visibility = Visibility.Visible;
                return;
            }

            // TODO: replace with real DB lookup
            // Admin credentials — works from either tab
            if (email == "admin@driveease.com" && password == "admin123")
            {
                SessionManager.CurrentUser = new User
                {
                    FirstName = "Admin",
                    LastName = "",
                    Username = email,
                    Role = "Admin"
                };
                MainWindow.Current.Navigate(typeof(AdminDashboardPage));
                return;
            }

            // Customer credentials
            if (_selectedRole == "Customer" && email == "ion@driveease.com" && password == "pass123")
            {
                SessionManager.CurrentUser = new User
                {
                    FirstName = "Ion",
                    LastName = "Popescu",
                    Username = email,
                    Role = "Customer"
                };
                MainWindow.Current.Navigate(typeof(DashboardPage));
                return;
            }

            // Car Renter credentials
            if (_selectedRole == "CarRenter" && email == "renter@driveease.com" && password == "rent123")
            {
                SessionManager.CurrentUser = new User
                {
                    FirstName = "Mihai",
                    LastName = "Urs",
                    Username = email,
                    Role = "CarRenter"
                };
                MainWindow.Current.Navigate(typeof(DashboardPage));
                return;
            }

            TxtError.Text = "Invalid email or password.";
            TxtError.Visibility = Visibility.Visible;
        }

        private void GoToSignUp_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.Navigate(typeof(SignUpPage));
        }
    }
}