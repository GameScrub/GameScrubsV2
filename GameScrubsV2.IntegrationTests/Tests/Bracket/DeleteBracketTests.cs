using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Bracket;

public class DeleteBracketTests : IntegrationTestBase
{
    public DeleteBracketTests(DatabaseFixture databaseFixture) : base(databaseFixture) { }

    [Fact]
    public async Task DeleteBracket_WithExistingBracket_ReturnsSuccess()
    {
        // Arrange - Create a bracket first
        var createRequest = new
        {
            Name = "Tournament to Delete",
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        var createResponse = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdBracket = JsonSerializer.Deserialize<BracketResponse>(createContent, DefaultJsonSerializerOptions);

        // Act
        var response = await HttpClient.DeleteAsync($"/api/brackets/{createdBracket!.Id}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        // Verify bracket is deleted
        var getResponse = await HttpClient.GetAsync($"/api/brackets/{createdBracket.Id}");
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBracket_WithNonExistentBracket_ReturnsNotFound()
    {
        // Act
        var response = await HttpClient.DeleteAsync("/api/brackets/99999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteBracket_WithInvalidId_ReturnsBadRequestOrNotFound()
    {
        // Act
        var response = await HttpClient.DeleteAsync("/api/brackets/invalid");

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