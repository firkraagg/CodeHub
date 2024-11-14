using System.ComponentModel.DataAnnotations;

namespace CodeHub.Data.Models
{
    public class User
    {
        [Key] public int Id { get; set; } = 1;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
