using System;
using System.Data.SqlClient;

namespace BetManager.Core.Domains
{
    public class ConnectionFactory
    {
        public static SqlConnection GetConnection(string name)
        {
            return new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[name].ConnectionString);
        }
    }
}
