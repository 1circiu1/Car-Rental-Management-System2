using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace Project.Views.Dashboard.CarRenter
{
    public sealed partial class MyCarsPage : Page
    {
        public MyCarsPage()
        {
            InitializeComponent();
        }

        private async void AddCar_Click(object sender, RoutedEventArgs e)
        {
            var brandBox = new TextBox { Header = "Brand", PlaceholderText = "BMW, Audi, Tesla..." };
            var modelBox = new TextBox { Header = "Model", PlaceholderText = "3 Series, A4, Model 3..." };
            var yearBox = new TextBox { Header = "Year", PlaceholderText = "2021" };
            var locationBox = new TextBox { Header = "Location", PlaceholderText = "Cluj-Napoca" };
            var priceBox = new TextBox { Header = "Price per day", PlaceholderText = "65" };

            var transmissionBox = new ComboBox
            {
                Header = "Transmission",
                PlaceholderText = "Select transmission",
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            transmissionBox.Items.Add("Automatic");
            transmissionBox.Items.Add("Manual");
            transmissionBox.Items.Add("Electric");

            var descriptionBox = new TextBox
            {
                Header = "Description",
                PlaceholderText = "Short description about the car...",
                AcceptsReturn = true,
                TextWrapping = TextWrapping.Wrap,
                Height = 90
            };

            var form = new StackPanel
            {
                Spacing = 12
            };

            form.Children.Add(brandBox);
            form.Children.Add(modelBox);
            form.Children.Add(yearBox);
            form.Children.Add(locationBox);
            form.Children.Add(priceBox);
            form.Children.Add(transmissionBox);
            form.Children.Add(descriptionBox);

            var dialog = new ContentDialog
            {
                Title = "Add New Car",
                Content = form,
                PrimaryButtonText = "Add Car",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                // Later: save car to database.
                // For now we only close the dialog.
            }
        }
    }
}