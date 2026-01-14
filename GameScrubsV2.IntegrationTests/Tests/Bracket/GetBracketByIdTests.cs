using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Bracket;

public class GetBracketByIdTests : IntegrationTestBase
{
    public GetBracketByIdTests(DatabaseFixture databaseFixture) : base(databaseFixture) { }

    [Fact]
    public async Task GetBracketById_WithExistingBracket_ReturnsBracket()
    {
        // Arrange - Create a bracket first
        var createRequest = new
        {
            Name = "Test Tournament",
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        var createResponse = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdBracket = JsonSerializer.Deserialize<BracketResponse>(createContent, DefaultJsonSerializerOptions);

        // Act
        var response = await HttpClient.GetAsync($"/api/brackets/{createdBracket!.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var bracket = JsonSerializer.Deserialize<BracketResponse>(content, DefaultJsonSerializerOptions);
        
        bracket.Should().NotBeNull();
        bracket!.Id.Should().Be(createdBracket.Id);
        bracket.Name.Should().Be(createRequest.Name);
        bracket.Game.Should().Be(createRequest.Game);
    }

    [Fact]
    public async Task GetBracketById_WithNonExistentBracket_ReturnsNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/brackets/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("invalid")]
    public async Task GetBracketById_WithInvalidId_ReturnsBadRequestOrNotFound(string id)
    {
        // Act
        var response = await HttpClient.GetAsync($"/api/brackets/{id}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.NotFound);
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