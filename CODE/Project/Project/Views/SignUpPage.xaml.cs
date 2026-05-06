using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CarRental.Backend.Models;
using CarRental.Backend.Services;

namespace Project.Views
{
    public sealed partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            this.InitializeComponent();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            TxtError.Visibility = Visibility.Collapsed;

            var firstName = TxtFirstName.Text.Trim();
            var lastName = TxtLastName.Text.Trim();
            var email = TxtEmail.Text.Trim();
            var phone = TxtPhone.Text.Trim();
            var password = TxtPassword.Password;

            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                TxtError.Text = "Please fill in all required fields.";
                TxtError.Visibility = Visibility.Visible;
                return;
            }

            if (password.Length < 8)
            {
                TxtError.Text = "Password must be at least 8 characters.";
                TxtError.Visibility = Visibility.Visible;
                return;
            }

            UserService userService = new UserService();

            User newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Username = email,
                Password = password,
                Role = "Customer"
            };

            userService.Register(newUser);

            SessionManager.CurrentUser = newUser;

            MainWindow.Current.Navigate(typeof(DashboardPage));
        }

        private void GoToSignIn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.Navigate(typeof(SignInPage));
        }
    }
}