using BetManager.Core.DbModels.Predictions;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.Domains.Predictions
{
    public class SpecificPredicitionManager : ISpecificPredicitionManager
    {
        public virtual ICollection<SpecificPredicition> GetAll(object input)
        {
            ICollection<SpecificPredicition> tips = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                tips = conn.Query<SpecificPredicition>("BM_SpecificPrediction_ALL", input, commandType: CommandType.StoredProcedure).ToList();
                conn.Close();
            }
            return tips;
        }

        public virtual SpecificPredicition GetById(int id)
        {
            SpecificPredicition tip = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                tip = conn.Query<SpecificPredicition>("BM_SpecificPrediction_DETAIL", new { @id = id }, commandType: CommandType.StoredProcedure).FirstOrDefault();
                conn.Close();
            }

            return tip;
        }
    }
}
