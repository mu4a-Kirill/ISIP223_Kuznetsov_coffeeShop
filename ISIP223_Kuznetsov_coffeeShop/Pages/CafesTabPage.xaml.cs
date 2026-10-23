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
    public partial class CafesTabPage : Page
    {
        public CafesTabPage()
        {
            InitializeComponent();
            LoadData();
            this.IsVisibleChanged += (s, e) => { if ((bool)e.NewValue) LoadData(); };
        }

        private void LoadData()
        {
            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                DataGridCafes.ItemsSource = db.Cafes.ToList();
            }
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AddEditCafePage(null));
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            var cafe = (sender as Button).DataContext as Cafes;
            NavigationService.Navigate(new AddEditCafePage(cafe));
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            var cafesToDelete = DataGridCafes.SelectedItems.Cast<Cafes>().ToList();
            if (cafesToDelete.Count == 0)
            {
                MessageBox.Show("Выберите кофейни для удаления!");
                return;
            }

            if (MessageBox.Show($"Удалить {cafesToDelete.Count} кофеен?\nВсе связанные заказы также будут удалены!",
                "Внимание", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                using (var db = new DB_Kuznetsov_coffeeShopEntities())
                {
                    foreach (var c in cafesToDelete)
                    {
                        var cafeInDb = db.Cafes.Find(c.Cafe_Id);
                        if (cafeInDb != null) db.Cafes.Remove(cafeInDb);
                    }
                    db.SaveChanges();
                }
                LoadData();
            }
        }
    }
}