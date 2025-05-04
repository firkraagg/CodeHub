using CodeHub.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeHub.Data.Entities
{
    public class ProblemHint
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string Hint { get; set; }

        [ForeignKey(nameof(Problem))]
        public int ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
    }
}