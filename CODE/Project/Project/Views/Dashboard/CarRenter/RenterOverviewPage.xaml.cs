using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Project.Views.Dashboard.CarRenter
{
    public sealed partial class RenterOverviewPage : Page
    {
        public RenterOverviewPage()
        {
            InitializeComponent();
        }

        private CarRenterDashboardPage GetDashboardPage()
        {
            FrameworkElement current = this;

            while (current != null)
            {
                if (current is CarRenterDashboardPage dashboard)
                    return dashboard;

                current = current.Parent as FrameworkElement;
            }

            return null;
        }

        private void GoToMyFleet_Click(object sender, RoutedEventArgs e)
        {
            GetDashboardPage()?.NavigateToMyFleet();
        }

        private void GoToBookingRequests_Click(object sender, RoutedEventArgs e)
        {
            GetDashboardPage()?.NavigateToBookingRequests();
        }

        private void GoToVehicleMonitoring_Click(object sender, RoutedEventArgs e)
        {
            GetDashboardPage()?.NavigateToVehicleMonitoring();
        }

        private void GoToRevenue_Click(object sender, RoutedEventArgs e)
        {
            GetDashboardPage()?.NavigateToRevenue();
        }
    }
}