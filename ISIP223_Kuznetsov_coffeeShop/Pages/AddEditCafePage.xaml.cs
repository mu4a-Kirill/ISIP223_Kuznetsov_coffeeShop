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
    public partial class AddEditCafePage : Page
    {
        private Cafes _currentCafe;

        public AddEditCafePage(Cafes cafe)
        {
            InitializeComponent();

            if (cafe != null)
            {
                _currentCafe = cafe;
                TbTitle.Text = "Редактирование кофейни";
                TbName.Text = cafe.Name;
                TbAddress.Text = cafe.Address;
                TbPhone.Text = cafe.Phone;
            }
            else
            {
                _currentCafe = new Cafes();
                TbTitle.Text = "Новая кофейня";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbName.Text) || string.IsNullOrWhiteSpace(TbAddress.Text) || string.IsNullOrWhiteSpace(TbPhone.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка");
                return;
            }

            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                _currentCafe.Name = TbName.Text.Trim();
                _currentCafe.Address = TbAddress.Text.Trim();
                _currentCafe.Phone = TbPhone.Text.Trim();

                if (_currentCafe.Cafe_Id == 0)
                    db.Cafes.Add(_currentCafe);
                else
                {
                    db.Cafes.Attach(_currentCafe);
                    db.Entry(_currentCafe).State = System.Data.Entity.EntityState.Modified;
                }

                try
                {
                    db.SaveChanges();
                    MessageBox.Show("Данные сохранены!", "Успех");
                    NavigationService.GoBack();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message);
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e) => NavigationService.GoBack();
    }
}