using BetManager.Web.Mvc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetManager.Web.Mvc;
using BetManager.Core.Domains.Accounts;

namespace BetManager.Web.Controllers.Accounts.Login
{
    public class LoginAccountHandler : IModelHandler<LoginAccountViewModel>
    {
        private readonly IUserManager _userManager;

        public LoginAccountHandler()
        {
            _userManager = new UserManager();
        }

        public ModelHandlerResult Handle(LoginAccountViewModel model)
        {
            var result = new ModelHandlerResult();
            var check = _userManager.Login(new Core.DbModels.Accounts.User
            {
                UserName = model.UserName,
                Password = model.Password
            }, model.IsPersistent);

            if (check)
            {
                result.HttpStatusCode = System.Net.HttpStatusCode.OK;
                result.Data = true;
            }
            else
            {
                result.HttpStatusCode = System.Net.HttpStatusCode.NotAcceptable;
                result.Data = false;
            }

            return result;
        }

        public void Logout()
        {
            _userManager.Logout();
        }
    }
}