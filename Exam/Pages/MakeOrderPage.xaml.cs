using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Exam.Pages
{
    /// <summary>
    /// Логика взаимодействия для OrderPage.xaml
    /// </summary>
    public partial class MakeOrderPage : Page
    {
        public static int OrderProductsCount;

        public static decimal OrderCost;

        public static decimal OrderDiscount;

        public static List<ExamProduct> examOrderList = new();

        public static List<ExamPickupPoint> examPickupPoints = new();

        public static List<int> existingPickupCodes = new();

        public MakeOrderPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateOrderList();
            DataAccessLayer.GetPickupPoints();

            //вывод информации о пользователе
            if (CurrentUser.IsGuest)
                CurrentUserLabel.Content = "Вы вошли как гость";
            else
                CurrentUserLabel.Content = $"{CurrentUser.UserName.Substring(0, 1)}.{CurrentUser.UserPatronymic.Substring(0, 1)}. {CurrentUser.UserSurname}";

            PickupPointsComboBox.ItemsSource = examPickupPoints;
        }

        private void CreateOrderList()//метод для вывода товаров в корзине на странцу
        {
            productsInOrderStackPanel.Children.Clear();
            int productsCount = examOrderList.Count;
            for (int i = 0; i < productsCount; i++)//в цикле создаются отдельные элементы в которых хранятся данные о товарах в корзине
            {
                Border productBorder = new Border();
                productBorder.Width = 600;
                productBorder.Margin = new Thickness(80, 5, 0, 5);
                productBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCC6600"));
                productBorder.BorderThickness = new(3);

                StackPanel productPanel = new StackPanel();
                productPanel.Tag = i;
                productPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFCC99"));

                Image productImage = new Image();
                productImage.Source = new BitmapImage(new Uri(examOrderList[i].ProductPhoto));
                productImage.Width = 200;
                productImage.Height = 200;
                productPanel.Children.Add(productImage);

                StackPanel articleNumberPanel = new StackPanel();
                articleNumberPanel.Orientation = Orientation.Horizontal;
                Label articleNumberLabel = new Label();
                articleNumberLabel.Content = "Артикул:";
                Label articleDataLabel = new Label();
                articleDataLabel.Content = examOrderList[i].ProductArticleNumber;
                articleNumberPanel.Children.Add(articleNumberLabel);
                articleNumberPanel.Children.Add(articleDataLabel);
                productPanel.Children.Add(articleNumberPanel);

                Label nameDataLabel = new Label();
                nameDataLabel.Content = examOrderList[i].ProductName;
                productPanel.Children.Add(nameDataLabel);

                Label desciptionDataLabel = new Label();
                desciptionDataLabel.Content = examOrderList[i].ProductDescription;
                productPanel.Children.Add(desciptionDataLabel);

                StackPanel categoryPanel = new StackPanel();
                categoryPanel.Orientation = Orientation.Horizontal;
                Label categoryLabel = new Label();
                categoryLabel.Content = "Категория товара:";
                Label categoryDataLabel = new Label();
                categoryDataLabel.Content = examOrderList[i].ProductCategory;
                categoryPanel.Children.Add(categoryLabel);
                categoryPanel.Children.Add(categoryDataLabel);
                productPanel.Children.Add(categoryPanel);

                StackPanel manufacturerPanel = new StackPanel();
                manufacturerPanel.Orientation = Orientation.Horizontal;
                Label manufacturerLabel = new Label();
                manufacturerLabel.Content = "Производитель товара:";
                Label manufacturerDataLabel = new Label();
                manufacturerDataLabel.Content = examOrderList[i].ProductManufacturer;
                manufacturerPanel.Children.Add(manufacturerLabel);
                manufacturerPanel.Children.Add(manufacturerDataLabel);
                productPanel.Children.Add(manufacturerPanel);

                DockPanel costDockPanel = new DockPanel();//DockPanel чтобы в будущем в этой строке метки со скидкой не смещались, а были по правому краю
                Label costLabel = new Label();
                costLabel.Content = "Цена товара:";
                TextBlock costDataTextBlock = new TextBlock();//TextBlock для возможности задания свойства зачеркнутости текста
                costDataTextBlock.Text = examOrderList[i].ProductCost.ToString();
                Label discountLabel = new Label();
                discountLabel.Content = $"Скидка:";
                discountLabel.FontSize = 12;
                Label discountDataLabel = new Label();
                discountDataLabel.FontSize = 12;
                discountDataLabel.Content = examOrderList[i].ProductDiscountAmount;
                costDockPanel.Children.Add(costLabel);
                costDockPanel.Children.Add(discountDataLabel);
                DockPanel.SetDock(discountDataLabel, Dock.Right);//размещение по правому краю, чтобы не было смещения при изменении ширины других меток
                costDockPanel.Children.Add(discountLabel);
                DockPanel.SetDock(discountLabel, Dock.Right);//размещение по правому краю, чтобы не было смещения при изменении ширины других меток
                if (examOrderList[i].ProductDiscountAmount > 0)//если скидка на товар есть, цена зачеркивается и создается метка с новой ценой
                {
                    costDataTextBlock.TextDecorations = TextDecorations.Strikethrough;
                    costDataTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
                    Label costWithDiscountDataLabel = new Label();
                    decimal resultCost = (decimal)Convert.ToDouble(costDataTextBlock.Text) * (100 - Convert.ToInt32(discountDataLabel.Content)) / 100;
                    costWithDiscountDataLabel.Content = resultCost;
                    costDockPanel.Children.Add(costWithDiscountDataLabel);
                }
                costDockPanel.Children.Add(costDataTextBlock);
                productPanel.Children.Add(costDockPanel);

                StackPanel productStatusPanel = new StackPanel();
                productStatusPanel.Orientation = Orientation.Horizontal;
                Label productStatusLabel = new Label();
                productStatusLabel.Content = "Статус:";
                Label productStatusDataLabel = new Label();
                productStatusDataLabel.Content = examOrderList[i].ProductStatus;
                productStatusPanel.Children.Add(productStatusLabel);
                productStatusPanel.Children.Add(productStatusDataLabel);
                productPanel.Children.Add(productStatusPanel);

                StackPanel productQuantityInStockPanel = new StackPanel();
                productQuantityInStockPanel.Orientation = Orientation.Horizontal;
                Label productQuantityInStockLabel = new Label();
                productQuantityInStockLabel.Content = "Количество на складе:";
                Label productQuantityInStockDataLabel = new Label();
                productQuantityInStockDataLabel.Content = examOrderList[i].ProductQuantityInStock;
                productQuantityInStockPanel.Children.Add(productQuantityInStockLabel);
                productQuantityInStockPanel.Children.Add(productQuantityInStockDataLabel);
                productPanel.Children.Add(productQuantityInStockPanel);

                DockPanel countPanel = new DockPanel();
                CountControl countControl = new CountControl();
                countControl.Tag = i;
                countControl.HorizontalAlignment = HorizontalAlignment.Left;
                countControl.Value = examOrderList[i].ProductCountInOrder;
                countControl.MaxValue = examOrderList[i].ProductQuantityInStock;
                countControl.countTextBox.Text = examOrderList[i].ProductCountInOrder.ToString();
                countControl.ValueChanged += CountControl_ValueChanged;
                Button deleteButton = new Button();
                deleteButton.Click += DeleteButton_Click;
                Image deleteImage = new Image();
                deleteButton.Width = 50;
                deleteButton.HorizontalAlignment = HorizontalAlignment.Right;
                deleteImage.Source = new BitmapImage(new Uri("pack://application:,,,/Images/delete.png"));
                deleteButton.Content = deleteImage;
                DockPanel.SetDock(deleteButton, Dock.Right);
                countPanel.Children.Add(deleteButton);
                countPanel.Children.Add(countControl);
                productPanel.Children.Add(countPanel);

                productBorder.Child = productPanel;
                productsInOrderStackPanel.Children.Add(productBorder);
            }
            UpdateProductsCount();
            UpdateDiscount();
            UpdateCost();
        }

        private void UpdateProductsCount()//Обновление количества товаров в корзине
        {
            int productsCount = examOrderList.Count;
            OrderProductsCount = 0;
            for (int i = 0; i < productsCount; i++)
            {
                OrderProductsCount += examOrderList[i].ProductCountInOrder;
                CountProductsInOrderLabel.Content = OrderProductsCount.ToString();
            }
            if (examOrderList.Count == 0)
                CountProductsInOrderLabel.Content = 0;
        }

        private void UpdateDiscount()//обновление общей скидки товаров в корзине
        {
            int productsCount = examOrderList.Count;
            OrderDiscount = 0;
            for (int i = 0; i < productsCount; i++)
            {
                OrderDiscount += (examOrderList[i].ProductCost - (examOrderList[i].ProductCost) * (100 - examOrderList[i].ProductDiscountAmount) / 100) * examOrderList[i].ProductCountInOrder;
                OrderDiscountLabel.Content = OrderDiscount.ToString("F2") + " руб.";
            }
            if (examOrderList.Count == 0)
                OrderDiscountLabel.Content = 0;
        }

        private void UpdateCost()//обновление общей стоимости товаров в корзине
        {
            int productsCount = examOrderList.Count;
            OrderCost = 0;
            for (int i = 0; i < productsCount; i++)
            {
                OrderCost += Convert.ToDecimal(Convert.ToDouble(examOrderList[i].ProductCost) * (100 - examOrderList[i].ProductDiscountAmount) / 100) * examOrderList[i].ProductCountInOrder;
                OrderCostLabel.Content = OrderCost.ToString("F2") + " руб.";
            }
            if (examOrderList.Count == 0)
                OrderCostLabel.Content = 0;
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)//удаление товара из корзины
        {
            Button deleteButton = sender as Button;
            DockPanel countPanel = deleteButton.Parent as DockPanel;
            StackPanel productPanel = countPanel.Parent as StackPanel;
            StackPanel productsInOrderStackPanel = productPanel.Parent as StackPanel;
            examOrderList.RemoveAt((int)productPanel.Tag);
            productsInOrderStackPanel.Children.Remove(productPanel);
            CreateOrderList();
        }

        private void CountControl_ValueChanged(object sender, RoutedEventArgs e)//изменение количества штук определенного товара в корзине
        {
            CountControl countControl = sender as CountControl;
            examOrderList[Convert.ToInt32(countControl.Tag)].ProductCountInOrder = countControl.Value;
            UpdateProductsCount();
            UpdateCost();
            UpdateDiscount();
            if (countControl.Value == 0)//если ставим количество 0, то товар удаляется
            {
                DockPanel countPanel = countControl.Parent as DockPanel;
                StackPanel productPanel = countPanel.Parent as StackPanel;
                StackPanel productsInOrderStackPanel = productPanel.Parent as StackPanel;
                examOrderList.RemoveAt((int)productPanel.Tag);
                productsInOrderStackPanel?.Children.Remove(productPanel);
                CreateOrderList();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)//возврат на страницу магазинна
        {
            App.CurrentFrame.GoBack();
        }

        private void MakeOrderButton_Click(object sender, RoutedEventArgs e)//создание записи о товаре в БД
        {
            if (examOrderList.Count != 0)
            {
                if (PickupPointsComboBox.SelectedItem != null)
                {
                    //получение даты доставки
                    Random rnd = new Random();
                    int travelDays = rnd.Next(3, 8); // Генерирует число от 3 до 7
                    DateTime currentDate = DateTime.Now;
                    DateTime deliveryDate = currentDate.AddDays(travelDays);

                    //получение кода выдачи
                    int pickupCode;
                    do
                    {
                        pickupCode = rnd.Next(1, 10001); // Генерирует число от 1 до 10000
                    }
                    while (existingPickupCodes.Contains(pickupCode));

                    ExamOrder newOrder = new ExamOrder()//создание элемента класса ExamOrder
                    {
                        OrderID = DataAccessLayer.GetLastOrderID(),
                        OrderStatus = "Новый",
                        OrderDate = currentDate,
                        OrderDeliveryDate = deliveryDate,
                        OrderPickupPoint = examPickupPoints[PickupPointsComboBox.SelectedIndex].OrderPickupPoint,
                        OrderPickupCode = pickupCode
                    };

                    DataAccessLayer.UpdateExamOrder(CurrentUser.UserID, newOrder.OrderStatus, newOrder.OrderDate, newOrder.OrderDeliveryDate, newOrder.OrderPickupPoint, newOrder.OrderPickupCode);

                    //добавление записей о товарах в новом заказе в БД
                    for (int i = 0; i < examOrderList.Count; i++)
                    {
                        DataAccessLayer.UpdateExamOrderProduct(DataAccessLayer.GetLastOrderID(), examOrderList[i].ProductArticleNumber, examOrderList[i].ProductCountInOrder);
                    }

                    //если пользователь вошел как гость, то создается отдельная запись о его заказе, которую он может получить пока не вышел из приложения,
                    //тк у него нет никаких данных чтобы этот заказ подтвердить, пусть печатает талончик
                    if (CurrentUser.IsGuest)
                    {
                        OrdersPage.createdByGuestOrdersList.Add(newOrder);
                    }

                    examOrderList.Clear();
                    productsInOrderStackPanel.Children.Clear();
                    App.CurrentFrame.Navigate(new OrdersPage());

                }
                else
                    WarnLabel.Content = "*Укажите пункт выдачи";
            }
            else WarnLabel.Content = "*Заказ не может быть пустым";
        }

        private void PickupPointsComboBox_GotFocus(object sender, RoutedEventArgs e)
        {
            WarnLabel.Content = string.Empty;
        }

        private void GoToYourOrders_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentFrame.Navigate(new OrdersPage());
        }
    }
}
