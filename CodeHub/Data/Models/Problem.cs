using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CodeHub.Data.Entities;

namespace CodeHub.Data.Models
{
    public class Problem
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Musíte zadať názov úlohy"), MaxLength(50)]
        public string Title { get; set; }
        [Required(ErrorMessage = "Musíte zadať popis úlohy"), MaxLength(200)]
        public string Description { get; set; }
        [Range(0, 100)]
        public double Acceptance { get; set; }

        [Required(ErrorMessage = "Vyberte obtiažnosť")]
        public int Difficulty { get; set; }
        [Required(ErrorMessage = "Vyberte programovací jazyk")]
        public string Language { get; set; }
        [Required(ErrorMessage = "Pole nebolo vyplnené"), MaxLength(30)]
        public string RequiredInput { get; set; }
        [Required(ErrorMessage = "Pole nebolo vyplnené"), MaxLength(30)]
        public string RequiredOutput { get; set; }
        public string Constraints { get; set; }
        public string Hints { get; set; }

        [ForeignKey("User")]
        public int UserID { get; set; }
        public virtual User User { get; set; }
    }
}
