// <copyright file="TestEntities.cs" company="Gasolutions SAS">
// Copyright (c) Gasolutions SAS. Todos los derechos reservados.
// </copyright>

namespace Gasolutions.Core.Repository.UnitTests
{
    /// <summary>
    /// Test entity class used for testing the generic repository.
    /// </summary>
    internal class TestEntity
    {
        /// <summary>
        /// Gets or sets the identifier for the test entity.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the test entity.
        /// </summary>
        public string? Name { get; set; }
    }
}
