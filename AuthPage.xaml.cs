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

namespace Gilmianova_41
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        private string captcha;
        public AuthPage()
        {
            InitializeComponent();
            CaptchaTextBox.Visibility = Visibility.Collapsed;
        }

        private void GenerateCaptcha()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder captchaBuilder = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < 4; i++) // Генерируем 4 случайных символа
            {
                captchaBuilder.Append(chars[random.Next(chars.Length)]);
            }
            captcha = captchaBuilder.ToString();

            DisplayCaptcha(); // Отображаем капчу на экране
        }

        private void DisplayCaptcha()
        {
            captchaOneWord.Text = captcha[0].ToString();
            captchaTwoWord.Text = captcha[1].ToString();
            captchaThreeWord.Text = captcha[2].ToString();
            captchaFourWord.Text = captcha[3].ToString();
        }

        private void HideCaptcha()
        {
            captchaOneWord.Text = "";
            captchaTwoWord.Text = "";
            captchaThreeWord.Text = "";
            captchaFourWord.Text = "";
        }

        private async void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            string login = TextBoxLogin.Text;
            string password = TextBoxPassword.Text;
            string captchaInput = CaptchaTextBox.Text;

            if (login == "" || password == "")
            {
                MessageBox.Show("есть пустые поля");
                return;
            }

            User user = Gilmianova_41Entities.GetContext().User.ToList().Find(p => p.UserLogin == login && p.UserPassword == password);
            if (user != null)
            {
                if (CaptchaTextBox.Visibility == Visibility.Visible)
                {
                    if (captchaInput == "")
                    {
                        MessageBox.Show("введите капчу");
                        return;
                    }

                    if (captchaInput != captcha)
                    {
                        MessageBox.Show("неверная капча");
                        CaptchaTextBox.Text = "";
                        TextBoxLogin.Text = "";
                        TextBoxPassword.Text = "";

                        GenerateCaptcha(); // Генерируем новую капчу 
                        CaptchaTextBox.Visibility = Visibility.Visible; // Показываем капчу

                        SignInButton.IsEnabled = false; // Блокируем кнопку входа 
                        await Task.Delay(10000); // Запускаем таймер блокировки 
                        SignInButton.IsEnabled = true;
                        return;

                    }
                }
                Manager.MainFrame.Navigate(new ProductPage(user));
                TextBoxLogin.Text = "";
                TextBoxPassword.Text = "";
                CaptchaTextBox.Text = "";
                CaptchaTextBox.Visibility = Visibility.Collapsed;
                HideCaptcha();
                return;

            }
            else
            {

                if (CaptchaTextBox.Visibility == Visibility.Visible)
                {
                    if (captchaInput == "")
                    {
                        MessageBox.Show("введите капчу");
                        return;
                    }

                    if (captchaInput != captcha)
                    {
                        MessageBox.Show("неверная капча");
                        CaptchaTextBox.Text = "";
                        TextBoxLogin.Text = "";
                        TextBoxPassword.Text = "";
                        GenerateCaptcha(); // Генерируем новую капчу 
                        CaptchaTextBox.Visibility = Visibility.Visible; // Показываем капчу

                        SignInButton.IsEnabled = false; // Блокируем кнопку входа 
                        await Task.Delay(10000); // Запускаем таймер блокировки 
                        SignInButton.IsEnabled = true;

                        return;

                    }

                }

                MessageBox.Show("введены неверные данные");
                TextBoxLogin.Text = "";
                TextBoxPassword.Text = "";
                CaptchaTextBox.Text = "";

                // Генерация капчи после неверного ввода данных
                GenerateCaptcha();
                CaptchaTextBox.Visibility = Visibility.Visible; // Показываем капчу

                //SignInButton.IsEnabled = false;
                //await Task.Delay(10000);
                //SignInButton.IsEnabled = true;
                return;
            }
        }

        private void SignInGuestButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new ProductPage(null));
            TextBoxLogin.Text = "";
            TextBoxPassword.Text = "";
            CaptchaTextBox.Visibility = Visibility.Collapsed;
            HideCaptcha();
        }
    }
}
