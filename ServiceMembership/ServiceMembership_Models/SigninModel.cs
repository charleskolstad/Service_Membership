using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceMembership_Models
{
    public abstract class SigninModel
    {
        [Required(ErrorMessage ="User Name is required.")]
        public string UserName { get; set; }
    }

    public class LoginModel : SigninModel
    {
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; }
    }

    public class RecoverModel : SigninModel
    {
        [Required(ErrorMessage ="Email is required.")]
        public string Email { get; set; }
    }
}
