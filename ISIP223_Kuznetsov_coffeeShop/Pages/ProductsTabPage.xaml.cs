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

    public partial class ProductsTabPage : Page
    {
        public ProductsTabPage()
        {
            InitializeComponent();
            LoadData();
            this.IsVisibleChanged += (s, e) => { if ((bool)e.NewValue) LoadData(); };
        }

        private void LoadData()
        {
            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                DataGridProducts.ItemsSource = db.Products.ToList();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditProductPage(null));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var product = (sender as Button).DataContext as Products;
            NavigationService.Navigate(new AddEditProductPage(product));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var productsToDelete = DataGridProducts.SelectedItems.Cast<Products>().ToList();
            if (productsToDelete.Count == 0)
            {
                MessageBox.Show("Выберите товары для удаления!");
                return;
            }

            if (MessageBox.Show($"Удалить {productsToDelete.Count} товаров?\nВсе связанные заказы также будут удалены!",
                "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var db = new DB_Kuznetsov_coffeeShopEntities())
                {
                    foreach (var p in productsToDelete)
                    {
                        var productInDb = db.Products.Find(p.Product_Id);
                        if (productInDb != null) db.Products.Remove(productInDb);
                    }
                    db.SaveChanges();
                }
                LoadData();
            }
        }
    }
}