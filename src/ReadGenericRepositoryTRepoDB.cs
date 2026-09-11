// <copyright file="ReadGenericRepositoryTRepoDB.cs" company="Gasolutions SAS">
// Copyright (c) Gasolutions SAS. Todos los derechos reservados.
// </copyright>

namespace Gasolutions.Core.Repository
{
    /// <summary>
    /// Generic repository implementation for read operations against a SQL Server database using RepoDb.
    /// </summary>
    /// <typeparam name="T">Entity type returned by queries.</typeparam>
    /// <typeparam name="TKey">Primary key type used for scalar operations.</typeparam>
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1649:FileNameMustMatchTypeName", Justification = "Reviewed.")]
    public class ReadGenericRepositoryRepoDB<T, TKey> : IReadGenericRepository<T, TKey>
        where T : class
        where TKey : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadGenericRepositoryRepoDB{T, TKey}"/> class.
        /// </summary>
        /// <param name="connectionString">Connection string used to connect to the database.</param>
        public ReadGenericRepositoryRepoDB(string connectionString)
        {
            this.ConnectionString = connectionString;
        }

        /// <summary>
        /// Gets or Sets the Connection string used by the repository to open SQL Server connections.
        /// </summary>
        private string ConnectionString { get; set; } = string.Empty;

        /// <inheritdoc/>
        public long Count()
        {
            using SqlConnection connection = new(this.ConnectionString);
            return connection.Count<T>(where: (object?)null);
        }

        /// <inheritdoc/>
        public long Count(IDbTransaction transaction)
        {
            IDbConnection connection = GetConnectionFromTransaction(transaction);
            return this.Count(connection, transaction);
        }

        /// <inheritdoc/>
        public long Count(IDbConnection connection, IDbTransaction transaction)
        {
            ValidateConnectionAndTransaction(connection, transaction);
            return connection.Count<T>(where: (object?)null, transaction: transaction);
        }

        /// <inheritdoc/>
        public long Count(object whereOrPrimaryKey)
        {
            using SqlConnection connection = new(this.ConnectionString);
            return connection.Count<T>(where: whereOrPrimaryKey);
        }

        /// <inheritdoc/>
        public long Count(object whereOrPrimaryKey, IDbTransaction transaction)
        {
            IDbConnection connection = GetConnectionFromTransaction(transaction);
            return this.Count(whereOrPrimaryKey, connection, transaction);
        }

        /// <inheritdoc/>
        public long Count(object whereOrPrimaryKey, IDbConnection connection, IDbTransaction transaction)
        {
            ValidateConnectionAndTransaction(connection, transaction);
            return connection.Count<T>(where: whereOrPrimaryKey, transaction: transaction);
        }

        /// <summary>
        /// Executes a query using a where object or primary key and returns matching entities.
        /// </summary>
        /// <param name="whereOrPrimaryKey">An object representing the WHERE clause or the primary key value.</param>
        /// <returns>An enumerable of entities of type <typeparamref name="T"/> matching the query.</returns>
        public IEnumerable<T> Query(object whereOrPrimaryKey)
        {
            using SqlConnection connection = new(this.ConnectionString);
            return connection.Query<T>(whereOrPrimaryKey);
        }

        /// <inheritdoc/>
        public IEnumerable<T> Query(object whereOrPrimaryKey, IDbTransaction transaction)
        {
            IDbConnection connection = GetConnectionFromTransaction(transaction);
            return this.Query(whereOrPrimaryKey, connection, transaction);
        }

        /// <inheritdoc/>
        public IEnumerable<T> Query(object whereOrPrimaryKey, IDbConnection connection, IDbTransaction transaction)
        {
            ValidateConnectionAndTransaction(connection, transaction);
            return connection.Query<T>(whereOrPrimaryKey, transaction: transaction);
        }

        /// <summary>
        /// Executes a query using a where object or primary key and returns matching entities ordered by the specified fields.
        /// </summary>
        /// <param name="whereOrPrimaryKey">An object representing the WHERE clause or the primary key value.</param>
        /// <param name="orderBy">Sequence of fields describing the ordering for the returned rows.</param>
        /// <returns>An enumerable of entities of type <typeparamref name="T"/> matching the query.</returns>
        public IEnumerable<T> Query(object whereOrPrimaryKey, IEnumerable<OrderField> orderBy)
        {
            using SqlConnection connection = new(this.ConnectionString);
            return connection.Query<T>(whereOrPrimaryKey, orderBy: orderBy);
        }

        /// <inheritdoc/>
        public IEnumerable<T> Query(object whereOrPrimaryKey, IEnumerable<string> orderBy)
        {
            using SqlConnection connection = new(this.ConnectionString);
            return connection.Query<T>(whereOrPrimaryKey, orderBy: this.ToOrderFields(orderBy));
        }

        /// <inheritdoc/>
        public IEnumerable<T> Query(object whereOrPrimaryKey, IEnumerable<string> orderBy, IDbTransaction transaction)
        {
            IDbConnection connection = GetConnectionFromTransaction(transaction);
            return this.Query(whereOrPrimaryKey, orderBy, connection, transaction);
        }

        /// <inheritdoc/>
        public IEnumerable<T> Query(object whereOrPrimaryKey, IEnumerable<string> orderBy, IDbConnection connection, IDbTransaction transaction)
        {
            ValidateConnectionAndTransaction(connection, transaction);
            return connection.Query<T>(whereOrPrimaryKey, orderBy: this.ToOrderFields(orderBy), transaction: transaction);
        }

        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/> from the database.
        /// </summary>
        /// <returns>An enumerable of all entities of type <typeparamref name="T"/>.</returns>
        public IEnumerable<T> QueryAll()
        {
            using SqlConnection connection = new(this.ConnectionString);
            return connection.QueryAll<T>();
        }

        /// <inheritdoc/>
        public IEnumerable<T> QueryAll(IDbTransaction transaction)
        {
            IDbConnection connection = GetConnectionFromTransaction(transaction);
            return this.QueryAll(connection, transaction);
        }

        /// <inheritdoc/>
        public IEnumerable<T> QueryAll(IDbConnection connection, IDbTransaction transaction)
        {
            ValidateConnectionAndTransaction(connection, transaction);
            return connection.QueryAll<T>(transaction: transaction);
        }

        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/> and optionally uses a memory cache key.
        /// </summary>
        /// <param name="cacheKey">Cache key used to store/retrieve results.</param>
        /// <param name="renewCache">If true, the cache entry is removed before querying.</param>
        /// <returns>An enumerable of all entities of type <typeparamref name="T"/>.</returns>
        public IEnumerable<T> QueryAll(string cacheKey, bool renewCache)
        {
            MemoryCache cache = this.CreateQueryAllCache(cacheKey, renewCache);
            using SqlConnection connection = new(this.ConnectionString);

            return connection.QueryAll<T>(cacheKey: cacheKey, cache: cache);
        }

        /// <inheritdoc/>
        public IEnumerable<T> QueryAll(string cacheKey, bool renewCache, IDbTransaction transaction)
        {
            IDbConnection connection = GetConnectionFromTransaction(transaction);
            return this.QueryAll(cacheKey, renewCache, connection, transaction);
        }

        /// <inheritdoc/>
        public IEnumerable<T> QueryAll(string cacheKey, bool renewCache, IDbConnection connection, IDbTransaction transaction)
        {
            ValidateConnectionAndTransaction(connection, transaction);
            MemoryCache cache = this.CreateQueryAllCache(cacheKey, renewCache);
            return connection.QueryAll<T>(cacheKey: cacheKey, cache: cache, transaction: transaction);
        }

        /// <summary>
        /// Returns the maximum value for the specified field in the given table filtered by the where object or primary key.
        /// </summary>
        /// <param name="tableName">Name of the table to query.</param>
        /// <param name="fieldName">Name of the field to compute the maximum for.</param>
        /// <param name="whereOrPrimaryKey">An object representing the WHERE clause or the primary key for filtering.</param>
        /// <returns>The maximum value cast to <typeparamref name="TKey"/>, or null if no rows exist.</returns>
        public TKey? Max(string tableName, string fieldName, object whereOrPrimaryKey)
        {
            RepoDb.Field field = new(fieldName);

            using SqlConnection connection = new(this.ConnectionString);
            object max = connection.Max(tableName, field, whereOrPrimaryKey);

            if (max == DBNull.Value)
            {
                return null;
            }

            return (TKey?)max;
        }

        /// <inheritdoc/>
        public TKey? Max(string fieldName, object whereOrPrimaryKey, IDbTransaction transaction)
        {
            IDbConnection connection = GetConnectionFromTransaction(transaction);
            return this.Max(fieldName, whereOrPrimaryKey, connection, transaction);
        }

        /// <inheritdoc/>
        public TKey? Max(string fieldName, object whereOrPrimaryKey, IDbConnection connection, IDbTransaction transaction)
        {
            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentNullException(nameof(fieldName), "Field name cannot be null or empty.");
            }

            ValidateConnectionAndTransaction(connection, transaction);
            RepoDb.Field field = new(fieldName);
            object max = connection.Max<T>(field, whereOrPrimaryKey, transaction: transaction);

            if (max == DBNull.Value)
            {
                return null;
            }

            return (TKey?)max;
        }

        /// <inheritdoc/>
        public TKey? Max(object whereOrPrimaryKey)
        {
            throw new NotSupportedException("Max(object whereOrPrimaryKey) is not supported by the current RepoDb API. Use Max(fieldName, whereOrPrimaryKey, ...) overload instead.");
        }

        /// <inheritdoc/>
        public TKey? Max(object whereOrPrimaryKey, IDbTransaction transaction)
        {
            IDbConnection connection = GetConnectionFromTransaction(transaction);
            return this.Max(whereOrPrimaryKey, connection, transaction);
        }

        /// <inheritdoc/>
        public TKey? Max(object whereOrPrimaryKey, IDbConnection connection, IDbTransaction transaction)
        {
            ValidateConnectionAndTransaction(connection, transaction);
            throw new NotSupportedException("Max(object whereOrPrimaryKey, connection, transaction) is not supported by the current RepoDb API. Use Max(fieldName, whereOrPrimaryKey, connection, transaction) overload instead.");
        }

        /// <summary>
        /// Executes an asynchronous query using a where object or primary key and returns matching entities.
        /// </summary>
        /// <param name="whereOrPrimaryKey">An object representing the WHERE clause or the primary key value.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of entities of type <typeparamref name="T"/> matching the query.</returns>
        public async Task<IEnumerable<T>> QueryAsync(object whereOrPrimaryKey)
        {
            using SqlConnection connection = new(this.ConnectionString);
            return await connection.QueryAsync<T>(whereOrPrimaryKey);
        }

        /// <summary>
        /// Executes an asynchronous query using a where object or primary key and returns matching entities ordered by the specified fields.
        /// </summary>
        /// <param name="whereOrPrimaryKey">An object representing the WHERE clause or the primary key value.</param>
        /// <param name="orderBy">Sequence of fields describing the ordering for the returned rows.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of entities of type <typeparamref name="T"/> matching the query.</returns>
        public async Task<IEnumerable<T>> QueryAsync(object whereOrPrimaryKey, IEnumerable<OrderField> orderBy)
        {
            using SqlConnection connection = new(this.ConnectionString);
            return await connection.QueryAsync<T>(whereOrPrimaryKey, orderBy: orderBy);
        }

        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/> from the database asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of all entities of type <typeparamref name="T"/>.</returns>
        public async Task<IEnumerable<T>> QueryAllAsync()
        {
            using SqlConnection connection = new(this.ConnectionString);
            return await connection.QueryAllAsync<T>();
        }

        /// <summary>
        /// Retrieves all entities of type <typeparamref name="T"/> and optionally uses a memory cache key asynchronously.
        /// </summary>
        /// <param name="cacheKey">Cache key used to store/retrieve results.</param>
        /// <param name="renewCache">If true, the cache entry is removed before querying.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable of all entities of type <typeparamref name="T"/>.</returns>
        public async Task<IEnumerable<T>> QueryAllAsync(string cacheKey, bool renewCache)
        {
            MemoryCache cache = this.CreateQueryAllCache(cacheKey, renewCache);
            using SqlConnection connection = new(this.ConnectionString);

            return await connection.QueryAllAsync<T>(cacheKey: cacheKey, cache: cache);
        }

        private static IDbConnection GetConnectionFromTransaction(IDbTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction), "Transaction cannot be null.");
            }

            return transaction.Connection
                ?? throw new InvalidOperationException("The provided transaction is not associated with a connection.");
        }

        private static void ValidateConnectionAndTransaction(IDbConnection connection, IDbTransaction transaction)
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

        private MemoryCache CreateQueryAllCache(string cacheKey, bool renewCache)
        {
            MemoryCache cache = [];

            if (renewCache)
            {
                cache.Remove(cacheKey);
            }

            return cache;
        }

        private IEnumerable<OrderField> ToOrderFields(IEnumerable<string> orderBy)
        {
            if (orderBy == null)
            {
                throw new ArgumentNullException(nameof(orderBy), "Order by fields cannot be null.");
            }

            List<OrderField> orderFields = [];
            foreach (string field in orderBy)
            {
                if (string.IsNullOrWhiteSpace(field))
                {
                    throw new ArgumentException("Order by fields cannot contain null or empty values.", nameof(orderBy));
                }

                orderFields.Add(new OrderField(field));
            }

            return orderFields;
        }
    }
}
