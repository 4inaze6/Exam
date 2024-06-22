using Exam.Pages;
using Microsoft.Data.SqlClient;

namespace Exam
{
    public static class DataAccessLayer
    {
        //получение строки подключения к БД 
        public static string ServerName { get; set; } = @"localhost";//имя сервера, где находится БД

        public static string DBName { get; set; } = "EXAM";//имя БД

        public static string Login { get; set; } = "";//Ваш логин для доступа к серверу

        public static string Password { get; set; } = "";//Ваш пароль для доступа к серверу

        public static string ConnectionString//создание строки подключения
        {
            get
            {
                SqlConnectionStringBuilder builder = new()
                {
                    DataSource = ServerName,
                    InitialCatalog = DBName,
                    UserID = Login,
                    Password = Password,
                    IntegratedSecurity = true,
                    TrustServerCertificate = true
                };
                return builder.ConnectionString;
            }
        }

        public static List<ExamProduct> GetExamProductDataFromDB(string subs, string sortMethod, double minDiscount, double maxDiscount)
        {
            List<ExamProduct> examProducts = new List<ExamProduct>();
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = "SELECT * FROM ExamProduct WHERE ProductDiscountAmount >= @minDiscount AND ProductDiscountAmount < @maxDiscount " +
                $"AND ProductName Like '%{subs}%' ORDER BY ProductCost*(100-ProductDiscountAmount)/100 {sortMethod}";//запрос для получения данных
            SqlCommand sqlCommand = new(query, connection);//получение данных из БД
            sqlCommand.Parameters.AddWithValue("@minDiscount", minDiscount);
            sqlCommand.Parameters.AddWithValue("@maxDiscount", maxDiscount);
            using (SqlDataReader reader = sqlCommand.ExecuteReader())//чтение полученных данных построчно
            {
                while (reader.Read())
                {
                    ExamProduct examProduct = new()//создание объекта класса с присваиванием его свойствам значений из таблицы в БД
                    {
                        ProductArticleNumber = reader["ProductArticleNumber"].ToString(),
                        ProductName = reader["ProductName"].ToString(),
                        ProductDescription = reader["ProductDescription"].ToString(),
                        ProductCategory = reader["ProductCategory"].ToString(),
                        ProductPhoto = reader["ProductPhoto"] != DBNull.Value ? reader["ProductPhoto"].ToString() : "",
                        ProductManufacturer = reader["ProductManufacturer"].ToString(),
                        ProductCost = Convert.ToDecimal(reader["ProductCost"]),
                        ProductDiscountAmount = Convert.ToInt32(reader["ProductDiscountAmount"]),
                        ProductQuantityInStock = Convert.ToInt32(reader["ProductQuantityInStock"]),
                        ProductStatus = reader["ProductStatus"].ToString(),
                    };
                    examProducts.Add(examProduct);
                }
            }
            return examProducts;
        }

        public static List<ExamOrder> GetExamOrderDataWithId(int id)
        {
            List<ExamOrder> examOrders = new();
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = "SELECT * FROM ExamOrder WHERE UserId = @id";//запрос для получения данных
            SqlCommand sqlCommand = new(query, connection);//получение данных из БД
            sqlCommand.Parameters.AddWithValue("@id", id);
            using (SqlDataReader reader = sqlCommand.ExecuteReader())//чтение полученных данных построчно
            {
                while (reader.Read())
                {
                    ExamOrder examOrder = new()//создание объекта класса с присваиванием его свойствам значений из таблицы в БД
                    {
                        OrderID = Convert.ToInt32(reader["OrderId"]),
                        UserID = Convert.ToInt32(reader["UserId"]),
                        OrderStatus = reader["OrderStatus"].ToString(),
                        OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                        OrderDeliveryDate = Convert.ToDateTime(reader["OrderDeliveryDate"]),
                        OrderPickupPoint = Convert.ToInt32(reader["OrderPickupPoint"]),
                        OrderPickupCode = Convert.ToInt32(reader["OrderPickupCode"]),
                    };
                    examOrders.Add(examOrder);
                }
            }
            return examOrders;
        }

        public static ExamProduct GetProductDataFromDB(int productId)
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = $"SELECT * FROM ExamProduct ORDER BY ProductArticleNumber OFFSET @productId ROWS FETCH NEXT 1 ROW ONLY";//запрос для получения данных товара по его порядковому номеру
            SqlCommand sqlCommand = new(query, connection);//получение данных из БД
            sqlCommand.Parameters.AddWithValue("@productId", productId - 1);
            using (SqlDataReader reader = sqlCommand.ExecuteReader())//чтение полученных данных построчно
            {
                ExamProduct examProduct = new()//создание объекта класса с присваиванием его свойствам значений из таблицы в БД
                {
                    ProductArticleNumber = reader["ProductArticleNumber"].ToString(),
                    ProductName = reader["ProductName"].ToString(),
                    ProductDescription = reader["ProductDescription"].ToString(),
                    ProductCategory = reader["ProductCategory"].ToString(),
                    ProductPhoto = reader["ProductPhoto"] != DBNull.Value ? reader["ProductPhoto"].ToString() : "",
                    ProductManufacturer = reader["ProductManufacturer"].ToString(),
                    ProductCost = Convert.ToDecimal(reader["ProductCost"]),
                    ProductDiscountAmount = Convert.ToInt32(reader["ProductDiscountAmount"]),
                    ProductQuantityInStock = Convert.ToInt32(reader["ProductQuantityInStock"]),
                    ProductStatus = reader["ProductStatus"].ToString(),
                    ProductCountInOrder = 0
                };
                return examProduct;//возврат данных товара из таблицы ExamProduct
            }
        }

        public static bool UserAuthorization(string login, string password)//метод для проверки существует ли пользователь, и если да, то заполняет статические свойства класса CurrentUser 
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = "SELECT * FROM ExamUser WHERE UserLogin = @login AND UserPassword = @password";//запрос для поиска пользователя, чьи логин и пароль совпадают с введенными
            SqlCommand sqlCommand = new(query, connection);//получение данных из БД
            sqlCommand.Parameters.AddWithValue("@login", login);
            sqlCommand.Parameters.AddWithValue("@password", password);
            SqlDataReader reader = sqlCommand.ExecuteReader();

            if (reader.Read())
            {
                CurrentUser.UserID = Convert.ToInt32(reader["UserID"]);
                CurrentUser.RoleID = Convert.ToInt32(reader["RoleID"]);
                CurrentUser.UserSurname = reader["UserSurname"].ToString();
                CurrentUser.UserName = reader["UserName"].ToString();
                CurrentUser.UserPatronymic = reader["UserPatronymic"].ToString();
                CurrentUser.UserLogin = reader["UserLogin"].ToString();
                CurrentUser.UserPassword = reader["UserPassword"].ToString();
                return true;//удачный вход
            }
            return false;
        }
        public static void GetPickupPoints()
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = $"SELECT * FROM ExamPickupPoint";//запрос для получения адресов пунктов выдачи 
            SqlCommand sqlCommand = new(query, connection);//получение данных из БД
            using (SqlDataReader reader = sqlCommand.ExecuteReader())//чтение полученных данных построчно
            {
                while (reader.Read())
                {
                    ExamPickupPoint point = new ExamPickupPoint()
                    {
                        OrderPickupPoint = Convert.ToInt32(reader["OrderPickupPoint"]),
                        Address = reader["Address"].ToString()
                    };
                    MakeOrderPage.examPickupPoints.Add(point);
                }

            }
        }

        public static void UpdateExamOrder(int userID, string orderStatus, DateTime orderDate, DateTime orderDeliveryDate, int orderPickupPoint, int orderPickupCode)
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = $"INSERT INTO ExamOrder(UserID, OrderStatus, OrderDate, OrderDeliveryDate, OrderPickupPoint, OrderPickupCode) VALUES(@userID, @orderStatus, @orderDate, @orderDeliveryDate, @orderPickupPoint, @orderPickupCode);";
            SqlCommand sqlCommand = new(query, connection);//получение данных из БД
            //sqlCommand.Parameters.AddWithValue("@orderID", orderID);
            sqlCommand.Parameters.AddWithValue("@userID", userID != 0 ? userID : DBNull.Value);
            sqlCommand.Parameters.AddWithValue("@orderStatus", orderStatus);
            sqlCommand.Parameters.AddWithValue("@orderDate", orderDate);
            sqlCommand.Parameters.AddWithValue("@orderDeliveryDate", orderDeliveryDate);
            sqlCommand.Parameters.AddWithValue("@orderPickupPoint", orderPickupPoint);
            sqlCommand.Parameters.AddWithValue("@orderPickupCode", orderPickupCode);
            sqlCommand.ExecuteNonQuery();
        }

        public static void UpdateExamOrderProduct(int orderID, string productArticleNumber, int amount)
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = $"INSERT INTO ExamOrderProduct VALUES(@orderID, @productArticleNumber, @amount);";
            SqlCommand sqlCommand = new(query, connection);//получение данных из БД
            //sqlCommand.Parameters.AddWithValue("@orderID", orderID);
            sqlCommand.Parameters.AddWithValue("@orderID", orderID);
            sqlCommand.Parameters.AddWithValue("@productArticleNumber", productArticleNumber);
            sqlCommand.Parameters.AddWithValue("@amount", amount);
            sqlCommand.ExecuteNonQuery();
        }

        public static void GetExistingPickupCodes()
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = $"SELECT OrderPickupCode FROM ExamOrder";//запрос для получения адресов пунктов выдачи 
            SqlCommand sqlCommand = new(query, connection);//получение данных из БД
            using (SqlDataReader reader = sqlCommand.ExecuteReader())//чтение полученных данных построчно
            {
                while (reader.Read())
                {
                    MakeOrderPage.existingPickupCodes.Add(Convert.ToInt32(reader["OrderPickupCode"]));
                }
            }
        }

        public static int GetLastOrderID()
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = $"SELECT Max(OrderID) FROM ExamOrder";
            SqlCommand sqlCommand = new(query, connection);//получение данных из БД
            try
            {
                return Convert.ToInt32(sqlCommand.ExecuteScalar());
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static string GetProductNameWithArticle(string article)
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = $"SELECT ProductName FROM ExamProduct Where ProductArticleNumber = @article";
            SqlCommand sqlCommand = new(query, connection);
            sqlCommand.Parameters.AddWithValue("@article", article);
            return sqlCommand.ExecuteScalar().ToString();
        }

        public static int GetProductAmountInOrderWithArticle(int orderId, string article)
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = $"SELECT Amount FROM ExamOrderProduct Where ProductArticleNumber = @article AND OrderID = @orderId";
            SqlCommand sqlCommand = new(query, connection);
            sqlCommand.Parameters.AddWithValue("@article", article);
            sqlCommand.Parameters.AddWithValue("@orderId", orderId);
            return Convert.ToInt32(sqlCommand.ExecuteScalar());
        }

        public static decimal GetSummOrder(int orderId)
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = $"SELECT SUM(ProductCost*(100-ProductDiscountAmount)/100*ExamOrderProduct.Amount) FROM ExamProduct INNER JOIN ExamOrderProduct " +
                $"ON ExamProduct.ProductArticleNumber = ExamOrderProduct.ProductArticleNumber INNER JOIN ExamOrder " +
                $"ON ExamOrderProduct.OrderID = ExamOrder.OrderID WHERE ExamOrder.OrderID = @orderId";
            SqlCommand sqlCommand = new(query, connection);
            sqlCommand.Parameters.AddWithValue("@orderId", orderId);
            return Convert.ToDecimal(sqlCommand.ExecuteScalar());
        }

        public static decimal GetDiscountOrder(int orderId)
        {
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = $"SELECT SUM((ProductCost - ProductCost*(100-ProductDiscountAmount)/100)*ExamOrderProduct.Amount) " +
                $"FROM ExamProduct INNER JOIN ExamOrderProduct " +
                $"ON ExamProduct.ProductArticleNumber = ExamOrderProduct.ProductArticleNumber INNER JOIN ExamOrder " +
                $"ON ExamOrderProduct.OrderID = ExamOrder.OrderID WHERE ExamOrder.OrderID = @orderId";
            SqlCommand sqlCommand = new(query, connection);
            sqlCommand.Parameters.AddWithValue("@orderId", orderId);
            return Convert.ToDecimal(sqlCommand.ExecuteScalar());
        }

        public static List<ExamOrderProduct> GetOrderProductWithOrderId(int orderId)
        {
            List<ExamOrderProduct> examOrderProducts = new();
            using SqlConnection connection = new(ConnectionString);
            connection.Open();//установка соединения с БД
            string query = $"SELECT * FROM ExamOrderProduct Where OrderID = @orderId";
            SqlCommand sqlCommand = new(query, connection);
            sqlCommand.Parameters.AddWithValue("@orderId", orderId);
            using (SqlDataReader reader = sqlCommand.ExecuteReader())//чтение полученных данных построчно
            {
                while (reader.Read())
                {
                    ExamOrderProduct examOrderProduct = new()//создание объекта класса с присваиванием его свойствам значений из таблицы в БД
                    {
                        OrderID = Convert.ToInt32(reader["OrderId"]),
                        ProductArticleNumber = reader["ProductArticleNumber"].ToString(),
                        Amount = Convert.ToInt32(reader["Amount"])
                    };
                    examOrderProducts.Add(examOrderProduct);
                }
            }
            return examOrderProducts;
        }
    }
}
