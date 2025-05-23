﻿using CodeHub.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeHub.Data.Entities
{
    public class ProblemAttempt
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Problem))]
        public int problemId { get; set; }

        [ForeignKey(nameof(User))]
        public int userId { get; set; }
        public DateTime AttemptedAt { get; set; } = DateTime.Now;
        public string SourceCode { get; set; } = string.Empty;
        public double Points { get; set; }
        public int PassedTestCases { get; set; }
        public Problem Problem { get; set; }
        public User User { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
