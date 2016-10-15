using BetManager.Core.DbModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.Domains
{
    public class TipManager : ITipManager
    {
        public virtual ICollection<Tip> GetAll()
        {
            ICollection<Tip> tips = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                tips = conn.Query<Tip>("BM_Tip_ALL", commandType: CommandType.StoredProcedure).ToList();
                conn.Close();
            }
            return tips;
        }

        public virtual Tip GetById(int id)
        {
            Tip tip = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                tip = conn.Query<Tip>("BM_Tip_DETAIL", new { @id = id }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                conn.Close();
            }

            return tip;
        }
    }
}
