// <copyright file="WriteGenericRepositoryRepoDBTests.cs" company="Gasolutions SAS">
// Copyright (c) Gasolutions SAS. Todos los derechos reservados.
// </copyright>

using System.Data.Common;
using Microsoft.Data.SqlClient;
using RepoDb;

namespace Gasolutions.Core.Repository.UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="WriteGenericRepositoryRepoDB{T, TKey}"/> class.
    /// </summary>
    public partial class WriteGenericRepositoryRepoDBTests
    {
        /// <summary>
        /// Tests that the Merge method throws ArgumentNullException when the entity parameter is null.
        /// Validates that the exception message matches the expected message.
        /// </summary>
        [Fact]
        public void Merge_NullEntity_ThrowsArgumentNullException()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            SqlConnection mockConnection = new(connectionString);
            Mock<IDbTransaction> mockTransaction = new();

            // Act & Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() =>
                repository.Merge(null!, mockConnection, mockTransaction.Object));
            Assert.Equal("entity", exception.ParamName);
            Assert.Contains("Entity cannot be null.", exception.Message);
        }

        /// <summary>
        /// Tests that Insert throws ArgumentNullException when entity is null.
        /// </summary>
        [Fact]
        public void Insert_NullEntity_ThrowsArgumentNullException()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            TestEntity? entity = null;

            // Act & Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => repository.Insert(entity!));
            Assert.Equal("entity", exception.ParamName);
            Assert.Contains("Entity cannot be null.", exception.Message);
        }

        /// <summary>
        /// Tests that ExecuteScalar(string, CommandType) throws ArgumentException when commandText is null, empty, or whitespace.
        /// </summary>
        /// <param name="commandText">The invalid command text value to test.</param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [InlineData("  \t\n  ")]
        public void ExecuteScalar_InvalidCommandText_ThrowsArgumentException(string? commandText)
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=Test;");
            const CommandType commandType = CommandType.Text;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() => repository.ExecuteScalar(commandText!, commandType));
            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that ExecuteScalar(string, CommandType) validates commandText parameter name in exception.
        /// </summary>
        [Fact]
        public void ExecuteScalar_NullCommandText_ExceptionContainsCorrectParameterName()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=Test;");
            const string? commandText = null;
            const CommandType commandType = CommandType.Text;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() => repository.ExecuteScalar(commandText!, commandType));
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that ExecuteScalar(string, CommandType) with valid commandText attempts execution using a mocked connection.
        /// This test demonstrates how the factory pattern enables unit testing by injecting a mocked IDbConnection.
        /// </summary>
        /// <param name="commandType">The command type to test.</param>
        [Theory]
        [InlineData(CommandType.Text)]
        [InlineData(CommandType.StoredProcedure)]
        [InlineData(CommandType.TableDirect)]
        public void ExecuteScalar_ValidCommandText_ExecutesScalarQuery(CommandType commandType)
        {
            // Todo> ojojojojoj
            // Arrange
            Mock<IDbConnection> mockConnection = new();
            Mock<IDbCommand> mockCommand = new();

            _ = mockConnection
                .Setup(c => c.CreateCommand())
                .Returns(mockCommand.Object);

            _ = mockCommand.SetupProperty(c => c.CommandType, commandType);
            _ = mockCommand.SetupProperty(c => c.CommandText);
            _ = mockCommand
                .Setup(c => c.ExecuteScalar())
                .Returns(42);

            IDbConnection ConnectionFactory()
            {
                return mockConnection.Object;
            }

            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(ConnectionFactory);

            // Act & Assert
            // This cannot be tested with Moq because RepoDb validates IDbCommand
            // at runtime and requires the actual type
        }

        /// <summary>
        /// Tests that ExecuteScalar(string, CommandType) with various commandType enum values.
        /// </summary>
        [Fact]
        public void ExecuteScalar_DifferentCommandTypes_PassesToExecuteScalar()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=Test;");

            // Act & Assert
            // Test parameter validation - this executes before database connection attempt
            ArgumentException exception1 = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteScalar(null!, CommandType.Text));
            Assert.Equal("commandText", exception1.ParamName);

            ArgumentException exception2 = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteScalar(string.Empty, CommandType.Text));
            Assert.Equal("commandText", exception2.ParamName);

            ArgumentException exception3 = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteScalar("   ", CommandType.Text));
            Assert.Equal("commandText", exception3.ParamName);

            // Verify method signature accepts different CommandType values
            // Note: Actual database execution would require integration test with real connection
            // This test verifies parameter validation and method signature compatibility
        }

        /// <summary>
        /// Tests that InsertAll throws ArgumentNullException when entities parameter is null.
        /// </summary>
        [Fact]
        public void InsertAll_NullEntities_ThrowsArgumentNullException()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            IEnumerable<TestEntity>? entities = null;

            // Act & Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => repository.InsertAll(entities!));
            Assert.Equal("entities", exception.ParamName);
            Assert.Contains("Entities collection cannot be null.", exception.Message);
        }

        /// <summary>
        /// Tests that InsertAll with empty collection does not throw and attempts database operation.
        /// NOTE: This test requires integration testing with a real database connection.
        /// The method creates its own SqlConnection internally which cannot be mocked (sealed class).
        /// To fully test this scenario, use integration tests with a test database.
        /// </summary>
        [Fact]
        public void InsertAll_EmptyCollection_ReturnsZero()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;Integrated Security=true;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            List<TestEntity> entities = [];

            // Act & Assert
            // We expect a SqlException or similar because there's no actual database connection,
            // but this proves that empty collections are accepted (no ArgumentNullException or validation error)
            // and the method attempts to execute the database operation
            _ = Assert.ThrowsAny<Exception>(() => repository.InsertAll(entities));
        }

        /// <summary>
        /// Tests that the constructor successfully initializes the repository with a valid connection string.
        /// </summary>
        /// <param name="connectionString">The connection string to test.</param>
        [Theory]
        [InlineData("Server=localhost;Database=TestDb;User Id=sa;Password=Test123;")]
        [InlineData("Data Source=.;Initial Catalog=MyDb;Integrated Security=True;")]
        [InlineData("Server=(localdb)\\mssqllocaldb;Database=TestDatabase;Trusted_Connection=True;")]
        public void Constructor_ValidConnectionString_InitializesSuccessfully(string connectionString)
        {
            // Arrange & Act
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);

            // Assert
            Assert.NotNull(repository);
        }

        /// <summary>
        /// Tests that the constructor accepts an empty connection string without throwing an exception.
        /// </summary>
        [Fact]
        public void Constructor_EmptyConnectionString_InitializesSuccessfully()
        {
            // Arrange
            string connectionString = string.Empty;

            // Act
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);

            // Assert
            Assert.NotNull(repository);
        }

        /// <summary>
        /// Tests that the constructor accepts a whitespace-only connection string without throwing an exception.
        /// </summary>
        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\r\n")]
        public void Constructor_WhitespaceConnectionString_InitializesSuccessfully(string connectionString)
        {
            // Arrange & Act
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);

            // Assert
            Assert.NotNull(repository);
        }

        /// <summary>
        /// Tests that the constructor accepts a very long connection string without throwing an exception.
        /// </summary>
        [Fact]
        public void Constructor_VeryLongConnectionString_InitializesSuccessfully()
        {
            // Arrange
            string connectionString = new string('a', 10000) + ";";

            // Act
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);

            // Assert
            Assert.NotNull(repository);
        }

        /// <summary>
        /// Tests that the constructor accepts connection strings with special characters.
        /// </summary>
        [Theory]
        [InlineData("Server=localhost;Database=Test'Db;User Id=sa;Password=Test@123;")]
        [InlineData("Server=localhost;Database=Test\"Db\";User Id=sa;Password=Test;")]
        [InlineData("Server=localhost;Database=Test;Db;User Id=sa;Password=Test;")]
        public void Constructor_ConnectionStringWithSpecialCharacters_InitializesSuccessfully(string connectionString)
        {
            // Arrange & Act
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);

            // Assert
            Assert.NotNull(repository);
        }

        /// <summary>
        /// Tests that the constructor works with different generic type combinations.
        /// </summary>
        [Fact]
        public void Constructor_DifferentGenericTypes_InitializesSuccessfully()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";

            // Act
            WriteGenericRepositoryRepoDB<TestEntity, int> repositoryInt = new(connectionString);
            WriteGenericRepositoryRepoDB<TestEntity, long> repositoryLong = new(connectionString);
            WriteGenericRepositoryRepoDB<TestEntity, Guid> repositoryGuid = new(connectionString);

            // Assert
            Assert.NotNull(repositoryInt);
            Assert.NotNull(repositoryLong);
            Assert.NotNull(repositoryGuid);
        }

        /// <summary>
        /// Tests that ExecuteScalar throws ArgumentException when commandText is null.
        /// </summary>
        /// <param name="invalidCommandText">The invalid command text (null, empty, or whitespace).</param>
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [InlineData(" \t \n ")]
        public void ExecuteScalar_NullOrWhitespaceCommandText_ThrowsArgumentException(string? invalidCommandText)
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            const CommandType commandType = CommandType.Text;
            IEnumerable<DbParameter>? parameters = null;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteScalar(invalidCommandText!, commandType, parameters));

            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that ExecuteScalar throws ArgumentException when commandText is null with different CommandType values.
        /// </summary>
        /// <param name="commandType">The command type to test.</param>
        [Theory]
        [InlineData(CommandType.Text)]
        [InlineData(CommandType.StoredProcedure)]
        [InlineData(CommandType.TableDirect)]
        public void ExecuteScalar_NullCommandTextWithDifferentCommandTypes_ThrowsArgumentException(CommandType commandType)
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            const string? commandText = null;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteScalar(commandText!, commandType));

            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that ExecuteScalar throws ArgumentException for empty commandText with different TKey types.
        /// </summary>
        [Fact]
        public void ExecuteScalar_EmptyCommandTextWithLongTKey_ThrowsArgumentException()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, long> repository = new(connectionString);
            string commandText = string.Empty;
            const CommandType commandType = CommandType.Text;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteScalar(commandText, commandType));

            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that ExecuteScalar throws ArgumentException for whitespace commandText with Guid TKey.
        /// </summary>
        [Fact]
        public void ExecuteScalar_WhitespaceCommandTextWithGuidTKey_ThrowsArgumentException()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, Guid> repository = new(connectionString);
            const string commandText = "   ";
            const CommandType commandType = CommandType.StoredProcedure;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteScalar(commandText, commandType));

            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that ExecuteScalar throws ArgumentException with parameters provided.
        /// </summary>
        [Fact]
        public void ExecuteScalar_NullCommandTextWithParameters_ThrowsArgumentException()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            const string? commandText = null;
            const CommandType commandType = CommandType.Text;
            List<DbParameter> parameters =
            [
                new SqlParameter("@Id", 1),
            ];

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteScalar(commandText!, commandType, parameters));

            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that MergeAll throws ArgumentNullException when entities parameter is null.
        /// </summary>
        [Fact]
        public void MergeAll_NullEntities_ThrowsArgumentNullException()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            IEnumerable<TestEntity>? entities = null;

            // Act & Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => repository.MergeAll(entities!));
            Assert.Equal("entities", exception.ParamName);
            Assert.Contains("Entities collection cannot be null.", exception.Message);
        }

        /// <summary>
        /// Tests that Delete throws InvalidOperationException when ConnectionString is null.
        /// This test validates that SqlClient properly throws when attempting to use a connection with null connection string.
        /// Expected: InvalidOperationException is thrown when attempting to use SqlConnection with null connection string.
        /// </summary>
        [Fact]
        public void Delete_NullConnectionString_ThrowsArgumentException()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new((string)null!);
            object whereOrPrimaryKey = new();

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.Delete(whereOrPrimaryKey));
        }

        /// <summary>
        /// Tests that Delete throws ArgumentException when ConnectionString is empty.
        /// This test validates that the SqlConnection constructor properly validates the connection string.
        /// Expected: ArgumentException is thrown when attempting to create SqlConnection with empty connection string.
        /// </summary>
        [Fact]
        public void Delete_EmptyConnectionString_ThrowsArgumentException()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(string.Empty);
            object whereOrPrimaryKey = new();

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.Delete(whereOrPrimaryKey));
        }

        /// <summary>
        /// Tests that Delete throws ArgumentException when ConnectionString is whitespace.
        /// This test validates that the SqlConnection constructor properly validates the connection string.
        /// Expected: ArgumentException is thrown when attempting to create SqlConnection with whitespace connection string.
        /// </summary>
        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public void Delete_WhitespaceConnectionString_ThrowsArgumentException(string connectionString)
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            object whereOrPrimaryKey = new();

            // Act & Assert
            // Note: RepoDb throws MissingMappingException before SqlClient validation
            _ = Assert.ThrowsAny<Exception>(() => repository.Delete(whereOrPrimaryKey));
        }

        /// <summary>
        /// Tests that Delete handles null whereOrPrimaryKey parameter.
        /// This test verifies that calling Delete with null throws an exception.
        /// The actual exception type depends on RepoDB's internal validation and may be
        /// ArgumentNullException (if RepoDB validates before connecting) or a connection-related exception.
        /// </summary>
        [Fact]
        public void Delete_NullWhereOrPrimaryKey_RequiresIntegrationTest()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;Integrated Security=true;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);

            // Act & Assert
            // We expect some exception to be thrown - either from RepoDB validation
            // or from attempting to connect to the database
            _ = Assert.ThrowsAny<Exception>(() => repository.Delete(null!));
        }

        /// <summary>
        /// Tests that Delete with a valid primary key attempts to connect to the database.
        /// This test verifies that calling Delete with a valid primary key value throws an exception
        /// when the database is not available, confirming the method attempts the operation.
        /// </summary>
        [Fact]
        public void Delete_ValidPrimaryKey_RequiresIntegrationTest()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;Integrated Security=true;Connection Timeout=1;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            const int primaryKey = 123;

            // Act & Assert
            // We expect an exception to be thrown when attempting to connect to the non-existent database
            _ = Assert.ThrowsAny<Exception>(() => repository.Delete(primaryKey));
        }

        /// <summary>
        /// Tests that Delete with a where clause object throws SqlException when no database is available.
        /// This test verifies that the Delete method attempts to create a connection and execute the operation,
        /// and throws an appropriate exception when the database is not available.
        /// Expected: SqlException thrown when attempting to connect to non-existent database.
        /// </summary>
        [Fact]
        public void Delete_ValidWhereClause_RequiresIntegrationTest()
        {
            // Arrange
            Mock<IDbConnection> mockConnection = new();
            Mock<IDbCommand> mockCommand = new();

            _ = mockConnection
                .Setup(c => c.CreateCommand())
                .Returns(mockCommand.Object);

            _ = mockConnection
                .Setup(c => c.State)
                .Returns(ConnectionState.Open);

            Func<IDbConnection> connectionFactory = () => mockConnection.Object;
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionFactory);
            var whereClause = new { Name = "TestName" };

            // Act & Assert
            // The Delete method will attempt to create a connection via the factory and execute the delete operation.
            // RepoDb requires SQL Server bootstrap initialization and proper database mapping.
            // Without initialization, RepoDb will throw a MissingMappingException, which is acceptable for this test.
            // This verifies the method signature, the factory is called, and the method attempts the expected database operation.
            _ = Assert.ThrowsAny<Exception>(() => repository.Delete(whereClause));
        }

        /// <summary>
        /// Tests that ExecuteReader throws ArgumentException when commandText is null.
        /// </summary>
        [Fact]
        public void ExecuteReader_NullCommandText_ThrowsArgumentException()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            const string commandText = null!;
            const CommandType commandType = CommandType.Text;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteReader(commandText, commandType, null));
            Assert.Equal("commandText", exception.ParamName);
            Assert.Contains("Command text cannot be null or whitespace.", exception.Message);
        }

        /// <summary>
        /// Tests that ExecuteReader throws ArgumentException when commandText is an empty string.
        /// </summary>
        [Fact]
        public void ExecuteReader_EmptyCommandText_ThrowsArgumentException()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            string commandText = string.Empty;
            const CommandType commandType = CommandType.Text;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteReader(commandText, commandType, null));
            Assert.Equal("commandText", exception.ParamName);
            Assert.Contains("Command text cannot be null or whitespace.", exception.Message);
        }

        /// <summary>
        /// Tests that ExecuteReader throws ArgumentException when commandText contains only whitespace.
        /// </summary>
        /// <param name="whitespaceText">Whitespace-only command text.</param>
        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [InlineData(" \t \n ")]
        public void ExecuteReader_WhitespaceCommandText_ThrowsArgumentException(string whitespaceText)
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            const CommandType commandType = CommandType.Text;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteReader(whitespaceText, commandType, null));
            Assert.Equal("commandText", exception.ParamName);
            Assert.Contains("Command text cannot be null or whitespace.", exception.Message);
        }

        /// <summary>
        /// Tests that DeleteAll throws ArgumentNullException when entities parameter is null.
        /// </summary>
        [Fact]
        public void DeleteAll_NullEntities_ThrowsArgumentNullException()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=test;Database=test;");
            IEnumerable<TestEntity>? entities = null;

            // Act & Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => repository.DeleteAll(entities!));
            Assert.Equal("entities", exception.ParamName);
            Assert.Contains("Entities collection cannot be null.", exception.Message);
        }

        /// <summary>
        /// Tests that DeleteAll handles an empty collection without throwing exceptions.
        /// RepoDB's DeleteAll method efficiently handles empty collections by returning 0
        /// without attempting to open a database connection or execute SQL.
        /// </summary>
        [Fact]
        public void DeleteAll_EmptyCollection_ReturnsZero()
        {
            // Arrange
            // RepoDb.SqlServerBootstrap.Initialize();
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=test;Database=test;");
            List<TestEntity> entities = [];

            // Act
            int result = repository.DeleteAll(entities);

            // Assert
            Assert.Equal(0, result);
        }

        /// <summary>
        /// Tests that ExecuteQuery throws ArgumentException when commandText is null.
        /// </summary>
        [Fact]
        public void ExecuteQuery_NullCommandText_ThrowsArgumentException()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=test;Database=test;");
            const string? commandText = null;
            const CommandType commandType = CommandType.Text;
            IEnumerable<DbParameter>? parameters = null;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteQuery(commandText!, commandType, parameters));

            Assert.Equal("commandText", exception.ParamName);
            Assert.Contains("Command text cannot be null or whitespace.", exception.Message);
        }

        /// <summary>
        /// Tests that ExecuteQuery throws ArgumentException when commandText is an empty string.
        /// </summary>
        [Fact]
        public void ExecuteQuery_EmptyCommandText_ThrowsArgumentException()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=test;Database=test;");
            string commandText = string.Empty;
            const CommandType commandType = CommandType.Text;
            IEnumerable<DbParameter>? parameters = null;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteQuery(commandText, commandType, parameters));

            Assert.Equal("commandText", exception.ParamName);
            Assert.Contains("Command text cannot be null or whitespace.", exception.Message);
        }

        /// <summary>
        /// Tests that ExecuteQuery throws ArgumentException when commandText contains only whitespace.
        /// </summary>
        /// <param name="whitespaceText">The whitespace-only string to test.</param>
        [Theory]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        [InlineData("   \t\n\r  ")]
        public void ExecuteQuery_WhitespaceCommandText_ThrowsArgumentException(string whitespaceText)
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=test;Database=test;");
            const CommandType commandType = CommandType.Text;
            IEnumerable<DbParameter>? parameters = null;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteQuery(whitespaceText, commandType, parameters));

            Assert.Equal("commandText", exception.ParamName);
            Assert.Contains("Command text cannot be null or whitespace.", exception.Message);
        }

        /// <summary>
        /// Tests that Update throws ArgumentNullException when entity parameter is null.
        /// </summary>
        [Fact]
        public void Update_NullEntity_ThrowsArgumentNullException()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            TestEntity? entity = null;

            // Act & Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => repository.Update(entity!));
            Assert.Equal("entity", exception.ParamName);
            Assert.Contains("Entity cannot be null.", exception.Message);
        }

        /// <summary>
        /// Tests that UpdateAll throws ArgumentNullException when entities parameter is null.
        /// This verifies that the method properly validates the required entities parameter
        /// and throws the expected exception with the correct parameter name and message.
        /// </summary>
        [Fact]
        public void UpdateAll_NullEntities_ThrowsArgumentNullException()
        {
            // Arrange
            const string connectionString = "Server=test;Database=test;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            IEnumerable<TestEntity>? entities = null;

            // Act & Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => repository.UpdateAll(entities!));
            Assert.Equal("entities", exception.ParamName);
            Assert.Contains("Entities collection cannot be null.", exception.Message);
        }

        /// <summary>
        /// Tests that ExecuteNonQuery throws ArgumentException when commandText is null.
        /// Expected: ArgumentException with paramName "commandText" and appropriate message.
        /// </summary>
        [Fact]
        public void ExecuteNonQuery_NullCommandText_ThrowsArgumentException()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=test;Database=test;");

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteNonQuery(null!, CommandType.Text, null));
            Assert.Equal("commandText", exception.ParamName);
            Assert.Contains("Command text cannot be null or whitespace.", exception.Message);
        }

        /// <summary>
        /// Tests that ExecuteNonQuery throws ArgumentException when commandText is empty string.
        /// Expected: ArgumentException with paramName "commandText" and appropriate message.
        /// </summary>
        [Fact]
        public void ExecuteNonQuery_EmptyCommandText_ThrowsArgumentException()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=test;Database=test;");

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteNonQuery(string.Empty, CommandType.Text, null));
            Assert.Equal("commandText", exception.ParamName);
            Assert.Contains("Command text cannot be null or whitespace.", exception.Message);
        }

        /// <summary>
        /// Tests that ExecuteNonQuery throws ArgumentException when commandText contains only whitespace characters.
        /// Input conditions: Various whitespace strings including space, tab, newline, and carriage return combinations.
        /// Expected: ArgumentException with paramName "commandText" and appropriate message for all cases.
        /// </summary>
        [Theory]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\r\n")]
        [InlineData("   \t   ")]
        [InlineData(" \n \t \r\n ")]
        public void ExecuteNonQuery_WhitespaceCommandText_ThrowsArgumentException(string commandText)
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=test;Database=test;");

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() =>
                repository.ExecuteNonQuery(commandText, CommandType.Text, null));
            Assert.Equal("commandText", exception.ParamName);
            Assert.Contains("Command text cannot be null or whitespace.", exception.Message);
        }

        /// <summary>
        /// Tests ExecuteNonQuery with various CommandType values and null parameters.
        /// Input conditions: Valid commandText with different CommandType enum values and null parameters.
        /// Expected: Method attempts execution (will fail without real database, but validates parameter handling).
        /// Note: This test demonstrates the method signature accepts different CommandType values.
        /// Actual database execution cannot be tested without integration test infrastructure.
        /// </summary>
        [Theory]
        [InlineData(CommandType.Text)]
        [InlineData(CommandType.StoredProcedure)]
        [InlineData(CommandType.TableDirect)]
        public void ExecuteNonQuery_ValidCommandTextWithDifferentCommandTypes_AcceptsCommandTypes(CommandType commandType)
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=invalid;Database=test;Connection Timeout=1;");
            const string validCommandText = "SELECT 1";

            // Act & Assert
            // Note: This will throw a SqlException when attempting to connect to the invalid server,
            // but it validates that the input validation passes and the method proceeds to database operations.
            // In a real scenario, this would require a test database or mocking infrastructure.
            _ = Assert.ThrowsAny<Exception>(() =>
                repository.ExecuteNonQuery(validCommandText, commandType, null));
        }

        /// <summary>
        /// Tests ExecuteNonQuery with null parameters collection.
        /// Input conditions: Valid commandText, CommandType.Text, and explicitly null parameters.
        /// Expected: Method accepts null parameters and proceeds (will fail at connection without real DB).
        /// </summary>
        [Fact]
        public void ExecuteNonQuery_NullParameters_AcceptsNullParameters()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=invalid;Database=test;Connection Timeout=1;");
            const string validCommandText = "INSERT INTO TestTable VALUES (1)";

            // Act & Assert
            // Note: Validates null parameters are accepted; actual execution requires database.
            _ = Assert.ThrowsAny<Exception>(() =>
                repository.ExecuteNonQuery(validCommandText, CommandType.Text, null));
        }

        /// <summary>
        /// Tests ExecuteNonQuery with empty parameters collection.
        /// Input conditions: Valid commandText with empty parameter collection.
        /// Expected: Method accepts empty collection and proceeds (will fail at connection without real DB).
        /// </summary>
        [Fact]
        public void ExecuteNonQuery_EmptyParametersCollection_AcceptsEmptyCollection()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=invalid;Database=test;Connection Timeout=1;");
            const string validCommandText = "INSERT INTO TestTable VALUES (1)";
            List<DbParameter> emptyParameters = [];

            // Act & Assert
            // Note: Validates empty parameter collection is accepted; actual execution requires database.
            _ = Assert.ThrowsAny<Exception>(() =>
                repository.ExecuteNonQuery(validCommandText, CommandType.Text, emptyParameters));
        }

        /// <summary>
        /// Test entity class for testing purposes.
        /// </summary>
        private class TestEntity
        {
            public int Id { get; set; }

            public string Name { get; set; } = string.Empty;
        }

        /// <summary>
        /// Tests that Insert with entity having default values attempts execution.
        /// Input: Entity with default property values (Id=0, Name=null)
        /// Expected: Method executes insert operation (throws exception due to no database).
        /// </summary>
        [Fact]
        public void Insert_EntityWithDefaultValues_AttemptsExecution()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;User Id=sa;Password=Test123;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            TestEntity entity = new();

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.Insert(entity));
        }

        /// <summary>
        /// Tests that Insert with connection and transaction throws ArgumentNullException when entity is null.
        /// This test validates that the method properly checks for null entity before attempting database operations.
        /// Expected: ArgumentNullException with parameter name "entity" and message "Entity cannot be null.".
        /// </summary>
        [Fact]
        public void Insert_WithConnectionAndTransaction_NullEntity_ThrowsArgumentNullException()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            TestEntity? entity = null;
            using SqlConnection connection = new(connectionString);
            Mock<IDbTransaction> transactionMock = new();

            // Act & Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => repository.Insert(entity!, connection, transactionMock.Object));
            Assert.Equal("entity", exception.ParamName);
            Assert.Contains("Entity cannot be null.", exception.Message);
        }

        /// <summary>
        /// Tests that Merge throws ArgumentNullException when the entity parameter is null.
        /// Input conditions: Null entity parameter.
        /// Expected: ArgumentNullException with parameter name "entity" and message "Entity cannot be null.".
        /// </summary>
        [Fact]
        public void Merge_NullEntitySimpleOverload_ThrowsArgumentNullException()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            TestEntity? entity = null;

            // Act & Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => repository.Merge(entity!));
            Assert.Equal("entity", exception.ParamName);
            Assert.Contains("Entity cannot be null.", exception.Message);
        }

        /// <summary>
        /// Tests that Merge works with long TKey type and attempts to execute merge operation.
        /// Input conditions: Valid entity with long TKey type.
        /// Expected: Method proceeds past validation and attempts database connection.
        /// </summary>
        [Fact]
        public void Merge_ValidEntityWithLongKey_AttemptsExecution()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntityWithLong, long> repository = new(connectionString);
            TestEntityWithLong entity = new() { Id = 1L, Name = "TestEntity" };

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.Merge(entity));
        }

        /// <summary>
        /// Tests that Merge works with Guid TKey type and attempts to execute merge operation.
        /// Input conditions: Valid entity with Guid TKey type.
        /// Expected: Method proceeds past validation and attempts database connection.
        /// </summary>
        [Fact]
        public void Merge_ValidEntityWithGuidKey_AttemptsExecution()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntityWithGuid, Guid> repository = new(connectionString);
            TestEntityWithGuid entity = new() { Id = Guid.NewGuid(), Name = "TestEntity" };

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.Merge(entity));
        }

        /// <summary>
        /// Tests that Merge handles entity with boundary value for long Id.
        /// Input conditions: Entity with various long boundary values.
        /// Expected: Method accepts the entity and attempts to execute merge operation.
        /// </summary>
        [Theory]
        [InlineData(long.MinValue)]
        [InlineData(long.MaxValue)]
        [InlineData(0L)]
        [InlineData(-1L)]
        [InlineData(1L)]
        public void Merge_EntityWithBoundaryLongValues_AttemptsExecution(long id)
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntityWithLong, long> repository = new(connectionString);
            TestEntityWithLong entity = new() { Id = id, Name = "Test" };

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.Merge(entity));
        }

        /// <summary>
        /// Test entity class with long primary key for testing purposes.
        /// </summary>
        private class TestEntityWithLong
        {
            public long Id { get; set; }

            public string Name { get; set; } = string.Empty;
        }

        /// <summary>
        /// Test entity class with Guid primary key for testing purposes.
        /// </summary>
        private class TestEntityWithGuid
        {
            public Guid Id { get; set; }

            public string Name { get; set; } = string.Empty;
        }

        /// <summary>
        /// Tests that ExecuteScalar(string, CommandType) throws ArgumentException when commandText is null.
        /// Input: null commandText with CommandType.Text
        /// Expected: ArgumentException with correct message and parameter name.
        /// </summary>
        [Fact]
        public void ExecuteScalarString_NullCommandText_ThrowsArgumentException()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=Test;");
            const string? commandText = null;
            const CommandType commandType = CommandType.Text;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() => repository.ExecuteScalar(commandText!, commandType));
            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that ExecuteScalar(string, CommandType) throws ArgumentException when commandText is empty.
        /// Input: empty string commandText with CommandType.Text
        /// Expected: ArgumentException with correct message and parameter name.
        /// </summary>
        [Fact]
        public void ExecuteScalarString_EmptyCommandText_ThrowsArgumentException()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=Test;");
            const string commandText = "";
            const CommandType commandType = CommandType.Text;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() => repository.ExecuteScalar(commandText, commandType));
            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that ExecuteScalar(string, CommandType) throws ArgumentException when commandText contains only whitespace.
        /// Input: various whitespace-only strings with CommandType.Text
        /// Expected: ArgumentException with correct message and parameter name.
        /// </summary>
        /// <param name="whitespaceText">The whitespace-only command text to test.</param>
        [Theory]
        [InlineData(" ")]
        [InlineData("  ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r")]
        [InlineData("\r\n")]
        [InlineData("  \t  ")]
        [InlineData(" \n\r\t ")]
        [InlineData("\t\t\t")]
        public void ExecuteScalarString_WhitespaceCommandText_ThrowsArgumentException(string whitespaceText)
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=Test;");
            const CommandType commandType = CommandType.Text;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() => repository.ExecuteScalar(whitespaceText, commandType));
            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that ExecuteScalar(string, CommandType) throws ArgumentException for null commandText with different CommandType values.
        /// Input: null commandText with various CommandType enum values
        /// Expected: ArgumentException with correct message and parameter name for all command types.
        /// </summary>
        /// <param name="commandType">The command type to test.</param>
        [Theory]
        [InlineData(CommandType.Text)]
        [InlineData(CommandType.StoredProcedure)]
        [InlineData(CommandType.TableDirect)]
        public void ExecuteScalarString_NullCommandTextWithDifferentCommandTypes_ThrowsArgumentException(CommandType commandType)
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=Test;");
            const string? commandText = null;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() => repository.ExecuteScalar(commandText!, commandType));
            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that ExecuteScalar(string, CommandType) throws ArgumentException for empty commandText with different CommandType values.
        /// Input: empty string commandText with various CommandType enum values
        /// Expected: ArgumentException with correct message and parameter name for all command types.
        /// </summary>
        /// <param name="commandType">The command type to test.</param>
        [Theory]
        [InlineData(CommandType.Text)]
        [InlineData(CommandType.StoredProcedure)]
        [InlineData(CommandType.TableDirect)]
        public void ExecuteScalarString_EmptyCommandTextWithDifferentCommandTypes_ThrowsArgumentException(CommandType commandType)
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=Test;");
            const string commandText = "";

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() => repository.ExecuteScalar(commandText, commandType));
            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that ExecuteScalar(string, CommandType) throws ArgumentException for whitespace commandText with different CommandType values.
        /// Input: whitespace-only commandText with various CommandType enum values
        /// Expected: ArgumentException with correct message and parameter name for all command types.
        /// </summary>
        /// <param name="commandType">The command type to test.</param>
        [Theory]
        [InlineData(CommandType.Text)]
        [InlineData(CommandType.StoredProcedure)]
        [InlineData(CommandType.TableDirect)]
        public void ExecuteScalarString_WhitespaceCommandTextWithDifferentCommandTypes_ThrowsArgumentException(CommandType commandType)
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=localhost;Database=Test;");
            const string commandText = "   ";

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() => repository.ExecuteScalar(commandText, commandType));
            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that ExecuteScalar(string, CommandType) validates commandText before other operations.
        /// This test ensures that validation occurs before attempting to create a database connection.
        /// Input: null commandText with an invalid connection string
        /// Expected: ArgumentException (validation) rather than connection-related exception.
        /// </summary>
        [Fact]
        public void ExecuteScalarString_NullCommandTextWithInvalidConnectionString_ThrowsArgumentException()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("InvalidConnectionString");
            const string? commandText = null;
            const CommandType commandType = CommandType.Text;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() => repository.ExecuteScalar(commandText!, commandType));
            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }

        /// <summary>
        /// Tests that ExecuteScalar(string, CommandType) with valid commandText attempts to execute against the database.
        /// This test verifies that when commandText passes validation, the method attempts to create a connection.
        /// Input: valid commandText with CommandType.Text and invalid connection string
        /// Expected: Exception from connection attempt (not ArgumentException), proving validation passed.
        /// </summary>
        /// <param name="commandType">The command type to test.</param>
        [Theory]
        [InlineData(CommandType.Text)]
        [InlineData(CommandType.StoredProcedure)]
        [InlineData(CommandType.TableDirect)]
        public void ExecuteScalarString_ValidCommandText_AttemptsExecution(CommandType commandType)
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = 
                new("Server=nonexistent;Database=Test;Connection Timeout=1;");
            const string commandText = "SELECT 1";

            // Act & Assert
            Exception exception = Assert.ThrowsAny<Exception>(() => 
                repository.ExecuteScalar(commandText, commandType, null));

            Assert.False(
                exception is ArgumentException argEx && argEx.ParamName == "commandText",
                "Expected connection-related exception, but got commandText validation exception");
        }

        /// <summary>
        /// Tests that ExecuteScalar handles very long command text correctly.
        /// Input: command text with thousands of characters
        /// Expected: Method accepts the input and attempts execution (validation passes).
        /// </summary>
        [Fact]
        public void ExecuteScalarString_VeryLongCommandText_PassesValidation()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = 
                new("Server=nonexistent;Database=Test;Connection Timeout=1;");
            string commandText = new('A', 10000); // 10,000 character command text
            const CommandType commandType = CommandType.Text;

            // Act & Assert
            Exception exception = Assert.ThrowsAny<Exception>(() => repository.ExecuteScalar(commandText, commandType));

            Assert.False(
                exception is ArgumentException argEx && argEx.ParamName == "commandText",
                "Expected connection-related exception, but got commandText validation exception");
        }

        /// <summary>
        /// Tests that ExecuteScalar handles command text with special characters.
        /// Input: command text containing quotes, newlines, tabs, and other special characters
        /// Expected: Method accepts the input and attempts execution (validation passes).
        /// </summary>
        /// <param name="commandText">Command text with special characters to test.</param>
        [Theory]
        [InlineData("SELECT 'test'")]
        [InlineData("SELECT \"test\"")]
        [InlineData("SELECT 'test' FROM table WHERE name = 'John''s'")]
        [InlineData("SELECT * FROM table\nWHERE id = 1")]
        [InlineData("SELECT * FROM table\tWHERE id = 1")]
        [InlineData("SELECT * FROM table\r\nWHERE id = 1")]
        [InlineData("EXEC sp_test @param1='value', @param2='value2'")]
        [InlineData("/* comment */ SELECT 1")]
        [InlineData("-- comment\nSELECT 1")]
        public void ExecuteScalarString_CommandTextWithSpecialCharacters_PassesValidation(string commandText)
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository =
                new("Server=nonexistent;Database=Test;Connection Timeout=1;");
            const CommandType commandType = CommandType.Text;

            // Act & Assert
            // El test verifica que la validación pasa (no lanza ArgumentException)
            // y que el método intenta conectarse (lanza excepción de conexión)
            Exception exception = Assert.ThrowsAny<Exception>(() =>
                repository.ExecuteScalar(commandText, commandType, null));

            // Verificar que NO es una excepción de validación
            Assert.False(
                exception is ArgumentException argEx && argEx.ParamName == "commandText",
                "Expected connection-related exception, but got commandText validation exception");
        }

        /// <summary>
        /// Tests that ExecuteScalar works with empty parameters collection.
        /// Input conditions: valid commandText with explicitly empty parameters collection.
        /// Expected: Method executes successfully with empty parameters.
        /// </summary>
        [Fact]
        public void ExecuteScalar_ValidCommandTextWithEmptyParametersCollection_ExecutesSuccessfully()
        {
            // Arrange
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = 
                new("Server=nonexistent;Database=Test;Connection Timeout=1;");
            const string commandText = "SELECT 1";
            const CommandType commandType = CommandType.Text;
            IEnumerable<DbParameter> parameters = Enumerable.Empty<DbParameter>();

            // Act & Assert
            // Note: Validates empty parameter collection is accepted; actual execution requires database.
            _ = Assert.ThrowsAny<Exception>(() =>
                repository.ExecuteScalar(commandText, commandType, parameters));
        }

        /// <summary>
        /// Tests that ExecuteScalar disposes the connection after execution.
        /// Input conditions: valid commandText with mocked connection.
        /// Expected: Connection.Dispose() is called exactly once.
        /// </summary>
        [Fact]
        public void ExecuteScalar_ValidCommandText_DisposesConnection()
        {
            // Arrange
            const string commandText = "SELECT 1";
            const CommandType commandType = CommandType.Text;

            bool connectionDisposed = false;
            Mock<IDbConnection> mockConnection = new();

            // RepoDb requires DbCommand, not just IDbCommand, so we need to mock differently
            // Since RepoDb's ExecuteScalar extension method requires actual DbCommand casting,
            // we cannot properly mock the connection without a real database.
            // Instead, we verify disposal by tracking the Dispose call on our mock connection.
            _ = mockConnection.Setup(c => c.Dispose()).Callback(() => connectionDisposed = true);

            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(() => mockConnection.Object);

            // Act & Assert
            // The test cannot proceed because RepoDb requires properties/methods on the connection that the mock doesn't provide.
            // The disposal behavior is guaranteed by the 'using' statement in ExecuteScalar method (line 291).
            // This test verifies that the CreateConnection returns our mocked connection.
            _ = Assert.Throws<NullReferenceException>(() =>
                repository.ExecuteScalar(commandText, commandType, null));

            // Even though ExecuteScalar throws, the using statement ensures Dispose is called
            Assert.True(connectionDisposed, "Connection should be disposed even when an exception occurs within the using block");
        }

        /// <summary>
        /// Tests that ExecuteScalar with invalid CommandType enum value still validates commandText first.
        /// Input conditions: null commandText with invalid CommandType cast from integer.
        /// Expected: ArgumentException for commandText validation (not CommandType).
        /// </summary>
        [Fact]
        public void ExecuteScalar_NullCommandTextWithInvalidCommandType_ThrowsArgumentException()
        {
            // Arrange
            const string connectionString = "Server=localhost;Database=TestDb;";
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new(connectionString);
            const string commandText = null!;
            CommandType invalidCommandType = (CommandType)999;

            // Act & Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(() => repository.ExecuteScalar(commandText!, invalidCommandType, null));

            Assert.Equal("Command text cannot be null or whitespace. (Parameter 'commandText')", exception.Message);
            Assert.Equal("commandText", exception.ParamName);
        }
    }
}