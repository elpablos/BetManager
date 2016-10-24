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

        private const string SqlLogin = @"select ID, Password, Salt, Odd, Form from CR_User where UserName=@UserName";

        private const string SqlUpdate = "update CR_User set Odd=@Odd, Form=@Form where ID=@ID";

        private const string SqlSelect = @"select ID, IsActive, UserName, LastLogin, Odd, Form from CR_User where ID=@ID";

        private const string SqlUpdateLogin = "update CR_User set LastLogin=@LastLogin where ID=@ID";

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

        public virtual int Update(User user)
        {
            int ret = -1;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                ret = conn.Execute(SqlUpdate, user, commandType: CommandType.Text);
                conn.Close();
            }
            return ret;
        }

        public virtual User GetById(int id)
        {
            User tip = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                tip = conn.Query<User>(SqlSelect, new { @id = id }, commandType: CommandType.Text).FirstOrDefault();
                conn.Close();
            }

            return tip;
        }

        public virtual bool Login(User user, bool isPersistent)
        {
            User loggedUser = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                loggedUser = conn.Query<User>(SqlLogin, user, commandType: CommandType.Text).FirstOrDefault();
               
                if (loggedUser != null)
                {
                    var hash = _cryptoService.Compute(user.Password, loggedUser.Salt);
                    if (_cryptoService.Compare(hash, loggedUser.Password))
                    {
                        var claims = new List<Claim>();

                        claims.Add(new Claim(ClaimTypes.Sid, loggedUser.ID.ToString()));
                        claims.Add(new Claim(ClaimTypes.IsPersistent, isPersistent.ToString()));
                        claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserName));
                        claims.Add(new Claim(ClaimTypes.Name, user.UserName));
                        claims.Add(new Claim("Odd", loggedUser.Odd.ToString()));
                        claims.Add(new Claim("Form", loggedUser.Form.ToString()));

                        var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                        // add to user here!
                        AuthenticationManager.SignIn(new AuthenticationProperties()
                        {
                            AllowRefresh = true,
                            IsPersistent = isPersistent,
                            ExpiresUtc = DateTime.UtcNow.AddDays(7)
                        }, identity);

                        conn.Execute(SqlUpdateLogin, new { @LastLogin = DateTime.Now, @ID = loggedUser.ID }, commandType: CommandType.Text);

                        return true;
                    }
                }

                conn.Close();
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
