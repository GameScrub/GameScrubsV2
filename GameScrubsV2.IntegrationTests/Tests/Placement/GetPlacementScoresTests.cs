using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Placement;

public class GetPlacementScoresTests : IntegrationTestBase
{
    public GetPlacementScoresTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task GetPlacementScores_WithExistingBracket_ReturnsScores()
    {
        // Arrange
        var bracket = await CreateTestBracket();

        // Act
        var response = await HttpClient.GetAsync($"/api/brackets/{bracket.Id}/placements/scores");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var scores = JsonSerializer.Deserialize<ScoreResponse[]>(content, DefaultJsonSerializerOptions);
        
        scores.Should().NotBeNull();
    }

    [Fact]
    public async Task GetPlacementScores_WithNonExistentBracket_ReturnsNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/brackets/99999/placements/scores");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPlacementScores_WithInvalidBracketId_ReturnsBadRequestOrNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/brackets/invalid/placements/scores");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetPlacementScores_WithSpecificPlacement_ReturnsPlacementScores()
    {
        // Arrange
        var bracket = await CreateTestBracket();

        // Act
        var response = await HttpClient.GetAsync($"/api/brackets/{bracket.Id}/placements/1/scores");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound);
        
        if (response.StatusCode == HttpStatusCode.OK)
        {
            var content = await response.Content.ReadAsStringAsync();
            var scores = JsonSerializer.Deserialize<ScoreResponse[]>(content, DefaultJsonSerializerOptions);
            scores.Should().NotBeNull();
        }
    }

    private async Task<BracketResponse> CreateTestBracket()
    {
        var createRequest = new
        {
            Name = "Scores Test Tournament",
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

    private record ScoreResponse(
        int Id,
        int PlacementId,
        int Score1,
        int Score2,
        DateTime CreatedAt
    );
}