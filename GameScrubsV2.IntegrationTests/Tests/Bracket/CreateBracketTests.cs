using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Bracket;

public class CreateBracketTests : IntegrationTestBase
{
    public CreateBracketTests(DatabaseFixture databaseFixture) : base(databaseFixture) { }

    [Fact]
    public async Task CreateBracket_WithValidData_ReturnsCreatedBracket()
    {
        // Arrange
        var createRequest = new
        {
            Name = "Test Tournament",
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var bracket = JsonSerializer.Deserialize<BracketResponse>(content, DefaultJsonSerializerOptions);
        
        bracket.Should().NotBeNull();
        bracket!.Name.Should().Be(createRequest.Name);
        bracket.Game.Should().Be(createRequest.Game);
        bracket.Type.Should().Be(createRequest.Type);
        bracket.Status.Should().Be(BracketStatus.Setup);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("ABC")]
    [InlineData("ABCD")]
    public async Task CreateBracket_WithInvalidName_ReturnsBadRequest(string invalidName)
    {
        // Arrange
        var createRequest = new
        {
            Name = invalidName,
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    [InlineData("ABC")]
    [InlineData("ABCD")]
    public async Task CreateBracket_WithInvalidGame_ReturnsBadRequest(string invalidGame)
    {
        // Arrange
        var createRequest = new
        {
            Name = "Valid Tournament Name",
            Game = invalidGame,
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBracket_WithPastStartDate_ReturnsBadRequest()
    {
        // Arrange
        var createRequest = new
        {
            Name = "Test Tournament",
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-1))
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateBracket_WithInvalidNameReturnsValidationError()
    {
        // Arrange
        var createRequest = new
        {
            Name = "A", // Too short - minimum is 5
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("validation"); // Should contain validation error details
    }

    [Fact]
    public async Task CreateBracket_WithLockCode_CreatesLockedBracket()
    {
        // Arrange
        var createRequest = new
        {
            Name = "Private Tournament",
            Game = "Counter-Strike 2",
            LockCode = 1234,
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var content = await response.Content.ReadAsStringAsync();
        var bracket = JsonSerializer.Deserialize<BracketResponse>(content, DefaultJsonSerializerOptions);
        
        bracket.Should().NotBeNull();
        bracket!.IsLocked.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Counter-Strike 2")]
    [InlineData("Test", "Counter-Strike 2")]
    [InlineData("Test Tournament", "")]
    [InlineData("Test Tournament", "CS")]
    public async Task CreateBracket_WithInvalidData_ReturnsBadRequest(string name, string game)
    {
        // Arrange
        var createRequest = new
        {
            Name = name,
            Game = game,
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
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