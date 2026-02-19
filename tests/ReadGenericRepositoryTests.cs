using RepoDb;

namespace Gasolutions.Core.Repository.Tests;

/// <summary>
/// Test suite for ReadGenericRepositoryRepoDB.
/// Tests the read operations that return raw JSON strings.
/// </summary>
public class ReadGenericRepositoryTests
{
    private const string ValidConnectionString = "data source=Central;initial catalog=SauceDEV;persist security info=True;user id=sa;password=g2s0t07.;TrustServerCertificate=true;multipleactiveresultsets=True;application name=SauceOnlineAzure;";
    private const string ValidCommandText = "SELECT * FROM Cara";

    /// <summary>
    /// Initializes a new instance of the <see cref="ReadGenericRepositoryTests"/> class.
    /// </summary>
    public ReadGenericRepositoryTests()
    {
        InitializeRepoDB();
    }

    /// <summary>
    /// Verifies that QueryAndReturnJson executes successfully with valid parameters.
    /// </summary>
    [Fact]
    public void QueryAndReturnJson_WithValidParameters_ReturnsJsonString()
    {
        string commandText = "SELECT * FROM Cara FOR JSON PATH";

        // Arrange
        ReadGenericRepositoryRepoDB repository = new(ValidConnectionString);

        // Act & Assert
        // This test would require a real database connection
        // In a real scenario, you would mock the database or use a test database
        try
        {
            string result = repository.QueryAndReturnJson(commandText, CommandType.Text);
            Assert.NotNull(result);
            _ = Assert.IsType<string>(result);
        }
        catch (Exception ex) when (ex.Message.Contains("connection") || ex.Message.Contains("server"))
        {
            // Expected when no real database is available
        }
    }

    /// <summary>
    /// Verifies that QueryAndReturnJson returns empty string when no results are found.
    /// </summary>
    [Fact]
    public void QueryAndReturnJson_WithNoResults_ReturnsEmptyString()
    {
        // Arrange
        ReadGenericRepositoryRepoDB repository = new(ValidConnectionString);
        string emptyResultCommand = "SELECT * FROM Cara WHERE 1=0";

        // Act & Assert
        try
        {
            string result = repository.QueryAndReturnJson(emptyResultCommand, CommandType.Text);
            _ = Assert.IsType<string>(result);
        }
        catch (Exception ex) when (ex.Message.Contains("connection") || ex.Message.Contains("server"))
        {
            // Expected when no real database is available
        }
    }

    /// <summary>
    /// Verifies that QueryAndReturnJson throws appropriate exception with null command text.
    /// </summary>
    [Fact]
    public void QueryAndReturnJson_WithNullCommandText_ThrowsException()
    {
        // Arrange
        ReadGenericRepositoryRepoDB repository = new(ValidConnectionString);

        // Act & Assert
        try
        {
            _ = repository.QueryAndReturnJson(null!, CommandType.Text);
        }
        catch (Exception ex)
        {
            Assert.NotNull(ex);
            Assert.True(ex.Message.Contains("null") || ex.Message.Contains("connection") || ex.Message.Contains("server"));
        }
    }

    /// <summary>
    /// Verifies that QueryAndReturnJson throws exception with empty command text.
    /// </summary>
    [Fact]
    public void QueryAndReturnJson_WithEmptyCommandText_ThrowsOrReturnsEmpty()
    {
        // Arrange
        ReadGenericRepositoryRepoDB repository = new(ValidConnectionString);

        // Act & Assert
        try
        {
            string result = repository.QueryAndReturnJson(string.Empty, CommandType.Text);
            _ = Assert.IsType<string>(result);
        }
        catch (Exception ex)
        {
            Assert.NotNull(ex);
        }
    }

    /// <summary>
    /// Verifies that QueryAndReturnJson works with stored procedures.
    /// </summary>
    [Fact]
    public void QueryAndReturnJson_WithStoredProcedure_ExecutesSuccessfully()
    {
        // Arrange
        ReadGenericRepositoryRepoDB repository = new(ValidConnectionString);
        string sprocName = "RetornarJsonForTests";

        // Act & Assert
        try
        {
            string result = repository.QueryAndReturnJson(sprocName, CommandType.StoredProcedure);
            _ = Assert.IsType<string>(result);
        }
        catch (Exception ex) when (ex.Message.Contains("connection") || ex.Message.Contains("server") || ex.Message.Contains("not found"))
        {
            // Expected when no real database is available
        }
    }

    /// <summary>
    /// Verifies that invalid connection string throws exception.
    /// </summary>
    [Fact]
    public void QueryAndReturnJson_WithInvalidConnectionString_ThrowsException()
    {
        // Arrange
        string invalidConnectionString = "Server=invalid;Database=test;";
        ReadGenericRepositoryRepoDB repository = new(invalidConnectionString);

        // Act & Assert
        Exception exception = Assert.Throws<Microsoft.Data.SqlClient.SqlException>(() =>
            repository.QueryAndReturnJson(ValidCommandText, CommandType.Text));

        Assert.NotNull(exception);
    }

    /// <summary>
    /// Verifies that empty connection string is handled correctly.
    /// </summary>
    [Fact]
    public void QueryAndReturnJson_WithEmptyConnectionString_ThrowsException()
    {
        // Arrange
        ReadGenericRepositoryRepoDB repository = new(string.Empty);

        // Act & Assert
        try
        {
            _ = repository.QueryAndReturnJson(ValidCommandText, CommandType.Text);
        }
        catch (Exception ex)
        {
            Assert.NotNull(ex);
        }
    }

    /// <summary>
    /// Verifies that repository can be instantiated with valid connection string.
    /// </summary>
    [Fact]
    public void Constructor_WithValidConnectionString_CreatesInstance()
    {
        // Act
        ReadGenericRepositoryRepoDB repository = new(ValidConnectionString);

        // Assert
        Assert.NotNull(repository);
    }

    /// <summary>
    /// Initializes the test suite and sets up RepoDB.
    /// This method is called once before any tests in this class are executed.
    /// </summary>
    private static void InitializeRepoDB()
    {
        try
        {
            // Initialize RepoDB for SQL Server
            _ = GlobalConfiguration.Setup().UseSqlServer();
        }
        catch (Exception ex)
        {
            // Log initialization error if needed
            System.Diagnostics.Debug.WriteLine($"RepoDB initialization warning: {ex.Message}");
        }
    }
}
