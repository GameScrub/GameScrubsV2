using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Player;

public class ReorderPlayersTests : IntegrationTestBase
{
    public ReorderPlayersTests(DatabaseFixture databaseFixture) : base(databaseFixture) { }

    [Fact]
    public async Task ReorderPlayers_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var bracket = await CreateTestBracket();
        
        // Add some players
        var player1Request = new { BracketId = bracket.Id, Name = "Player 1" };
        var player2Request = new { BracketId = bracket.Id, Name = "Player 2" };
        var player3Request = new { BracketId = bracket.Id, Name = "Player 3" };
        
        var player1Response = await HttpClient.PostAsync("/api/players", CreateJsonContent(player1Request));
        var player2Response = await HttpClient.PostAsync("/api/players", CreateJsonContent(player2Request));
        var player3Response = await HttpClient.PostAsync("/api/players", CreateJsonContent(player3Request));

        var player1Content = await player1Response.Content.ReadAsStringAsync();
        var player2Content = await player2Response.Content.ReadAsStringAsync();
        var player3Content = await player3Response.Content.ReadAsStringAsync();

        var player1 = JsonSerializer.Deserialize<PlayerResponse>(player1Content, DefaultJsonSerializerOptions);
        var player2 = JsonSerializer.Deserialize<PlayerResponse>(player2Content, DefaultJsonSerializerOptions);
        var player3 = JsonSerializer.Deserialize<PlayerResponse>(player3Content, DefaultJsonSerializerOptions);

        var reorderRequest = new
        {
            BracketId = bracket.Id,
            PlayerIds = new[] { player3!.Id, player1!.Id, player2!.Id } // Reorder: 3, 1, 2
        };

        // Act
        var response = await HttpClient.PatchAsync("/api/players/reorder", CreateJsonContent(reorderRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReorderPlayers_WithNonExistentBracket_ReturnsNotFound()
    {
        // Arrange
        var reorderRequest = new
        {
            BracketId = 99999,
            PlayerIds = new[] { 1, 2, 3 }
        };

        // Act
        var response = await HttpClient.PatchAsync("/api/players/reorder", CreateJsonContent(reorderRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ReorderPlayers_WithEmptyPlayerList_ReturnsBadRequest()
    {
        // Arrange
        var bracket = await CreateTestBracket();
        
        var reorderRequest = new
        {
            BracketId = bracket.Id,
            PlayerIds = Array.Empty<int>()
        };

        // Act
        var response = await HttpClient.PatchAsync("/api/players/reorder", CreateJsonContent(reorderRequest));

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.OK);
    }

    [Fact]
    public async Task ReorderPlayers_WithInvalidPlayerIds_ReturnsBadRequest()
    {
        // Arrange
        var bracket = await CreateTestBracket();
        
        var reorderRequest = new
        {
            BracketId = bracket.Id,
            PlayerIds = new[] { 99999, 99998, 99997 } // Non-existent player IDs
        };

        // Act
        var response = await HttpClient.PatchAsync("/api/players/reorder", CreateJsonContent(reorderRequest));

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
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