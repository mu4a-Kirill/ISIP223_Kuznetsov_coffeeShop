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

    public partial class AddEditOrderPage : Page
    {
        private Orders _currentOrder;

        public AddEditOrderPage(Orders order)
        {
            InitializeComponent();

            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                CmbCafe.ItemsSource = db.Cafes.ToList();
                CmbUser.ItemsSource = db.Users.ToList();
                CmbProduct.ItemsSource = db.Products.ToList();
            }

            if (order != null)
            {
                _currentOrder = order;
                TbTitle.Text = "Редактирование заказа";

                CmbCafe.SelectedValue = order.Cafe_Id;
                CmbUser.SelectedValue = order.User_Id;
                CmbProduct.SelectedValue = order.Product_Id;
                TbQuantity.Text = order.Quantity.ToString();
                DpOrderDate.SelectedDate = order.Order_Date;

                foreach (ComboBoxItem item in CmbStatus.Items)
                {
                    if (item.Content.ToString() == order.Status)
                    {
                        CmbStatus.SelectedItem = item;
                        break;
                    }
                }
            }
            else
            {
                _currentOrder = new Orders();
                TbTitle.Text = "Новый заказ";
                DpOrderDate.SelectedDate = DateTime.Now; 
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (CmbCafe.SelectedItem == null || CmbUser.SelectedItem == null || CmbProduct.SelectedItem == null)
            {
                MessageBox.Show("Выберите кофейню, сотрудника и товар!", "Ошибка");
                return;
            }

            if (!int.TryParse(TbQuantity.Text, out int quantity) || quantity <= 0)
            {
                MessageBox.Show("Введите корректное количество (целое число больше 0)!", "Ошибка");
                return;
            }

            if (DpOrderDate.SelectedDate == null)
            {
                MessageBox.Show("Выберите дату заказа!", "Ошибка");
                return;
            }

            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                _currentOrder.Cafe_Id = (int)CmbCafe.SelectedValue;
                _currentOrder.User_Id = (int)CmbUser.SelectedValue;
                _currentOrder.Product_Id = (int)CmbProduct.SelectedValue;
                _currentOrder.Quantity = quantity;
                _currentOrder.Order_Date = DpOrderDate.SelectedDate.Value;
                _currentOrder.Status = ((ComboBoxItem)CmbStatus.SelectedItem).Content.ToString();

                if (_currentOrder.Order_Id == 0)
                    db.Orders.Add(_currentOrder);
                else
                {
                    db.Orders.Attach(_currentOrder);
                    db.Entry(_currentOrder).State = System.Data.Entity.EntityState.Modified;
                }

                try
                {
                    db.SaveChanges();
                    MessageBox.Show("Заказ сохранен!", "Успех");
                    NavigationService.GoBack();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
    }
}