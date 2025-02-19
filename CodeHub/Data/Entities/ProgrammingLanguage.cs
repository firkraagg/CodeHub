using System.ComponentModel.DataAnnotations;

namespace CodeHub.Data.Entities
{
    public class ProgrammingLanguage
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string ApiName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string DisplayName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string MonacoName { get; set; } = string.Empty;

        [Required, MaxLength(50)] 
        public string Version { get; set; } = string.Empty;
    }
}
