using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BetManager.Core.Dbs
{
    public class DbXmlDataImporter
    {
        public string ConnectionString { get; private set; }

        public DbXmlDataImporter()
            : this(Properties.Settings.Default.ConnectionString)
        { }

        public DbXmlDataImporter(string connectionString)
        {
            ConnectionString = connectionString;
        }

        public int? ImportXmlData(string company, string displayName, string xml)
        {
            System.Diagnostics.Trace.Write(string.Format("Start importing XML {0}", displayName), "DbXmlImporter");

            using (SqlConnection connection = new SqlConnection(
              ConnectionString))
            {
                SqlCommand command = new SqlCommand("DataXmlExport_NEW", connection);
                command.CommandType = CommandType.StoredProcedure;
                //command.Parameters.Add("@Company", SqlDbType.NVarChar, 255);
                //command.Parameters.Add("@DisplayName", SqlDbType.NVarChar, 255);
                var xmlParam = command.Parameters.Add("@XMLData", SqlDbType.Xml);
                var output = command.Parameters.Add("@ID", SqlDbType.Int);
                output.Direction = ParameterDirection.Output;

                connection.Open();

                command.Parameters.AddWithValue("@Company", company);
                command.Parameters.AddWithValue("@DisplayName", displayName);
                //command.Parameters.AddWithValue("@XMLData", xml);
                //command.Parameters["@XMLData"].Value = xml;

                xmlParam.Value = new SqlXml(XmlReader.Create(new System.IO.StringReader(xml)));
                command.ExecuteNonQuery();

                System.Diagnostics.Trace.Write(string.Format("Importing finished", displayName), "DbXmlImporter");
                return output.Value as int?;
            }


        }
    
        /// <summary>
        /// iFortuna - zpracování importu v DB
        /// </summary>
        /// <param name="id">id importu</param>
        public void IFortunaImport(int id)
        {
            System.Diagnostics.Trace.Write(string.Format("Start importing iFortuna {0}", id), "IFortunaImport");

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("IF_DataXmlExport_IMPORT", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                command.Parameters.AddWithValue("@ID", id);
                command.ExecuteNonQuery();
            }

            System.Diagnostics.Trace.Write(string.Format("IFortuna Importing finished", id), "IFortunaImport");
        }

        /// <summary>
        /// Vitisport - zpracování importu v DB
        /// </summary>
        /// <param name="id">id importu</param>
        public void VitiImport(int id)
        {
            System.Diagnostics.Trace.Write(string.Format("Start importing Vitisport {0}", id), "IFortunaImport");

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("VT_DataXmlExport_IMPORT", connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();

                command.Parameters.AddWithValue("@ID", id);
                command.ExecuteNonQuery();
            }

            System.Diagnostics.Trace.Write(string.Format("Viti Importing finished", id), "VitiImport");
        }
    }
}
