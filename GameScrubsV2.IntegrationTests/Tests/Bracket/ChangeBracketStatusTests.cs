using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Bracket;

public class ChangeBracketStatusTests : IntegrationTestBase
{
    public ChangeBracketStatusTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task ChangeBracketStatus_WithValidStatus_ReturnsSuccess()
    {
        // Arrange - Create a bracket first
        var createRequest = new
        {
            Name = "Status Test Tournament",
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        var createResponse = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdBracket = JsonSerializer.Deserialize<BracketResponse>(createContent, DefaultJsonSerializerOptions);

        var statusRequest = new
        {
            Status = BracketStatus.Started
        };

        // Act
        var response = await HttpClient.PatchAsync($"/api/brackets/{createdBracket!.Id}/status", CreateJsonContent(statusRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        // Verify status was changed
        var getResponse = await HttpClient.GetAsync($"/api/brackets/{createdBracket.Id}");
        var getContent = await getResponse.Content.ReadAsStringAsync();
        var updatedBracket = JsonSerializer.Deserialize<BracketResponse>(getContent, DefaultJsonSerializerOptions);
        
        updatedBracket!.Status.Should().Be(BracketStatus.Started);
    }

    [Fact]
    public async Task ChangeBracketStatus_WithNonExistentBracket_ReturnsNotFound()
    {
        // Arrange
        var statusRequest = new
        {
            Status = BracketStatus.Started
        };

        // Act
        var response = await HttpClient.PatchAsync("/api/brackets/99999/status", CreateJsonContent(statusRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(BracketStatus.Started)]
    [InlineData(BracketStatus.Completed)]
    [InlineData(BracketStatus.OnHold)]
    public async Task ChangeBracketStatus_WithDifferentStatuses_ReturnsSuccess(BracketStatus newStatus)
    {
        // Arrange - Create a bracket first
        var createRequest = new
        {
            Name = "Multi Status Test Tournament",
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        var createResponse = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdBracket = JsonSerializer.Deserialize<BracketResponse>(createContent, DefaultJsonSerializerOptions);

        var statusRequest = new
        {
            Status = newStatus
        };

        // Act
        var response = await HttpClient.PatchAsync($"/api/brackets/{createdBracket!.Id}/status", CreateJsonContent(statusRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private record BracketResponse(
        int Id,
        string Name,
        string Game,
        bool IsLocked,
        BracketType Type,
        BracketStatus Status,
        CompetitionType Competition,
        DateTime StartDate,
        DateTime CreatedDate
    );
}