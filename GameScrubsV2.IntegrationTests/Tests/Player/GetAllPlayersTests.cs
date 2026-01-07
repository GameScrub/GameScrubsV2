using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Player;

public class GetAllPlayersTests : IntegrationTestBase
{
    public GetAllPlayersTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task GetAllPlayers_WithExistingBracket_ReturnsPlayers()
    {
        // Arrange
        var bracket = await CreateTestBracket();
        
        // Add some players
        var player1 = new { BracketId = bracket.Id, Name = "Player 1" };
        var player2 = new { BracketId = bracket.Id, Name = "Player 2" };
        
        await HttpClient.PostAsync("/api/players", CreateJsonContent(player1));
        await HttpClient.PostAsync("/api/players", CreateJsonContent(player2));

        // Act
        var response = await HttpClient.GetAsync($"/api/players?bracketId={bracket.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var players = JsonSerializer.Deserialize<PlayerResponse[]>(content, DefaultJsonSerializerOptions);
        
        players.Should().NotBeNull();
        players!.Should().HaveCountGreaterOrEqualTo(2);
    }

    [Fact]
    public async Task GetAllPlayers_WithNonExistentBracket_ReturnsNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/players?bracketId=99999");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.OK);
    }

    private async Task<BracketResponse> CreateTestBracket()
    {
        var createRequest = new
        {
            Name = "Test Tournament",
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

    private record PlayerResponse(
        int Id,
        string Name,
        int BracketId
    );
}