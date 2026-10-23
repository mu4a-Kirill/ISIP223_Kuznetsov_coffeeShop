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
    public partial class UsersTabPage : Page
    {
        public UsersTabPage()
        {
            InitializeComponent();
            LoadData();
            this.IsVisibleChanged += (s, e) => { if ((bool)e.NewValue) LoadData(); };
        }

        private void LoadData()
        {
            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                DataGridUsers.ItemsSource = db.Users.Include("User_Roles").ToList();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditUserPage(null));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var user = (sender as Button).DataContext as Users;
            NavigationService.Navigate(new AddEditUserPage(user));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var usersToDelete = DataGridUsers.SelectedItems.Cast<Users>().ToList();
            if (usersToDelete.Count == 0) return;

            if (MessageBox.Show($"Удалить {usersToDelete.Count} пользователей?", "Внимание", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                using (var db = new DB_Kuznetsov_coffeeShopEntities())
                {
                    foreach (var u in usersToDelete)
                    {
                        var userInDb = db.Users.Find(u.User_Id);
                        if (userInDb != null) db.Users.Remove(userInDb);
                    }
                    db.SaveChanges();
                }
                LoadData();
            }
        }
    }
}