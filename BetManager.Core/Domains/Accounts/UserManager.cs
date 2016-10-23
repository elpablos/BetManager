using BetManager.Core.DbModels.Accounts;
using Dapper;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using SimpleCrypto;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BetManager.Core.Domains.Accounts
{
    public class UserManager : IUserManager
    {
        private const string SqlInsert = @"insert into CR_User ([UserName], [Password], [Salt], [LastLogin])
values (@UserName, @Password, @Salt, @LastLogin)";

        private const string SqlLogin = @"select Password, Salt from CR_User where UserName=@UserName";

        private readonly ICryptoService _cryptoService;

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.Current.GetOwinContext().Authentication; }
        }

        public UserManager()
        {
            _cryptoService = new PBKDF2();
        }

        public virtual int Insert(User user)
        {
            user.Salt = _cryptoService.GenerateSalt();
            user.Password = _cryptoService.Compute(user.Password, user.Salt);

            int ret = -1;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                ret = conn.Execute(SqlInsert, user, commandType: CommandType.Text);
                conn.Close();
            }
            return ret;
        }

        public virtual bool Login(User user, bool isPersistent)
        {
            User loggedUser = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                loggedUser = conn.Query<User>(SqlLogin, user, commandType: CommandType.Text).FirstOrDefault();
                conn.Close();
            }

            if (loggedUser != null)
            {
                var hash = _cryptoService.Compute(user.Password, loggedUser.Salt);
                if (_cryptoService.Compare(hash, loggedUser.Password))
                {
                    var claims = new List<Claim>();

                    // create *required* claims
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserName));
                    claims.Add(new Claim(ClaimTypes.Name, user.UserName));

                    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                    // add to user here!
                    AuthenticationManager.SignIn(new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = isPersistent,
                        ExpiresUtc = DateTime.UtcNow.AddDays(7)
                    }, identity);

                    return true;
                }
            }

            return false;

        }

        public virtual void Logout()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie,
                                    DefaultAuthenticationTypes.ExternalCookie);
        }
    }
}
