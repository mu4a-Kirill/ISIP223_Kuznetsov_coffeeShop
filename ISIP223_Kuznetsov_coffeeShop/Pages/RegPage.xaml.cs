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
    public partial class RegPage : Page
    {
        public RegPage()
        {
            InitializeComponent();
        }

        private void TbLogin_TextChanged(object sender, TextChangedEventArgs e)
        {
            LblLoginHint.Visibility = string.IsNullOrEmpty(TbLogin.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void TbFIO_TextChanged(object sender, TextChangedEventArgs e)
        {
            LblFioHint.Visibility = string.IsNullOrEmpty(TbFIO.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void PbPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LblPassHint.Visibility = string.IsNullOrEmpty(PbPassword.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void PbPasswordConfirm_PasswordChanged(object sender, RoutedEventArgs e)
        {
            LblPassConfirmHint.Visibility = string.IsNullOrEmpty(PbPasswordConfirm.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        private void BtnBackToAuth_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
        private void BtnRegister_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbLogin.Text) ||
                string.IsNullOrWhiteSpace(TbFIO.Text) ||
                string.IsNullOrEmpty(PbPassword.Password) ||
                string.IsNullOrEmpty(PbPasswordConfirm.Password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Login == TbLogin.Text);
                if (existingUser != null)
                {
                    MessageBox.Show("Пользователь с таким логином уже существует!", "Ошибка");
                    return;
                }
            }

            if (PbPassword.Password.Length < 6)
            {
                MessageBox.Show("Пароль слишком короткий, должно быть минимум 6 символов!", "Ошибка");
                return;
            }

            bool hasNumber = false;
            bool isEnglishOnly = true;

            for (int i = 0; i < PbPassword.Password.Length; i++)
            {
                char c = PbPassword.Password[i];

                if (c >= '0' && c <= '9')
                    hasNumber = true;
                else if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z')))
                    isEnglishOnly = false;
            }

            if (!isEnglishOnly)
            {
                MessageBox.Show("Используйте только английскую раскладку!", "Ошибка");
                return;
            }
            if (!hasNumber)
            {
                MessageBox.Show("Пароль должен содержать хотя бы одну цифру!", "Ошибка");
                return;
            }

            if (PbPassword.Password != PbPasswordConfirm.Password)
            {
                MessageBox.Show("Пароли не совпадают!", "Ошибка");
                return;
            }

            try
            {
                using (var db = new DB_Kuznetsov_coffeeShopEntities())
                {
                    int selectedRoleId = int.Parse(((ComboBoxItem)CmbRole.SelectedItem).Tag.ToString());

                    var newUser = new Users
                    {
                        Login = TbLogin.Text,
                        Password = AuthPage.GetHash(PbPassword.Password), 
                        FIO = TbFIO.Text,
                        Role_Id = selectedRoleId
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();

                    MessageBox.Show("Пользователь успешно зарегистрирован!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                    NavigationService.GoBack();
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка");
            }
        }
    }
}
