using System.Net;
using System.Net.Http.Json;
using F1Api.Models;

namespace F1Api.Test.Integration.Features
{
    [TestFixture]
    public class DriverIntegrationTests
    {
        private CustomWebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void Setup()
        {
            _factory = new CustomWebApplicationFactory<Program>();

            _client = _factory.CreateClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task GetDriverById_ExistingId_ReturnsDriver()
        {
            // Arrange
            var driverId = 848;

            // Act
            var response = await _client.GetAsync($"/api/Drivers/{driverId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var driver = await response.Content.ReadFromJsonAsync<Driver>();
            Assert.That(driver, Is.Not.Null);
            Assert.That(driver.Id, Is.EqualTo(driverId));
            Assert.That(driver.Forename, Is.EqualTo("Alexander"));
            Assert.That(driver.Surname, Is.EqualTo("Albon"));

            TestContext.WriteLine($"Driver found: {driver.Forename} {driver.Surname} (ID: {driver.Id})");
        }

        [Test]
        public async Task GetDriverById_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var nonExistingDriverId = 99999;

            // Act
            var response = await _client.GetAsync($"/api/Drivers/{nonExistingDriverId}");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
        }
    }
}