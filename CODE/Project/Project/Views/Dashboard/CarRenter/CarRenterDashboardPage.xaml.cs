using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using Project.Views.Auth;


namespace Project.Views.Dashboard.CarRenter
{
    public sealed partial class CarRenterDashboardPage : Page
    {
        private Button _activeNavButton;

        public CarRenterDashboardPage()
        {
            this.InitializeComponent();
            this.Loaded += CarRenterDashboardPage_Loaded;
        }

        private void CarRenterDashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserInfo();
            SetActivePage(BtnOverview, typeof(RenterOverviewPage), "Owner Overview", "+ List a car");
        }

        private void LoadUserInfo()
        {
            var user = SessionManager.CurrentUser;
            if (user == null) return;

            SidebarUserName.Text = $"{user.FirstName} {user.LastName}";
            SidebarUserRole.Text = "Car Renter";

            string initials = "";
            if (!string.IsNullOrEmpty(user.FirstName)) initials += user.FirstName[0];
            if (!string.IsNullOrEmpty(user.LastName)) initials += user.LastName[0];

            AvatarInitials.Text = initials.ToUpper();
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            

           

            var btn = sender as Button;
            if (btn == null || btn == _activeNavButton) return; 
            
            System.Diagnostics.Debug.WriteLine($"Clicked: {btn.Tag}");

            switch (btn.Tag?.ToString())
            {
                case "Overview":
                    SetActivePage(btn, typeof(RenterOverviewPage), "Owner Overview", "+ List a car");
                    break;

                case "MyFleet":
                    SetActivePage(btn, typeof(MyCarsPage), "My Fleet", "+ List a car");
                    break;

                case "BookingRequests":
                    SetActivePage(btn, typeof(RentalRequestsPage), "Booking Requests", null);
                    break;

                case "Revenue":
                    SetActivePage(btn, typeof(EarningsPage), "Revenue", null);
                    break;
                case "VehicleMonitoring":
                    SetActivePage(btn, typeof(VehicleMonitoringPage), "Vehicle Monitoring", null);
                    break;
            }
        }

        private void SetActivePage(Button btn, Type pageType, string title, string actionLabel)
        {
            if (_activeNavButton != null)
                SetNavButtonInactive(_activeNavButton);

            _activeNavButton = btn;
            SetNavButtonActive(_activeNavButton);

            PageTitle.Text = title;
            PageSubtitle.Text = DateTime.Now.ToString("dddd, dd MMMM yyyy");

            if (actionLabel != null)
            {
                BtnPrimaryAction.Content = actionLabel;
                BtnPrimaryAction.Visibility = Visibility.Visible;
            }
            else
            {
                BtnPrimaryAction.Visibility = Visibility.Collapsed;
            }

            ContentFrame.Navigate(pageType);
        }

        private void SetNavButtonActive(Button btn)
        {
            btn.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(30, 255, 255, 255));

            var panel = btn.Content as StackPanel;
            if (panel == null) return;

            foreach (var child in panel.Children)
            {
                if (child is TextBlock tb)
                    tb.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));

                if (child is FontIcon fi)
                    fi.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
            }
        }

        private void SetNavButtonInactive(Button btn)
        {
            btn.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));

            var panel = btn.Content as StackPanel;
            if (panel == null) return;

            foreach (var child in panel.Children)
            {
                if (child is TextBlock tb)
                    tb.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(204, 255, 255, 255));

                if (child is FontIcon fi)
                    fi.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(204, 255, 255, 255));
            }
        }

        private void PrimaryAction_Click(object sender, RoutedEventArgs e)
        {
            SetActivePage(BtnMyFleet, typeof(MyCarsPage), "My Fleet", "+ List a car");
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.CurrentUser = null;
            MainWindow.Current.Navigate(typeof(SignInPage));
        }

        public void NavigateToMyFleet()
        {
            SetActivePage(BtnMyFleet, typeof(MyCarsPage), "My Fleet", "+ List a car");
        }

        public void NavigateToBookingRequests()
        {
            SetActivePage(BtnBookingRequests, typeof(RentalRequestsPage), "Booking Requests", null);
        }

        public void NavigateToRevenue()
        {
            SetActivePage(BtnRevenue, typeof(EarningsPage), "Revenue", null);
        }

        public void NavigateToVehicleMonitoring()
        {
            SetActivePage(BtnVehicleStatus, typeof(VehicleMonitoringPage), "Vehicle Monitoring", null);
        }
    }
}