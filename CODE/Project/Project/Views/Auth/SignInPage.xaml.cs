using CarRental.Backend.Models;
using CarRental.Backend.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Project.Views.Dashboard.Admin;
using Project.Views.Dashboard.CarRenter;
using Project.Views.Dashboard.Customer;

namespace Project.Views.Auth
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
            ResetRoleButton(BtnAdminRole);

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

            UserService userService = new UserService();

            User user = userService.Login(email, password, _selectedRole);

            if (user == null)
            {
                TxtError.Text = "Invalid email or password.";
                TxtError.Visibility = Visibility.Visible;
                return;
            }

            SessionManager.CurrentUser = user;

            if (user.Role == "Admin")
            {
                MainWindow.Current.Navigate(typeof(AdminDashboardPage));
            }
           
            else if (user.Role == "CarRenter")
            {
                MainWindow.Current.Navigate(typeof(CarRenterDashboardPage));
            }
            else
            {
                MainWindow.Current.Navigate(typeof(CustomerDashboardPage));
            }
        }

        private void GoToSignUp_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.Navigate(typeof(SignUpPage));
        }
    }
}