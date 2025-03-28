using CodeHub.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace CodeHub.Data.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? DisplayName { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Group { get; set; }

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        public bool LdapUser { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string Role { get; set; } = "Študent";
        public byte[] ProfileImage { get; set; } = null!;
        public List<Problem> Problems { get; set; } = new();
        public ICollection<ProblemAttempt> SolvedProblems { get; set; }

    }
}
