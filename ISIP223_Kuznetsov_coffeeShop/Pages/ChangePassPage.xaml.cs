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
    public partial class ChangePassPage : Page
    {
        public ChangePassPage()
        {
            InitializeComponent();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TbLogin.Text) ||
                string.IsNullOrEmpty(PbOldPassword.Password) ||
                string.IsNullOrEmpty(PbNewPassword.Password) ||
                string.IsNullOrEmpty(PbConfirmPassword.Password))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка");
                return;
            }

            string hashedOldPass = AuthPage.GetHash(PbOldPassword.Password);

            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {
                var user = db.Users.FirstOrDefault(u => u.Login == TbLogin.Text && u.Password == hashedOldPass);

                if (user == null)
                {
                    MessageBox.Show("Неверный логин или старый пароль!", "Ошибка");
                    return;
                }

                if (PbNewPassword.Password.Length < 6)
                {
                    MessageBox.Show("Новый пароль слишком короткий (минимум 6 символов)!", "Ошибка");
                    return;
                }

                bool hasNumber = false;
                bool isEnglish = true;
                foreach (char c in PbNewPassword.Password)
                {
                    if (c >= '0' && c <= '9') hasNumber = true;
                    else if (!((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))) isEnglish = false;
                }

                if (!isEnglish)
                {
                    MessageBox.Show("Используйте только английскую раскладку!", "Ошибка");
                    return;
                }
                if (!hasNumber)
                {
                    MessageBox.Show("Новый пароль должен содержать хотя бы одну цифру!", "Ошибка");
                    return;
                }

                if (PbNewPassword.Password != PbConfirmPassword.Password)
                {
                    MessageBox.Show("Новые пароли не совпадают!", "Ошибка");
                    return;
                }

                user.Password = AuthPage.GetHash(PbNewPassword.Password);
                db.SaveChanges();

                MessageBox.Show("Пароль успешно изменен! Войдите с новым паролем.", "Успех");
                NavigationService.Navigate(new AuthPage()); 
            }
        }

        private void BtnBack_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}