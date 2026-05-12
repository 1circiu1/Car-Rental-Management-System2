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

namespace Project.Views.Dashboard.CarRenter
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RentalRequestsPage : Page
    {
        public RentalRequestsPage()
        {
            InitializeComponent();
        }
        private async void Details_Click(object sender, RoutedEventArgs e)
        {
            var content = new StackPanel
            {
                Spacing = 12
            };

            content.Children.Add(new TextBlock
            {
                Text = "Customer Information",
                FontSize = 18,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });

            content.Children.Add(new TextBlock
            {
                Text = "Name: Andrei M."
            });

            content.Children.Add(new TextBlock
            {
                Text = "Requested Vehicle: Tesla Model 3"
            });

            content.Children.Add(new TextBlock
            {
                Text = "Rental Period: May 14 - May 17"
            });

            content.Children.Add(new TextBlock
            {
                Text = "Estimated Total: $270"
            });

            content.Children.Add(new TextBlock
            {
                Text = "Pickup Location: Cluj-Napoca"
            });

            content.Children.Add(new TextBlock
            {
                Text = "Additional Notes: Customer requested airport pickup."
            });

            var dialog = new ContentDialog
            {
                Title = "Rental Request Details",
                Content = content,
                CloseButtonText = "Close",
                XamlRoot = this.XamlRoot
            };

            await dialog.ShowAsync();
        }
    }
}

