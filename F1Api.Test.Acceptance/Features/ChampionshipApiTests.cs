using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using F1Api.Models;
using NUnit.Framework;

namespace F1Api.Test.Acceptance.Features
{
    [TestFixture]
    public class ChampionshipApiTests
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
        public async Task GetDriverChampionshipByYear_ReturnsChampionshipData()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/championships/2025");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var championshipData = await response.Content.ReadFromJsonAsync<List<DriverChampionshipSummary>>(_jsonOptions);
            Assert.That(championshipData, Is.Not.Null);
            Assert.That(championshipData.Count, Is.GreaterThan(0));

            var hamiltonData = championshipData.FirstOrDefault(d => d.FullName.Contains("Hamilton"));
            var albonData = championshipData.FirstOrDefault(d => d.FullName.Contains("Albon"));
            var sainzData = championshipData.FirstOrDefault(d => d.FullName.Contains("Sainz"));

            Assert.That(hamiltonData, Is.Not.Null, "Hamilton should be in championship data");
            Assert.That(albonData, Is.Not.Null, "Albon should be in championship data");
            Assert.That(sainzData, Is.Not.Null, "Sainz should be in championship data");

            foreach (var driver in championshipData)
            {
                Assert.That(driver.DriverId, Is.GreaterThan(0));
                Assert.That(driver.DriverReference, Is.Not.Null.And.Not.Empty);
                Assert.That(driver.FullName, Is.Not.Null.And.Not.Empty);
                Assert.That(driver.Position, Is.GreaterThan(0));
                Assert.That(driver.TotalPoints, Is.GreaterThanOrEqualTo(0));
                Assert.That(driver.Wins, Is.GreaterThanOrEqualTo(0));
            }

            // Verify that positions are in the correct order
            for (int i = 0; i < championshipData.Count - 1; i++)
            {
                Assert.That(championshipData[i].Position, Is.LessThan(championshipData[i + 1].Position));
            }

            Assert.That(hamiltonData.TotalPoints, Is.EqualTo(43));
            Assert.That(hamiltonData.Wins, Is.EqualTo(1));
            Assert.That(hamiltonData.Position, Is.EqualTo(1));
        }

        [Test]
        public async Task GetDriverChampionshipByYear_WithInvalidYear_ReturnsNotFound()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/championships/1900");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetDriverChampionshipByYear_WithDifferentYears_ReturnsDifferentResults()
        {
            // Arrange & Act
            var response2025 = await _client.GetAsync("/api/championships/2025");
            var response2026 = await _client.GetAsync("/api/championships/2026");

            Assert.That(response2025.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(response2026.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var championship2025 = await response2025.Content.ReadFromJsonAsync<List<DriverChampionshipSummary>>(_jsonOptions);
            var championship2026 = await response2026.Content.ReadFromJsonAsync<List<DriverChampionshipSummary>>(_jsonOptions);

            Assert.That(championship2025, Is.Not.Null);
            Assert.That(championship2026, Is.Not.Null);

            // In 2025, Hamilton is in position 1, while in 2026, Albon is in position 1
            var hamilton2025 = championship2025.FirstOrDefault(d => d.FullName.Contains("Hamilton"));
            var hamilton2026 = championship2026.FirstOrDefault(d => d.FullName.Contains("Hamilton"));
            var albon2025 = championship2025.FirstOrDefault(d => d.FullName.Contains("Albon"));
            var albon2026 = championship2026.FirstOrDefault(d => d.FullName.Contains("Albon"));

            Assert.That(hamilton2025, Is.Not.Null);
            Assert.That(hamilton2026, Is.Not.Null);
            Assert.That(albon2025, Is.Not.Null);
            Assert.That(albon2026, Is.Not.Null);

            // Verify positions differ between years
            Assert.That(hamilton2025.Position, Is.Not.EqualTo(hamilton2026.Position));
            Assert.That(albon2025.Position, Is.Not.EqualTo(albon2026.Position));
        }
    }
}