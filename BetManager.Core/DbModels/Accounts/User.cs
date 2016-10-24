using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.DbModels.Accounts
{
    public class User
    {
        public int ID { get; set; }

        public bool IsActive { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public string Salt { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public virtual ICollection<Role> Roles { get; set; }

        public int Form { get; set; }

        public double Odd { get; set; }

        public User()
        {
            Roles = new HashSet<Role>();
        }
    }
}
