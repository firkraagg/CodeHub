﻿using System.ComponentModel.DataAnnotations;

namespace CodeHub.Data.Models
{
    public class RegistrationModel
    {
        [Required(ErrorMessage = "Zadajte prezývku.")]
        [MaxLength(50, ErrorMessage = "Prezývka je príliš dlhá. Nemôže presiahnuť viac ako 50 znakov.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadajte e-mailovú adresu.")]
        [RegularExpression(@"^[a-zA-Z0-9]+@[a-zA-Z]+\.[a-zA-Z]+$", ErrorMessage = "Zlý formát e-mailu.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadajte heslo.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,24}$", ErrorMessage = "Zlý formát hesla.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrďte heslo.")]
        [Compare("Password", ErrorMessage = "Heslá sa musia zhodovať.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class LoginModel
    {
        [Required(ErrorMessage = "Zadajte meno.")]
        public string Nickname { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadajte heslo.")]
        public string Password { get; set; } = string.Empty;
    }

    public class EditModel
    {
        [MaxLength(50, ErrorMessage = "Celé meno nemôže byť dlhšie ako 50 znakov.")]
        public string DisplayName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadajte prezývku.")]
        [MaxLength(50, ErrorMessage = "Prezývka je príliš dlhá. Nemôže presiahnuť viac ako 50 znakov.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadajte e-mailovú adresu.")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Zlý formát e-mailu.")]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20, ErrorMessage = "Študijná skupina môže mať maximálne 10 znakov.")]
        public string Group { get; set; } = string.Empty;
    }
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Zadajte staré heslo.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zadajte nové heslo.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,24}$", ErrorMessage = "Zlý formát hesla.")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Potvrďte heslo.")]
        [Compare("NewPassword", ErrorMessage = "Heslá sa musia zhodovať.")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}