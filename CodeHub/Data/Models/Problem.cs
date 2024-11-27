using System.ComponentModel.DataAnnotations;
using CodeHub.Data.Entities;

namespace CodeHub.Data.Models
{
    public class Problem
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public double Acceptance { get; set; }
        public int Difficulty { get; set; }
        public char Language { get; set; }
        public string RequiredInput { get; set; }
        public string RequiredOutput { get; set; }
        public string Constraints { get; set; }
        public User User { get; set; }
    }
}
