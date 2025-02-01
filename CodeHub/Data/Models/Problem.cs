using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CodeHub.Data.Entities;

namespace CodeHub.Data.Models
{
    public class Problem
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Title { get; set; }
        [Required, MaxLength(200)]
        public string Description { get; set; }
        [Range(1, 100)]
        public double Acceptance { get; set; }
        [Required, Range(1, 3)]
        public int Difficulty { get; set; }
        [Required]
        public char Language { get; set; }
        [Required, MaxLength(30)]
        public string RequiredInput { get; set; }
        [Required, MaxLength(30)]
        public string RequiredOutput { get; set; }
        public string Constraints { get; set; }
        [ForeignKey("User")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
    }
}
