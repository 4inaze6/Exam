namespace Exam
{
    public class CurrentUser
    {
        public static bool IsGuest { get; set; } = false;
        public static int UserID { get; set; }
        public static int RoleID { get; set; }
        public static string UserSurname { get; set; }
        public static string UserName { get; set; }
        public static string UserPatronymic { get; set; }
        public static string UserLogin { get; set; }
        public static string UserPassword { get; set; }
    }
}
