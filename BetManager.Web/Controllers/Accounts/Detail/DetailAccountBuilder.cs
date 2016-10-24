using BetManager.Web.Mvc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetManager.Web.Mvc;
using BetManager.Core.Domains.Accounts;

namespace BetManager.Web.Controllers.Accounts.Detail
{
    public class DetailAccountBuilder : IModelBuilder<DetailAccountViewModel, int>
    {
        private readonly IUserManager _userManager;
        public DetailAccountBuilder()
        {
            _userManager = new UserManager();
        }

        public ModelBuilderResult<DetailAccountViewModel> Build(int id)
        {
            var result = new ModelBuilderResult<DetailAccountViewModel>();

            var user = _userManager.GetById(id);

            result.Model = new DetailAccountViewModel
            {
                ID = user.ID,
                UserName = user.UserName,
                LastLogin = user.LastLogin,
                Form = user.Form,
                Odd = user.Odd
            };

            return result;
        }
    }
}