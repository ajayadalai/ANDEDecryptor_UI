using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBDecryptor.Data
{
    public class DataAccess : IDisposable
    {
        /// <summary>
        /// The connection.
        /// </summary>
        private static SQLiteConnection dbConnection = null;

        private static string dbPath;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="platformDetails"></param>
        public DataAccess(string datasource)
        {
            dbPath = datasource;
        }

        private static void CreateDBIfNotExists()
        {
            var directoryName = System.IO.Path.GetDirectoryName(dbPath);
            if (!System.IO.Directory.Exists(directoryName))
            {
                System.IO.Directory.CreateDirectory(directoryName);
            }
            if (!System.IO.File.Exists(dbPath))
            {
                //System.IO.File.Create(dbPath);
                SQLiteConnection.CreateFile(dbPath);

            }
            CreateSettingsTable();

        }

        private static void CreateSettingsTable()
        {
            if (!TableExists("Certificate"))
            {
                string sqlCommand = "CREATE TABLE Certificate (Path Text, Password Text, IsDefault int, AddedOn Text)";
                ExecuteSQLCommand(sqlCommand);
            }
        }


        private static void ExecuteSQLCommand(string commandText, Dictionary<string, object> parameters = null)
        {
            var command = new SQLiteCommand(commandText, DbConnection);
            command.Connection = DbConnection;
            DbConnection.Open();
            if (null != parameters && parameters.Count > 0)
            {
                foreach (var pair in parameters)
                {
                    command.Parameters.AddWithValue(pair.Key, pair.Value);
                }
            }
            command.ExecuteNonQuery();
            DbConnection.Close();
        }

        public static bool TableExists(string tableName)
        {
            SQLiteCommand command = new SQLiteCommand("SELECT name FROM sqlite_master WHERE name='" + tableName + "'", DbConnection);
            DbConnection.Open();
            var result = command.ExecuteScalar();
            DbConnection.Close();
            return result != null && result.ToString() == tableName ? true : false;
        }

        /// <summary>
        /// Gets the connection.
        /// </summary>
        /// <value>The connection.</value>
        private static SQLiteConnection DbConnection
        {
            get
            {
                if (dbConnection == null)
                {
                    dbConnection = new SQLiteConnection(string.Format("Data Source={0}", dbPath));
                    CreateDBIfNotExists();
                }
                return dbConnection;

            }
        }

        public void Dispose()
        {
            if (null != DbConnection)
            {
                if (DbConnection.State != ConnectionState.Closed)
                {
                    DbConnection.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(SQLiteCommand command)
        {
            try
            {
                command.Connection = DbConnection;
                OpenConnection();
                var adapter = new SQLiteDataAdapter(command);
                var dt = new DataTable();
                adapter.Fill(dt);
                adapter.Dispose();
                CloseConnection();
                return dt;
            }
            catch (Exception ex)
            {
                var e = ex;
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string commandText)
        {
            SQLiteCommand cmd = new SQLiteCommand(commandText);
            return ExecuteDataTable(cmd);
        }

        public int ExecuteSQL(string sql, Dictionary<string, object> parameters = null)
        {
            int numberOfRowsAffected;

            OpenConnection();
            var command = new SQLiteCommand(sql, DbConnection);
            if (null != parameters && parameters.Count > 0)
            {
                foreach (var pair in parameters)
                {
                    command.Parameters.AddWithValue(pair.Key, pair.Value);
                }
            }
            numberOfRowsAffected = command.ExecuteNonQuery();
            CloseConnection();
            return numberOfRowsAffected;
        }


        public object ExecuteScalar(string sql, Dictionary<string, object> parameters = null)
        {
            object retVal;

            OpenConnection();
            var command = new SQLiteCommand(sql, DbConnection);
            if (null != parameters && parameters.Count > 0)
            {
                foreach (var pair in parameters)
                {
                    command.Parameters.AddWithValue(pair.Key, pair.Value);
                }
            }
            retVal = command.ExecuteScalar();
            CloseConnection();
            return retVal;
        }

        private void OpenConnection()
        {
            if (DbConnection.State != ConnectionState.Open)
            {
                DbConnection.Open();
            }
        }

        private void CloseConnection()
        {
            if (DbConnection.State != ConnectionState.Closed)
            {
                DbConnection.Close();
            }
        }
    }
}