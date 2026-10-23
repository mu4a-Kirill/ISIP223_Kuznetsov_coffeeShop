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

    public partial class AddEditProductPage : Page
    {
        private Products _currentProduct;

        public AddEditProductPage(Products product)
        {
            InitializeComponent();

            if (product != null)
            {
                _currentProduct = product;
                TbTitle.Text = "Редактирование товара";

                TbName.Text = product.Name;
                TbCost.Text = product.Cost.ToString();
                TbStock.Text = product.StockQuantity.ToString();
            }
            else
            {
                _currentProduct = new Products();
                TbTitle.Text = "Новый товар";
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbName.Text))
            {
                MessageBox.Show("Введите название товара!", "Ошибка");
                return;
            }

            if (!decimal.TryParse(TbCost.Text, out decimal cost) || cost < 0)
            {
                MessageBox.Show("Введите корректную цену (число больше или равно 0)!", "Ошибка");
                return;
            }

            if (!int.TryParse(TbStock.Text, out int stock) || stock < 0)
            {
                MessageBox.Show("Введите корректное количество (целое число больше или равно 0)!", "Ошибка");
                return;
            }

            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                _currentProduct.Name = TbName.Text.Trim();
                _currentProduct.Cost = cost;
                _currentProduct.StockQuantity = stock;

                if (_currentProduct.Product_Id == 0) 
                {
                    db.Products.Add(_currentProduct);
                }
                else 
                {
                    db.Products.Attach(_currentProduct);
                    db.Entry(_currentProduct).State = System.Data.Entity.EntityState.Modified;
                }

                try
                {
                    db.SaveChanges();
                    MessageBox.Show("Данные сохранены!", "Успех");
                    NavigationService.GoBack();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка");
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}