namespace GameScrubsV2.Models;

public partial class PlayerList
{
	public int Id { get; set; }

	public int BracketId { get; set; }

	public string? PlayerName { get; set; }

	public int Seed { get; set; }

	public int Score { get; set; }

	public virtual Bracket Bracket { get; set; } = null!;
}