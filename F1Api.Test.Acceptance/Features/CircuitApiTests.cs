using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using F1Api.Models;
using NUnit.Framework;

namespace F1Api.Test.Acceptance.Features
{
    [TestFixture]
    public class CircuitApiTests
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
        public async Task GetAllCircuits_ReturnsAllSeededCircuits()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/circuits");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var circuits = await response.Content.ReadFromJsonAsync<List<Circuit>>(_jsonOptions);
            Assert.That(circuits, Is.Not.Null);
            Assert.That(circuits.Count, Is.EqualTo(3));

            Assert.That(circuits.Any(c => c.Name == "Monaco Circuit"), Is.True);
            Assert.That(circuits.Any(c => c.Name == "Silverstone Circuit"), Is.True);
            Assert.That(circuits.Any(c => c.Name == "Monza Circuit"), Is.True);
        }

        [Test]
        public async Task GetCircuitById_ReturnsCorrectCircuit()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/circuits/1");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var circuit = await response.Content.ReadFromJsonAsync<Circuit>(_jsonOptions);
            Assert.That(circuit, Is.Not.Null);
            Assert.That(circuit.Id, Is.EqualTo(1));
            Assert.That(circuit.Name, Is.EqualTo("Monaco Circuit"));
            Assert.That(circuit.Country, Is.EqualTo("Monaco"));
            Assert.That(circuit.Location, Is.EqualTo("Monte Carlo"));
        }

        [Test]
        public async Task GetCircuitById_WithIdThatDoesntExist_ReturnsNotFound()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/circuits/999");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }

        [Test]
        public async Task GetCircuitById_WithInvalidId_ReturnsBadRequest()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/circuits/-1");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task GetCircuitSummaries_ReturnsAllCircuitSummaries()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/circuits/summaries");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var summaries = await response.Content.ReadFromJsonAsync<List<CircuitSummary>>(_jsonOptions);
            Assert.That(summaries, Is.Not.Null);
            Assert.That(summaries.Count, Is.EqualTo(3));

            // Verify basic properties for each summary
            Assert.That(summaries.Any(s => s.Name == "Monaco Circuit"), Is.True);
            Assert.That(summaries.Any(s => s.Name == "Silverstone Circuit"), Is.True);
            Assert.That(summaries.Any(s => s.Name == "Monza Circuit"), Is.True);

            // Verify TotalRacesCompleted property exists on each circuit
            foreach (var summary in summaries)
            {
                Assert.That(summary.TotalRacesCompleted, Is.GreaterThanOrEqualTo(0));
            }
        }

        [Test]
        public async Task GetCircuitSummaryById_ReturnsCorrectCircuitSummary()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/circuits/1/summaries");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var summary = await response.Content.ReadFromJsonAsync<CircuitSummary>(_jsonOptions);
            Assert.That(summary, Is.Not.Null);
            Assert.That(summary.CircuitId, Is.EqualTo(1));
            Assert.That(summary.Name, Is.EqualTo("Monaco Circuit"));
            Assert.That(summary.Country, Is.EqualTo("Monaco"));
            Assert.That(summary.TotalRacesCompleted, Is.GreaterThanOrEqualTo(0));
            
            // Verify that summary contains additional properties
            Assert.That(summary.FastestLapTime, Is.Not.Null.Or.Empty);
            Assert.That(summary.FastestLapDriver, Is.Not.Null.Or.Empty);
        }

        [Test]
        public async Task GetCircuitSummaryById_WithInvalidId_ReturnsNotFound()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/circuits/999/summaries");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}