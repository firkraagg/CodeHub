using CodeHub.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeHub.Data.Entities
{
    public class TestCase
    {
        [Required]
        public int Id { get; set; }
        public string Arguments { get; set; } = string.Empty;

        [Required(ErrorMessage = "Očakávaný výstup je povinný.")]
        public string ExpectedOutput { get; set; } = string.Empty;

        [Required(ErrorMessage = "Výber dátového typu je povinný.")]
        public string OutputType { get; set; } = "string";

        [ForeignKey(nameof(Problem))]
        public int ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
    }
}