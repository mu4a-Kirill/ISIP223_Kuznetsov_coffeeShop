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

    public partial class MyOrdersPage : Page
    {
        private Users _currentUser;

        public MyOrdersPage(Users user)
        {
            InitializeComponent();
            _currentUser = user;
            LoadOrders();
        }

        private void LoadOrders()
        {
            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                var myOrders = db.Orders
                                 .Where(o => o.User_Id == _currentUser.User_Id)
                                 .ToList();

                DataGridOrders.ItemsSource = myOrders;
            }
        }
    }
}






