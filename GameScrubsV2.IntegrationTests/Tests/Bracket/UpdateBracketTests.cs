using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Bracket;

public class UpdateBracketTests : IntegrationTestBase
{
    public UpdateBracketTests(IntegrationTestFactory factory) : base(factory) { }

    [Fact]
    public async Task UpdateBracket_WithValidData_ReturnsSuccess()
    {
        // Arrange - Create a bracket first
        var createRequest = new
        {
            Name = "Original Tournament",
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        var createResponse = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdBracket = JsonSerializer.Deserialize<BracketResponse>(createContent, DefaultJsonSerializerOptions);

        var updateRequest = new
        {
            Name = "Updated Tournament",
            Game = "Counter-Strike 2",
            Type = BracketType.Single_16,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(10))
        };

        // Act
        var response = await HttpClient.PutAsync($"/api/brackets/{createdBracket!.Id}", CreateJsonContent(updateRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var updatedBracket = JsonSerializer.Deserialize<BracketResponse>(content, DefaultJsonSerializerOptions);
        
        updatedBracket.Should().NotBeNull();
        updatedBracket!.Name.Should().Be(updateRequest.Name);
        updatedBracket.Type.Should().Be(updateRequest.Type);
    }

    [Fact]
    public async Task UpdateBracket_WithNonExistentBracket_ReturnsNotFound()
    {
        // Arrange
        var updateRequest = new
        {
            Name = "Updated Tournament",
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        // Act
        var response = await HttpClient.PutAsync("/api/brackets/99999", CreateJsonContent(updateRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData("", "Counter-Strike 2")]
    [InlineData("Updated Tournament", "")]
    public async Task UpdateBracket_WithInvalidData_ReturnsBadRequest(string name, string game)
    {
        // Arrange - Create a bracket first
        var createRequest = new
        {
            Name = "Original Tournament",
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        var createResponse = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));
        var createContent = await createResponse.Content.ReadAsStringAsync();
        var createdBracket = JsonSerializer.Deserialize<BracketResponse>(createContent, DefaultJsonSerializerOptions);

        var updateRequest = new
        {
            Name = name,
            Game = game,
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        // Act
        var response = await HttpClient.PutAsync($"/api/brackets/{createdBracket!.Id}", CreateJsonContent(updateRequest));

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