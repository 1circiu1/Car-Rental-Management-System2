using CarRental.Backend.Data;
using CarRental.Backend.Models;
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
using Project.Views.Dashboard.Customer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Project.Views.Dashboard
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CarsPage : Page
    {
        public CarsPage()
        {
            InitializeComponent();
            LoadCars();
        }

        private void LoadCars()
        {
            using var db = new AppDbContext();
            CarsGrid.ItemsSource = db.Cars.ToList();
        }

        private void CarsGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var selectedCar = e.ClickedItem as Car;

            if (selectedCar != null)
            {
                Frame.Navigate(typeof(CarDetailsPage), selectedCar.CarId);
            }
        }
    }
}
