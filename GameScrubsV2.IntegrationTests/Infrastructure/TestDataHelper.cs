using GameScrubsV2.Enums;
using GameScrubsV2.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GameScrubsV2.IntegrationTests.Infrastructure;

public static class TestDataHelper
{
    public static async Task<Bracket> CreateBracket(
        IntegrationTestFactory factory,
        string? name = null,
        string? game = null,
        BracketType? type = null,
        CompetitionType? competition = null,
        DateTime? startDate = null,
        BracketStatus? status = null,
        int? lockCode = null,
        bool? isLocked = null)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameScrubsV2DbContext>();

        var bracket = new Bracket
        {
            Name = name ?? "Test Tournament",
            Game = game ?? "Test Game",
            Type = type ?? BracketType.Single_8,
            Competition = competition ?? CompetitionType.VideoGames,
            StartDate = startDate ?? DateTime.UtcNow.AddDays(7),
            Status = status ?? BracketStatus.Setup,
            CreatedDate = DateTime.UtcNow,
            LockCode = lockCode,
            IsLocked = isLocked ?? (lockCode != null)
        };

        dbContext.Brackets.Add(bracket);
        await dbContext.SaveChangesAsync();

        return bracket;
    }

    public static async Task<PlayerList> AddPlayer(
        IntegrationTestFactory factory,
        int bracketId,
        string? playerName = null,
        int? seed = null)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameScrubsV2DbContext>();

        var player = new PlayerList
        {
            BracketId = bracketId,
            PlayerName = playerName ?? "Test Player",
            Seed = seed ?? 0
        };

        dbContext.PlayerLists.Add(player);
        await dbContext.SaveChangesAsync();

        return player;
    }

    public static async Task<List<PlayerList>> AddPlayers(
        IntegrationTestFactory factory,
        int bracketId,
        int count,
        string? namePrefix = null)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameScrubsV2DbContext>();

        var players = new List<PlayerList>();
        var prefix = namePrefix ?? "Player";

        for (int i = 0; i < count; i++)
        {
            var player = new PlayerList
            {
                BracketId = bracketId,
                PlayerName = $"{prefix} {i + 1}",
                Seed = i
            };

            players.Add(player);
        }

        dbContext.PlayerLists.AddRange(players);
        await dbContext.SaveChangesAsync();

        return players;
    }

    public static async Task<Placement> CreatePlacement(
        IntegrationTestFactory factory,
        int bracketId,
        PlacementStatus? status = null,
        string? playerName = null)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameScrubsV2DbContext>();

        var placement = new Placement
        {
            BracketId = bracketId,
            Status = status ?? PlacementStatus.Default,
            PlayerName = playerName ?? "Test Player"
        };

        dbContext.Placements.Add(placement);
        await dbContext.SaveChangesAsync();

        return placement;
    }

    public static async Task CleanupBracket(IntegrationTestFactory factory, int bracketId)
    {
        using var scope = factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameScrubsV2DbContext>();

        // Remove related data first
        var placements = dbContext.Placements.Where(placement => placement.BracketId == bracketId);
        dbContext.Placements.RemoveRange(placements);

        var players = dbContext.PlayerLists.Where(player => player.BracketId == bracketId);
        dbContext.PlayerLists.RemoveRange(players);

        // Remove the bracket
        var bracket = await dbContext.Brackets.FindAsync(bracketId);
        if (bracket != null)
        {
            dbContext.Brackets.Remove(bracket);
        }

        await dbContext.SaveChangesAsync();
    }

}