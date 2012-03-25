namespace ProjectManagement
{
    public class UserAccount
    {
        public string Username { get; set; }

        public UserStatus Status { get; set; }
    }

    public enum UserStatus
    {
        Active,
        Inactive
    }
}