using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using Project.Views.Auth;


namespace Project.Views.Dashboard.Customer
{
    public sealed partial class CustomerDashboardPage : Page
    {
        private Button _activeNavButton;

        public CustomerDashboardPage()
        {
            this.InitializeComponent();
            this.Loaded += DashboardPage_Loaded;
        }

        private void DashboardPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUserInfo();
            SetActivePage(BtnOverview, typeof(OverviewPage), "Overview", "+ New reservation");
        }

        private void LoadUserInfo()
        {
            var user = SessionManager.CurrentUser;
            if (user == null) return;

            SidebarUserName.Text = $"{user.FirstName} {user.LastName}";
            SidebarUserRole.Text = user.Role;

            string initials = "";
            if (!string.IsNullOrEmpty(user.FirstName)) initials += user.FirstName[0];
            if (!string.IsNullOrEmpty(user.LastName)) initials += user.LastName[0];
            AvatarInitials.Text = initials.ToUpper();

            if (user.Role != "Admin")
            {
                BtnSettings.Visibility = Visibility.Collapsed;
            }
        }

        private void NavButton_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            if (btn == null || btn == _activeNavButton) return;

            switch (btn.Tag?.ToString())
            {
                case "Overview":
                    SetActivePage(btn, typeof(OverviewPage), "Overview", "+ New reservation");
                    break;
                case "Reservations":
                    SetActivePage(btn, typeof(ReservationsPage), "My reservations", "+ New reservation");
                    break;
                case "Cars":
                    SetActivePage(btn, typeof(CarsPage), "Available cars", null);
                    break;
                case "Profile":
                    SetActivePage(btn, typeof(ProfilePage), "My profile", null);
                    break;
                case "Settings":
                    SetActivePage(btn, typeof(SettingsPage), "Settings", null);
                    break;
                case "Favorites":
                    SetActivePage(btn, typeof(FavoritesPage), "Favorites", null);
                    break;
                case "Support":
                    SetActivePage(btn, typeof(SupportPage), "Help & Support", null);
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
                if (child is TextBlock tb) tb.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
                if (child is FontIcon fi) fi.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
            }
        }

        private void SetNavButtonInactive(Button btn)
        {
            btn.Background = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
            var panel = btn.Content as StackPanel;
            if (panel == null) return;
            foreach (var child in panel.Children)
            {
                if (child is TextBlock tb) tb.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(204, 255, 255, 255));
                if (child is FontIcon fi) fi.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(204, 255, 255, 255));
            }
        }

       private void PrimaryAction_Click(object sender, RoutedEventArgs e)
        {
            ContentFrame.Navigate(typeof(NewReservationPage));
        }

        private void SignOut_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.CurrentUser = null;
            MainWindow.Current.Navigate(typeof(SignInPage));
        }
    }
}