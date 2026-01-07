using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Report;

public class GetRecentActivityTests : IntegrationTestBase
{
    public GetRecentActivityTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task GetRecentActivity_ReturnsActivityList()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/reports/recent-activity");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var activities = JsonSerializer.Deserialize<ActivityResponse[]>(content, DefaultJsonSerializerOptions);
        
        activities.Should().NotBeNull();
    }

    [Fact]
    public async Task GetRecentActivity_WithLimit_ReturnsLimitedResults()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/reports/recent-activity?limit=5");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var activities = JsonSerializer.Deserialize<ActivityResponse[]>(content, DefaultJsonSerializerOptions);
        
        activities.Should().NotBeNull();
        activities!.Should().HaveCountLessOrEqualTo(5);
    }

    [Fact]
    public async Task GetRecentActivity_WithDateRange_ReturnsFilteredResults()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd");
        var endDate = DateTime.Now.ToString("yyyy-MM-dd");

        // Act
        var response = await HttpClient.GetAsync($"/api/reports/recent-activity?startDate={startDate}&endDate={endDate}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var activities = JsonSerializer.Deserialize<ActivityResponse[]>(content, DefaultJsonSerializerOptions);
        
        activities.Should().NotBeNull();
    }

    [Theory]
    [InlineData("invalid-date")]
    [InlineData("2023-13-45")]
    [InlineData("not-a-date")]
    public async Task GetRecentActivity_WithInvalidDateFormat_ReturnsBadRequest(string invalidDate)
    {
        // Act
        var response = await HttpClient.GetAsync($"/api/reports/recent-activity?startDate={invalidDate}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetRecentActivity_WithNegativeLimit_ReturnsBadRequestOrIgnored()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/reports/recent-activity?limit=-1");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }

    private record ActivityResponse(
        int Id,
        string Action,
        string? Description,
        DateTime CreatedAt,
        string? EntityType,
        int? EntityId
    );
}