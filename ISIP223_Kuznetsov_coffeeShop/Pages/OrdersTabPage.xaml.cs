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
using System.Data.Entity;

namespace ISIP223_Kuznetsov_coffeeShop.Pages
{
    public partial class OrdersTabPage : Page
    {
        public OrdersTabPage()
        {
            InitializeComponent();
            LoadData();
            this.IsVisibleChanged += (s, e) => { if ((bool)e.NewValue) LoadData(); };
        }

        private void LoadData()
        {
            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                DataGridOrders.ItemsSource = db.Orders
                    .Include(o => o.Cafes)
                    .Include(o => o.Users)
                    .Include(o => o.Products)
                    .OrderByDescending(o => o.Order_Date) 
                    .ToList();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditOrderPage(null));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var order = (sender as Button).DataContext as Orders;
            NavigationService.Navigate(new AddEditOrderPage(order));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var ordersToDelete = DataGridOrders.SelectedItems.Cast<Orders>().ToList();
            if (ordersToDelete.Count == 0)
            {
                MessageBox.Show("Выберите заказы для удаления!");
                return;
            }

            if (MessageBox.Show($"Удалить {ordersToDelete.Count} заказов?", "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var db = new DB_Kuznetsov_coffeeShopEntities())
                {
                    foreach (var o in ordersToDelete)
                    {
                        var orderInDb = db.Orders.Find(o.Order_Id);
                        if (orderInDb != null) db.Orders.Remove(orderInDb);
                    }
                    db.SaveChanges();
                }
                LoadData();
            }
        }
    }
}