using System.ComponentModel.DataAnnotations;

namespace CodeHub.Data.Entities
{
    public class Tip
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(150)]
        public string Text { get; set; }

    }
}
