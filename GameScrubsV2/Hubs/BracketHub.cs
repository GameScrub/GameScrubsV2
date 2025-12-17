using Microsoft.AspNetCore.SignalR;

namespace GameScrubsV2.Hubs;

/// <summary>
/// SignalR hub for real-time bracket updates
/// </summary>
public class BracketHub : Hub
{
	/// <summary>
	/// Join a bracket room to receive updates for a specific bracket
	/// </summary>
	/// <param name="bracketId">The ID of the bracket to subscribe to</param>
	public async Task JoinBracket(string bracketId) =>
		await Groups.AddToGroupAsync(Context.ConnectionId, $"bracket-{bracketId}");

	/// <summary>
	/// Leave a bracket room
	/// </summary>
	/// <param name="bracketId">The ID of the bracket to unsubscribe from</param>
	public async Task LeaveBracket(string bracketId) =>
		await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"bracket-{bracketId}");

	public override async Task OnDisconnectedAsync(Exception? exception) =>
		await base.OnDisconnectedAsync(exception);
}
