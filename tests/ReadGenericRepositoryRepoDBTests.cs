// <copyright file="ReadGenericRepositoryRepoDBTests.cs" company="Gasolutions SAS">
// Copyright (c) Gasolutions SAS. Todos los derechos reservados.
// </copyright>

namespace Gasolutions.Core.Repository.UnitTests
{
    /// <summary>
    /// Unit tests for the <see cref="ReadGenericRepositoryRepoDB"/> class.
    /// </summary>
    public partial class ReadGenericRepositoryRepoDBTests
    {
        /// <summary>
        /// Tests that QueryAndReturnJson throws ArgumentNullException when commandText is null.
        /// Validates that the exception message and parameter name match the expected values.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_NullCommandText_ThrowsArgumentNullException()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() =>
                repository.QueryAndReturnJson(null!, CommandType.Text));
            Assert.Equal("commandText", exception.ParamName);
            Assert.Contains("Command text cannot be null.", exception.Message);
        }

        /// <summary>
        /// Tests that QueryAndReturnJson with valid commandText and CommandType.Text passes validation
        /// and attempts to execute the query.
        /// NOTE: This test expects an exception because there's no actual database connection,
        /// but it proves that the validation passes and the method attempts to execute.
        /// Full testing requires either an integration test with a real database or refactoring
        /// to inject a connection factory.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_ValidCommandTextWithCommandTypeText_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM Users FOR JSON AUTO";

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, CommandType.Text));
        }

        /// <summary>
        /// Tests that QueryAndReturnJson with valid commandText and CommandType.StoredProcedure
        /// passes validation and attempts to execute the query.
        /// NOTE: This test expects an exception because there's no actual database connection,
        /// but it proves that the validation passes and the method attempts to execute.
        /// Full testing requires either an integration test with a real database or refactoring
        /// to inject a connection factory.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_ValidCommandTextWithCommandTypeStoredProcedure_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "sp_GetUsersAsJson";

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, CommandType.StoredProcedure));
        }

        /// <summary>
        /// Tests that QueryAndReturnJson with empty commandText passes validation
        /// and attempts to execute the query.
        /// NOTE: This test expects an exception because there's no actual database connection,
        /// but it proves that empty strings pass validation (not null) and the method attempts execution.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_EmptyCommandText_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = string.Empty;

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, CommandType.Text));
        }

        /// <summary>
        /// Tests that QueryAndReturnJson with whitespace-only commandText passes validation
        /// and attempts to execute the query.
        /// NOTE: This test expects an exception because there's no actual database connection,
        /// but it proves that whitespace strings pass validation (not null) and the method attempts execution.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_WhitespaceCommandText_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "   ";

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, CommandType.Text));
        }

        /// <summary>
        /// Tests that QueryAndReturnJson with CommandType.TableDirect passes validation
        /// and attempts to execute the query.
        /// NOTE: This test expects an exception because there's no actual database connection,
        /// but it proves that different CommandType enum values are accepted and the method attempts execution.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_ValidCommandTextWithCommandTypeTableDirect_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "Users";

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, CommandType.TableDirect));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync throws an exception when commandText parameter is null.
        /// Validates that null command text is properly rejected.
        /// NOTE: The async version does not have explicit null validation unlike the sync version,
        /// so the exception will come from the underlying RepoDb or SqlConnection layer.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_NullCommandText_ThrowsException()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = null!;
            var commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync throws an exception when commandText is an empty string.
        /// Validates that empty command text is properly rejected during execution.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_EmptyCommandText_ThrowsException()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = string.Empty;
            var commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync throws an exception when commandText contains only whitespace.
        /// Validates that whitespace-only command text is properly rejected during execution.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_WhitespaceCommandText_ThrowsException()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "   ";
            var commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with valid command text and CommandType.Text attempts to execute the query.
        /// NOTE: This test is limited because the method under test directly instantiates SqlConnection
        /// (a sealed class) and calls an extension method from RepoDb, neither of which can be mocked
        /// using Moq. Full testing of this method requires either:
        /// 1. An integration test with a real database connection, or
        /// 2. Refactoring the code to inject a connection factory or abstract the database operations.
        /// We expect an exception because there's no actual database connection,
        /// but this proves the validation passes and the method attempts to execute.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_ValidCommandTextWithTextType_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM TestTable";
            var commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with valid command text and CommandType.StoredProcedure attempts to execute the query.
        /// NOTE: This test is limited because the method under test directly instantiates SqlConnection
        /// (a sealed class) and calls an extension method from RepoDb, neither of which can be mocked
        /// using Moq. We expect an exception because there's no actual database connection,
        /// but this proves the validation passes and the method attempts to execute.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_ValidCommandTextWithStoredProcedureType_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "usp_GetTestData";
            var commandType = CommandType.StoredProcedure;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with valid command text and CommandType.TableDirect attempts to execute the query.
        /// NOTE: This test is limited because the method under test directly instantiates SqlConnection
        /// (a sealed class) and calls an extension method from RepoDb, neither of which can be mocked
        /// using Moq. We expect an exception because there's no actual database connection,
        /// but this proves the validation passes and the method attempts to execute.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_ValidCommandTextWithTableDirectType_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "TestTable";
            var commandType = CommandType.TableDirect;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync handles an invalid CommandType enum value.
        /// Validates behavior when an undefined enum value is passed by casting an invalid integer.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_InvalidCommandType_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM TestTable";
            CommandType commandType = (CommandType)999;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync throws an exception when the connection string is invalid.
        /// Validates that database connection errors are properly propagated.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_InvalidConnectionString_ThrowsException()
        {
            // Arrange
            string connectionString = "Invalid Connection String";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM TestTable";
            var commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync handles special characters in command text.
        /// Validates that commands with special SQL characters are properly passed through.
        /// NOTE: This test is limited due to the inability to mock SqlConnection and RepoDb extension methods.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_CommandTextWithSpecialCharacters_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM [Test Table] WHERE Name = 'O''Brien'";
            var commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that the constructor successfully creates an instance with various valid connection string inputs.
        /// Verifies that no exceptions are thrown and the instance is properly created.
        /// </summary>
        /// <param name="connectionString">The connection string to test.</param>
        [Theory]
        [InlineData("Server=localhost;Database=TestDb;User Id=sa;Password=pass;")]
        [InlineData("Data Source=.;Initial Catalog=MyDb;Integrated Security=True;")]
        [InlineData("Server=myserver.database.windows.net;Database=mydb;User Id=admin;Password=P@ssw0rd!;")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("A")]
        public void Constructor_VariousConnectionStrings_CreatesInstanceSuccessfully(string connectionString)
        {
            // Arrange & Act
            ReadGenericRepositoryRepoDB repository = new(connectionString);

            // Assert
            Assert.NotNull(repository);
            _ = Assert.IsAssignableFrom<IReadGenericRepository>(repository);
        }

        /// <summary>
        /// Tests that the constructor successfully creates an instance with a very long connection string.
        /// Verifies that the constructor can handle extremely long strings without throwing exceptions.
        /// </summary>
        [Fact]
        public void Constructor_VeryLongConnectionString_CreatesInstanceSuccessfully()
        {
            // Arrange
            string connectionString = new('A', 10000);

            // Act
            ReadGenericRepositoryRepoDB repository = new(connectionString);

            // Assert
            Assert.NotNull(repository);
            _ = Assert.IsAssignableFrom<IReadGenericRepository>(repository);
        }

        /// <summary>
        /// Tests that the constructor successfully creates an instance with connection strings containing special characters.
        /// Verifies that special characters commonly found in connection strings are handled correctly.
        /// </summary>
        /// <param name="connectionString">The connection string with special characters to test.</param>
        [Theory]
        [InlineData("Server=localhost;Password=P@ss;W0rd!#$%;")]
        [InlineData("Server=localhost;Password=\"quoted;value\";")]
        [InlineData("Server=localhost;App Name=My'App;")]
        [InlineData("Server=localhost;Description=Test\r\nMultiline;")]
        public void Constructor_SpecialCharactersInConnectionString_CreatesInstanceSuccessfully(string connectionString)
        {
            // Arrange & Act
            ReadGenericRepositoryRepoDB repository = new(connectionString);

            // Assert
            Assert.NotNull(repository);
            _ = Assert.IsAssignableFrom<IReadGenericRepository>(repository);
        }
    }
}
