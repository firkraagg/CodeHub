using System.ComponentModel.DataAnnotations;

namespace CodeHub.Data.Entities
{
    public class ProgrammingLanguage
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string MonacoName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string Version { get; set; } = string.Empty;

        [Required]
        public string DockerImage { get; set; } = string.Empty;
    }
}
