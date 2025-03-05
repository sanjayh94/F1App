using F1Api.Models;
using F1Api.Repository;
using F1Api.Services;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace F1Api.Test.Unit.Services
{
    [TestFixture]
    public class CircuitServiceTests
    {
        private Mock<ICircuitRepository> _mockRepository;
        private Mock<ILogger<CircuitService>> _mockLogger;
        private CircuitService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<ICircuitRepository>();
            _mockLogger = new Mock<ILogger<CircuitService>>();
            _service = new CircuitService(_mockRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllCircuitsAsync_ShouldReturnAllCircuits()
        {
            // Arrange
            var expectedCircuits = new List<Circuit>
            {
                new Circuit { Id = 1, Name = "Circuit 1", Country = "Country 1" },
                new Circuit { Id = 2, Name = "Circuit 2", Country = "Country 2" }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedCircuits);

            // Act
            var result = await _service.GetAllCircuitsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(expectedCircuits.Count, result.Count());
            CollectionAssert.AreEquivalent(expectedCircuits, result);
        }

        [Test]
        public async Task GetAllCircuitsAsync_WhenNoCircuits_ShouldReturnEmptyCollection()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Circuit>());

            // Act
            var result = await _service.GetAllCircuitsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count());
        }

        [Test]
        public async Task GetAllCircuitsAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var expectedException = new Exception("Database connection error");
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ThrowsAsync(expectedException);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetAllCircuitsAsync());
            Assert.AreEqual(expectedException.Message, ex.Message);
        }

        [Test]
        public async Task GetCircuitByIdAsync_WithValidId_ShouldReturnCircuit()
        {
            // Arrange
            var circuitId = 1;
            var expectedCircuit = new Circuit
            {
                Id = circuitId,
                Name = "Monaco",
                Country = "Monaco",
                CircuitReference = "monaco",
                Location = "Monte Carlo",
                Latitude = 43.7347,
                Longitude = 7.4205
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(circuitId))
                .ReturnsAsync(expectedCircuit);

            // Act
            var result = await _service.GetCircuitByIdAsync(circuitId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(circuitId, result.Id);
            Assert.AreEqual("Monaco", result.Name);
        }

        [Test]
        public async Task GetCircuitByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = 999;
            _mockRepository.Setup(repo => repo.GetByIdAsync(invalidId))
                .ReturnsAsync((Circuit)null);

            // Act
            var result = await _service.GetCircuitByIdAsync(invalidId);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task GetCircuitByIdAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var circuitId = 1;
            var expectedException = new Exception("Database connection error");
            _mockRepository.Setup(repo => repo.GetByIdAsync(circuitId))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetCircuitByIdAsync(circuitId));
            Assert.AreEqual(expectedException.Message, ex.Message);
        }
    }
}