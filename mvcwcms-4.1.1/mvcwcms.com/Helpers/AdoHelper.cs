using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.Caching;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;

namespace MVCwCMS.Helpers
{
    /// <summary>
    /// ADO.NET helper class for MS SQL Server Database. To interact with a different DBMS only this class needs to be changed. 
    /// </summary>
    public class AdoHelper : IDisposable
    {
        private static object ThisLock = new object();

        // Internal members
        protected string _connString = null;
        protected SqlConnection _conn = null;
        protected SqlTransaction _trans = null;
        protected bool _disposed = false;
        protected int _commandTimeout;

        /// <summary>
        /// Sets or returns the connection string use by all instances of this class.
        /// </summary>
        public static string ConnectionString { get; set; }

        /// <summary>
        /// Returns the current SqlTransaction object or null if no transaction
        /// is in effect.
        /// </summary>
        public SqlTransaction Transaction { get { return _trans; } }

        /// <summary>
        /// Constructor using global connection string.
        /// </summary>
        /// <param name="commandTimeout">Sets the wait time before terminating the attempt to execute a command and generating an error. Use 0 to set infinite timeout.</param>
        public AdoHelper(int commandTimeout = 30)
        {
            _connString = ConnectionString;
            _commandTimeout = commandTimeout;
            Connect();
        }

        /// <summary>
        /// Constructure using connection string override
        /// </summary>
        /// <param name="connString">Connection string for this instance</param>
        /// <param name="commandTimeout">Sets the wait time before terminating the attempt to execute a command and generating an error. Use 0 to set infinite timeout.</param>
        public AdoHelper(string connString, int commandTimeout = 30)
        {
            _connString = connString;
            _commandTimeout = commandTimeout;
            Connect();
        }

        // Creates a SqlConnection using the current connection string
        protected void Connect()
        {
            _conn = new SqlConnection(_connString);
            _conn.Open();
        }

        /// <summary>
        /// Creates a ReturnValue parameter to pass in input to the stored procedure
        /// </summary>
        /// <param name="paramName">The ReturnValue parameter name</param>
        /// <returns></returns>
        public SqlParameter CreateParamReturnValue(string paramName)
        {
            SqlParameter parm = new SqlParameter();
            parm.ParameterName = paramName;
            parm.Value = null;
            parm.Direction = ParameterDirection.ReturnValue;
            return parm;
        }

        /// <summary>
        /// Retrieves the value of a ReturnValue parameter after the stored procedure is executed
        /// </summary>
        /// <param name="paramObject">The ReturnValue parameter object</param>
        /// <returns></returns>
        public int? GetParamReturnValue(SqlParameter paramObject)
        {
            if (paramObject.IsNotNull())
                return paramObject.Value.ConvertTo<int?>(null, true);
            else
                return null;
        }

        /// <summary>
        /// Constructs a SqlCommand with the given parameters. This method is normally called
        /// from the other methods and not called directly. But here it is if you need access
        /// to it.
        /// </summary>
        /// <param name="qry">SQL query or stored procedure name</param>
        /// <param name="type">Type of SQL command</param>
        /// <param name="args">Query arguments. Arguments should be in pairs where one is the
        /// name of the parameter and the second is the value. The very last argument can
        /// optionally be a SqlParameter object for specifying a custom argument type</param>
        /// <returns></returns>
        public SqlCommand CreateCommand(string qry, CommandType type, params object[] args)
        {
            SqlCommand cmd = new SqlCommand(qry, _conn);

            // Associate with current transaction, if any
            if (_trans != null)
                cmd.Transaction = _trans;

            // Set command type
            cmd.CommandType = type;

            // Set command timeout
            cmd.CommandTimeout = _commandTimeout;

            // Construct SQL parameters
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is string && i < (args.Length - 1))
                {
                    SqlParameter parm = new SqlParameter();
                    parm.ParameterName = (string)args[i];
                    parm.Value = args[++i] ?? DBNull.Value;
                    cmd.Parameters.Add(parm);
                }
                else if (args[i] is SqlParameter)
                {
                    cmd.Parameters.Add((SqlParameter)args[i]);
                }
                else throw new ArgumentException("Invalid number or type of arguments supplied");
            }
            return cmd;
        }

        public int ConnectionContextExecuteNonQuery(string script)
        {
            Server server = new Server(new ServerConnection(_conn));
            return server.ConnectionContext.ExecuteNonQuery(script);
        }

        /// <summary>
        /// Executes a query that returns no results
        /// </summary>
        /// <param name="qry">Query text</param>
        /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
        /// <returns>The number of rows affected</returns>
        public int ExecNonQuery(string qry, params object[] args)
        {
            using (SqlCommand cmd = CreateCommand(qry, CommandType.Text, args))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a stored procedure that returns no results
        /// </summary>
        /// <param name="proc">Name of stored proceduret</param>
        /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
        /// <returns>The number of rows affected</returns>
        public int ExecNonQueryProc(string proc, params object[] args)
        {
            using (SqlCommand cmd = CreateCommand(proc, CommandType.StoredProcedure, args))
            {
                return cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Executes a query that returns a single value
        /// </summary>
        /// <param name="qry">Query text</param>
        /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
        /// <returns>Value of first column and first row of the results</returns>
        public object ExecScalar(string qry, params object[] args)
        {
            using (SqlCommand cmd = CreateCommand(qry, CommandType.Text, args))
            {
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Executes a query that returns a single value
        /// </summary>
        /// <param name="proc">Name of stored proceduret</param>
        /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
        /// <returns>Value of first column and first row of the results</returns>
        public object ExecScalarProc(string qry, params object[] args)
        {
            using (SqlCommand cmd = CreateCommand(qry, CommandType.StoredProcedure, args))
            {
                return cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Executes a query and returns the results as a SqlDataReader
        /// </summary>
        /// <param name="qry">Query text</param>
        /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
        /// <returns>Results as a SqlDataReader</returns>
        public SqlDataReader ExecDataReader(string qry, params object[] args)
        {
            using (SqlCommand cmd = CreateCommand(qry, CommandType.Text, args))
            {
                return cmd.ExecuteReader();
            }
        }

        /// <summary>
        /// Executes a stored procedure and returns the results as a SqlDataReader
        /// </summary>
        /// <param name="proc">Name of stored proceduret</param>
        /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
        /// <returns>Results as a SqlDataReader</returns>
        public SqlDataReader ExecDataReaderProc(string qry, params object[] args)
        {
            using (SqlCommand cmd = CreateCommand(qry, CommandType.StoredProcedure, args))
            {
                return cmd.ExecuteReader();
            }
        }

        /// <summary>
        /// Executes a query and returns the results as a DataSet
        /// </summary>
        /// <param name="qry">Query text</param>
        /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
        /// <returns>Results as a DataSet</returns>
        public DataSet ExecDataSet(string qry, params object[] args)
        {
            using (SqlCommand cmd = CreateCommand(qry, CommandType.Text, args))
            {
                SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                return ds;
            }
        }

        /// <summary>
        /// Executes a stored procedure and returns the results as a Data Set
        /// </summary>
        /// <param name="proc">Name of stored procedure</param>
        /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
        /// <returns>Results as a DataSet</returns>
        public DataSet ExecDataSetProc(string qry, params object[] args)
        {
            using (SqlCommand cmd = CreateCommand(qry, CommandType.StoredProcedure, args))
            {
                SqlDataAdapter adapt = new SqlDataAdapter(cmd);
                DataSet ds = new DataSet();
                adapt.Fill(ds);
                return ds;
            }
        }

        /// <summary>
        /// Executes a stored procedure while caches and returns the result as a List&lt;TSource&gt;
        /// </summary>
        /// <typeparam name="TSource">The Model for the returned Dataset</typeparam>
        /// <param name="storedProcedure">Name of the stored procedure</param>
        /// <param name="force">If true reloads the data from the database</param>
        /// <param name="replaceResourceKeys">If true replaces all the tokens {#ResourceKey} with the relative resource value from the files under the Resources folder</param>
        /// <param name="overwriteResultFunction">Delegate function used to overwrite/manipulate the data returned from the Stored Procedure</param>
        /// <param name="cacheKey">If null or empty it will be equal to storedProcedure</param>
        /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
        /// <returns>Results as a List&lt;TSource&gt;</returns>
        public static List<TSource> ExecCachedListProc<TSource>(string storedProcedure, bool force = false, bool replaceResourceKeys = true, Func<List<TSource>, List<TSource>> overwriteResultFunction = null, string cacheKey = null, params object[] args)
        {
            HttpContext context = HttpContext.Current;

            if (cacheKey.IsEmptyOrWhiteSpace())
            {
                cacheKey = storedProcedure;
            }

            if (force || context.Cache[cacheKey] == null) //Double check locking
            {
                lock (ThisLock)
                {
                    if (force || context.Cache[cacheKey] == null) //Double check locking
                    {
                        using (AdoHelper db = new AdoHelper())
                        {
                            var returnValue = db.CreateParamReturnValue("returnValue");
                            List<object> argsList = args.ToList();
                            argsList.Add(returnValue);
                            using (DataSet ds = db.ExecDataSetProc(storedProcedure, argsList.ToArray()))
                            {
                                if (db.GetParamReturnValue(returnValue) == 0)
                                {
                                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                                    {
                                        if (overwriteResultFunction.IsNotNull())
                                        {
                                            context.Cache.Insert(cacheKey, overwriteResultFunction(ds.Tables[0].ToList<TSource>(replaceResourceKeys)));
                                        }
                                        else
                                        {
                                            context.Cache.Insert(cacheKey, ds.Tables[0].ToList<TSource>(replaceResourceKeys));
                                        }
                                    }
                                    else
                                    {
                                        context.Cache.Remove(cacheKey);
                                    }
                                }
                                else
                                {
                                    context.Cache.Remove(cacheKey);
                                    throw new Exception("The stored procedure " + storedProcedure + " returned the error code " + db.GetParamReturnValue(returnValue));
                                }
                            }
                        }
                    }
                }
            }

            return context.Cache[cacheKey] as List<TSource>;
        }

        /// <summary>
        /// Executes a stored procedure and returns the result as a List&lt;TSource&gt;
        /// </summary>
        /// <typeparam name="TSource">The Model for the returned Dataset</typeparam>
        /// <param name="storedProcedure">Name of the stored procedure</param>
        /// <param name="replaceResourceKeys">If true replaces all the tokens {#ResourceKey} with the relative resource value from the files under the Resources folder</param>
        /// <param name="overwriteResultFunction">Delegate function used to overwrite/manipulate the data returned from the Stored Procedure</param>
        /// <param name="commandTimeout">Sets the wait time before terminating the attempt to execute a command and generating an error. Use 0 to set infinite timeout.</param>
        /// <param name="args">Any number of parameter name/value pairs and/or SQLParameter arguments</param>
        /// <returns>Results as a List&lt;TSource&gt;</returns>
        public static List<TSource> ExecListProc<TSource>(string storedProcedure, bool replaceResourceKeys = true, Func<List<TSource>, List<TSource>> overwriteResultFunction = null, int commandTimeout = 30, params object[] args)
        {
            List<TSource> result = null;

            using (AdoHelper db = new AdoHelper(commandTimeout))
            {
                var returnValue = db.CreateParamReturnValue("returnValue");
                List<object> argsList = args.ToList();
                argsList.Add(returnValue);
                using (DataSet ds = db.ExecDataSetProc(storedProcedure, argsList.ToArray()))
                {
                    if (db.GetParamReturnValue(returnValue) == 0)
                    {
                        if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            if (overwriteResultFunction.IsNotNull())
                            {
                                result = overwriteResultFunction(ds.Tables[0].ToList<TSource>(replaceResourceKeys));
                            }
                            else
                            {
                                result = ds.Tables[0].ToList<TSource>(replaceResourceKeys);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("The stored procedure " + storedProcedure + " returned the error code " + db.GetParamReturnValue(returnValue));
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Begins a transaction
        /// </summary>
        /// <returns>The new SqlTransaction object</returns>
        public SqlTransaction BeginTransaction()
        {
            Rollback();
            _trans = _conn.BeginTransaction();
            return Transaction;
        }

        /// <summary>
        /// Commits any transaction in effect.
        /// </summary>
        public void Commit()
        {
            if (_trans != null)
            {
                _trans.Commit();
                _trans = null;
            }
        }

        /// <summary>
        /// Rolls back any transaction in effect.
        /// </summary>
        public void Rollback()
        {
            if (_trans != null)
            {
                _trans.Rollback();
                _trans = null;
            }
        }

        /// <summary>
        /// Checks if the database specified in the connString parameter is valid or not
        /// </summary>
        /// <returns></returns>
        public static bool IsDatabaseValid(string connString)
        {
            bool result = false;

            try
            {
                AdoHelper adoHelper = new AdoHelper(connString);

                result = true;
            }
            catch (Exception)
            {
                //Swallows the exception as this method is only intended to test for the Database validity
            }

            return result;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                // Need to dispose managed resources if being called manually
                if (disposing)
                {
                    if (_conn != null)
                    {
                        Rollback();
                        _conn.Dispose();
                        _conn = null;
                    }
                }
                _disposed = true;
            }
        }

        #endregion
    }
}