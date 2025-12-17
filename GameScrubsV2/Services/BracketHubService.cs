using GameScrubsV2.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace GameScrubsV2.Services;


/// <summary>
/// Service for broadcasting bracket updates via SignalR
/// </summary>
public interface IBracketHubService
{
	/// <summary>
	/// Notify all clients in a bracket group that a match score was updated
	/// </summary>
	/// <param name="bracketId">The ID of the bracket</param>
	/// <param name="matchId">The ID of the match</param>
	Task NotifyMatchScoreUpdated(int bracketId, int matchId);

	/// <summary>
	/// Notify all clients in a bracket group that the bracket status changed
	/// </summary>
	/// <param name="bracketId">The ID of the bracket</param>
	/// <param name="newStatus">The new status</param>
	Task NotifyBracketStatusChanged(int bracketId, string newStatus);
}

/// <summary>
/// Service for broadcasting bracket updates via SignalR
/// </summary>
public class BracketHubService : IBracketHubService
{
	private readonly IHubContext<BracketHub> _hubContext;
	private readonly ILogger<BracketHubService> _logger;

	public BracketHubService(IHubContext<BracketHub> hubContext, ILogger<BracketHubService> logger)
	{
		_hubContext = hubContext;
		_logger = logger;
	}

	///<inheritdoc />
	public async Task NotifyMatchScoreUpdated(int bracketId, int matchId)
	{
		_logger.LogDebug("Broadcasting match score updated for bracket {BracketId}, match {MatchId}", bracketId, matchId);
		await _hubContext.Clients.Group($"bracket-{bracketId}").SendAsync("MatchScoreUpdated", new { bracketId, matchId });
	}

	///<inheritdoc />
	public async Task NotifyBracketStatusChanged(int bracketId, string newStatus)
	{
		_logger.LogDebug("Broadcasting bracket status changed for bracket {BracketId} to {NewStatus}", bracketId, newStatus);
		await _hubContext.Clients.Group($"bracket-{bracketId}").SendAsync("BracketStatusChanged", new { bracketId, newStatus });
	}
}
