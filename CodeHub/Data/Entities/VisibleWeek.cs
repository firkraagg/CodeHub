using CodeHub.Data.Models;
using System.ComponentModel.DataAnnotations;

namespace CodeHub.Data.Entities
{
    public class VisibleWeek
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int NumberOfWeek { get; set; }
    }
}
