using System.Net;
using System.Text.Json;
using FluentAssertions;
using GameScrubsV2.Enums;
using GameScrubsV2.IntegrationTests.Infrastructure;
using GameScrubsV2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using static GameScrubsV2.Common.Json.SerializerOptions;

namespace GameScrubsV2.IntegrationTests.Tests.Placement;

/// <summary>
/// Grand final reset for Double Elimination 8 (grand finals w15,w16→w17, reset r1,r2→w17).
/// </summary>
public class GrandFinalResetTests : IntegrationTestBase
{
    private const string WinnersBracketPlayer = "Winners Champ";
    private const string LosersBracketPlayer = "Losers Champ";

    public GrandFinalResetTests(DatabaseFixture databaseFixture) : base(databaseFixture) { }

    [Fact]
    public async Task CreateBracket_WithoutGrandFinalReset_DefaultsToOn()
    {
        // Arrange
        var createRequest = new
        {
            Name = "Reset Default Tournament",
            Game = "Street Fighter 6",
            Type = BracketType.Double_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7))
        };

        // Act
        var response = await HttpClient.PostAsync("/api/brackets", CreateJsonContent(createRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var content = await response.Content.ReadAsStringAsync();
        var bracket = JsonSerializer.Deserialize<BracketResponse>(content, DefaultJsonSerializerOptions)!;
        bracket.GrandFinalReset.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateBracket_ChangingGrandFinalResetAfterStart_ReturnsBadRequest()
    {
        // Arrange
        var bracket = await CreateStartedBracket(grandFinalReset: true);

        var updateRequest = new
        {
            bracket.Id,
            Name = "Reset Update Tournament",
            Game = "Street Fighter 6",
            Type = BracketType.Double_8,
            Competition = CompetitionType.VideoGames,
            StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(7)),
            GrandFinalReset = false
        };

        // Act
        var response = await HttpClient.PutAsync("/api/brackets", CreateJsonContent(updateRequest));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        (await GetBracket(bracket.Id)).GrandFinalReset.Should().BeTrue();
    }

    [Fact]
    public async Task GrandFinals_WinnersBracketPlayerWins_IsChampionWithoutReset()
    {
        // Arrange
        var bracket = await CreateStartedBracket(grandFinalReset: true);
        var (winnersPlayer, _) = await CreateGrandFinals(bracket.Id);

        // Act
        var response = await SetWinner(bracket.Id, winnersPlayer.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var placements = await GetPlacements(bracket.Id);
        placements.Should().ContainSingle(placement => placement.BracketPlace == "w17")
            .Which.Should().Match<Models.Placement>(placement => placement.PlayerName == WinnersBracketPlayer && placement.Score == 1);
        placements.Should().NotContain(placement => placement.BracketPlace == "r1" || placement.BracketPlace == "r2");
    }

    [Fact]
    public async Task GrandFinals_LosersBracketPlayerWins_PlaysReset()
    {
        // Arrange
        var bracket = await CreateStartedBracket(grandFinalReset: true);
        var (winnersPlayer, losersPlayer) = await CreateGrandFinals(bracket.Id);

        // Act
        var response = await SetWinner(bracket.Id, losersPlayer.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var placements = await GetPlacements(bracket.Id);
        placements.Should().NotContain(placement => placement.BracketPlace == "w17");
        placements.Single(placement => placement.BracketPlace == "r1")
            .Should().Match<Models.Placement>(placement => placement.PlayerName == WinnersBracketPlayer && placement.Score == 2);
        placements.Single(placement => placement.BracketPlace == "r2")
            .Should().Match<Models.Placement>(placement => placement.PlayerName == LosersBracketPlayer && placement.Score == 2);
        placements.Single(placement => placement.Id == winnersPlayer.Id).Status.Should().Be(PlacementStatus.Loser);
        placements.Single(placement => placement.Id == losersPlayer.Id).Status.Should().Be(PlacementStatus.Winner);
    }

    [Fact]
    public async Task Reset_Winner_IsChampion()
    {
        // Arrange
        var bracket = await CreateStartedBracket(grandFinalReset: true);
        var (_, losersPlayer) = await CreateGrandFinals(bracket.Id);
        await SetWinner(bracket.Id, losersPlayer.Id);
        var resetWinnersPlayer = (await GetPlacements(bracket.Id)).Single(placement => placement.BracketPlace == "r1");

        // Act
        var response = await SetWinner(bracket.Id, resetWinnersPlayer.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var placements = await GetPlacements(bracket.Id);
        placements.Should().ContainSingle(placement => placement.BracketPlace == "w17")
            .Which.Should().Match<Models.Placement>(placement => placement.PlayerName == WinnersBracketPlayer
                                                                 && placement.Score == 1
                                                                 && placement.PreviousBracketPlace == "r1");
    }

    [Fact]
    public async Task GrandFinals_ChangedAfterReset_ClearsReset()
    {
        // Arrange
        var bracket = await CreateStartedBracket(grandFinalReset: true);
        var (winnersPlayer, losersPlayer) = await CreateGrandFinals(bracket.Id);
        await SetWinner(bracket.Id, losersPlayer.Id);
        var resetLosersPlayer = (await GetPlacements(bracket.Id)).Single(placement => placement.BracketPlace == "r2");
        await SetWinner(bracket.Id, resetLosersPlayer.Id);

        // Act
        var response = await SetWinner(bracket.Id, winnersPlayer.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var placements = await GetPlacements(bracket.Id);
        placements.Should().NotContain(placement => placement.BracketPlace == "r1" || placement.BracketPlace == "r2");
        placements.Should().ContainSingle(placement => placement.BracketPlace == "w17")
            .Which.PlayerName.Should().Be(WinnersBracketPlayer);
        placements.Single(placement => placement.Id == losersPlayer.Id).Status.Should().Be(PlacementStatus.Loser);
    }

    [Fact]
    public async Task GrandFinals_WithoutReset_LosersBracketPlayerIsChampion()
    {
        // Arrange
        var bracket = await CreateStartedBracket(grandFinalReset: false);
        var (_, losersPlayer) = await CreateGrandFinals(bracket.Id);

        // Act
        var response = await SetWinner(bracket.Id, losersPlayer.Id);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var placements = await GetPlacements(bracket.Id);
        placements.Should().ContainSingle(placement => placement.BracketPlace == "w17")
            .Which.PlayerName.Should().Be(LosersBracketPlayer);
        placements.Should().NotContain(placement => placement.BracketPlace == "r1" || placement.BracketPlace == "r2");
    }

    private async Task<Models.Bracket> CreateStartedBracket(bool grandFinalReset)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameScrubsV2DbContext>();

        var bracket = new Models.Bracket
        {
            Name = "Grand Final Reset Tournament",
            Game = "Street Fighter 6",
            Type = BracketType.Double_8,
            Competition = CompetitionType.VideoGames,
            Status = BracketStatus.Started,
            GrandFinalReset = grandFinalReset,
            StartDate = DateTime.UtcNow.AddDays(7),
            CreatedDate = DateTime.UtcNow
        };

        dbContext.Brackets.Add(bracket);
        await dbContext.SaveChangesAsync();

        return bracket;
    }

    private async Task<(Models.Placement WinnersPlayer, Models.Placement LosersPlayer)> CreateGrandFinals(int bracketId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameScrubsV2DbContext>();

        var winnersPlayer = new Models.Placement
        {
            BracketId = bracketId,
            PlayerName = WinnersBracketPlayer,
            BracketPlace = "w15",
            Score = 2,
            Status = PlacementStatus.Default
        };

        var losersPlayer = new Models.Placement
        {
            BracketId = bracketId,
            PlayerName = LosersBracketPlayer,
            BracketPlace = "w16",
            Score = 2,
            Status = PlacementStatus.Default
        };

        dbContext.Placements.AddRange(winnersPlayer, losersPlayer);
        await dbContext.SaveChangesAsync();

        return (winnersPlayer, losersPlayer);
    }

    private Task<HttpResponseMessage> SetWinner(int bracketId, int placementId)
        => HttpClient.PostAsync($"/api/brackets/{bracketId}/placements/score/{placementId}", null);

    private async Task<List<Models.Placement>> GetPlacements(int bracketId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameScrubsV2DbContext>();

        return await dbContext.Placements
            .AsNoTracking()
            .Where(placement => placement.BracketId == bracketId)
            .ToListAsync();
    }

    private async Task<Models.Bracket> GetBracket(int bracketId)
    {
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameScrubsV2DbContext>();

        return await dbContext.Brackets.AsNoTracking().SingleAsync(bracket => bracket.Id == bracketId);
    }

    private record BracketResponse(int Id, BracketType Type, bool GrandFinalReset);
}
