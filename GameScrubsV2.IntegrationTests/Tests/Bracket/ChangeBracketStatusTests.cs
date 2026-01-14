using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;

using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Bracket;

public class ChangeBracketStatusTests : IntegrationTestBase
{
    public ChangeBracketStatusTests(DatabaseFixture databaseFixture) : base(databaseFixture) { }

    [Fact]
    public async Task ChangeBracketStatus_WithValidStatus_ReturnsSuccess()
    {
        // Arrange - Create a bracket using direct database insert
        var createdBracket = await TestDataHelper.CreateBracket(
            Factory,
            name: "Status Test Tournament",
            game: "Counter-Strike 2",
            type: BracketType.Single_8,
            competition: CompetitionType.VideoGames,
            startDate: DateTime.UtcNow.AddDays(7)
        );

        // Add players to the bracket
        await TestDataHelper.AddPlayers(Factory, createdBracket.Id, 7);

        var statusRequest = new
        {
            BracketId = createdBracket.Id,
            Status = BracketStatus.Started
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets/change-status", CreateJsonContent(statusRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify status was changed
        var getResponse = await HttpClient.GetAsync($"/api/brackets/{createdBracket.Id}");
        var getContent = await getResponse.Content.ReadAsStringAsync();
        var updatedBracket = JsonSerializer.Deserialize<BracketResponse>(getContent, DefaultJsonSerializerOptions);

        updatedBracket!.Status.Should().Be(BracketStatus.Started);
    }

    [Fact]
    public async Task ChangeBracketStatus_WithNonExistentBracket_ReturnsNotFound()
    {
        // Arrange
        var statusRequest = new
        {
	        BracketId = -1,
	        Status = BracketStatus.Started
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets/change-status", CreateJsonContent(statusRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ChangeBracketStatus_ToSetup_RemovesAllPlacements()
    {
        // Arrange - Create a bracket and add some existing placements
        var bracket = await TestDataHelper.CreateBracket(Factory, status: BracketStatus.Started);
        await TestDataHelper.AddPlayers(Factory, bracket.Id, 4);

        // Add some placements manually
        await TestDataHelper.CreatePlacement(Factory, bracket.Id, playerName: "Test Player 1");
        await TestDataHelper.CreatePlacement(Factory, bracket.Id, playerName: "Test Player 2");

        var statusRequest = new
        {
            BracketId = bracket.Id,
            Status = BracketStatus.Setup
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets/change-status", CreateJsonContent(statusRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify placements were removed
        var dbContext = await GetDbContext();
        var placements = dbContext.Placements.Where(placement => placement.BracketId == bracket.Id).ToList();
        placements.Should().BeEmpty();
    }

    [Fact]
    public async Task ChangeBracketStatus_ToStarted_CreatesPlacementsForAllPlayers()
    {
        // Arrange - Create bracket with players
        var bracket = await TestDataHelper.CreateBracket(Factory, type: BracketType.Single_8);
        await TestDataHelper.AddPlayers(Factory, bracket.Id, 8);

        var statusRequest = new
        {
            BracketId = bracket.Id,
            Status = BracketStatus.Started
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets/change-status", CreateJsonContent(statusRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify placements were created
        var dbContext = await GetDbContext();
        var placements = dbContext.Placements.Where(placement => placement.BracketId == bracket.Id).ToList();

        // Should have 8 placements for Single_8 bracket
        placements.Should().HaveCount(8);

        // Verify placements have correct data
        foreach (var placement in placements)
        {
            placement.BracketId.Should().Be(bracket.Id);
            placement.Status.Should().Be(PlacementStatus.Default);
            placement.BracketPlace.Should().StartWith("w"); // Winner's bracket positions
        }
    }

    [Theory]
    [InlineData(BracketStatus.OnHold)]
    [InlineData(BracketStatus.Completed)]
    public async Task ChangeBracketStatus_ToOnHoldOrCompleted_UpdatesStatusOnly(BracketStatus newStatus)
    {
        // Arrange - Create a bracket with existing placements
        var bracket = await TestDataHelper.CreateBracket(Factory, status: BracketStatus.Started);
        await TestDataHelper.AddPlayers(Factory, bracket.Id, 4);
        await TestDataHelper.CreatePlacement(Factory, bracket.Id, playerName: "Test Player");

        var statusRequest = new
        {
            BracketId = bracket.Id,
            Status = newStatus
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets/change-status", CreateJsonContent(statusRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify status was changed but placements remain
        var dbContext = await GetDbContext();
        var updatedBracket = await dbContext.Brackets.FindAsync(bracket.Id);
        updatedBracket!.Status.Should().Be(newStatus);

        var placements = dbContext.Placements.Where(placement => placement.BracketId == bracket.Id).ToList();
        placements.Should().NotBeEmpty(); // Placements should remain unchanged
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