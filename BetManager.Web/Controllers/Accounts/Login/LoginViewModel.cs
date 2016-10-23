using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BetManager.Web.Controllers.Accounts.Login
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Položka je povinná")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Položka je povinná")]
        public string Password { get; set; }

        public bool IsPersistent { get; set; }
    }
}