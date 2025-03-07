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
    public class ChampionshipServiceTests
    {
        private Mock<IChampionshipRepository> _mockChampionshipRepository;
        private Mock<IDriverRepository> _mockDriverRepository;
        private Mock<ILogger<ChampionshipService>> _mockLogger;
        private ChampionshipService _service;

        [SetUp]
        public void Setup()
        {
            _mockChampionshipRepository = new Mock<IChampionshipRepository>();
            _mockDriverRepository = new Mock<IDriverRepository>();
            _mockLogger = new Mock<ILogger<ChampionshipService>>();
            _service = new ChampionshipService(
                _mockChampionshipRepository.Object,
                _mockDriverRepository.Object,
                _mockLogger.Object);
        }

        [Test]
        public async Task GetDriverChampionshipByYearAsync_ReturnsCorrectSummaries()
        {
            // Arrange
            var year = 2023;
            var driverStandings = new List<DriverStanding>
            {
                new DriverStanding { DriverId = 1, RaceId = 2, Points = 43, Position = 1, Wins = 2 },
                new DriverStanding { DriverId = 2, RaceId = 2, Points = 40, Position = 2, Wins = 1 }
            };

            var drivers = new List<Driver>
            {
                new Driver { Id = 1, Forename = "Lewis", Surname = "Hamilton", Nationality = "British", DriverReference = "hamilton" },
                new Driver { Id = 2, Forename = "Max", Surname = "Verstappen", Nationality = "Dutch", DriverReference = "verstappen" }
            };

            _mockChampionshipRepository.Setup(repo => repo.GetDriverStandingsByYearAsync(year))
                .ReturnsAsync(driverStandings);

            _mockDriverRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(drivers);

            // Act
            var result = await _service.GetDriverChampionshipByYearAsync(year);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(2));

            var hamiltonSummary = result.FirstOrDefault(d => d.DriverId == 1);
            var verstappenSummary = result.FirstOrDefault(d => d.DriverId == 2);

            Assert.NotNull(hamiltonSummary);
            Assert.NotNull(verstappenSummary);

            Assert.That(hamiltonSummary.TotalPoints, Is.EqualTo(43));
            Assert.That(verstappenSummary.TotalPoints, Is.EqualTo(40));

            Assert.That(hamiltonSummary.Wins, Is.EqualTo(2));
            Assert.That(verstappenSummary.Wins, Is.EqualTo(1));

            Assert.That(hamiltonSummary.Position, Is.EqualTo(1));
            Assert.That(verstappenSummary.Position, Is.EqualTo(2));
        }

        [Test]
        public async Task GetDriverChampionshipByYearAsync_WithNoData_ReturnsEmptyList()
        {
            // Arrange
            var year = 2025;
            _mockChampionshipRepository.Setup(repo => repo.GetDriverStandingsByYearAsync(year))
                .ReturnsAsync(new List<DriverStanding>());

            _mockDriverRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(new List<Driver>());

            // Act
            var result = await _service.GetDriverChampionshipByYearAsync(year);

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(0));
        }

        [Test]
        public async Task GetDriverChampionshipByYearAsync_WhenRepositoryThrowsException_ShouldPropagateException()
        {
            // Arrange
            int year = 2023;
            var expectedException = new Exception("Database connection error");
            _mockChampionshipRepository.Setup(repo => repo.GetDriverStandingsByYearAsync(year))
                .ThrowsAsync(expectedException);

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(() => _service.GetDriverChampionshipByYearAsync(year));
            Assert.That(ex.Message, Is.EqualTo(expectedException.Message));
        }

        [Test]
        public async Task GetDriverChampionshipByYearAsync_ReturnsDriversInCorrectPositionOrder()
        {
            // Arrange
            int year = 2023;
            var driverStandings = new List<DriverStanding>
            {
                new DriverStanding { DriverId = 1, RaceId = 2, Points = 43, Position = 1, Wins = 2 },
                new DriverStanding { DriverId = 2, RaceId = 2, Points = 40, Position = 2, Wins = 1 },
                new DriverStanding { DriverId = 3, RaceId = 2, Points = 25, Position = 3, Wins = 0 }
            };

            var drivers = new List<Driver>
            {
                new Driver { Id = 1, Forename = "Lewis", Surname = "Hamilton", Nationality = "British", DriverReference = "hamilton" },
                new Driver { Id = 2, Forename = "Max", Surname = "Verstappen", Nationality = "Dutch", DriverReference = "verstappen" },
                new Driver { Id = 3, Forename = "Charles", Surname = "Leclerc", Nationality = "Monegasque", DriverReference = "leclerc" }
            };

            _mockChampionshipRepository.Setup(repo => repo.GetDriverStandingsByYearAsync(year))
                .ReturnsAsync(driverStandings);

            _mockDriverRepository.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(drivers);

            // Act
            var result = await _service.GetDriverChampionshipByYearAsync(year);
            var resultList = result.ToList();

            // Assert
            Assert.NotNull(result);
            Assert.That(result.Count(), Is.EqualTo(3));

            Assert.That(resultList[0].Position, Is.EqualTo(1));
            Assert.That(resultList[1].Position, Is.EqualTo(2));
            Assert.That(resultList[2].Position, Is.EqualTo(3));

            Assert.That(resultList[0].DriverId, Is.EqualTo(1)); // Hamilton
            Assert.That(resultList[1].DriverId, Is.EqualTo(2)); // Verstappen
            Assert.That(resultList[2].DriverId, Is.EqualTo(3)); // Leclerc
        }
    }
}