using ISIP223_Kuznetsov_coffeeShop.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

namespace ISIP223_Kuznetsov_coffeeShop
{

    public partial class AuthPage : Page
    {
        private int failedAttempts = 0;
        public AuthPage()
        {
            InitializeComponent();
        }

        public static string GetHash(string password)
        {
            using (var hash = SHA1.Create())
            {
                return string.Concat(hash.ComputeHash(Encoding.UTF8.GetBytes(password)).Select(x => x.ToString("X2")));
            }
        }

        private void ButtonEnter_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(TextBoxLogin.Text) || string.IsNullOrEmpty(PasswordBox.Password))
            {
                MessageBox.Show("Заполните логин и пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }


            string hashedPassword = GetHash(PasswordBox.Password);

            using (var db = new DB_Kuznetsov_coffeeShopEntities())
            {

                var user = db.Users.FirstOrDefault(u => u.Login == TextBoxLogin.Text && u.Password == hashedPassword);


                if (user == null)
                {
                    failedAttempts++;

                    MessageBox.Show($"Пользователь с такими данными не найден!\nНеудачных попыток: {failedAttempts}",
                                    "Ошибка входа", MessageBoxButton.OK, MessageBoxImage.Error);

                    if (failedAttempts >= 3)
                    {
                        CaptchaSwitch(true);
                        CaptchaChange();
                    }
                    return;
                }

                MessageBox.Show($"Добро пожаловать, {user.FIO}!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                failedAttempts = 0;


                if (user.Role_Id == 1)
                {

                    NavigationService.Navigate(new AdminPage());
                }
                else
                {
                    NavigationService.Navigate(new UserPage(user));
                }
            }
        }


        private void ButtonReg_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegPage());
        }

        private void ButtonChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new ChangePassPage());
        }

        private void CaptchaChange()
        {
            string allowchar = "A,B,C,D,E,F,G,H,K,M,N,P,R,S,T,U,V,W,X,Y,Z,2,3,4,5,6,7,8,9";
            string[] ar = allowchar.Split(',');
            string pwd = "";
            Random r = new Random();
            for (int i = 0; i < 5; i++)
            {
                pwd += ar[r.Next(0, ar.Length)];
            }
            captcha.Text = pwd;
        }

        private void CaptchaSwitch(bool showCaptcha)
        {
            // блок/анлок полей ввода
            TextBoxLogin.IsEnabled = !showCaptcha;
            PasswordBox.IsEnabled = !showCaptcha;
            ButtonEnter.IsEnabled = !showCaptcha;
            ButtonReg.IsEnabled = !showCaptcha;
            ButtonChangePassword.IsEnabled = !showCaptcha;
            // показать или скрыть капчу
            var captchaVisibility = showCaptcha ? Visibility.Visible : Visibility.Collapsed;
            labelCaptcha.Visibility = captchaVisibility;
            captcha.Visibility = captchaVisibility;
            captchaInput.Visibility = captchaVisibility;
            submitCaptcha.Visibility = captchaVisibility;
        }

        private void submitCaptcha_Click(object sender, RoutedEventArgs e)
        {
            if (captchaInput.Text != captcha.Text)
            {
                MessageBox.Show("Неверно введена капча!");
                CaptchaChange(); 
                captchaInput.Clear();
            }
            else
            {
                MessageBox.Show("Капча введена успешно. Можете продолжить.");
                CaptchaSwitch(false); 
                failedAttempts = 0;   
                captchaInput.Clear();
            }
        }
    }
}