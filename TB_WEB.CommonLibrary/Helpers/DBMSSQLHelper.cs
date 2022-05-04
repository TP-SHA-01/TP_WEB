using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TB_WEB.CommonLibrary.Encrypt;
using TB_WEB.CommonLibrary.Log;

namespace TB_WEB.CommonLibrary.Helpers
{
    public class DBMSSQLHelper : DBHelperBase, IDisposable
    {
        private string ConnectionString;

        // Track whether Dispose has been called.
        private bool disposed = false;

        private SqlConnection conn;
        private int connecttimeout = 30;

        private string _ConnectionString
        {
            get { return ConnectionString; }
            set { ConnectionString = value; }
        }

        public DBMSSQLHelper()
        {
            //CommonFunc comm = new CommonFunc();
            Decryption de = new Decryption();
            //UserInfo.DBConnString = de.DecryptDBConnectionString(System.Configuration.ConfigurationManager.AppSettings["DBCONNSTRING"].ToString());
            connecttimeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ConnectTimeout"].ToString());
            _ConnectionString = de.DecryptDBConnectionString(System.Configuration.ConfigurationManager.AppSettings["DBMSSQLCONNSTRING"].ToString());

            conn = new SqlConnection(_ConnectionString);

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

                int retVal = 0;
                using (SqlDataAdapter _Adapter = new SqlDataAdapter("SELECT count(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '" + Table + "' and COLUMN_NAME = '" + Column + "'", conn))
                {
                    DataTable dt = new DataTable();
                    _Adapter.SelectCommand.CommandTimeout = connecttimeout;
                    _Adapter.Fill(dt);

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        retVal = Convert.ToInt32(dt.Rows[0][0].ToString());
                    }
                }

                return retVal;
            }
            catch (Exception ex)
            {
                LogHelper.Error("DBMSSQLHelper > CheckColumnExist: Table=" + Table + "; Column = " + Column);
                LogHelper.Error(ex.Message);
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
                    LogHelper.Error("DBMSSQLHelper > ExecDataTable: " + qry);
                }

                DataTable dt = new DataTable();
                using (SqlDataAdapter _Adapter = new SqlDataAdapter(qry, conn))
                {
                    _Adapter.SelectCommand.CommandTimeout = connecttimeout;
                    _Adapter.SelectCommand.Prepare();
                    _Adapter.Fill(dt);
                }
                return dt;
            }
            catch (Exception ex)
            {
                LogHelper.Error("DBMSSQLHelper > ExecDataTable: " + qry);
                LogHelper.Error(ex.Message);
                return null;
            }
        }

        public DataTable ExecDataTable(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
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
                    LogHelper.Error("DBMSSQLHelper > ExecuteTranQuery: " + commandText + "DBMSSQLHelper > ExecuteTranQuery > SqlParameter[]: " + GetLoopParams(commandParameters));
                }

                //create a command and prepare it for execution
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, conn, (SqlTransaction)null, commandType, commandText, commandParameters);

                //create the DataAdapter & DataSet
                SqlDataAdapter da = new SqlDataAdapter(cmd);
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
                LogHelper.Error("DBMSSQLHelper > ExecuteTranQuery: " + commandText + "DBMSSQLHelper > ExecuteTranQuery > SqlParameter[]: " + GetLoopParams(commandParameters));
                LogHelper.Error(ex.Message);

                return null;
            }

        }


        public DataSet ExecuteDataset(CommandType commandType, string commandText, params SqlParameter[] commandParameters)
        {
            try
            {

                if (string.IsNullOrEmpty(commandText))
                {
                    throw (new Exception("SQL is blank"));
                }

                //create a command and prepare it for execution
                if (conn.State != ConnectionState.Open)
                {
                    conn.Open();
                }

                if (Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DebugLevel"].ToString()) > 1)
                {
                    LogHelper.Error("DBMSSQLHelper > ExecuteTranQuery: " + commandText + "DBMSSQLHelper > ExecuteTranQuery > SqlParameter[]: " + GetLoopParams(commandParameters));
                }

                DataSet ds = new DataSet();

                using (SqlCommand cmd = new SqlCommand())
                {
                    PrepareCommand(cmd, conn, (SqlTransaction)null, commandType, commandText, commandParameters);

                    //create the DataAdapter & DataSet
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.SelectCommand.CommandTimeout = connecttimeout;
                        da.SelectCommand.Prepare();
                        //fill the DataSet using default values for DataTable names, etc.
                        da.Fill(ds);
                    }
                }
                //return the dataset
                return ds;
            }
            catch (Exception ex)
            {
                LogHelper.Error("DBMSSQLHelper > ExecuteTranQuery: " + commandText + "DBMSSQLHelper > ExecuteTranQuery > SqlParameter[]: " + GetLoopParams(commandParameters));
                LogHelper.Error(ex.Message);

                return null;
            }
        }

        public DataSet ExecuteDataset_noparam(CommandType commandType, string commandText)
        {
            DataSet ds = new DataSet();

            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }

            //create a command and prepare it for execution
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.CommandText = commandText;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;

                //create the DataAdapter & DataSet
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    da.SelectCommand.CommandTimeout = connecttimeout;
                    da.SelectCommand.Prepare();

                    //fill the DataSet using default values for DataTable names, etc.
                    da.Fill(ds);
                }

            }
            //return the dataset
            return ds;
        }

        public bool ExecuteTranQuery(CommandType commandType, string commandText, SqlParameter[] commandParameters = null)
        {

            if (string.IsNullOrEmpty(commandText))
            {
                throw (new Exception("SQL is blank"));
            }
            if (conn.State != ConnectionState.Open)
            {
                try
                {
                    conn.Open();
                }
                catch (Exception ex)
                {
                    LogHelper.Error("DBMSSQLHelper > ExecuteTranQuery: " + commandText + "DBMSSQLHelper > ExecuteTranQuery > SqlParameter[]: " + GetLoopParams(commandParameters));
                    LogHelper.Error(ex.Message);
                    return false;
                }
            }
            SqlTransaction transaction = conn.BeginTransaction();
            try
            {
                if (Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["DebugLevel"].ToString()) > 1)
                {
                    LogHelper.Error("DBMSSQLHelper > ExecuteTranQuery: " + commandText + "DBMSSQLHelper > ExecuteTranQuery > SqlParameter[]: " + GetLoopParams(commandParameters));
                }
                //create a command and prepare it for execution
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, conn, transaction, commandType, commandText, commandParameters);
                cmd.CommandTimeout = connecttimeout;
                cmd.ExecuteNonQuery();
                transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {

                LogHelper.Error("DBMSSQLHelper > ExecuteQuery: " + commandText + "DBMSSQLHelper > ExecuteQuery > SqlParameter[]: " + GetLoopParams(commandParameters));
                LogHelper.Error(ex.Message);
                try
                {
                    transaction.Rollback();
                }
                catch (Exception rollbackException)
                {
                    LogHelper.Error("DBMSSQLHelper > RollBack Exception Type : " + rollbackException.GetType() + "DBMSSQLHelper > RollBack Exception Msg  : " + rollbackException.Message);
                }
                return false;
            }
            finally
            {
                transaction.Dispose();
                conn.Close();
            }
        }

        public bool ExecuteQuery(CommandType commandType, string commandText, SqlParameter[] commandParameters = null)
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
                    LogHelper.Error("DBMSSQLHelper > ExecuteQuery: " + commandText  + "DBMSSQLHelper > ExecuteQuery > SqlParameter[]: " + GetLoopParams(commandParameters));
                }

                //create a command and prepare it for execution
                SqlCommand cmd = new SqlCommand();
                PrepareCommand(cmd, conn, (SqlTransaction)null, commandType, commandText, commandParameters);
                cmd.CommandTimeout = connecttimeout;
                cmd.ExecuteNonQuery();

                //return the dataset
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex.Message);

                return false;
            }

        }

        private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
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

        private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
        {
            foreach (SqlParameter p in commandParameters)
            {
                //check for derived output value with no value assigned
                if ((p.Direction == ParameterDirection.InputOutput) && (p.Value == null))
                {
                    p.Value = DBNull.Value;
                }

                command.Parameters.Add(p);
            }
        }
    }
}
