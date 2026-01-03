using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using APDL.API.Domain.DockAggregate.DTO;
using Xunit;

namespace APDL.Tests.E2E
{
    public class DocksE2ETests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public DocksE2ETests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _client.DefaultRequestHeaders.Add("Authorization", "Bearer test-token");
        }

        [Fact]
        public async Task GetAllDocks_ShouldReturnOk()
        {
            var response = await _client.GetAsync("/api/Docks");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var docks = await response.Content.ReadFromJsonAsync<DockDto[]>();
            Assert.NotNull(docks);
        }

        [Fact]
        public async Task CreateDock_ShouldReturnCreated()
        {
            var createDto = new CreateDockDto
            {
                DockName = "Test Dock E2E",
                DockLength = 350,
                DockDraft = 15
            };

            var response = await _client.PostAsJsonAsync("/api/Docks", createDto);

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var createdDock = await response.Content.ReadFromJsonAsync<DockDto>();
            Assert.NotNull(createdDock);
            Assert.Equal("Test Dock E2E", createdDock.DockName);
            Assert.Equal(350, createdDock.DockLength);
            Assert.Equal(15, createdDock.DockDraft);
        }

        [Fact]
        public async Task CreateAndGetDock_EndToEndFlow_ShouldWork()
        {
            var createDto = new CreateDockDto
            {
                DockName = "E2E Flow Dock",
                DockLength = 400,
                DockDraft = 18
            };

            var createResponse = await _client.PostAsJsonAsync("/api/Docks", createDto);
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
            var createdDock = await createResponse.Content.ReadFromJsonAsync<DockDto>();
            Assert.NotNull(createdDock);

            var getResponse = await _client.GetAsync($"/api/Docks/{createdDock.Id}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
            var retrievedDock = await getResponse.Content.ReadFromJsonAsync<DockDto>();

            Assert.NotNull(retrievedDock);
            Assert.Equal(createdDock.Id, retrievedDock.Id);
            Assert.Equal("E2E Flow Dock", retrievedDock.DockName);
        }

        [Fact]
        public async Task GetDockById_WithNonExistentId_ShouldReturnNotFound()
        {
            var nonExistentId = Guid.NewGuid();

            var response = await _client.GetAsync($"/api/Docks/{nonExistentId}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetDocksCapableOfVessel_ShouldReturnOk()
        {
            var vesselLength = 350;
            var vesselDraft = 15;

            var response = await _client.GetAsync($"/api/Docks/capable?vesselLength={vesselLength}&vesselDraft={vesselDraft}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var docks = await response.Content.ReadFromJsonAsync<DockDto[]>();
            Assert.NotNull(docks);
        }
    }
}

