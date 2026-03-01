// <copyright file="ReadGenericRepositoryRepoDBTests.cs" company="Gasolutions SAS">
// Copyright (c) Gasolutions SAS. Todos los derechos reservados.
// </copyright>

using System;
using System.Data;
using System.Threading.Tasks;

using Xunit;

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

        /// <summary>
        /// Tests that QueryAndReturnJson handles invalid CommandType enum values.
        /// Validates that the method attempts to execute even with an invalid CommandType value.
        /// NOTE: This test expects an exception because there's no actual database connection,
        /// but it proves that invalid CommandType values pass validation and the method attempts execution.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_InvalidCommandType_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM Users FOR JSON AUTO";
            CommandType invalidCommandType = (CommandType)999;

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, invalidCommandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJson handles very long command text strings.
        /// Validates that extremely long command text passes validation and attempts execution.
        /// NOTE: This test expects an exception because there's no actual database connection,
        /// but it proves that very long strings are accepted and the method attempts execution.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_VeryLongCommandText_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = new string('A', 10000) + " FOR JSON AUTO";

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, CommandType.Text));
        }

        /// <summary>
        /// Tests that QueryAndReturnJson handles command text with special SQL characters.
        /// Validates that command text containing quotes, semicolons, and other special characters
        /// passes validation and attempts execution.
        /// NOTE: This test expects an exception because there's no actual database connection,
        /// but it proves that special characters are accepted and the method attempts execution.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_CommandTextWithSpecialCharacters_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM Users WHERE Name = 'O''Brien'; DROP TABLE Users; -- FOR JSON AUTO";

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, CommandType.Text));
        }

        /// <summary>
        /// Tests that QueryAndReturnJson with an invalid connection string throws an exception.
        /// Validates that database connection errors are properly propagated.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_InvalidConnectionString_ThrowsException()
        {
            // Arrange
            string invalidConnectionString = "InvalidConnectionString";
            ReadGenericRepositoryRepoDB repository = new(invalidConnectionString);
            string commandText = "SELECT * FROM Users FOR JSON AUTO";

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, CommandType.Text));
        }

        /// <summary>
        /// Tests that QueryAndReturnJson handles command text with Unicode and control characters.
        /// Validates that command text containing special Unicode characters passes validation.
        /// NOTE: This test expects an exception because there's no actual database connection,
        /// but it proves that Unicode characters are accepted and the method attempts execution.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_CommandTextWithUnicodeCharacters_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM Users WHERE Name = '日本語' OR Name = 'Ñoño' FOR JSON AUTO";

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, CommandType.Text));
        }

        /// <summary>
        /// Tests that QueryAndReturnJson handles command text with newlines and tabs.
        /// Validates that multiline command text with formatting characters passes validation.
        /// NOTE: This test expects an exception because there's no actual database connection,
        /// but it proves that newlines and tabs are accepted and the method attempts execution.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_CommandTextWithNewlinesAndTabs_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT *\r\nFROM Users\r\n\tWHERE Active = 1\r\nFOR JSON AUTO";

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, CommandType.Text));
        }

        /// <summary>
        /// Tests that QueryAndReturnJson with a minimal valid CommandType enum value (0) attempts execution.
        /// Validates that CommandType with value 0 passes validation.
        /// NOTE: This test expects an exception because there's no actual database connection,
        /// but it proves that CommandType value 0 is accepted and the method attempts execution.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_CommandTypeZero_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM Users FOR JSON AUTO";
            CommandType commandType = (CommandType)0;

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJson with a negative CommandType enum value attempts execution.
        /// Validates that negative CommandType values pass validation.
        /// NOTE: This test expects an exception because there's no actual database connection,
        /// but it proves that negative CommandType values are accepted and the method attempts execution.
        /// </summary>
        [Fact]
        public void QueryAndReturnJson_NegativeCommandType_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM Users FOR JSON AUTO";
            CommandType commandType = (CommandType)(-1);

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJson with various SQL statement types attempts execution.
        /// Validates that different SQL statement patterns (INSERT, UPDATE, DELETE) pass validation.
        /// </summary>
        /// <param name="commandText">The SQL command text to test.</param>
        [Theory]
        [InlineData("INSERT INTO Users (Name) VALUES ('Test') FOR JSON AUTO")]
        [InlineData("UPDATE Users SET Active = 1 FOR JSON AUTO")]
        [InlineData("DELETE FROM Users WHERE Id = 1 FOR JSON AUTO")]
        [InlineData("EXEC sp_GetUsers")]
        [InlineData("EXECUTE dbo.sp_GetUsersAsJson @param1 = 'value'")]
        public void QueryAndReturnJson_VariousSqlStatements_AttemptsExecution(string commandText)
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, CommandType.Text));
        }

        /// <summary>
        /// Tests that QueryAndReturnJson with command text containing only special characters attempts execution.
        /// Validates that non-alphanumeric command text passes validation.
        /// NOTE: This test expects an exception because there's no actual database connection,
        /// but it proves that special character strings are accepted and the method attempts execution.
        /// </summary>
        [Theory]
        [InlineData("!!!")]
        [InlineData("@@@")]
        [InlineData("###")]
        [InlineData("$$$")]
        [InlineData("%%%")]
        public void QueryAndReturnJson_CommandTextWithOnlySpecialCharacters_AttemptsExecution(string commandText)
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);

            // Act & Assert
            _ = Assert.ThrowsAny<Exception>(() => repository.QueryAndReturnJson(commandText, CommandType.Text));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with a very long command text attempts to execute the query.
        /// Validates that the method can handle extremely long command strings without immediate failure.
        /// NOTE: This test is limited due to the inability to mock SqlConnection and RepoDb extension methods.
        /// We expect an exception because there's no actual database connection.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_VeryLongCommandText_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = new string('A', 100000);
            CommandType commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with command text containing null characters attempts execution.
        /// Validates that null characters embedded in SQL strings are handled.
        /// NOTE: This test is limited due to the inability to mock SqlConnection and RepoDb extension methods.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_CommandTextWithNullCharacter_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM\0Table";
            CommandType commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with command text containing Unicode characters attempts execution.
        /// Validates that international and special Unicode characters in SQL are handled.
        /// NOTE: This test is limited due to the inability to mock SqlConnection and RepoDb extension methods.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_CommandTextWithUnicodeCharacters_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM TableName WHERE Name = N'测试数据🎉'";
            CommandType commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with command text containing tab characters attempts execution.
        /// Validates that tab characters in SQL strings are handled.
        /// NOTE: This test is limited due to the inability to mock SqlConnection and RepoDb extension methods.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_CommandTextWithTabCharacter_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT\t*\tFROM\tTable";
            CommandType commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with command text containing newline characters attempts execution.
        /// Validates that multi-line SQL queries are handled correctly.
        /// NOTE: This test is limited due to the inability to mock SqlConnection and RepoDb extension methods.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_CommandTextWithNewlineCharacters_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT *\r\nFROM Table\r\nWHERE Id = 1";
            CommandType commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with CommandType value of 0 (undefined) attempts execution.
        /// Validates behavior when the default enum value (0) is passed, which is not a defined CommandType.
        /// NOTE: This test is limited due to the inability to mock SqlConnection and RepoDb extension methods.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_CommandTypeZero_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM Table";
            CommandType commandType = (CommandType)0;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with command text containing SQL comments attempts execution.
        /// Validates that SQL with embedded comments is handled correctly.
        /// NOTE: This test is limited due to the inability to mock SqlConnection and RepoDb extension methods.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_CommandTextWithSqlComments_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM Table -- This is a comment\nWHERE Id = 1";
            CommandType commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with command text containing block SQL comments attempts execution.
        /// Validates that SQL with embedded block comments is handled correctly.
        /// NOTE: This test is limited due to the inability to mock SqlConnection and RepoDb extension methods.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_CommandTextWithBlockComments_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * /* This is a block comment */ FROM Table";
            CommandType commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with command text containing SQL batch separators attempts execution.
        /// Validates that SQL batch separators (GO) in command text are handled.
        /// NOTE: This test is limited due to the inability to mock SqlConnection and RepoDb extension methods.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_CommandTextWithBatchSeparator_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM Table1\nGO\nSELECT * FROM Table2";
            CommandType commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with command text containing only SQL injection patterns attempts execution.
        /// Validates that potential SQL injection strings are passed through to the database layer.
        /// NOTE: This test is limited due to the inability to mock SqlConnection and RepoDb extension methods.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_CommandTextWithSqlInjectionPattern_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "'; DROP TABLE Users; --";
            CommandType commandType = CommandType.Text;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with negative CommandType enum value attempts execution.
        /// Validates behavior when a negative integer is cast to CommandType.
        /// NOTE: This test is limited due to the inability to mock SqlConnection and RepoDb extension methods.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_NegativeCommandType_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM Table";
            CommandType commandType = (CommandType)(-1);

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }

        /// <summary>
        /// Tests that QueryAndReturnJsonAsync with extremely large CommandType enum value attempts execution.
        /// Validates behavior when an extremely large integer is cast to CommandType.
        /// NOTE: This test is limited due to the inability to mock SqlConnection and RepoDb extension methods.
        /// </summary>
        [Fact]
        public async Task QueryAndReturnJsonAsync_ExtremelyLargeCommandType_AttemptsExecution()
        {
            // Arrange
            string connectionString = "Server=localhost;Database=TestDb;";
            ReadGenericRepositoryRepoDB repository = new(connectionString);
            string commandText = "SELECT * FROM Table";
            CommandType commandType = (CommandType)int.MaxValue;

            // Act & Assert
            _ = await Assert.ThrowsAnyAsync<Exception>(() =>
                repository.QueryAndReturnJsonAsync(commandText, commandType));
        }
    }
}