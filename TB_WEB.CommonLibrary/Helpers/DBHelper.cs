using System;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using TB_WEB.CommonLibrary.Encrypt;

namespace TB_WEB.CommonLibrary.Helpers
{
    public class DBHelper : DBHelperBase, IDisposable
    {
        private OleDbConnection conn;
        private string ConnectionString;
        // Track whether Dispose has been called.
        private bool disposed = false;

        private string _ConnectionString
        {
            get { return ConnectionString; }
            set { ConnectionString = value; }
        }

        public static OleDbType DBTypeMapping(string inType)
        {
            switch (inType.ToLower())
            {
                case "bigint":
                    return OleDbType.BigInt;
                    break;

                case "timestamp":
                case "binary":
                    return OleDbType.Binary;
                    break;

                case "bit":
                    return OleDbType.Boolean;
                    break;

                case "char":
                    return OleDbType.Char;
                    break;

                case "money":
                case "smallmoney ":
                    return OleDbType.Currency;
                    break;

                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    return OleDbType.DBTimeStamp;
                    break;

                case "float":
                    return OleDbType.Double;
                    break;

                case "uniqueidentifier":
                    return OleDbType.Guid;
                    break;

                case "identity":
                case "int":
                    return OleDbType.Integer;
                    break;

                case "image":
                    return OleDbType.LongVarBinary;
                    break;

                case "text":
                    return OleDbType.LongVarChar;
                    break;

                case "ntext":
                    return OleDbType.VarWChar;
                    break;

                case "decimal":
                case "numeric":
                    return OleDbType.Decimal;
                    break;

                case "real":
                    return OleDbType.Single;
                    break;

                case "smallInt":
                    return OleDbType.SmallInt;
                    break;

                case "tinyInt":
                    return OleDbType.UnsignedTinyInt;
                    break;

                case "varbinary":
                    return OleDbType.VarBinary;
                    break;

                case "varchar":
                    return OleDbType.VarChar;
                    break;

                case "sql_variant":
                    return OleDbType.Variant;
                    break;

                case "nvarchar":
                    return OleDbType.VarWChar;
                    break;

                case "nchar":
                    return OleDbType.WChar;
                    break;

                default:
                    return OleDbType.Boolean;
                    break;
            }
        }

        public DBHelper()
        {
            Decryption de = new Decryption();
            string connStr = ConfigurationManager.AppSettings["DBCONNSTRING"];
            _ConnectionString = de.DecryptDBConnectionString(connStr);
            //_ConnectionString = "Provider=SQLOLEDB;server=LINDAMA-PC;database=TopoceanPOTracing;uid=TopUser1;pwd=TopUser1";
            //UserInfo.DBConnString = _ConnectionString;

            conn = new OleDbConnection(_ConnectionString);
            de = null;
        }

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        // Dispose(bool disposing) executes in two distinct scenarios.
        // If disposing equals true, the method has been called directly
        // or indirectly by a user's code. Managed and unmanaged resources
        // can be disposed.
        // If disposing equals false, the method has been called by the
        // runtime from inside the finalizer and you should not reference
        // other objects. Only unmanaged resources can be disposed.
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                    //comm = null;
                    // Dispose managed resources.
                    if (conn != null)
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }

                        conn.Dispose();

                    }
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.


                // Note disposing has been done.
                disposed = true;

            }
        }

        public void CloseConnection()
        {
            if (conn != null)
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public int CheckColumnExist(string Table, string Column)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }
                int connecttimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ConnectTimeout"].ToString());
                DataTable dt = new DataTable();
                if (Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DebugLevel"].ToString()) > 1)
                {
                    SaveLog("DBHelper > CheckColumnExist: SELECT count(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + Table + "' and COLUMN_NAME = '" + Column + "'", "I");
                }
                OleDbDataAdapter _Adapter = new OleDbDataAdapter("SELECT count(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + Table + "' and COLUMN_NAME = '" + Column + "'", conn);
                _Adapter.SelectCommand.CommandTimeout = connecttimeout;
                _Adapter.Fill(dt);
                return Convert.ToInt32(dt.Rows[0][0].ToString());
            }
            catch (Exception ex)
            {
                SaveLog("DBHelper > CheckColumnExist: Table=" + Table + "; Column = " + Column, "E");
                SaveLog(ex.ToString(), "E");

                return -1;
            }
        }

        public DataTable ExecDataTable(string qry)
        {
            try
            {
                if (string.IsNullOrEmpty(qry))
                {
                    throw (new Exception("SQL is blank"));
                }

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                if (Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DebugLevel"].ToString()) > 1)
                {
                    SaveLog("DBHelper > ExecDataTable: " + qry, "I");
                }

                int connecttimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ConnectTimeout"].ToString());
                DataTable dt = new DataTable();
                OleDbDataAdapter _Adapter = new OleDbDataAdapter(qry, conn);
                _Adapter.SelectCommand.CommandTimeout = connecttimeout;
                _Adapter.SelectCommand.Prepare();
                _Adapter.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                SaveLog("DBHelper > ExecDataTable: " + qry, "E");
                SaveLog(ex.ToString(), "E");

                return null;
            }

        }

        public DataTable ExecDataTable(CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            try
            {
                if (string.IsNullOrEmpty(commandText))
                {
                    throw (new Exception("SQL is blank"));
                }

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                if (Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DebugLevel"].ToString()) > 1)
                {
                    SaveLog("DBHelper > ExecDataTable: " + commandText, "I");
                    SaveLog("DBHelper > ExecDataTable > OleDbParameter[]: " + GetLoopParams(commandParameters), "I");
                }

                //create a command and prepare it for execution
                OleDbCommand cmd = new OleDbCommand();
                PrepareCommand(cmd, conn, (OleDbTransaction)null, commandType, commandText, commandParameters);

                //create the DataAdapter & DataSet
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                int connecttimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ConnectTimeout"].ToString());

                da.SelectCommand.CommandTimeout = connecttimeout;
                da.SelectCommand.Prepare();

                DataTable dt = new DataTable();
                //fill the dataTable using default values for DataTable names, etc.
                da.Fill(dt);

                //return the dataTable
                return dt;
            }
            catch (Exception ex)
            {
                SaveLog("DBHelper > ExecDataTable: " + commandText, "E");
                SaveLog(ex.ToString(), "E");

                return null;
            }

        }

        public DataSet ExecuteDataset(CommandType commandType, string commandText, params OleDbParameter[] commandParameters)
        {
            try
            {
                if (string.IsNullOrEmpty(commandText))
                {
                    throw (new Exception("SQL is blank"));
                }

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                if (Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DebugLevel"].ToString()) > 1)
                {
                    SaveLog("DBHelper > ExecuteDataset: " + commandText, "I");
                    SaveLog("DBHelper > ExecuteDataset > OleDbParameter[]: " + GetLoopParams(commandParameters), "I");
                }

                //create a command and prepare it for execution
                OleDbCommand cmd = new OleDbCommand();
                PrepareCommand(cmd, conn, (OleDbTransaction)null, commandType, commandText, commandParameters);

                //create the DataAdapter & DataSet
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                int connecttimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ConnectTimeout"].ToString());

                da.SelectCommand.CommandTimeout = connecttimeout;
                da.SelectCommand.Prepare();
                DataSet ds = new DataSet();

                //fill the DataSet using default values for DataTable names, etc.
                da.Fill(ds);

                //return the dataset
                return ds;
            }
            catch (Exception ex)
            {
                SaveLog("DBHelper > ExecuteDataset: " + commandText, "E");
                SaveLog("DBHelper > OleDbParameter[]: " + GetLoopParams(commandParameters), "E");
                SaveLog(ex.ToString(), "E");

                return null;
            }
        }

        public DataSet ExecuteDataset_noparam(CommandType commandType, string commandText)
        {
            try
            {
                if (string.IsNullOrEmpty(commandText))
                {
                    throw (new Exception("SQL is blank"));
                }

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                if (Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DebugLevel"].ToString()) > 1)
                {
                    SaveLog("DBHelper > ExecuteDataset_noparam: " + commandText, "I");
                }

                //create a command and prepare it for execution
                OleDbCommand cmd = new OleDbCommand();

                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                //create the DataAdapter & DataSet
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                int connecttimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ConnectTimeout"].ToString());
                da.SelectCommand.CommandTimeout = connecttimeout;
                da.SelectCommand.Prepare();
                DataSet ds = new DataSet();

                //fill the DataSet using default values for DataTable names, etc.
                da.Fill(ds);

                //return the dataset
                return ds;
            }
            catch (Exception ex)
            {
                SaveLog("DBHelper > ExecuteDataset_noparam: " + commandText, "E");
                SaveLog(ex.ToString(), "E");

                return null;
            }
        }

        public bool ExecuteQuery(CommandType commandType, string commandText, OleDbParameter[] commandParameters = null)
        {
            try
            {
                if (string.IsNullOrEmpty(commandText))
                {
                    throw (new Exception("SQL is blank"));
                }

                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                if (Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DebugLevel"].ToString()) > 1)
                {
                    SaveLog("DBHelper > ExecuteQuery: " + commandText, "I");
                    SaveLog("DBHelper > ExecuteQuery > OleDbParameter[]: " + GetLoopParams(commandParameters), "I");
                }

                //create a command and prepare it for execution
                OleDbCommand cmd = new OleDbCommand();
                PrepareCommand(cmd, conn, (OleDbTransaction)null, commandType, commandText, commandParameters);

                int connecttimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ConnectTimeout"].ToString());
                cmd.CommandTimeout = connecttimeout;
                cmd.ExecuteNonQuery();

                //return the dataset
                return true;
            }
            catch (Exception ex)
            {
                SaveLog("DBHelper > ExecuteDataset: " + commandText, "E");
                SaveLog("DBHelper > OleDbParameter[]: " + GetLoopParams(commandParameters), "E");
                SaveLog(ex.ToString(), "E");

                return false;
            }

        }


        private static void PrepareCommand(OleDbCommand command, OleDbConnection connection, OleDbTransaction transaction, CommandType commandType, string commandText, OleDbParameter[] commandParameters)
        {
            //if the provided connection is not open, we will open it
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }

            //associate the connection with the command
            command.Connection = connection;

            //set the command text (stored procedure name or OleDb statement)
            command.CommandText = commandText;

            //if we were provided a transaction, assign it.
            if (transaction != null)
            {
                command.Transaction = transaction;
            }

            //set the command type
            command.CommandType = commandType;

            //attach the command parameters if they are provided
            if (commandParameters != null)
            {
                AttachParameters(command, commandParameters);
            }

            return;
        }


        private static void AttachParameters(OleDbCommand command, OleDbParameter[] commandParameters)
        {
            foreach (OleDbParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }

                command.Parameters.Add(p);
            }
        }

        private void SaveLog(string msg, string type = "I")
        {
            try
            {
                //CommonFunc.SaveLog(msg, type);
            }
            catch (Exception ex)
            {

            }
        }

    }
}
