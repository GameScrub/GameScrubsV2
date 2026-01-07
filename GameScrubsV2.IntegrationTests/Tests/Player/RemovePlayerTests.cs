using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Player;

public class RemovePlayerTests : IntegrationTestBase
{
    public RemovePlayerTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task RemovePlayer_WithExistingPlayer_ReturnsSuccess()
    {
        // Arrange
        var bracket = await CreateTestBracket();
        
        // Add a player first
        var addPlayerRequest = new { BracketId = bracket.Id, Name = "Player to Remove" };
        var addResponse = await HttpClient.PostAsync("/api/players", CreateJsonContent(addPlayerRequest));
        
        var addContent = await addResponse.Content.ReadAsStringAsync();
        var addedPlayer = JsonSerializer.Deserialize<PlayerResponse>(addContent, DefaultJsonSerializerOptions);

        // Act
        var response = await HttpClient.DeleteAsync($"/api/players/{addedPlayer!.Id}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task RemovePlayer_WithNonExistentPlayer_ReturnsNotFound()
    {
        // Act
        var response = await HttpClient.DeleteAsync("/api/players/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemovePlayer_WithInvalidId_ReturnsBadRequestOrNotFound()
    {
        // Act
        var response = await HttpClient.DeleteAsync("/api/players/invalid");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task RemovePlayer_VerifyPlayerIsDeleted()
    {
        // Arrange
        var bracket = await CreateTestBracket();
        
        // Add a player first
        var addPlayerRequest = new { BracketId = bracket.Id, Name = "Player to Verify Delete" };
        var addResponse = await HttpClient.PostAsync("/api/players", CreateJsonContent(addPlayerRequest));
        
        var addContent = await addResponse.Content.ReadAsStringAsync();
        var addedPlayer = JsonSerializer.Deserialize<PlayerResponse>(addContent, DefaultJsonSerializerOptions);

        // Act - Remove the player
        var deleteResponse = await HttpClient.DeleteAsync($"/api/players/{addedPlayer!.Id}");
        deleteResponse.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        // Assert - Verify player list no longer contains the deleted player
        var getPlayersResponse = await HttpClient.GetAsync($"/api/players?bracketId={bracket.Id}");
        if (getPlayersResponse.StatusCode == HttpStatusCode.OK)
        {
            var playersContent = await getPlayersResponse.Content.ReadAsStringAsync();
            var players = JsonSerializer.Deserialize<PlayerResponse[]>(playersContent, DefaultJsonSerializerOptions);
            
            players.Should().NotContain(p => p.Id == addedPlayer.Id);
        }
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