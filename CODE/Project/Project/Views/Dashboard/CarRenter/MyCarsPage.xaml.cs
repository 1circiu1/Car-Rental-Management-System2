using CarRental.Backend.Data;
using CarRental.Backend.Models;
using CarRental.Backend.Services;
using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Linq;
using System.IO;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Project.Views.Dashboard.CarRenter
{
    public sealed partial class MyCarsPage : Page
    {
        public MyCarsPage()
        {
            InitializeComponent();
            LoadCars();
        }

        private void LoadCars()
        {
            if (!SessionManager.IsLoggedIn)
                return;

            using var db = new AppDbContext();
            var carService = new CarService(db);

            int currentUserId = SessionManager.CurrentUser.UserId;

            var cars = carService.GetCarsByOwner(currentUserId);

            TotalCarsText.Text = cars.Count.ToString();
            AvailableCarsText.Text = cars.Count(c => c.Status == CarStatus.Available).ToString();
            RentedCarsText.Text = cars.Count(c => c.Status == CarStatus.Rented).ToString();

            CarsListPanel.Children.Clear();

            foreach (var car in cars)
            {
                CarsListPanel.Children.Add(CreateCarCard(car));
            }
        }

        private Border CreateCarCard(Car car)
        {
            var statusText = car.Status.ToString();

            var statusBackground = car.Status switch
            {
                CarStatus.Available => new SolidColorBrush(Windows.UI.Color.FromArgb(34, 52, 199, 89)),
                CarStatus.Rented => new SolidColorBrush(Windows.UI.Color.FromArgb(34, 255, 149, 0)),
                CarStatus.Maintenance => new SolidColorBrush(Windows.UI.Color.FromArgb(34, 255, 59, 48)),
                _ => new SolidColorBrush(Windows.UI.Color.FromArgb(34, 120, 120, 120))
            };

            var statusForeground = car.Status switch
            {
                CarStatus.Available => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 52, 199, 89)),
                CarStatus.Rented => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 149, 0)),
                CarStatus.Maintenance => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 59, 48)),
                _ => new SolidColorBrush(Windows.UI.Color.FromArgb(255, 120, 120, 120))
            };

            var card = new Border
            {
                Padding = new Thickness(16),
                CornerRadius = new CornerRadius(10),
                BorderThickness = new Thickness(1),
                BorderBrush = (Brush)Application.Current.Resources["CardStrokeColorDefaultBrush"]
            };

            var grid = new Grid
            {
                ColumnSpacing = 16
            };

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            FrameworkElement imageContent;

            if (!string.IsNullOrWhiteSpace(car.ImagePath))
            {
                imageContent = new Image
                {
                    Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(new Uri(car.ImagePath)),
                    Stretch = Stretch.UniformToFill
                };
            }
            else
            {
                imageContent = new FontIcon
                {
                    Glyph = "\uE804",
                    FontSize = 28,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
            }

            var iconBox = new Border
            {
                Width = 72,
                Height = 72,
                CornerRadius = new CornerRadius(10),
                Background = (Brush)Application.Current.Resources["CardBackgroundFillColorSecondaryBrush"],
                Child = imageContent
            };
        

            Grid.SetColumn(iconBox, 0);

            var infoPanel = new StackPanel
            {
                Spacing = 6
            };

            var titleRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8
            };

            titleRow.Children.Add(new TextBlock
            {
                Text = $"{car.Brand} {car.Model}",
                FontSize = 17,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });

            titleRow.Children.Add(new Border
            {
                Padding = new Thickness(8, 3, 8, 3),
                CornerRadius = new CornerRadius(10),
                Background = statusBackground,
                Child = new TextBlock
                {
                    Text = statusText,
                    FontSize = 11,
                    Foreground = statusForeground
                }
            });

            infoPanel.Children.Add(titleRow);

            infoPanel.Children.Add(new TextBlock
            {
                Text = $"{car.Year} - {car.PlateNumber}",
                FontSize = 13,
                Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            });

            infoPanel.Children.Add(new TextBlock
            {
                Text = $"${car.PricePerDay} / day",
                FontSize = 14,
                FontWeight = Microsoft.UI.Text.FontWeights.SemiBold
            });

            Grid.SetColumn(infoPanel, 1);

            var actionsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Spacing = 8,
                VerticalAlignment = VerticalAlignment.Center
            };

            var editButton = new Button
            {
                Content = "Edit",
                Tag = car.CarId
            };
            editButton.Click += EditCar_Click;

            var maintenanceButton = new Button
            {
                Content = car.Status == CarStatus.Maintenance ? "Available" : "Maintenance",
                Tag = car.CarId
            };

            maintenanceButton.Click += ToggleMaintenance_Click;

            var removeButton = new Button
            {
                Content = "Remove",
                Tag = car.CarId
            };

            removeButton.Click += RemoveCar_Click;

            actionsPanel.Children.Add(editButton);
            actionsPanel.Children.Add(maintenanceButton);
            actionsPanel.Children.Add(removeButton);

            Grid.SetColumn(actionsPanel, 2);

            grid.Children.Add(iconBox);
            grid.Children.Add(infoPanel);
            grid.Children.Add(actionsPanel);

            card.Child = grid;

            return card;
        }

        private async void EditCar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            int carId = (int)button.Tag;

            using var db = new AppDbContext();
            var carService = new CarService(db);

            var car = carService.GetCarById(carId);

            if (car == null)
                return;

            string selectedImagePath = car.ImagePath ?? "";

            var brandBox = new TextBox
            {
                Header = "Brand",
                Text = car.Brand
            };

            var modelBox = new TextBox
            {
                Header = "Model",
                Text = car.Model
            };

            var yearBox = new TextBox
            {
                Header = "Year",
                Text = car.Year.ToString()
            };

            var plateBox = new TextBox
            {
                Header = "Plate Number",
                Text = car.PlateNumber
            };

            var priceBox = new TextBox
            {
                Header = "Price per day",
                Text = car.PricePerDay.ToString()
            };

            var imageText = new TextBlock
            {
                Text = string.IsNullOrWhiteSpace(selectedImagePath)
                    ? "No image selected"
                    : Path.GetFileName(selectedImagePath),
                FontSize = 12,
                Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            };

            var selectImageButton = new Button
            {
                Content = "Change Car Image"
            };

            selectImageButton.Click += async (s, args) =>
            {
                var picker = new FileOpenPicker();

                var hwnd = WindowNative.GetWindowHandle(MainWindow.Current);
                InitializeWithWindow.Initialize(picker, hwnd);

                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");

                var file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    selectedImagePath = file.Path;
                    imageText.Text = Path.GetFileName(file.Path);
                }
            };

            var statusBox = new ComboBox
            {
                Header = "Status",
                HorizontalAlignment = HorizontalAlignment.Stretch
            };

            statusBox.Items.Add(CarStatus.Available);
            statusBox.Items.Add(CarStatus.Rented);
            statusBox.Items.Add(CarStatus.Maintenance);
            statusBox.SelectedItem = car.Status;

            var form = new StackPanel
            {
                Spacing = 12
            };

            form.Children.Add(brandBox);
            form.Children.Add(modelBox);
            form.Children.Add(yearBox);
            form.Children.Add(plateBox);
            form.Children.Add(priceBox);
            form.Children.Add(statusBox);
            form.Children.Add(selectImageButton);
            form.Children.Add(imageText);

            var dialog = new ContentDialog
            {
                Title = "Edit Car",
                Content = form,
                PrimaryButtonText = "Save Changes",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
                return;

            if (!int.TryParse(yearBox.Text, out int year))
                return;

            if (!decimal.TryParse(priceBox.Text, out decimal price))
                return;

            car.Brand = brandBox.Text;
            car.Model = modelBox.Text;
            car.Year = year;
            car.PlateNumber = plateBox.Text;
            car.PricePerDay = price;
            car.ImagePath = selectedImagePath;

            if (statusBox.SelectedItem is CarStatus selectedStatus)
            {
                car.Status = selectedStatus;
            }

            carService.UpdateCar(car);

            LoadCars();
        }

        private async void RemoveCar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            int carId = (int)button.Tag;

            var dialog = new ContentDialog
            {
                Title = "Remove car",
                Content = "Are you sure you want to remove this car?",
                PrimaryButtonText = "Remove",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = this.XamlRoot
            };

            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
                return;

            using var db = new AppDbContext();
            var carService = new CarService(db);

            carService.DeleteCar(carId);

            LoadCars();
        }

        private void ToggleMaintenance_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            int carId = (int)button.Tag;

            using var db = new AppDbContext();
            var carService = new CarService(db);

            carService.ToggleMaintenance(carId);

            LoadCars();
        }

        private async void AddCar_Click(object sender, RoutedEventArgs e)
        {
            string selectedImagePath = "";

            var brandBox = new TextBox { Header = "Brand", PlaceholderText = "BMW, Audi, Tesla..." };
            var modelBox = new TextBox { Header = "Model", PlaceholderText = "3 Series, A4, Model 3..." };
            var yearBox = new TextBox { Header = "Year", PlaceholderText = "2021" };
            var plateBox = new TextBox { Header = "Plate Number", PlaceholderText = "CJ-10-ABC" };
            var priceBox = new TextBox { Header = "Price per day", PlaceholderText = "65" };

            var imageText = new TextBlock
            {
                Text = "No image selected",
                FontSize = 12,
                Foreground = (Brush)Application.Current.Resources["TextFillColorSecondaryBrush"]
            };

            var selectImageButton = new Button
            {
                Content = "Select Car Image"
            };

            selectImageButton.Click += async (s, args) =>
            {
                var picker = new FileOpenPicker();

                var hwnd = WindowNative.GetWindowHandle(MainWindow.Current);
                InitializeWithWindow.Initialize(picker, hwnd);

                picker.FileTypeFilter.Add(".jpg");
                picker.FileTypeFilter.Add(".jpeg");
                picker.FileTypeFilter.Add(".png");

                var file = await picker.PickSingleFileAsync();

                if (file != null)
                {
                    selectedImagePath = file.Path;
                    imageText.Text = Path.GetFileName(file.Path);
                }
            };

            var form = new StackPanel
            {
                Spacing = 12
            };

            form.Children.Add(brandBox);
            form.Children.Add(modelBox);
            form.Children.Add(yearBox);
            form.Children.Add(plateBox);
            form.Children.Add(priceBox);
            form.Children.Add(selectImageButton);
            form.Children.Add(imageText);

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

            if (result != ContentDialogResult.Primary)
                return;

            if (!int.TryParse(yearBox.Text, out int year))
                return;

            if (!decimal.TryParse(priceBox.Text, out decimal price))
                return;

            using var db = new AppDbContext();
            var carService = new CarService(db);

            var car = new Car
            {
                Brand = brandBox.Text,
                Model = modelBox.Text,
                Year = year,
                PlateNumber = plateBox.Text,
                PricePerDay = price,
                ImagePath = selectedImagePath,
                Status = CarStatus.Available,
                UserId = SessionManager.CurrentUser.UserId
            };

            carService.AddCar(car);

            LoadCars();
        }
    }
}
