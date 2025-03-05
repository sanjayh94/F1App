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
    public class DriverServiceTests
    {
        private Mock<IDriverRepository> _mockRepository;
        private Mock<ILogger<DriverService>> _mockLogger;
        private DriverService _service;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IDriverRepository>();
            _mockLogger = new Mock<ILogger<DriverService>>();
            _service = new DriverService(_mockRepository.Object, _mockLogger.Object);
        }

        [Test]
        public async Task GetAllDriversAsync_ShouldReturnAllDrivers()
        {
            // Arrange
            var expectedDrivers = new List<Driver>
            {
                new Driver { Id = 1, Forename = "Lewis", Surname = "Hamilton", Nationality = "British" },
                new Driver { Id = 2, Forename = "Max", Surname = "Verstappen", Nationality = "Dutch" }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedDrivers);

            // Act
            var result = await _service.GetAllDriversAsync();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(expectedDrivers.Count, result.Count());
            CollectionAssert.AreEquivalent(expectedDrivers, result);
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllDriversAsync_WhenNoDrivers_ShouldReturnEmptyCollection()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Driver>());

            // Act
            var result = await _service.GetAllDriversAsync();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetAllDriversAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var expectedException = new Exception("Database connection error");
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ThrowsAsync(expectedException);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetAllDriversAsync());
            Assert.AreEqual(expectedException.Message, ex.Message);
            _mockRepository.Verify(repo => repo.GetAllAsync(), Times.Once);
        }

        [Test]
        public async Task GetDriverByIdAsync_WithValidId_ShouldReturnDriver()
        {
            // Arrange
            var driverId = 1;
            var expectedDriver = new Driver
            {
                Id = driverId,
                Forename = "Lewis",
                Surname = "Hamilton",
                Nationality = "British",
                Number = 44,
                Code = "HAM"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(driverId))
                .ReturnsAsync(expectedDriver);

            // Act
            var result = await _service.GetDriverByIdAsync(driverId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(driverId, result.Id);
            Assert.AreEqual("Lewis", result.Forename);
            Assert.AreEqual("Hamilton", result.Surname);
            _mockRepository.Verify(repo => repo.GetByIdAsync(driverId), Times.Once);
        }

        [Test]
        public async Task GetDriverByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = 999;
            _mockRepository.Setup(repo => repo.GetByIdAsync(invalidId))
                .ReturnsAsync((Driver)null);

            // Act
            var result = await _service.GetDriverByIdAsync(invalidId);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetByIdAsync(invalidId), Times.Once);
        }

        [Test]
        public async Task GetDriverByIdAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var driverId = 1;
            var expectedException = new Exception("Database connection error");
            _mockRepository.Setup(repo => repo.GetByIdAsync(driverId))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetDriverByIdAsync(driverId));
            Assert.AreEqual(expectedException.Message, ex.Message);
            _mockRepository.Verify(repo => repo.GetByIdAsync(driverId), Times.Once);
        }
    }
}