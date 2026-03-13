// <copyright file="WriteGenericRepositoryRepoDB.cs" company="Gasolutions SAS">
// Copyright (c) Gasolutions SAS. Todos los derechos reservados.
// </copyright>
namespace Gasolutions.Core.Repository
{
    /// <summary>
    /// Repository implementation for write operations against a relational database (SQL Server).
    /// </summary>
    /// <typeparam name="T">Entity type handled by the repository.</typeparam>
    /// <typeparam name="TKey">Primary key type returned by insert/merge operations.</typeparam>
    public class WriteGenericRepositoryRepoDB<T, TKey> : IWriteGenericRepository<T, TKey>
        where T : class
        where TKey : struct
    {
        private readonly string connectionString;
        private readonly Func<IDbConnection>? connectionFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteGenericRepositoryRepoDB{T, TKey}"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string used to open SQL Server connections.</param>
        public WriteGenericRepositoryRepoDB(string connectionString)
        {
            this.connectionString = connectionString;
            this.connectionFactory = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteGenericRepositoryRepoDB{T, TKey}"/> class.
        /// Initializes a new instance with a connection factory (for testing).
        /// </summary>
        public WriteGenericRepositoryRepoDB(Func<IDbConnection> connectionFactory)
        {
            this.connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
            this.connectionString = string.Empty;
        }

        /// <summary>
        /// Inserts the specified entity into the database and returns the generated primary key.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        /// <returns>The generated primary key of type <typeparamref name="TKey"/>.</returns>
        public TKey Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            using IDbConnection connection = this.CreateConnection();
            return (TKey)((SqlConnection)connection).Insert(entity);
        }

        /// <summary>
        /// Inserts the specified entity using an existing open connection and transaction.
        /// </summary>
        /// <param name="entity">Entity to insert.</param>
        /// <param name="connection">Open SQL connection to use.</param>
        /// <param name="transaction">Database transaction to enlist the operation in.</param>
        /// <returns>The generated primary key of type <typeparamref name="TKey"/>.</returns>
        public TKey Insert(T entity, SqlConnection connection, IDbTransaction transaction)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            return (TKey)connection.Insert(entity, transaction: transaction);
        }

        /// <summary>
        /// Inserts multiple entities in a single operation.
        /// </summary>
        /// <param name="entities">Collection of entities to insert.</param>
        /// <returns>The number of inserted rows.</returns>
        public int InsertAll(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null.");
            }

            using IDbConnection connection = this.CreateConnection();

            return connection.InsertAll(entities);
        }

        /// <inheritdoc/>
        public TKey BulkInsert(IEnumerable<T> entities, IEnumerable<BulkInsertMapItem>? mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, string? hints = null, int? batchSize = null, bool isReturnIdentity = false, bool usePhysicalPseudoTempTable = false, SqlTransaction? transaction = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null.");
            }

            using IDbConnection connection = this.CreateConnection();

            int rta = ((SqlConnection)connection).BulkInsert(entities, mappings, options, hints, null, batchSize, isReturnIdentity, usePhysicalPseudoTempTable, transaction);

            return (TKey)(object)rta;
        }

        /// <summary>
        /// Merges (upserts) the specified entity and returns the primary key.
        /// </summary>
        /// <param name="entity">Entity to merge.</param>
        /// <returns>The primary key value of the merged entity.</returns>
        public TKey Merge(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            using IDbConnection connection = this.CreateConnection();
            return (TKey)connection.Merge(entity);
        }

        /// <summary>
        /// Merges (upserts) the specified entity using the provided qualifiers to identify duplicates.
        /// </summary>
        /// <param name="entity">Entity to merge.</param>
        /// <param name="qualifiers">Fields used as qualifiers for the merge operation.</param>
        /// <returns>The primary key value of the merged entity.</returns>
        public TKey Merge(T entity, IEnumerable<Field> qualifiers)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            if (qualifiers == null)
            {
                throw new ArgumentNullException(nameof(qualifiers), "Qualifiers cannot be null.");
            }

            using IDbConnection connection = this.CreateConnection();
            return (TKey)connection.Merge(entity, qualifiers: qualifiers);
        }

        /// <summary>
        /// Merges (upserts) the specified entity using an existing connection and transaction.
        /// </summary>
        /// <param name="entity">Entity to merge.</param>
        /// <param name="connection">Open SQL connection to use.</param>
        /// <param name="transaction">Database transaction to enlist the operation in.</param>
        /// <returns>The primary key value of the merged entity.</returns>
        public TKey Merge(T entity, SqlConnection connection, IDbTransaction transaction)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            return (TKey)connection.Merge(entity, transaction: transaction);
        }

        /// <summary>
        /// Merges a collection of entities in a single operation.
        /// </summary>
        /// <param name="entities">Collection of entities to merge.</param>
        /// <returns>The number of processed rows.</returns>
        public int MergeAll(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null.");
            }

            using IDbConnection connection = this.CreateConnection();
            return connection.MergeAll(entities);
        }

        /// <summary>
        /// Deletes records by primary key or by a where clause object.
        /// </summary>
        /// <param name="whereOrPrimaryKey">Primary key value or a where object describing the rows to delete.</param>
        /// <returns>The number of deleted rows.</returns>
        public int Delete(object whereOrPrimaryKey)
        {
            if (whereOrPrimaryKey == null)
            {
                throw new ArgumentNullException(nameof(whereOrPrimaryKey), "Where clause or primary key cannot be null.");
            }

            using IDbConnection connection = this.CreateConnection();
            return connection.Delete<T>(whereOrPrimaryKey);
        }

        /// <summary>
        /// Deletes all the supplied entities.
        /// </summary>
        /// <param name="entities">Entities to delete.</param>
        /// <returns>The number of deleted rows.</returns>
        public int DeleteAll(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null.");
            }

            if (!entities.Any())
            {
                return 0;
            }

            using IDbConnection connection = this.CreateConnection();
            return connection.DeleteAll(entities);
        }

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">Entity with updated values.</param>
        /// <returns>The number of affected rows.</returns>
        public int Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            using IDbConnection connection = this.CreateConnection();
            return connection.Update(entity);
        }

        /// <summary>
        /// Updates the specified entity using an existing connection and transaction.
        /// </summary>
        /// <param name="entity">Entity with updated values.</param>
        /// <param name="connection">Open SQL connection to use.</param>
        /// <param name="transaction">Database transaction to enlist the operation in.</param>
        /// <returns>The number of affected rows.</returns>
        public int Update(T entity, SqlConnection connection, IDbTransaction transaction)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            return connection.Update(entity, transaction: transaction);
        }

        /// <summary>
        /// Updates a collection of entities in a single operation.
        /// </summary>
        /// <param name="entities">Entities to update.</param>
        /// <returns>The number of affected rows.</returns>
        public int UpdateAll(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null.");
            }

            // Early return for empty collections to avoid unnecessary database connection
            if (!entities.Any())
            {
                return 0;
            }

            using IDbConnection connection = this.CreateConnection();
            return connection.UpdateAll(entities);
        }

        /// <summary>
        /// Executes a non-query command (INSERT/UPDATE/DELETE) against the database.
        /// </summary>
        /// <param name="commandText">SQL command text or stored procedure name.</param>
        /// <param name="commandType">Type of the command (Text or StoredProcedure).</param>
        /// <param name="parameters">Optional parameters to be added to the command.</param>
        /// <returns>The number of affected rows.</returns>
        public int ExecuteNonQuery(string commandText, CommandType commandType, IEnumerable<DbParameter>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null or whitespace.", nameof(commandText));
            }

            using IDbConnection connection = this.CreateConnection();
            using IDbCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;

            if (parameters != null)
            {
                foreach (DbParameter parameter in parameters)
                {
                    _ = command.Parameters.Add(parameter);
                }
            }

            connection.Open();
            return command.ExecuteNonQuery();
        }

        /// <summary>
        /// Executes a scalar command and returns a typed result.
        /// </summary>
        /// <param name="commandText">SQL command text or stored procedure name.</param>
        /// <param name="commandType">Type of the command (Text or StoredProcedure).</param>
        /// <param name="parameters">Optional parameters to be passed to the command.</param>
        /// <returns>The scalar result cast to <typeparamref name="TKey"/>.</returns>
        public TKey ExecuteScalar(string commandText, CommandType commandType, IEnumerable<DbParameter>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null or whitespace.", nameof(commandText));
            }

            using IDbConnection connection = this.CreateConnection();
            return (TKey)connection.ExecuteScalar(commandText, parameters, commandType);
        }

        /// <summary>
        /// Executes a reader command and returns a data reader for streaming results.
        /// </summary>
        /// <param name="commandText">SQL command text or stored procedure name.</param>
        /// <param name="commandType">Type of the command (Text or StoredProcedure).</param>
        /// <param name="parameters">Optional parameters to be passed to the command.</param>
        /// <returns>An <see cref="IDataReader"/> instance with the query results.</returns>
        public IDataReader ExecuteReader(string commandText, CommandType commandType, IEnumerable<DbParameter>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null or whitespace.", nameof(commandText));
            }

            using IDbConnection connection = this.CreateConnection();
            return connection.ExecuteReader(commandText, parameters, commandType);
        }

        /// <summary>
        /// Executes a query and maps the results to a sequence of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="commandText">SQL command text or stored procedure name.</param>
        /// <param name="commandType">Type of the command (Text or StoredProcedure).</param>
        /// <param name="parameters">Optional parameters to be passed to the command.</param>
        /// <returns>An enumerable of entities returned by the query.</returns>
        public IEnumerable<T> ExecuteQuery(string commandText, CommandType commandType, IEnumerable<DbParameter>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null or whitespace.", nameof(commandText));
            }

            using IDbConnection connection = this.CreateConnection();
            return connection.ExecuteQuery<T>(commandText, parameters, commandType);
        }

        /// <summary>
        /// Executes a scalar command that returns a string.
        /// </summary>
        /// <param name="commandText">SQL command text or stored procedure name.</param>
        /// <param name="commandType">Type of the command (Text or StoredProcedure).</param>
        /// <returns>The scalar string result.</returns>
        public string ExecuteScalar(string commandText, CommandType commandType)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null or whitespace.", nameof(commandText));
            }

            using IDbConnection connection = this.CreateConnection();
            return (string)connection.ExecuteScalar(commandText, null, commandType);
        }

        private IDbConnection CreateConnection()
        {
            return this.connectionFactory != null
                ? this.connectionFactory()
                : new SqlConnection(this.connectionString);
        }
    }
}
