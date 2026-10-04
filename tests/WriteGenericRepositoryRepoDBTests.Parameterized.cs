// Copyright (c) Gasolutions SAS. Todos los derechos reservados.

namespace Gasolutions.Core.Repository.UnitTests
{
    public partial class WriteGenericRepositoryRepoDBTests
    {
        [Fact]
        public void ExecuteReader_ConParametrosYTransaccion_AdjuntaTodosLosParametros()
        {
            Mock<IDbConnection> connection = new();
            Mock<IDbTransaction> transaction = new();
            Mock<IDbCommand> command = new();
            Mock<IDataReader> reader = new();
            Mock<IDataParameterCollection> parameterCollection = new();
            List<object> addedParameters = [];
            SqlParameter first = new("@First", 1);
            SqlParameter second = new("@Second", 2);
            transaction.SetupGet(item => item.Connection).Returns(connection.Object);
            connection.Setup(item => item.CreateCommand()).Returns(command.Object);
            command.SetupGet(item => item.Parameters).Returns(parameterCollection.Object);
            parameterCollection.Setup(item => item.Add(It.IsAny<object>()))
                .Callback<object>(parameter => addedParameters.Add(parameter))
                .Returns(0);
            command.Setup(item => item.ExecuteReader()).Returns(reader.Object);
            WriteGenericRepositoryRepoDB<TestEntity, int> repository = new("Server=unused;");

            using IDataReader actual = repository.ExecuteReader(
                "SELECT @First + @Second;", CommandType.Text, connection.Object, transaction.Object, [first, second]);

            Assert.Equal(2, addedParameters.Count);
            Assert.Same(first, addedParameters[0]);
            Assert.Same(second, addedParameters[1]);
            command.VerifySet(item => item.Transaction = transaction.Object);
        }
    }
}
