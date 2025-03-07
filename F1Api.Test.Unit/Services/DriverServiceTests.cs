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
                new Driver { Id = 848, Forename = "Alexander", Surname = "Albon", Nationality = "Thai" },
                new Driver { Id = 832, Forename = "Carlos", Surname = "Sainz", Nationality = "Spanish" }
            };

            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(expectedDrivers);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(expectedDrivers.Count));
            CollectionAssert.AreEquivalent(expectedDrivers, result);
        }

        [Test]
        public async Task GetAllDriversAsync_WhenNoDrivers_ShouldReturnEmptyCollection()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Driver>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetAllDriversAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var expectedException = new Exception("Database connection error");
            _mockRepository.Setup(repo => repo.GetAllAsync())
                .ThrowsAsync(expectedException);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetAllAsync());
            Assert.That(ex.Message, Is.EqualTo(expectedException.Message));
        }

        [Test]
        public async Task GetDriverByIdAsync_WithValidId_ShouldReturnDriver()
        {
            // Arrange
            var driverId = 848;
            var expectedDriver = new Driver
            {
                Id = driverId,
                Forename = "Alexander",
                Surname = "Albon",
                Nationality = "Thai",
                Number = 23,
                Code = "ALB"
            };

            _mockRepository.Setup(repo => repo.GetByIdAsync(driverId))
                .ReturnsAsync(expectedDriver);

            // Act
            var result = await _service.GetByIdAsync(driverId);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Id, Is.EqualTo(driverId));
            Assert.That(result.Forename, Is.EqualTo("Alexander"));
            Assert.That(result.Surname, Is.EqualTo("Albon"));
        }

        [Test]
        public async Task GetDriverByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = 999;
            _mockRepository.Setup(repo => repo.GetByIdAsync(invalidId))
                .ReturnsAsync((Driver)null);

            // Act
            var result = await _service.GetByIdAsync(invalidId);

            // Assert
            Assert.Null(result);
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
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetByIdAsync(driverId));
            Assert.That(ex.Message, Is.EqualTo(expectedException.Message));
        }

        [Test]
        public async Task GetDriverSummariesAsync_ShouldReturnAllDriverSummaries()
        {
            // Arrange
            var expectedSummaries = new List<DriverSummary>
            {
                new DriverSummary {
                    DriverId = 848,
                    FullName = "Alexander Albon",
                    Nationality = "Thai",
                    PodiumCount = 0,
                    TotalRacesEntered = 1
                },
                new DriverSummary {
                    DriverId = 832,
                    FullName = "Carlos Sainz",
                    Nationality = "Spanish",
                    PodiumCount = 50,
                    TotalRacesEntered = 100
                }
            };

            _mockRepository.Setup(repo => repo.GetSummariesAsync())
                .ReturnsAsync(expectedSummaries);

            // Act
            var result = await _service.GetSummariesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(expectedSummaries.Count));
            CollectionAssert.AreEquivalent(expectedSummaries, result);
        }

        [Test]
        public async Task GetDriverSummariesAsync_WhenNoSummaries_ShouldReturnEmptyCollection()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetSummariesAsync())
                .ReturnsAsync(new List<DriverSummary>());

            // Act
            var result = await _service.GetSummariesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetDriverSummariesAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var expectedException = new Exception("Database connection error");
            _mockRepository.Setup(repo => repo.GetSummariesAsync())
                .ThrowsAsync(expectedException);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetSummariesAsync());
            Assert.That(ex.Message, Is.EqualTo(expectedException.Message));
        }

        [Test]
        public async Task GetDriverSummaryByIdAsync_WithValidId_ShouldReturnDriverSummary()
        {
            // Arrange
            var driverId = 848;
            var expectedSummary = new DriverSummary
            {
                DriverId = driverId,
                FullName = "Alexander Albon",
                Nationality = "Thai",
                PodiumCount = 50,
                TotalRacesEntered = 100
            };

            _mockRepository.Setup(repo => repo.GetSummaryByIdAsync(driverId))
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _service.GetSummaryByIdAsync(driverId);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.DriverId, Is.EqualTo(driverId));
            Assert.That(result.FullName, Is.EqualTo("Alexander Albon"));
            Assert.That(result.Nationality, Is.EqualTo("Thai"));
            Assert.That(result.PodiumCount, Is.EqualTo(50));
        }

        [Test]
        public async Task GetDriverSummaryByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = 999;
            _mockRepository.Setup(repo => repo.GetSummaryByIdAsync(invalidId))
                .ReturnsAsync((DriverSummary)null);

            // Act
            var result = await _service.GetSummaryByIdAsync(invalidId);

            // Assert
            Assert.Null(result);
        }

        [Test]
        public async Task GetDriverSummaryByIdAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var driverId = 1;
            var expectedException = new Exception("Database connection error");
            _mockRepository.Setup(repo => repo.GetSummaryByIdAsync(driverId))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetSummaryByIdAsync(driverId));
            Assert.That(ex.Message, Is.EqualTo(expectedException.Message));
        }
    }
}