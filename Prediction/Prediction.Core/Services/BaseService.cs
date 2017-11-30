using System;
using System.Data.SQLite;

namespace Prediction.Core.Services
{
    public abstract class BaseService : IDisposable
    {
        public string ConnectionString { get; private set; }
        public BaseService(string connectionString = null)
        {
            if (connectionString == null)
            {
                ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            }
            else
            {
                ConnectionString = connectionString;
            }
        }

        private SQLiteConnection _connection;
        public SQLiteConnection GetConnection
        {
            get
            {
                if (_connection == null)
                {
                    _connection = new SQLiteConnection(ConnectionString);
                    _connection.Open();
                }
                return _connection;
            }
        }

        public void Dispose()
        {
            if (_connection != null)
            {
                _connection.Dispose();
                _connection = null;
            }
        }
    }
}
