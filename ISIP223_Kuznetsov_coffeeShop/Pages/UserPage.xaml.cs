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
    public partial class UserPage : Page
    {
        private Users _currentUser;

        public UserPage(Users user)
        {
            InitializeComponent();
            _currentUser = user; 

            UpdateProducts();
        }

        private void UpdateProducts()
        {
            if (!IsInitialized) return;

            List<Products> allProducts;
            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                allProducts = db.Products.ToList();
            }

            if (!string.IsNullOrWhiteSpace(TbSearch.Text))
            {
                allProducts = allProducts.Where(p => p.Name.ToLower().Contains(TbSearch.Text.ToLower())).ToList();
            }

            if (CbInStock.IsChecked == true)
            {
                allProducts = allProducts.Where(p => p.StockQuantity > 0).ToList();
            }

            if (CmbSort.SelectedIndex == 0) 
            {
                allProducts = allProducts.OrderBy(p => p.Cost).ToList();
            }
            else 
            {
                allProducts = allProducts.OrderByDescending(p => p.Cost).ToList();
            }

            ListProducts.ItemsSource = allProducts;
        }
        private void BtnMyOrders_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MyOrdersPage(_currentUser));
        }
        private void TbSearch_TextChanged(object sender, TextChangedEventArgs e) => UpdateProducts();
        private void CmbSort_SelectionChanged(object sender, SelectionChangedEventArgs e) => UpdateProducts();
        private void CbInStock_Checked(object sender, RoutedEventArgs e) => UpdateProducts();
        private void CbInStock_Unchecked(object sender, RoutedEventArgs e) => UpdateProducts();

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            TbSearch.Text = "";
            CmbSort.SelectedIndex = 0;
            CbInStock.IsChecked = false;
        }

    }
}