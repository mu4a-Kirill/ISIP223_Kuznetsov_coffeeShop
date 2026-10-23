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

    public partial class AddEditUserPage : Page
    {
        private Users _currentUser;

        public AddEditUserPage(Users user)
        {
            InitializeComponent();

            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                CmbRole.ItemsSource = db.User_Roles.ToList();
            }

            if (user != null)
            {
                _currentUser = user;
                TbTitle.Text = "Редактирование пользователя";
                TbLogin.Text = user.Login;
                TbFIO.Text = user.FIO;
                CmbRole.SelectedValue = user.Role_Id;

                LblPassword.Content = "Новый пароль";
            }
            else
            {
                _currentUser = new Users();
                TbTitle.Text = "Новый пользователь";
                LblPassword.Content = "Пароль";
            }

            DataContext = _currentUser;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            string login = TbLogin.Text.Trim();
            string fio = TbFIO.Text.Trim();
            var selectedRole = CmbRole.SelectedItem as User_Roles;

            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(fio) || selectedRole == null)
            {
                MessageBox.Show("Заполните все поля и выберите роль!", "Ошибка");
                return;
            }

            if (_currentUser.User_Id == 0 && string.IsNullOrEmpty(PbPassword.Password))
            {
                MessageBox.Show("Для нового пользователя необходимо задать пароль!", "Ошибка");
                return;
            }

            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                _currentUser.Login = login;
                _currentUser.FIO = fio;
                _currentUser.Role_Id = selectedRole.Role_Id;

                if (_currentUser.User_Id == 0)
                {
                    _currentUser.Password = AuthPage.GetHash(PbPassword.Password);
                    db.Users.Add(_currentUser);
                }
                else
                {
                    if (!string.IsNullOrEmpty(PbPassword.Password))
                    {
                        _currentUser.Password = AuthPage.GetHash(PbPassword.Password);
                    }

                    db.Users.Attach(_currentUser);
                    db.Entry(_currentUser).State = System.Data.Entity.EntityState.Modified;
                }

                try
                {
                    db.SaveChanges();
                    MessageBox.Show("Данные сохранены!", "Успех");
                    NavigationService.GoBack();
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show("Ошибка: " + ex.Message, "Ошибка");
                }
            }
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}