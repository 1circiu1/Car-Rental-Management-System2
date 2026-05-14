using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using CarRental.Backend.Models;
using CarRental.Backend.Services;
using Project.Views.Dashboard.Customer;
using Project.Views.Dashboard.CarRenter;

namespace Project.Views.Auth
{
    public sealed partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            TxtError.Visibility = Visibility.Collapsed;

            var firstName = TxtFirstName.Text.Trim();
            var lastName = TxtLastName.Text.Trim();
            var email = TxtEmail.Text.Trim();
            var phone = TxtPhone.Text.Trim();
            var password = TxtPassword.Password;

            if (string.IsNullOrEmpty(firstName) ||
                string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password))
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

            if (RoleComboBox.SelectedItem is not ComboBoxItem selectedRoleItem)
            {
                TxtError.Text = "Please choose a role.";
                TxtError.Visibility = Visibility.Visible;
                return;
            }

            string selectedRole = selectedRoleItem.Content.ToString();

            UserService userService = new UserService();

            User newUser = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Username = email,
                Password = password,
                Role = selectedRole
            };

            userService.Register(newUser, phone);



            SessionManager.CurrentUser = newUser;

            if (selectedRole == "Customer")
            {
                MainWindow.Current.Navigate(typeof(CustomerDashboardPage));
            }
            else if (selectedRole == "CarRenter")
            {
                MainWindow.Current.Navigate(typeof(CarRenterDashboardPage));
            }
        }

        private void GoToSignIn_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Current.Navigate(typeof(SignInPage));
        }
    }
}