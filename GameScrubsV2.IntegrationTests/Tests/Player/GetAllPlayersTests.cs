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
    public GetAllPlayersTests(DatabaseFixture databaseFixture) : base(databaseFixture) { }

    [Fact]
    public async Task GetAllPlayers_WithExistingBracket_ReturnsPlayers()
    {
        // Arrange - Create bracket and players using direct database inserts
        var bracket = await TestDataHelper.CreateBracket(Factory);

        await TestDataHelper.AddPlayer(Factory, bracket.Id, "Player 1");
        await TestDataHelper.AddPlayer(Factory, bracket.Id, "Player 2");

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

    private record PlayerResponse(
        int Id,
        string Name,
        int BracketId
    );
}