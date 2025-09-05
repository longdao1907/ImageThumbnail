namespace AuthAPI.Core.Domain.Entities
{
    /// <summary>
    /// Represents a user account.
    /// </summary>
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
