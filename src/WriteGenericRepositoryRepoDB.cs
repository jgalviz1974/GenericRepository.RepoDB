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
        public TKey Insert(T entity, IDbConnection connection, IDbTransaction transaction)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            return (TKey)connection.Insert(entity, transaction: transaction);
        }

        /// <inheritdoc/>
        public TKey Insert(T entity, IDbTransaction transaction)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.Insert(entity, connection, transaction);
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
        public int InsertAll(IEnumerable<T> entities, IDbConnection connection, IDbTransaction transaction)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null.");
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            return connection.InsertAll(entities, transaction: transaction);
        }

        /// <inheritdoc/>
        public int InsertAll(IEnumerable<T> entities, IDbTransaction transaction)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.InsertAll(entities, connection, transaction);
        }

        /// <inheritdoc/>
        public TKey BulkInsert(IEnumerable<T> entities, IEnumerable<BulkInsertMapItem>? mappings = null, SqlBulkCopyOptions options = SqlBulkCopyOptions.Default, string? hints = null, int? batchSize = null, bool isReturnIdentity = false, bool usePhysicalPseudoTempTable = false, SqlTransaction? transaction = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null.");
            }

            using IDbConnection connection = this.CreateConnection();

            int rta = ((SqlConnection)connection).BulkInsert(entities, transaction: transaction);

            return (TKey)(object)rta;
        }

        /// <inheritdoc/>
        public TKey BulkInsert(
            IEnumerable<T> entities,
            IEnumerable<Gasolutions.Core.Repository.Interfaces.BulkInsertColumnMap>? mappings = null,
            Gasolutions.Core.Repository.Interfaces.BulkInsertOptions? options = null,
            IDbTransaction? transaction = null)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null.");
            }

            bool isReturnIdentity = this.GetOptionValue<bool>(options, "ReturnIdentity", "IsReturnIdentity");
            SqlTransaction? sqlTransaction = transaction as SqlTransaction;

            TKey result = this.BulkInsert(
                entities,
                null,
                SqlBulkCopyOptions.Default,
                null,
                null,
                isReturnIdentity,
                false,
                sqlTransaction);

            return isReturnIdentity ? result : default;
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
        public TKey Merge(T entity, IDbConnection connection, IDbTransaction transaction)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            return (TKey)connection.Merge(entity, transaction: transaction);
        }

        /// <inheritdoc/>
        public TKey Merge(T entity, IDbTransaction transaction)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.Merge(entity, connection, transaction);
        }

        /// <inheritdoc/>
        public TKey Merge(T entity, IEnumerable<string> qualifiers)
        {
            IEnumerable<Field> fields = this.ToQualifierFields(qualifiers);
            return this.Merge(entity, fields);
        }

        /// <inheritdoc/>
        public TKey Merge(T entity, IEnumerable<string> qualifiers, IDbTransaction transaction)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.Merge(entity, qualifiers, connection, transaction);
        }

        /// <inheritdoc/>
        public TKey Merge(T entity, IEnumerable<string> qualifiers, IDbConnection connection, IDbTransaction transaction)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            IEnumerable<Field> fields = this.ToQualifierFields(qualifiers);
            return (TKey)connection.Merge(entity, qualifiers: fields, transaction: transaction);
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

        /// <inheritdoc/>
        public int MergeAll(IEnumerable<T> entities, IDbConnection connection, IDbTransaction transaction)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null.");
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            return connection.MergeAll(entities, transaction: transaction);
        }

        /// <inheritdoc/>
        public int MergeAll(IEnumerable<T> entities, IDbTransaction transaction)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.MergeAll(entities, connection, transaction);
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
        /// Deletes records by primary key or by a where clause object using an existing connection and transaction.
        /// </summary>
        /// <param name="whereOrPrimaryKey">Primary key value or a where object describing the rows to delete.</param>
        /// <param name="connection">Open SQL connection to use.</param>
        /// <param name="transaction">Database transaction to enlist the operation in.</param>
        /// <returns>The number of deleted rows.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the whereOrPrimaryKey, connection, or transaction is null.</exception>
        public int Delete(object whereOrPrimaryKey, IDbConnection connection, IDbTransaction transaction)
        {
            if (whereOrPrimaryKey == null)
            {
                throw new ArgumentNullException(nameof(whereOrPrimaryKey), "Where clause or primary key cannot be null.");
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            return connection.Delete<T>(whereOrPrimaryKey, transaction: transaction);
        }

        /// <inheritdoc/>
        public int Delete(object whereOrPrimaryKey, IDbTransaction transaction)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.Delete(whereOrPrimaryKey, connection, transaction);
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

        /// <inheritdoc/>
        public int DeleteAll(IEnumerable<T> entities, IDbConnection connection, IDbTransaction transaction)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null.");
            }

            if (!entities.Any())
            {
                return 0;
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            return connection.DeleteAll(entities, transaction: transaction);
        }

        /// <inheritdoc/>
        public int DeleteAll(IEnumerable<T> entities, IDbTransaction transaction)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.DeleteAll(entities, connection, transaction);
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
        public int Update(T entity, IDbConnection connection, IDbTransaction transaction)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity), "Entity cannot be null.");
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            return connection.Update(entity, transaction: transaction);
        }

        /// <inheritdoc/>
        public int Update(T entity, IDbTransaction transaction)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.Update(entity, connection, transaction);
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

        /// <inheritdoc/>
        public int UpdateAll(IEnumerable<T> entities, IDbConnection connection, IDbTransaction transaction)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities), "Entities collection cannot be null.");
            }

            if (!entities.Any())
            {
                return 0;
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            return connection.UpdateAll(entities, transaction: transaction);
        }

        /// <inheritdoc/>
        public int UpdateAll(IEnumerable<T> entities, IDbTransaction transaction)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.UpdateAll(entities, connection, transaction);
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

        /// <inheritdoc/>
        public int ExecuteNonQuery(string commandText, CommandType commandType, IDbTransaction transaction, IEnumerable<DbParameter>? parameters = null)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.ExecuteNonQuery(commandText, commandType, connection, transaction, parameters);
        }

        private int ExecuteNonQuery(string commandText, CommandType commandType, IDbConnection connection, IDbTransaction transaction, IEnumerable<DbParameter>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null or whitespace.", nameof(commandText));
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            using IDbCommand command = connection.CreateCommand();
            command.CommandText = commandText;
            command.CommandType = commandType;
            command.Transaction = transaction;
            this.AddParameters(command, parameters);
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

        /// <inheritdoc/>
        public TKey ExecuteScalar(string commandText, CommandType commandType, IDbTransaction transaction, IEnumerable<DbParameter>? parameters = null)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.ExecuteScalar(commandText, commandType, connection, transaction, parameters);
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

        /// <inheritdoc/>
        public IDataReader ExecuteReader(string commandText, CommandType commandType, IDbTransaction transaction, IEnumerable<DbParameter>? parameters = null)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.ExecuteReader(commandText, commandType, connection, transaction, parameters);
        }

        /// <inheritdoc/>
        public IDataReader ExecuteReader(string commandText, CommandType commandType, IDbConnection connection, IDbTransaction transaction, IEnumerable<DbParameter>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null or whitespace.", nameof(commandText));
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            return connection.ExecuteReader(
                commandText,
                parameters,
                transaction: transaction,
                commandType: commandType);
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

        /// <inheritdoc/>
        public IEnumerable<T> ExecuteQuery(string commandText, CommandType commandType, IDbTransaction transaction, IEnumerable<DbParameter>? parameters = null)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.ExecuteQuery(commandText, commandType, connection, transaction, parameters);
        }

        /// <inheritdoc/>
        public IEnumerable<T> ExecuteQuery(string commandText, CommandType commandType, IDbConnection connection, IDbTransaction transaction, IEnumerable<DbParameter>? parameters = null)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null or whitespace.", nameof(commandText));
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            return connection.ExecuteQuery<T>(
                commandText,
                parameters,
                transaction: transaction,
                commandType: commandType);
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

        /// <inheritdoc/>
        public string ExecuteScalar(string commandText, CommandType commandType, IDbTransaction transaction)
        {
            IDbConnection connection = this.GetConnectionFromTransaction(transaction);
            return this.ExecuteScalar(commandText, commandType, connection, transaction);
        }

        /// <inheritdoc/>
        public string ExecuteScalar(string commandText, CommandType commandType, IDbConnection connection, IDbTransaction transaction)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null or whitespace.", nameof(commandText));
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            return (string)connection.ExecuteScalar(
                commandText,
                null,
                transaction: transaction,
                commandType: commandType);
        }

        private TKey ExecuteScalar(string commandText, CommandType commandType, IDbConnection connection, IDbTransaction transaction, IEnumerable<DbParameter>? parameters)
        {
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentException("Command text cannot be null or whitespace.", nameof(commandText));
            }

            this.ValidateConnectionAndTransaction(connection, transaction);
            return (TKey)connection.ExecuteScalar(
                commandText,
                parameters,
                transaction: transaction,
                commandType: commandType);
        }

        private IDbConnection CreateConnection()
        {
            return this.connectionFactory != null
                ? this.connectionFactory()
                : new SqlConnection(this.connectionString);
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

        private void ValidateConnectionAndTransaction(IDbConnection connection, IDbTransaction transaction)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection), "Connection cannot be null.");
            }

            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null.");
            }
        }

        private void AddParameters(IDbCommand command, IEnumerable<DbParameter>? parameters)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (DbParameter parameter in parameters)
            {
                _ = command.Parameters.Add(parameter);
            }
        }

        private IEnumerable<Field> ToQualifierFields(IEnumerable<string> qualifiers)
        {
            if (qualifiers == null)
            {
                throw new ArgumentNullException(nameof(qualifiers), "Qualifiers cannot be null.");
            }

            List<Field> fields = [];
            foreach (string qualifier in qualifiers)
            {
                if (string.IsNullOrWhiteSpace(qualifier))
                {
                    throw new ArgumentException("Qualifiers cannot contain null or empty values.", nameof(qualifiers));
                }

                fields.Add(new Field(qualifier));
            }

            return fields;
        }

        private IEnumerable<BulkInsertMapItem>? ToBulkInsertMapItems(IEnumerable<Gasolutions.Core.Repository.Interfaces.BulkInsertColumnMap>? mappings)
        {
            if (mappings == null)
            {
                return null;
            }

            List<BulkInsertMapItem> results = [];

            foreach (object mapping in mappings)
            {
                string? source = this.GetOptionValue<string>(mapping, "SourceColumn", "Source", "PropertyName", "SourceProperty", "MemberName");
                string? destination = this.GetOptionValue<string>(mapping, "DestinationColumn", "Destination", "ColumnName", "TargetColumn");

                if (!string.IsNullOrWhiteSpace(source) && !string.IsNullOrWhiteSpace(destination))
                {
                    results.Add(new BulkInsertMapItem(source, destination));
                }
            }

            return results.Count > 0 ? results : null;
        }

        private TValue GetOptionValue<TValue>(object? source, params string[] propertyNames)
        {
            if (source == null)
            {
                return default!;
            }

            Type type = source.GetType();
            foreach (string propertyName in propertyNames)
            {
                System.Reflection.PropertyInfo? property = type.GetProperty(propertyName);
                if (property == null)
                {
                    continue;
                }

                object? value = property.GetValue(source);
                if (value == null)
                {
                    return default!;
                }

                if (value is TValue typedValue)
                {
                    return typedValue;
                }
            }

            return default!;
        }
    }
}
