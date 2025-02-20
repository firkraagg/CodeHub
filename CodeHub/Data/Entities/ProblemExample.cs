using CodeHub.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CodeHub.Data.Entities
{
    public class ProblemExample
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Input { get; set; }

        [Required, MaxLength(100)]
        public string Output { get; set; }

        [MaxLength(250)]
        public string? Explanation { get; set; }

        [ForeignKey(nameof(Problem))]
        public int ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
    }
}
