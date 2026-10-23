using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ISIP223_Kuznetsov_coffeeShop.Pages
{

    public partial class AdminPage : Page
    {
        public AdminPage()
        {
            InitializeComponent();
        }

        private void BtnUsers_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new UsersTabPage());
        private void BtnProducts_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new ProductsTabPage());

        private void BtnOrders_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("потом");
        }

        private void BtnCafes_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("потом");
        }

        private void BtnCharts_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new DiagrammPage());
        }
    }
}