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
    public AddPlayerTests(DatabaseFixture databaseFixture) : base(databaseFixture) { }

    [Fact]
    public async Task AddPlayer_WithValidData_ReturnsSuccess()
    {
        // Arrange - Create bracket using direct database insert
        var bracket = await TestDataHelper.CreateBracket(Factory);

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
        // Arrange - Create bracket using direct database insert
        var bracket = await TestDataHelper.CreateBracket(Factory);

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

}