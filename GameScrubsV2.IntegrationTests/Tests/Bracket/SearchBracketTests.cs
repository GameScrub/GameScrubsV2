using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Bracket;

public class SearchBracketTests : IntegrationTestBase
{
    public SearchBracketTests(DatabaseFixture databaseFixture) : base(databaseFixture) { }

    [Fact]
    public async Task SearchBracket_ReturnsPagedResults()
    {
        // Arrange - Create some brackets
        for (int i = 1; i <= 3; i++)
        {
            var createRequest = new
            {
                Name = $"Tournament {i}",
                Game = "Counter-Strike 2",
                Type = BracketType.Single_8,
                Competition = CompetitionType.VideoGames,
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
            };
            await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));
        }

        var searchRequest = new
        {
            PageNumber = 1,
            PageSize = 2
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets/search", CreateJsonContent(searchRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SearchResponse>(content, DefaultJsonSerializerOptions);
        
        result.Should().NotBeNull();
        result!.Brackets.Should().HaveCountLessOrEqualTo(2);
        result.TotalCount.Should().BeGreaterOrEqualTo(3);
    }

    [Fact]
    public async Task SearchBracket_WithNameFilter_ReturnsFilteredResults()
    {
        // Arrange - Create brackets with different names
        var createRequest1 = new
        {
            Name = "Chess Tournament",
            Game = "Chess",
            Type = BracketType.Single_8,
            Competition = CompetitionType.BoardGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };
        
        var createRequest2 = new
        {
            Name = "Gaming Tournament",
            Game = "Counter-Strike 2",
            Type = BracketType.Single_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest1));
        await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest2));

        var searchRequest = new
        {
            Name = "Chess"
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets/search", CreateJsonContent(searchRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SearchResponse>(content, DefaultJsonSerializerOptions);
        
        result.Should().NotBeNull();
        result!.Brackets.Should().HaveCount(1);
        result.Brackets[0].Name.Should().Contain("Chess");
    }

    [Fact]
    public async Task SearchBracket_WithEmptyRequest_ReturnsAllBrackets()
    {
        // Arrange
        var searchRequest = new { };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets/search", CreateJsonContent(searchRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<SearchResponse>(content, DefaultJsonSerializerOptions);
        
        result.Should().NotBeNull();
    }

    private record SearchResponse(
        BracketResponse[] Brackets,
        int PageNumber,
        int PageSize,
        int TotalPages,
        int TotalCount,
        bool HasPreviousPage,
        bool HasNextPage
    );

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