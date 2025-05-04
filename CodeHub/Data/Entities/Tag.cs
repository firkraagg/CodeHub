using CodeHub.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace CodeHub.Data.Entities
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public ICollection<Problem> Problems { get; set; }
    }
}