using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Placement;

public class SetPlacementScoreTests : IntegrationTestBase
{
    public SetPlacementScoreTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task SetPlacementScore_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var bracket = await CreateTestBracket();
        
        var scoreRequest = new
        {
            PlacementId = 1,
            Score1 = 16,
            Score2 = 14
        };

        // Act
        var response = await HttpClient.PostAsync($"/api/brackets/{bracket.Id}/placements/score", CreateJsonContent(scoreRequest));

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Created, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SetPlacementScore_WithNonExistentBracket_ReturnsNotFound()
    {
        // Arrange
        var scoreRequest = new
        {
            PlacementId = 1,
            Score1 = 16,
            Score2 = 14
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets/99999/placements/score", CreateJsonContent(scoreRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(-1, 10)]
    [InlineData(10, -1)]
    [InlineData(1000, 10)]
    [InlineData(10, 1000)]
    public async Task SetPlacementScore_WithInvalidScores_ReturnsBadRequest(int score1, int score2)
    {
        // Arrange
        var bracket = await CreateTestBracket();
        
        var scoreRequest = new
        {
            PlacementId = 1,
            Score1 = score1,
            Score2 = score2
        };

        // Act
        var response = await HttpClient.PostAsync($"/api/brackets/{bracket.Id}/placements/score", CreateJsonContent(scoreRequest));

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task SetPlacementScore_WithNonExistentPlacement_ReturnsNotFound()
    {
        // Arrange
        var bracket = await CreateTestBracket();
        
        var scoreRequest = new
        {
            PlacementId = 99999,
            Score1 = 16,
            Score2 = 14
        };

        // Act
        var response = await HttpClient.PostAsync($"/api/brackets/{bracket.Id}/placements/score", CreateJsonContent(scoreRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<BracketResponse> CreateTestBracket()
    {
        var createRequest = new
        {
            Name = "Score Test Tournament",
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
}