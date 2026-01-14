using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Placement;

public class GetBracketPlacementTests : IntegrationTestBase
{
    public GetBracketPlacementTests(DatabaseFixture databaseFixture) : base(databaseFixture) { }

    [Fact]
    public async Task GetBracketPlacement_WithExistingBracket_ReturnsPlacements()
    {
        // Arrange
        var bracket = await CreateTestBracket();

        // Act
        var response = await HttpClient.GetAsync($"/api/brackets/{bracket.Id}/placements");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var placements = JsonSerializer.Deserialize<PlacementResponse[]>(content, DefaultJsonSerializerOptions);
        
        placements.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBracketPlacement_WithNonExistentBracket_ReturnsNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/brackets/99999/placements");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBracketPlacement_WithInvalidBracketId_ReturnsBadRequestOrNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/brackets/invalid/placements");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    private async Task<BracketResponse> CreateTestBracket()
    {
        var createRequest = new
        {
            Name = "Placement Test Tournament",
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        var response = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<BracketResponse>(content, DefaultJsonSerializerOptions)!;
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

    private record PlacementResponse(
        int Id,
        int BracketId,
        string? PlayerName,
        int Position
    );
}