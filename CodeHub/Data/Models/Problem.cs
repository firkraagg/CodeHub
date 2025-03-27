using CodeHub.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeHub.Data.Models
{
    public class Problem
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Musíte zadať názov úlohy"), MaxLength(150)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Musíte zadať popis úlohy"), MaxLength(1000)]
        public string Description { get; set; }

        [Range(0, 100)]
        public double Acceptance { get; set; }

        [Required, Range(1, 5, ErrorMessage = "Vyberte obťažnosť")]
        public int Difficulty { get; set; }

        [MaxLength(1500)]
        public string? DefaultCode { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<ProblemExample> Examples { get; set; }

        public virtual ICollection<ProblemConstraint> Constraints { get; set; }

        public virtual ICollection<ProblemHint> Hints { get; set; }

        public virtual ICollection<TestCase> TestCases { get; set; }

        [ForeignKey(nameof(ProgrammingLanguage)), Required, Range(1, Int32.MaxValue, ErrorMessage = "Vyberte programovací jazyk")]
        public int LanguageID { get; set; }
        public virtual ProgrammingLanguage ProgrammingLanguage { get; set; }

        [ForeignKey(nameof(User))]
        public int UserID { get; set; }
        public virtual User User { get; set; }

        public ICollection<Tag> Tags { get; set; }
        public ICollection<ProblemAttempt> SolvedByUsers { get; set; }
    }
}
