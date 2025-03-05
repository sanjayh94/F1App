using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using F1Api.Models;
using NUnit.Framework;

namespace F1Api.Test.Acceptance.Features
{
    [TestFixture]
    public class DriverApiTests
    {
        private HttpClient _client;
        private TestWebApplicationFactory _factory;
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        [OneTimeSetUp]
        public void Setup()
        {
            _factory = new TestWebApplicationFactory();
            _client = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task GetAllDrivers_ReturnsAllSeededDrivers()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/drivers");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var drivers = await response.Content.ReadFromJsonAsync<List<Driver>>(_jsonOptions);
            Assert.That(drivers, Is.Not.Null);
            Assert.That(drivers.Count, Is.GreaterThan(0)); 

            Assert.That(drivers.Any(d => d.Surname == "Hamilton"), Is.True);
            Assert.That(drivers.Any(d => d.Surname == "Verstappen"), Is.True);
        }

        [Test]
        public async Task GetDriverById_ReturnsCorrectDriver()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/drivers/1");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var driver = await response.Content.ReadFromJsonAsync<Driver>(_jsonOptions);
            Assert.That(driver, Is.Not.Null);
            Assert.That(driver.Id, Is.EqualTo(1));

            Assert.That(driver.Forename, Is.EqualTo("Lewis"));
            Assert.That(driver.Surname, Is.EqualTo("Hamilton"));
            Assert.That(driver.Nationality, Is.EqualTo("British"));
        }

        [Test]
        public async Task GetDriverById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/drivers/999");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetDriverSummaries_ReturnsAllDriverSummaries()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/drivers/summaries");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var summaries = await response.Content.ReadFromJsonAsync<List<DriverSummary>>(_jsonOptions);
            Assert.That(summaries, Is.Not.Null);
            Assert.That(summaries.Count, Is.EqualTo(3));

            // Verify full names for each summary
            Assert.That(summaries.Any(s => s.FullName.Contains("Hamilton")), Is.True);
            Assert.That(summaries.Any(s => s.FullName.Contains("Verstappen")), Is.True);

            // Verify summary properties exist on each driver
            foreach (var summary in summaries)
            {
                Assert.That(summary.TotalRacesEntered, Is.GreaterThanOrEqualTo(0));
                Assert.That(summary.PodiumCount, Is.GreaterThanOrEqualTo(0));
            }
        }

        [Test]
        public async Task GetDriverSummaryById_ReturnsCorrectDriverSummary()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/drivers/summaries/1");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var summary = await response.Content.ReadFromJsonAsync<DriverSummary>(_jsonOptions);
            Assert.That(summary, Is.Not.Null);
            Assert.That(summary.DriverId, Is.EqualTo(1));
            Assert.That(summary.FullName, Is.EqualTo("Lewis Hamilton"));
            Assert.That(summary.Nationality, Is.EqualTo("British"));
            Assert.That(summary.TotalRacesEntered, Is.GreaterThanOrEqualTo(0));
            Assert.That(summary.PodiumCount, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        public async Task GetDriverSummaryById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/drivers/summaries/999");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}