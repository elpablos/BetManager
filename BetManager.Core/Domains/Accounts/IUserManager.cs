using BetManager.Core.DbModels.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.Domains.Accounts
{
    public interface IUserManager
    {
        int Insert(User user);

        bool Login(User user, bool isPersistent);

        void Logout();
    }
}
