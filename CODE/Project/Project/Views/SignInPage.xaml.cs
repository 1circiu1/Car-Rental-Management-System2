using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using CarRental.Backend.Models;
using CarRental.Backend.Services;

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

            UserService userService = new UserService();

            User user = userService.Login(email, password);

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
            else
            {
                MainWindow.Current.Navigate(typeof(DashboardPage));
            }
        }

        private void GoToSignUp_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.Navigate(typeof(SignUpPage));
        }
    }
}