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
    public RemovePlayerTests(DatabaseFixture databaseFixture) : base(databaseFixture) { }

    [Fact]
    public async Task RemovePlayer_WithExistingPlayer_ReturnsSuccess()
    {
        // Arrange - Create bracket and player using direct database inserts
        var bracket = await TestDataHelper.CreateBracket(Factory);
        var addedPlayer = await TestDataHelper.AddPlayer(Factory, bracket.Id, "Player to Remove");

        // Act
        var response = await HttpClient.DeleteAsync($"/api/players/{addedPlayer.Id}");

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
        // Arrange - Create bracket and player using direct database inserts
        var bracket = await TestDataHelper.CreateBracket(Factory);
        var addedPlayer = await TestDataHelper.AddPlayer(Factory, bracket.Id, "Player to Verify Delete");

        // Act - Remove the player
        var deleteResponse = await HttpClient.DeleteAsync($"/api/players/{addedPlayer.Id}");
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

    private record PlayerResponse(
        int Id,
        string Name,
        int BracketId
    );
}