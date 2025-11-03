using BlazorApp.Models;
using BlazorApp.Tests;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace BlazorApp.IntegrationTests
{
    public class CustomerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;

        public CustomerTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetCustomers_ReturnsPagedResult_WithSeededCustomers()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<BlazorAppContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                Seeding.InitializeTestDB(db);
            }

            var response = await _httpClient.GetAsync("/api/customers");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PagedResult<Customer>>();
            result.Should().NotBeNull();
            result!.Customers.Count.Should().BeGreaterThanOrEqualTo(2);
        }

        [Fact]
        public async Task GetCustomerById_ReturnsCustomer_WhenExists()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<BlazorAppContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                Seeding.InitializeTestDB(db);
            }

            var response = await _httpClient.GetAsync("/api/customers/CUST001");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var customer = await response.Content.ReadFromJsonAsync<Customer>();
            customer.Should().NotBeNull();
            customer!.Id.Should().Be("CUST001");
        }

        [Fact]
        public async Task DeleteCustomer_RemovesCustomer()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<BlazorAppContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
                Seeding.InitializeTestDB(db);
            }

            // delete existing
            var del = await _httpClient.DeleteAsync("/api/customers/CUST002");
            del.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // now getting it should return 404
            var get = await _httpClient.GetAsync("/api/customers/CUST002");
            get.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateCustomer_CreatesNewCustomer()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<BlazorAppContext>();

                db.Database.EnsureDeleted();
                db.Database.EnsureCreated();
            }

            var dto = new CustomerDto
            {
                CompanyName = "NewCo",
                ContactName = "Zed",
                Address = "3 Third St",
                City = "NewCity",
                Region = "NewRegion",
                PostalCode = "11111",
                Country = "NewCountry",
                Phone = "555-0300"
            };

            var response = await _httpClient.PostAsJsonAsync("/api/customers", dto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<Customer>();
            created.Should().NotBeNull();
            created!.CompanyName.Should().Be(dto.CompanyName);
        }
    }
}
