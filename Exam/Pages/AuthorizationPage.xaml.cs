using Exam.Pages;
using System.Windows;
using System.Windows.Controls;

namespace Exam
{
    public partial class AuthorizationPage : Page
    {
        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private void AuthorizeButton_Click(object sender, RoutedEventArgs e)//при нажатии на кнопку идет заполнение данных текущего пользователя, если такой зарегестрирован
        {
            bool userExists = DataAccessLayer.UserAuthorization(authorizationLoginTextBox.Text, authorizationPasswordTextBox.Password);
            if (userExists)
            {
                CurrentUser.IsGuest = false;
                App.CurrentFrame.Navigate(new ShopPage());
            }
            else
                IncorrectDataLabel.Visibility = Visibility.Visible;
        }

        private void GuestButton_Click(object sender, RoutedEventArgs e)//вход как гость
        {
            CurrentUser.IsGuest = true;
            App.CurrentFrame.Navigate(new ShopPage());
        }

        private void AuthorizationPasswordTextBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            IncorrectDataLabel.Visibility = Visibility.Hidden;
        }

        private void AuthorizationLoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            IncorrectDataLabel.Visibility = Visibility.Hidden;
        }
    }
}
