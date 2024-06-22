using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using Button = System.Windows.Controls.Button;
using Label = System.Windows.Controls.Label;
using Orientation = System.Windows.Controls.Orientation;

namespace Exam.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        public static List<ExamOrder> createdByGuestOrdersList = new();

        public static List<ExamOrder> examCreatedOrdersList = new();

        public OrdersPage()
        {
            InitializeComponent();

            examCreatedOrdersList = CurrentUser.IsGuest? createdByGuestOrdersList : DataAccessLayer.GetExamOrderDataWithId(CurrentUser.UserID);
            //если пользователь зашел как гость, то список заказов заполняется из createdByGuestOrdersList, до момента,
            //пока кто-либо не зайдет в авторизованный аккаунт или приложение не будет перезапущено, гостю следует сохранить талоны заказов в txt, чтобы не потерять свои заказы
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            GetOrderList();

            //вывод данных о текущем пользователе
            if (CurrentUser.IsGuest)
                CurrentUserLabel.Content = "Вы вошли как гость";
            else
                CurrentUserLabel.Content = $"{CurrentUser.UserName.Substring(0, 1)}.{CurrentUser.UserPatronymic.Substring(0, 1)}. {CurrentUser.UserSurname}";

            examCreatedOrdersList = CurrentUser.IsGuest ? createdByGuestOrdersList : DataAccessLayer.GetExamOrderDataWithId(CurrentUser.UserID);
        }

        private void GetOrderList()//метод для вывода заказов текущего пользователя на странцу
        {
            YourOrdersStackPanel.Children.Clear();
            int productsCount = examCreatedOrdersList.Count;
            for (int i = 0; i < productsCount; i++)//в цикле создаются отдельные элементы в которых хранятся данные о товарах
            {
                List<ExamOrderProduct> examOrderProducts = DataAccessLayer.GetOrderProductWithOrderId(examCreatedOrdersList[i].OrderID);

                Border orderBorder = new Border();
                orderBorder.Width = 600;
                orderBorder.Margin = new Thickness(80, 5, 0, 5);
                orderBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCC6600"));
                orderBorder.BorderThickness = new(3);

                StackPanel orderPanel = new StackPanel();
                orderPanel.Tag = i;
                orderPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFCC99"));

                Label orderIdLabel = new Label();
                orderIdLabel.Content = $"Заказ №{examCreatedOrdersList[i].OrderID}";
                orderPanel.Children.Add(orderIdLabel);

                Label orderDateLabel = new Label();
                orderDateLabel.Content = $"Дата: {examCreatedOrdersList[i].OrderDate}";
                orderPanel.Children.Add(orderDateLabel);

                StackPanel orderCompositionPanel = new StackPanel();
                orderCompositionPanel.Orientation = Orientation.Horizontal;
                Label orderCompositionLabel = new Label();
                string orderComposition = "";
                for (int j = 0; j < examOrderProducts.Count; j++)
                {
                    orderComposition += $"\n{DataAccessLayer.GetProductNameWithArticle(examOrderProducts[j].ProductArticleNumber)}" +
                        $"({DataAccessLayer.GetProductAmountInOrderWithArticle(examCreatedOrdersList[i].OrderID, examOrderProducts[j].ProductArticleNumber)})";
                }
                examCreatedOrdersList[i].ProductsInOrder = $"Состав заказа:{orderComposition}";
                orderCompositionLabel.Content = examCreatedOrdersList[i].ProductsInOrder;
                orderCompositionPanel.Children.Add(orderCompositionLabel);
                orderPanel.Children.Add(orderCompositionPanel);

                Label orderSumLabel = new Label();
                decimal orderSum = DataAccessLayer.GetSummOrder(examCreatedOrdersList[i].OrderID);
                orderSumLabel.Content = $"Сумма заказа: " + orderSum.ToString("F2");
                orderPanel.Children.Add(orderSumLabel);

                Label orderDiscountLabel = new Label();
                decimal orderDiscount = DataAccessLayer.GetDiscountOrder(examCreatedOrdersList[i].OrderID);
                orderDiscountLabel.Content = $"Сумма скидки в заказе: " + orderDiscount.ToString("F2");
                orderPanel.Children.Add(orderDiscountLabel);

                Label orderPickupPointLabel = new Label();
                orderPickupPointLabel.Content = $"Пункт выдачи: {examCreatedOrdersList[i].OrderPickupPoint}";
                orderPanel.Children.Add(orderPickupPointLabel);

                DockPanel dockPanel = new();
                Label orderPickupCodeLabel = new Label();
                orderPickupCodeLabel.Content = $"Код получения: {examCreatedOrdersList[i].OrderPickupCode}";
                Button printOrderButton = new();
                printOrderButton.Content = "Распечатать талон заказа";
                printOrderButton.Margin = new(5);
                printOrderButton.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFCC99"));
                printOrderButton.BorderThickness = new Thickness(4);
                printOrderButton.Click += PrintOrderButton_Click;
                printOrderButton.Tag = $"Заказ №{examCreatedOrdersList[i].OrderID}\nДата: {examCreatedOrdersList[i].OrderDate}\n" +
                    $"Состав заказа:{orderComposition}\nСумма заказа: {orderSum.ToString("F2")}\nСумма скидки в заказе: {orderDiscount.ToString("F2")}\n" +
                    $"Пункт выдачи: {examCreatedOrdersList[i].OrderPickupPoint}\nКод получения: {examCreatedOrdersList[i].OrderPickupCode}";
                DockPanel.SetDock(printOrderButton, Dock.Right);
                dockPanel.Children.Add(printOrderButton);
                dockPanel.Children.Add(orderPickupCodeLabel);
                orderPanel.Children.Add(dockPanel);

                orderBorder.Child = orderPanel;
                YourOrdersStackPanel.Children.Add(orderBorder);
            }
        }

        private void PrintOrderButton_Click(object sender, RoutedEventArgs e)//печать талончика заказа в txt-файл
        {
            Button printOrderButton = (Button)sender;
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Текстовый файл (*.txt)|*.txt";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                // Текст для сохранения
                string textToSave = printOrderButton.Tag.ToString();

                // Запись текста в файл
                File.WriteAllText(saveFileDialog.FileName, textToSave);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentFrame.Navigate(new ShopPage());
        }
    }
}
