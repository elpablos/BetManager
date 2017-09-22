using BetManager.Web.Mvc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetManager.Web.Mvc;
using BetManager.Core.Domains.Accounts;

namespace BetManager.Web.Controllers.Accounts.Detail
{
    public class DetailAccountHandler : IModelHandler<DetailAccountViewModel>
    {
        private readonly IUserManager _userManager;
        public DetailAccountHandler()
        {
            _userManager = new UserManager();
        }

        public ModelHandlerResult Handle(DetailAccountViewModel model)
        {
            var result = new ModelHandlerResult();

            var x =_userManager.Update(new Core.DbModels.Accounts.User
            {
                ID = model.ID,
                Odd = model.Odd,
                Category = model.Category
            });

            result.Data = x > 0;

            return result;
        }
    }
}