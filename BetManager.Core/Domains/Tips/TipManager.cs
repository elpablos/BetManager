using BetManager.Core.DbModels.Tips;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.Domains.Tips
{
    public class TipManager : ITipManager
    {
        public virtual ICollection<Tip> GetAll(object input)
        {
            ICollection<Tip> tips = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                tips = conn.Query<Tip>("BM_Tip_ALL", input, commandType: CommandType.StoredProcedure).ToList();
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

        public virtual ICollection<TipDetailGraph> GetGraph(object input)
        {
            ICollection<TipDetailGraph> tips = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                tips = conn.Query<TipDetailGraph>("BM_Tip_DETAIL_Graph", input, commandType: CommandType.StoredProcedure).ToList();
                conn.Close();
            }

            return tips;
        }

        public virtual int TipGenerate()
        {
            int ret = -1;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                ret = conn.Execute("BM_Tip_GENERATE", null, commandType: CommandType.StoredProcedure);
                conn.Close();
            }
            return ret;
        }

        public virtual ICollection<TipAllPoisson> GetAllPoisson(object input)
        {
            ICollection<TipAllPoisson> tips = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                tips = conn.Query<TipAllPoisson>("BM_Event_DETAIL_Poisson", input, commandType: CommandType.StoredProcedure).ToList();
                conn.Close();
            }
            return tips;
        }
    }
}
