using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DrinkingBuddy.Models
{
    // Models used as parameters to AccountController actions.

    public class AddExternalLoginBindingModel
    {
        [Required]
        [Display(Name = "External access token")]
        public string ExternalAccessToken { get; set; }
    }

    public class ChangePasswordBindingModel
    {
       
        public string OldPassword { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string NewPassword { get; set; }
         public string PhoneNumber { get; set; }

      
    }


    public class UserUpdateModel
    {
        
        public int PatronsID { get; set; }
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Required]
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Required]
        [Display(Name = "Suburd")]
        public string Suburb { get; set; }
        [Required]
        [Display(Name = "PostCode")]
        public string PostCode { get; set; }
       
        [Required]
        [Display(Name = "Date of Time")]
        public DateTime DateOfBirth { get; set; }
        [Required]
        [Display(Name = "PhoneNumber")]
        public string PhoneNumber { get; set; }

       
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

       
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }


    public class RegisterBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {10} characters long.", MinimumLength = 10)]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }
       

    }


    public class RegistersocialBindingModel
    {
       
        public string EmailAddress { get; set; }

      
        public string FirstName { get; set; }

       
        public string LastName { get; set; }

       
        public string Password { get; set; }

       
        public string ConfirmPassword { get; set; }

       
        public string PhoneNumber { get; set; }

        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }


    }



    public class RegisterExternalBindingModel
    {
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class RemoveLoginBindingModel
    {
        [Required]
        [Display(Name = "Login provider")]
        public string LoginProvider { get; set; }

        [Required]
        [Display(Name = "Provider key")]
        public string ProviderKey { get; set; }
    }

    public class SetPasswordBindingModel
    {
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResponseModel
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public object Data { get; set; }

    }

    public class RegisterResponseModel
    {
        public List<string> Message { get; set; }
        public string Status { get; set; }
        public object Data { get; set; }

    }


    public class LoginModel
    {
      public string Email { get; set; }
      public string Password { get; set; }
      public string DeviceToken { get; set; }
      public string DeviceType { get; set; }
      public DateTime LastLogOn { get; set; }
    }

    public class UpdateLoginDevice
    {
        public int PatronsID { get; set; }
        public string DeviceToken { get; set; }
        public string DeviceType { get; set; }

    }
   
}
