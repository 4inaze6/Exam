using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Exam.Pages
{
    /// <summary>
    /// Логика взаимодействия для ShopPage.xaml
    /// </summary>
    public partial class ShopPage : Page
    {
        public bool searchTextBoxIsFill = false;//свойство, показывающее заполнена ли чем либо строка поиска или нет

        public double MinDiscount { get; set; } = 0;

        public double MaxDiscount { get; set; } = 100;

        public string SortMethod { get; set; } = "ASC";

        public string SearchedText { get; set; } = "";


        public static List<ExamProduct> examProducts = new();

        public ShopPage()
        {
            InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            CreateProductsList(SearchedText, SortMethod, MinDiscount, MaxDiscount);//Вызов метода для вывода товаров на странцу

            SortComboBox.SelectionChanged += Filter_SelectionChanged;
            DiscountFilterComboBox.SelectionChanged += Filter_SelectionChanged;
            searchTextBox.TextChanged += SearchTextBox_TextChanged;
            searchTextBox.Foreground = new SolidColorBrush(Colors.Gray);

            //вывод информации о пользователе
            if (CurrentUser.IsGuest)
                CurrentUserLabel.Content = "Вы вошли как гость";
            else
                CurrentUserLabel.Content = $"{CurrentUser.UserName.Substring(0, 1)}.{CurrentUser.UserPatronymic.Substring(0, 1)}. {CurrentUser.UserSurname}";

            if (MakeOrderPage.examOrderList.Count == 0)
                orderButton.Visibility = Visibility.Hidden;
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)//обновление списка товаров по поисковой строке
        {
            if (searchTextBox.Text != string.Empty)
            {
                searchTextBoxIsFill = true;//если мы что-то написали в строку поиска, свойство searchTextBoxIsFill становится true
            }
            else
            {
                SearchedText = searchTextBox.Text;
                CreateProductsList(SearchedText, SortMethod, MinDiscount, MaxDiscount);
                searchTextBoxIsFill = false;//если мы стерли текст в строке поиска, свойство searchTextBoxIsFill становится false
            }
            if (searchedProductNameLabel != null)
            {
                searchedProductNameLabel.Content = searchTextBox.Text;
                searchedProductNameLabel.Content = searchTextBox.Text.Length > 7 ? searchTextBox.Text.Substring(0, 7) + "..." : searchTextBox.Text;
            }
            if (searchTextBoxIsFill)
            {
                SearchedText = searchTextBox.Text;
                CreateProductsList(SearchedText, SortMethod, MinDiscount, MaxDiscount);//Вызов метода для вывода товаров на странцу
            }
        }

        //вывод стандартной надписи Найти в Ароматном мире серым цветов в строке поиска, если там пусто
        private void Page_GotFocus(object sender, RoutedEventArgs e)//обработчик события, срабатывает, когда мы нажимаем на страницу, но не на строку поиска
        {
            if (!searchTextBox.IsFocused && searchTextBox.Text == string.Empty)//если строка поиска пустая и потеряла фокус, то ей устанавливаются новый свойства: Foreground и Text
            {
                searchTextBox.TextChanged -= SearchTextBox_TextChanged;
                searchTextBox.Foreground = new SolidColorBrush(Colors.Gray);
                searchTextBox.Text = "Найти в Ароматном мире";
                searchTextBox.TextChanged += SearchTextBox_TextChanged;
                searchedProductNameLabel.Content = string.Empty;
                searchTextBoxIsFill = false;//строка поиска не заполнена текстом для поиска
            }
        }

        private void SearchTextBox_GotFocus(object sender, RoutedEventArgs e)//обработчик события, срабатывает, когда строка поиска получила фокус, то есть тогда, когда мы на нее нажали
        {
            if (searchTextBoxIsFill == false)//если строка поиска не была заполнена текстом для поиска, то
            {
                searchTextBox.Text = string.Empty;//строка поиска очищается от текста "Найти в Ароматном мире"
                searchTextBox.Foreground = new SolidColorBrush(Colors.Black);//цвет снова ставится на черный
            }
        }

        private void CreateProductsList(string subs, string sortMethod, double min, double max)//метод для вывода товаров на странцу, согласно условиям фильтрации
        {
            examProducts.Clear();
            productsStackPanel.Children.Clear();
            examProducts = DataAccessLayer.GetExamProductDataFromDB(subs, sortMethod, min, max);
            int productsCount = examProducts.Count;
            for (int i = 0; i < productsCount; i++)//в цикле создаются отдельные элементы в которых хранятся данные о товарах
            {
                //для обводки панели одного товара
                Border productBorder = new Border();
                productBorder.Width = 600;
                productBorder.Margin = new Thickness(80, 5, 0, 5);
                productBorder.BorderBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFCC6600"));
                productBorder.BorderThickness = new(3);

                StackPanel productPanel = new StackPanel();
                if (examProducts[i].ProductDiscountAmount > 15)
                    productPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#7fff00"));
                else
                    productPanel.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFFFCC99"));

                Label nameDataLabel = new Label();
                nameDataLabel.Content = examProducts[i].ProductName;
                productPanel.Children.Add(nameDataLabel);

                Label desciptionDataLabel = new Label();
                desciptionDataLabel.Content = examProducts[i].ProductDescription;
                productPanel.Children.Add(desciptionDataLabel);

                StackPanel manufacturerPanel = new StackPanel();
                manufacturerPanel.Orientation = Orientation.Horizontal;
                Label manufacturerLabel = new Label();
                manufacturerLabel.Content = "Производитель товара:";
                Label manufacturerDataLabel = new Label();
                manufacturerDataLabel.Content = examProducts[i].ProductManufacturer;
                manufacturerPanel.Children.Add(manufacturerLabel);
                manufacturerPanel.Children.Add(manufacturerDataLabel);
                productPanel.Children.Add(manufacturerPanel);

                DockPanel costDockPanel = new DockPanel();//DockPanel чтобы в будущем в этой строке метки со скидкой не смещались, а были по правому краю
                Label costLabel = new Label();
                costLabel.Content = "Цена товара:";
                TextBlock costDataTextBlock = new TextBlock();//TextBlock для возможности задания свойства зачеркнутости текста
                costDataTextBlock.Text = examProducts[i].ProductCost.ToString();
                Label discountLabel = new Label();
                discountLabel.Content = $"Скидка:";
                discountLabel.FontSize = 12;
                Label discountDataLabel = new Label();
                discountDataLabel.FontSize = 12;
                discountDataLabel.Content = examProducts[i].ProductDiscountAmount;
                costDockPanel.Children.Add(costLabel);
                costDockPanel.Children.Add(discountDataLabel);
                DockPanel.SetDock(discountDataLabel, Dock.Right);//размещение по правому краю, чтобы не было смещения при изменении ширины других меток
                costDockPanel.Children.Add(discountLabel);
                DockPanel.SetDock(discountLabel, Dock.Right);//размещение по правому краю, чтобы не было смещения при изменении ширины других меток
                costDockPanel.Children.Add(costDataTextBlock);
                if (examProducts[i].ProductDiscountAmount > 0)//если скидка на товар есть, цена зачеркивается и создается метка с новой ценой
                {
                    costDataTextBlock.TextDecorations = TextDecorations.Strikethrough;
                    costDataTextBlock.VerticalAlignment = VerticalAlignment.Bottom;
                    Label costWithDiscountDataLabel = new Label();
                    decimal resultCost = (decimal)Convert.ToDouble(costDataTextBlock.Text) * (100 - Convert.ToInt32(discountDataLabel.Content)) / 100;
                    costWithDiscountDataLabel.Content = resultCost;
                    costDockPanel.Children.Add(costWithDiscountDataLabel);
                }
                productPanel.Children.Add(costDockPanel);

                ContextMenu productContextMenu = new();
                MenuItem addProductItem = new MenuItem();
                addProductItem.Header = "Добавить к заказу";
                addProductItem.Click += AddProduct_Click;
                productContextMenu.Items.Add(addProductItem);
                productPanel.ContextMenu = productContextMenu;
                productPanel.Tag = i.ToString();

                productBorder.Child = productPanel;
                productsStackPanel.Children.Add(productBorder);
                searchedProductsCount.Content = productsStackPanel.Children.Count + " из 20";
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)//добавление товара в корзину
        {
            // Получение источника события (MenuItem)
            MenuItem menuItem = sender as MenuItem;

            // Проверка, что источник события не null
            if (menuItem != null)
            {
                // Получение контекстного меню, которому принадлежит MenuItem
                ContextMenu productContextMenu = menuItem.Parent as ContextMenu;

                // Проверка, что контекстное меню не null
                if (productContextMenu != null)
                {
                    // Получение элемента управления, для которого было вызвано контекстное меню
                    StackPanel productPanel = productContextMenu.PlacementTarget as StackPanel;

                    // Проверка, что элемент управления не null и имеет свойство Tag
                    if (productPanel != null && productPanel.Tag != null)
                    {
                        var existingProduct = MakeOrderPage.examOrderList.FirstOrDefault(p => p.ProductName == examProducts[Convert.ToInt32(productPanel.Tag)].ProductName);
                        if (existingProduct != null)
                        {
                            existingProduct.ProductCountInOrder += 1;
                        }
                        else
                        {
                            examProducts[Convert.ToInt32(productPanel.Tag)].ProductCountInOrder += 1;
                            MakeOrderPage.examOrderList.Add(examProducts[Convert.ToInt32(productPanel.Tag)]);
                            orderButton.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }

        private void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)//изменение условий вывода товаров с помощью фильтра
        {
            if (SortComboBox.SelectedIndex == 0)
                SortMethod = "ASC";
            else SortMethod = "DESC";

            if (DiscountFilterComboBox.SelectedIndex == 0)
            {
                MinDiscount = 0;
                MaxDiscount = 10;
            }

            if (DiscountFilterComboBox.SelectedIndex == 1)
            {
                MinDiscount = 10;
                MaxDiscount = 15;
            }

            if (DiscountFilterComboBox.SelectedIndex == 2)
            {
                MinDiscount = 15;
                MaxDiscount = 100;
            }

            if (DiscountFilterComboBox.SelectedIndex == 3)
            {
                MinDiscount = 0;
                MaxDiscount = 100;
            }

            CreateProductsList(SearchedText, SortMethod, MinDiscount, MaxDiscount);//обновление списка товаров с фильтрацией
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentFrame.Navigate(new MakeOrderPage());
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentFrame.Navigate(new AuthorizationPage());
        }

        private void GoToYourOrders_Click(object sender, RoutedEventArgs e)
        {
            App.CurrentFrame.Navigate(new OrdersPage());
        }
    }
}