// <copyright file="ReadGenericRepositoryRepoDB.cs" company="Gasolutions SAS">
// Copyright (c) Gasolutions SAS. Todos los derechos reservados.
// </copyright>

namespace Gasolutions.Core.Repository
{
    /// <summary>
    /// Lightweight repository implementation for read operations that return raw JSON strings.
    /// </summary>
    public class ReadGenericRepositoryRepoDB : IReadGenericRepository
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadGenericRepositoryRepoDB"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string used to open SQL Server connections.</param>
        public ReadGenericRepositoryRepoDB(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets or sets the connection string used to connect to the SQL Server database. This property is initialized through the constructor and is used internally to create database connections for executing queries.
        /// </summary>
        private string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Executes the specified command and returns the first JSON string result (or empty string when no rows).
        /// </summary>
        /// <param name="commandText">The SQL command text or stored procedure name to execute.</param>
        /// <param name="commandType">The type of the command (text or stored procedure).</param>
        /// <returns>The first JSON string returned by the query, or an empty string if there are no results.</returns>
        public string QueryAndReturnJson(string commandText, CommandType commandType)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException(nameof(commandText), "Command text cannot be null.");
            }

            using SqlConnection connection = new(this.ConnectionString);

            IEnumerable<string> result = connection.ExecuteQuery<string>(
                commandText,
                commandType: commandType);

            return result?.FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Asynchronously executes the specified command and returns the first JSON string result (or empty string when no rows).
        /// </summary>
        /// <param name="commandText">The SQL command text or stored procedure name to execute.</param>
        /// <param name="commandType">The type of the command (text or stored procedure).</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the first JSON string returned by the query, or an empty string if there are no results.</returns>
        public async Task<string> QueryAndReturnJsonAsync(string commandText, CommandType commandType)
        {
            using SqlConnection connection = new(this.ConnectionString);

            IEnumerable<string> result = await connection.ExecuteQueryAsync<string>(
                commandText,
                commandType: commandType);

            return result?.FirstOrDefault() ?? string.Empty;
        }

        /// <summary>
        /// Returns the maximum value for the specified field in the given table filtered by the where object or primary key asynchronously.
        /// </summary>
        /// <param name="tableName">Name of the table to query.</param>
        /// <param name="fieldName">Name of the field to compute the maximum for.</param>
        /// <param name="whereOrPrimaryKey">An object representing the WHERE clause or the primary key for filtering.</param>
        public object? Max(string tableName, string fieldName, object whereOrPrimaryKey)
        {
            RepoDb.Field field = new(fieldName);

            using SqlConnection connection = new(this.ConnectionString);
            object max = connection.Max(tableName, field, whereOrPrimaryKey);

            if (max == DBNull.Value)
            {
                return null;
            }

            return max;
        }

        /// <summary>
        /// Returns the maximum value for the specified field in the given table filtered by the where object or primary key asynchronously.
        /// </summary>
        /// <param name="tableName">Name of the table to query.</param>
        /// <param name="fieldName">Name of the field to compute the maximum for.</param>
        /// <param name="whereOrPrimaryKey">An object representing the WHERE clause or the primary key for filtering.</param>
        /// <param name="connection">The SQL connection to use for the query.</param>
        /// <param name="transaction">The SQL transaction to use for the query.</param>
        /// <returns>The maximum value for the specified field, or null if no rows match the filter.</returns>
        public object? Max(string tableName, string fieldName, object whereOrPrimaryKey, IDbConnection connection, IDbTransaction transaction)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName), "Field name cannot be null or empty.");
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName), "Table name cannot be null or empty.");
            }

            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection), "Connection cannot be null.");
            }

            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null.");
            }

            RepoDb.Field field = new(fieldName);
            object max = connection.Max(tableName, field, whereOrPrimaryKey, transaction: transaction);

            if (max == DBNull.Value)
            {
                return null;
            }

            return max;
        }

        /// <inheritdoc/>
        public object? Max(string tableName, string fieldName, object whereOrPrimaryKey, IDbTransaction transaction)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.Max(tableName, fieldName, whereOrPrimaryKey, connection, transaction);
        }

        /// <inheritdoc/>
        public T ExecuteScalar<T>(string commandText, CommandType commandType)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException(nameof(commandText), "Command text cannot be null.");
            }

            using SqlConnection connection = new(this.ConnectionString);

            T result = connection.ExecuteScalar<T>(
                commandText,
                commandType: commandType);

            return result;
        }

        /// <inheritdoc/>
        public string QueryAndReturnJson(string commandText, CommandType commandType, IDbConnection connection, IDbTransaction transaction)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException(nameof(commandText), "Command text cannot be null.");
            }

            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection), "Connection cannot be null.");
            }

            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null.");
            }

            IEnumerable<string> result = connection.ExecuteQuery<string>(
                commandText,
                transaction: transaction,
                commandType: commandType);

            return result?.FirstOrDefault() ?? string.Empty;
        }

        /// <inheritdoc/>
        public string QueryAndReturnJson(string commandText, CommandType commandType, IDbTransaction transaction)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.QueryAndReturnJson(commandText, commandType, connection, transaction);
        }

        /// <inheritdoc/>
        public T ExecuteScalar<T>(string commandText, CommandType commandType, IDbConnection connection, IDbTransaction transaction)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException(nameof(commandText), "Command text cannot be null.");
            }

            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection), "Connection cannot be null.");
            }

            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null.");
            }

            T result = connection.ExecuteScalar<T>(
                commandText,
                transaction: transaction,
                commandType: commandType);

            return result;
        }

        /// <inheritdoc/>
        public T ExecuteScalar<T>(string commandText, CommandType commandType, IDbTransaction transaction)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.ExecuteScalar<T>(commandText, commandType, connection, transaction);
        }

        private IDbConnection GetConnectionFromTransaction(IDbTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null.");
            }

            return transaction.Connection
                ?? throw new InvalidOperationException("The provided transaction is not associated with a connection.");
        }
    }
}
