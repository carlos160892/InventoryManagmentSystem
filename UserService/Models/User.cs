namespace UserService.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; } // Hash de la contraseña
        public string Role { get; set; } // Por ejemplo: "Admin", "Customer"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
