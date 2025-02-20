﻿using CodeHub.Data.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CodeHub.Data.Entities
{
    public class ProblemConstraint
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string Constraint { get; set; }

        [ForeignKey(nameof(Problem))]
        public int ProblemId { get; set; }
        public virtual Problem Problem { get; set; }
    }
}
