using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBDecryptor.Data
{
    public class CertificateRepository : RepositoryBase
    {
        public override string TableName
        {
            get
            {
                return "Certificate";
            }
        }

        public CertificateRepository():base(ConfigurationManager.AppSettings["DatabasePath"])
        {

        }

        public int SaveCertificate(string path, string password)
        {
            using (var dataAccess = new DataAccess(DBPath))
            {
                dataAccess.ExecuteSQL(string.Format("Update {0} Set IsDefault = 0", TableName));
                string command = string.Empty;

                command = string.Format(@"INSERT INTO {0} (Path, Password, IsDefault, AddedOn) Values(""{1}"",""{2}"",{3}, ""{4}"")", TableName, path, password, 1, 
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                
                return dataAccess.ExecuteSQL(command);
            }
        }

        public Certificate GetCertificate()
        {
            var retVal = new Certificate();
            using (var dataAccess = new DataAccess(DBPath))
            {
                var results = dataAccess.ExecuteDataTable(string.Format("select * from {0} where IsDefault = 1", TableName));
                if(results != null && results.Rows != null && results.Rows.Count  > 0)
                {
                    string path = Convert.ToString(results.Rows[results.Rows.Count - 1]["Path"]);
                    string password = Convert.ToString(results.Rows[results.Rows.Count - 1]["Password"]);
                    retVal.Path = path;
                    retVal.Password = password;
                }

                return retVal;
            }
        }
    }
}
