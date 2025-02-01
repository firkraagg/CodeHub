using System.ComponentModel.DataAnnotations;

namespace CodeHub.Data.Entities
{
    public class ProgrammingLanguage
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string LanguageName { get; set; } = string.Empty;
    }
}
