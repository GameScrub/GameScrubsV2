using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Player;

public class AddPlayerTests : IntegrationTestBase
{
    public AddPlayerTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task AddPlayer_WithValidData_ReturnsSuccess()
    {
        // Arrange
        var bracket = await CreateTestBracket();

        var addPlayerRequest = new
        {
            BracketId = bracket.Id,
            Name = "Test Player"
        };

        // Act
        var response = await HttpClient.PostAsync("/api/players", CreateJsonContent(addPlayerRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AddPlayer_WithNonExistentBracket_ReturnsNotFound()
    {
        // Arrange
        var addPlayerRequest = new
        {
            BracketId = 99999,
            Name = "Test Player"
        };

        // Act
        var response = await HttpClient.PostAsync("/api/players", CreateJsonContent(addPlayerRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("ThisIsAVeryLongPlayerNameThatExceedsTheMaximumLengthAllowed")]
    public async Task AddPlayer_WithInvalidName_ReturnsBadRequest(string playerName)
    {
        // Arrange
        var bracket = await CreateTestBracket();

        var addPlayerRequest = new
        {
            BracketId = bracket.Id,
            Name = playerName
        };

        // Act
        var response = await HttpClient.PostAsync("/api/players", CreateJsonContent(addPlayerRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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

        var response = await HttpClient.PostAsync("/api/bracket", CreateJsonContent(createRequest));
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