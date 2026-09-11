// Copyright (c) Gasolutions SAS. Todos los derechos reservados.

namespace Gasolutions.Core.Repository.UnitTests
{
    public class RepositoryOverloadsCoverageTests
    {
        [Fact]
        public void ReadGeneric_Max_WithEmptyFieldName_ThrowsArgumentNullException()
        {
            ReadGenericRepositoryRepoDB repository = new("Server=localhost;Database=TestDb;");
            IDbConnection connection = new Mock<IDbConnection>().Object;
            IDbTransaction transaction = new Mock<IDbTransaction>().Object;

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
                repository.Max("Table", string.Empty, new { Id = 1 }, connection, transaction));

            Assert.Equal("fieldName", exception.ParamName);
        }

        [Fact]
        public void ReadGeneric_Max_WithTransactionWithoutConnection_ThrowsInvalidOperationException()
        {
            ReadGenericRepositoryRepoDB repository = new("Server=localhost;Database=TestDb;");
            IDbTransaction transaction = CreateTransactionWithoutConnection();

            _ = Assert.Throws<InvalidOperationException>(() =>
                repository.Max("Table", "Id", new { Id = 1 }, transaction));
        }

        [Fact]
        public void ReadGeneric_QueryAndReturnJson_WithTransactionWithoutConnection_ThrowsInvalidOperationException()
        {
            ReadGenericRepositoryRepoDB repository = new("Server=localhost;Database=TestDb;");
            IDbTransaction transaction = CreateTransactionWithoutConnection();

            _ = Assert.Throws<InvalidOperationException>(() =>
                repository.QueryAndReturnJson("SELECT 1", CommandType.Text, transaction));
        }

        [Fact]
        public void ReadGeneric_ExecuteScalar_WithTransactionWithoutConnection_ThrowsInvalidOperationException()
        {
            ReadGenericRepositoryRepoDB repository = new("Server=localhost;Database=TestDb;");
            IDbTransaction transaction = CreateTransactionWithoutConnection();

            _ = Assert.Throws<InvalidOperationException>(() =>
                repository.ExecuteScalar<int>("SELECT 1", CommandType.Text, transaction));
        }

        [Fact]
        public async Task ReadGeneric_QueryAndReturnJsonAsync_WithInvalidConnection_ThrowsException()
        {
            ReadGenericRepositoryRepoDB repository = new("Server=localhost;Database=TestDb;User Id=sa;Password=test;");

            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync("SELECT 1", CommandType.Text));
        }

        [Fact]
        public void ReadGenericT_Count_WithTransactionWithoutConnection_ThrowsInvalidOperationException()
        {
            ReadGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IDbTransaction transaction = CreateTransactionWithoutConnection();

            _ = Assert.Throws<InvalidOperationException>(() => repository.Count(transaction));
        }

        [Fact]
        public void ReadGenericT_Query_WithNullOrderBy_ThrowsArgumentNullException()
        {
            ReadGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
                repository.Query(new { Id = 1 }, (IEnumerable<string>)null!));

            Assert.Equal("orderBy", exception.ParamName);
        }

        [Fact]
        public void ReadGenericT_Query_WithWhitespaceOrderBy_ThrowsArgumentException()
        {
            ReadGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IEnumerable<string> orderBy = ["Id", " "];
            IDbConnection connection = new Mock<IDbConnection>().Object;
            IDbTransaction transaction = new Mock<IDbTransaction>().Object;

            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.Query(new { Id = 1 }, orderBy, connection, transaction));

            Assert.Equal("orderBy", exception.ParamName);
        }

        [Fact]
        public void ReadGenericT_QueryAll_WithTransactionWithoutConnection_ThrowsInvalidOperationException()
        {
            ReadGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IDbTransaction transaction = CreateTransactionWithoutConnection();

            _ = Assert.Throws<InvalidOperationException>(() => repository.QueryAll(transaction));
        }

        [Fact]
        public void ReadGenericT_MaxField_WithEmptyFieldName_ThrowsArgumentNullException()
        {
            ReadGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IDbConnection connection = new Mock<IDbConnection>().Object;
            IDbTransaction transaction = new Mock<IDbTransaction>().Object;

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
                repository.Max(string.Empty, new { Id = 1 }, connection, transaction));

            Assert.Equal("fieldName", exception.ParamName);
        }

        [Fact]
        public void ReadGenericT_MaxObject_ThrowsNotSupportedException()
        {
            ReadGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");

            _ = Assert.Throws<NotSupportedException>(() => repository.Max(new { Id = 1 }));
        }

        [Fact]
        public void ReadGenericT_MaxObjectWithConnection_ThrowsNotSupportedException()
        {
            ReadGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IDbConnection connection = new Mock<IDbConnection>().Object;
            IDbTransaction transaction = new Mock<IDbTransaction>().Object;

            _ = Assert.Throws<NotSupportedException>(() =>
                repository.Max(new { Id = 1 }, connection, transaction));
        }

        [Fact]
        public async Task ReadGenericT_QueryAsync_WithInvalidConnection_ThrowsException()
        {
            ReadGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;User Id=sa;Password=test;");

            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAsync(new { Id = 1 }));
        }

        [Fact]
        public async Task ReadGenericT_QueryAsyncWithOrderBy_WithInvalidConnection_ThrowsException()
        {
            ReadGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;User Id=sa;Password=test;");

            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAsync(new { Id = 1 }, [new RepoDb.OrderField("Id")]));
        }

        [Fact]
        public async Task ReadGenericT_QueryAllAsync_WithInvalidConnection_ThrowsException()
        {
            ReadGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;User Id=sa;Password=test;");

            _ = await Assert.ThrowsAnyAsync<Exception>(() => repository.QueryAllAsync());
        }

        [Fact]
        public async Task ReadGenericT_QueryAllAsyncWithCache_WithInvalidConnection_ThrowsException()
        {
            ReadGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;User Id=sa;Password=test;");

            _ = await Assert.ThrowsAnyAsync<Exception>(() => repository.QueryAllAsync("cache-key", renewCache: true));
        }

        [Fact]
        public void WriteGeneric_Insert_WithTransactionWithoutConnection_ThrowsInvalidOperationException()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IDbTransaction transaction = CreateTransactionWithoutConnection();

            _ = Assert.Throws<InvalidOperationException>(() => repository.Insert(new TestEntity(), transaction));
        }

        [Fact]
        public void WriteGeneric_InsertAll_WithTransactionWithoutConnection_ThrowsInvalidOperationException()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IDbTransaction transaction = CreateTransactionWithoutConnection();

            _ = Assert.Throws<InvalidOperationException>(() =>
                repository.InsertAll([new TestEntity()], transaction));
        }

        [Fact]
        public void WriteGeneric_Merge_WithNullStringQualifiers_ThrowsArgumentNullException()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
                repository.Merge(new TestEntity(), (IEnumerable<string>)null!));

            Assert.Equal("qualifiers", exception.ParamName);
        }

        [Fact]
        public void WriteGeneric_Merge_WithWhitespaceStringQualifier_ThrowsArgumentException()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IDbConnection connection = new Mock<IDbConnection>().Object;
            IDbTransaction transaction = new Mock<IDbTransaction>().Object;

            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.Merge(new TestEntity(), ["Id", " "], connection, transaction));

            Assert.Equal("qualifiers", exception.ParamName);
        }

        [Fact]
        public void WriteGeneric_MergeAll_WithTransactionWithoutConnection_ThrowsInvalidOperationException()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IDbTransaction transaction = CreateTransactionWithoutConnection();

            _ = Assert.Throws<InvalidOperationException>(() =>
                repository.MergeAll([new TestEntity()], transaction));
        }

        [Fact]
        public void WriteGeneric_Delete_WithTransactionWithoutConnection_ThrowsInvalidOperationException()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IDbTransaction transaction = CreateTransactionWithoutConnection();

            _ = Assert.Throws<InvalidOperationException>(() => repository.Delete(new { Id = 1 }, transaction));
        }

        [Fact]
        public void WriteGeneric_DeleteAll_WithEmptyCollectionAndConnectionOverload_ReturnsZero()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IDbConnection connection = new Mock<IDbConnection>().Object;
            IDbTransaction transaction = new Mock<IDbTransaction>().Object;

            int result = repository.DeleteAll([], connection, transaction);

            Assert.Equal(0, result);
        }

        [Fact]
        public void WriteGeneric_UpdateAll_WithEmptyCollectionAndConnectionOverload_ReturnsZero()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IDbConnection connection = new Mock<IDbConnection>().Object;
            IDbTransaction transaction = new Mock<IDbTransaction>().Object;

            int result = repository.UpdateAll([], connection, transaction);

            Assert.Equal(0, result);
        }

        [Fact]
        public void WriteGeneric_ExecuteNonQuery_WithNullTransaction_ThrowsArgumentNullException()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
                repository.ExecuteNonQuery("SELECT 1", CommandType.Text, (IDbTransaction)null!));

            Assert.Equal("transaction", exception.ParamName);
        }

        [Fact]
        public void WriteGeneric_ExecuteScalarTyped_WithNullTransaction_ThrowsArgumentNullException()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
                repository.ExecuteScalar("SELECT 1", CommandType.Text, (IDbTransaction)null!, (IEnumerable<DbParameter>?)null));

            Assert.Equal("transaction", exception.ParamName);
        }

        [Fact]
        public void WriteGeneric_ExecuteReader_WithNullTransaction_ThrowsArgumentNullException()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
                repository.ExecuteReader("SELECT 1", CommandType.Text, (IDbTransaction)null!));

            Assert.Equal("transaction", exception.ParamName);
        }

        [Fact]
        public void WriteGeneric_ExecuteQuery_WithNullTransaction_ThrowsArgumentNullException()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
                repository.ExecuteQuery("SELECT 1", CommandType.Text, (IDbTransaction)null!));

            Assert.Equal("transaction", exception.ParamName);
        }

        [Fact]
        public void WriteGeneric_ExecuteScalarString_WithNullTransaction_ThrowsArgumentNullException()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IDbConnection connection = new Mock<IDbConnection>().Object;

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
                repository.ExecuteScalar("SELECT 1", CommandType.Text, connection, (IDbTransaction)null!));

            Assert.Equal("transaction", exception.ParamName);
        }

        [Fact]
        public void WriteGeneric_BulkInsertNewOverload_WithNullEntities_ThrowsArgumentNullException()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");
            IEnumerable<TestEntity>? entities = null;

            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
                repository.BulkInsert(entities!, null, null, null));

            Assert.Equal("entities", exception.ParamName);
        }

        [Fact]
        public void WriteGeneric_BulkInsertNewOverload_WithReturnIdentityFalse_ReturnsDefault()
        {
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=TestDb;");

            int result = repository.BulkInsert([], null, null, null);

            Assert.Equal(default, result);
        }

        private static IDbTransaction CreateTransactionWithoutConnection()
        {
            Mock<IDbTransaction> transaction = new();
            _ = transaction.SetupGet(t => t.Connection).Returns((IDbConnection?)null);
            return transaction.Object;
        }
    }
}
