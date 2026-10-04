// Copyright (c) Gasolutions SAS. Todos los derechos reservados.

namespace Gasolutions.Core.Repository.UnitTests
{
    public partial class ReadGenericRepositoryRepoDBTests
    {
        [Fact]
        public void QueryAndReturnJson_ConParametroYTransaccion_AdjuntaParametroYRetornaJson()
        {
            Mock<IDbConnection> connection = new();
            Mock<IDbTransaction> transaction = new();
            Mock<IDbCommand> command = new();
            Mock<IDataReader> reader = new();
            Mock<IDataParameterCollection> parameterCollection = new();
            List<object> addedParameters = [];
            SqlParameter parameter = new("@StationId", 123);
            ConfigureCommand(connection, transaction, command, reader, parameterCollection, addedParameters);
            reader.Setup(item => item.Read()).Returns(true);
            reader.Setup(item => item.IsDBNull(0)).Returns(false);
            reader.Setup(item => item.GetString(0)).Returns("[{\"stationId\":123}]");
            ReadGenericRepositoryRepoDB repository = new("Server=unused;");

            string result = repository.QueryAndReturnJson("SELECT @StationId FOR JSON PATH", CommandType.Text, [parameter], connection.Object, transaction.Object);

            Assert.Equal("[{\"stationId\":123}]", result);
            Assert.Single(addedParameters);
            Assert.Same(parameter, addedParameters[0]);
            command.VerifySet(item => item.CommandText = "SELECT @StationId FOR JSON PATH");
            command.VerifySet(item => item.CommandType = CommandType.Text);
            command.VerifySet(item => item.Transaction = transaction.Object);
        }

        [Fact]
        public void ExecuteReader_ConParametrosYMultiplesConjuntos_DelegaElLectorAlMapperAntesDeLiberarlo()
        {
            Mock<IDbConnection> connection = new();
            Mock<IDbTransaction> transaction = new();
            Mock<IDbCommand> command = new();
            Mock<IDataReader> reader = new();
            Mock<IDataParameterCollection> parameterCollection = new();
            List<object> addedParameters = [];
            SqlParameter parameter = new("@CompanyId", 456);
            ConfigureCommand(connection, transaction, command, reader, parameterCollection, addedParameters);
            reader.SetupSequence(item => item.Read()).Returns(true).Returns(true);
            reader.Setup(item => item.NextResult()).Returns(true);
            reader.SetupSequence(item => item.GetInt32(0)).Returns(3).Returns(7);
            ReadGenericRepositoryRepoDB repository = new("Server=unused;");

            (int WorkItems, int Messages) result = repository.ExecuteReader(
                "SELECT @CompanyId; SELECT @CompanyId;", CommandType.Text, [parameter], connection.Object, transaction.Object,
                dataReader =>
                {
                    bool hasWorkItems = dataReader.Read();
                    int workItems = dataReader.GetInt32(0);
                    bool hasMessages = dataReader.NextResult() && dataReader.Read();
                    int messages = dataReader.GetInt32(0);
                    return (hasWorkItems ? workItems : 0, hasMessages ? messages : 0);
                });

            Assert.Equal((3, 7), result);
            Assert.Single(addedParameters);
            Assert.Same(parameter, addedParameters[0]);
            reader.Verify(item => item.Dispose(), Times.Once);
        }

        private static void ConfigureCommand(
            Mock<IDbConnection> connection,
            Mock<IDbTransaction> transaction,
            Mock<IDbCommand> command,
            Mock<IDataReader> reader,
            Mock<IDataParameterCollection> parameterCollection,
            List<object> addedParameters)
        {
            transaction.SetupGet(item => item.Connection).Returns(connection.Object);
            connection.Setup(item => item.CreateCommand()).Returns(command.Object);
            command.SetupGet(item => item.Parameters).Returns(parameterCollection.Object);
            parameterCollection.Setup(item => item.Add(It.IsAny<object>()))
                .Callback<object>(parameter => addedParameters.Add(parameter))
                .Returns(0);
            command.Setup(item => item.ExecuteReader()).Returns(reader.Object);
        }
    }
}
