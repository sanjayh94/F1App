using F1Api.Models;
using F1Api.Repository;
using F1Api.Services;
using Microsoft.Extensions.Logging;
using Moq;

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
            var result = await _service.GetAllAsync();

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
            var result = await _service.GetAllAsync();

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
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetAllAsync());
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
            var result = await _service.GetByIdAsync(circuitId);

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
            var result = await _service.GetByIdAsync(invalidId);

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
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetByIdAsync(circuitId));
            Assert.AreEqual(expectedException.Message, ex.Message);
        }

        [Test]
        public async Task GetCircuitSummariesAsync_ShouldReturnAllCircuitSummaries()
        {
            // Arrange
            var expectedSummaries = new List<CircuitSummary>
            {
                new CircuitSummary { 
                    CircuitId = 1, 
                    Name = "Monaco Circuit", 
                    Country = "Monaco",
                    FastestLapDriver = "Lewis Hamilton",
                    TotalRacesCompleted = 50
                },
                new CircuitSummary { 
                    CircuitId = 2, 
                    Name = "Silverstone Circuit", 
                    Country = "United Kingdom",
                    FastestLapDriver = "Max Verstappen",
                    TotalRacesCompleted = 50
                }
            };

            _mockRepository.Setup(repo => repo.GetSummariesAsync())
                .ReturnsAsync(expectedSummaries);

            // Act
            var result = await _service.GetSummariesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(expectedSummaries.Count, result.Count());
            CollectionAssert.AreEquivalent(expectedSummaries, result);
            _mockRepository.Verify(repo => repo.GetSummariesAsync(), Times.Once);
        }

        [Test]
        public async Task GetCircuitSummariesAsync_WhenNoSummaries_ShouldReturnEmptyCollection()
        {
            // Arrange
            _mockRepository.Setup(repo => repo.GetSummariesAsync())
                .ReturnsAsync(new List<CircuitSummary>());

            // Act
            var result = await _service.GetSummariesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(0, result.Count());
            _mockRepository.Verify(repo => repo.GetSummariesAsync(), Times.Once);
        }

        [Test]
        public async Task GetCircuitSummariesAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var expectedException = new Exception("Database connection error");
            _mockRepository.Setup(repo => repo.GetSummariesAsync())
                .ThrowsAsync(expectedException);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetSummariesAsync());
            Assert.AreEqual(expectedException.Message, ex.Message);
            _mockRepository.Verify(repo => repo.GetSummariesAsync(), Times.Once);
        }

        [Test]
        public async Task GetCircuitSummaryByIdAsync_WithValidId_ShouldReturnCircuitSummary()
        {
            // Arrange
            var circuitId = 1;
            var expectedSummary = new CircuitSummary
            {
                CircuitId = circuitId,
                Name = "Monaco Circuit",
                Country = "Monaco",
                Location = "Monte Carlo",
                FastestLapDriver = "Lewis Hamilton",
                FastestLapTime = "1:12.909",
                FastestLapRaceYear = 2021,
                TotalRacesCompleted = 50
            };

            _mockRepository.Setup(repo => repo.GetSummaryByIdAsync(circuitId))
                .ReturnsAsync(expectedSummary);

            // Act
            var result = await _service.GetSummaryByIdAsync(circuitId);

            // Assert
            Assert.NotNull(result);
            Assert.AreEqual(circuitId, result.CircuitId);
            Assert.AreEqual("Monaco Circuit", result.Name);
            Assert.AreEqual("Lewis Hamilton", result.FastestLapDriver);
            _mockRepository.Verify(repo => repo.GetSummaryByIdAsync(circuitId), Times.Once);
        }

        [Test]
        public async Task GetCircuitSummaryByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var invalidId = 999;
            _mockRepository.Setup(repo => repo.GetSummaryByIdAsync(invalidId))
                .ReturnsAsync((CircuitSummary)null);

            // Act
            var result = await _service.GetSummaryByIdAsync(invalidId);

            // Assert
            Assert.Null(result);
            _mockRepository.Verify(repo => repo.GetSummaryByIdAsync(invalidId), Times.Once);
        }

        [Test]
        public async Task GetCircuitSummaryByIdAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            var circuitId = 1;
            var expectedException = new Exception("Database connection error");
            _mockRepository.Setup(repo => repo.GetSummaryByIdAsync(circuitId))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetSummaryByIdAsync(circuitId));
            Assert.AreEqual(expectedException.Message, ex.Message);
            _mockRepository.Verify(repo => repo.GetSummaryByIdAsync(circuitId), Times.Once);
        }
    }
}