using System.ComponentModel.DataAnnotations;
using System.Web.Security;

namespace Informer.Models
{


    public class RegisterExternalLoginModel
    {
        [Required]
        [Display(Name = @"Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = @"Email адрес содержит ошибки")]
        [Display(Name = @"Email адрес")]
        public string Email { get; set; }

        public string ExternalLoginData { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = @"Текущий пароль")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = @"Значение ""{0}"" должно содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = @"Новый пароль")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = @"Подтверждение пароля")]
        [Compare("NewPassword", ErrorMessage = @"Новый пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
    }


    public class LoginModel
    {
        [Required]
        [Display(Name = @"Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = @"Пароль")]
        public string Password { get; set; }

        [Display(Name = @"Запомнить меня")]
        public bool RememberMe { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = @"Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = @"Значение ""{0}"" должно содержать не менее {2} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = @"Пароль")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", ErrorMessage = @"Email адрес содержит ошибки")] 
        [Display(Name = @"Email адрес")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = @"Подтверждение пароля")]
        [Compare("Password", ErrorMessage = @"Пароль и его подтверждение не совпадают.")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }
        public string ProviderDisplayName { get; set; }
        public string ProviderUserId { get; set; }
    }


    public interface IMembershipService
    {
        int MinPasswordLength { get; }
        bool ValidateUser(string userName, string password);
        MembershipCreateStatus CreateUser(string userName, string password, string email);
        bool ChangePassword(string userName, string oldPassword, string newPassword);
        // Additions to Interface for EmailConfirmation...
        void SendConfirmationEmail(string userName);
    }

}
