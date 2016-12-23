namespace Dixon.Core.Services
{
    public abstract class BaseService
    {
        public string ConnectionString { get; private set; }

        public BaseService()
            : this("ConnectionString")
        {

        }

        public BaseService(string connectionStringName)
        {
            ConnectionString = System.Configuration.ConfigurationManager.
                ConnectionStrings[connectionStringName].ConnectionString;
        }
    }
}
