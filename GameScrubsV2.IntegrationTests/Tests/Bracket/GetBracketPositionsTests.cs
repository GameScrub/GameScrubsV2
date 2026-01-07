using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Bracket;

public class GetBracketPositionsTests : IntegrationTestBase
{
    public GetBracketPositionsTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task GetBracketPositions_WithExistingBracket_ReturnsPositions()
    {
        // Arrange - Create a bracket first
        var createRequest = new
        {
            Name = "Positions Test Tournament",
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        var createResponse = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdBracket = JsonSerializer.Deserialize<BracketResponse>(createContent, DefaultJsonSerializerOptions);

        // Act
        var response = await HttpClient.GetAsync($"/api/brackets/{createdBracket!.Id}/positions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var positions = JsonSerializer.Deserialize<PositionResponse[]>(content, DefaultJsonSerializerOptions);
        
        positions.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBracketPositions_WithNonExistentBracket_ReturnsNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/brackets/99999/positions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetBracketPositions_WithInvalidId_ReturnsBadRequestOrNotFound()
    {
        // Act
        var response = await HttpClient.GetAsync("/api/brackets/invalid/positions");

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

    private record PositionResponse(
        int Id,
        int BracketId,
        int Position,
        string? PlayerName
    );
}