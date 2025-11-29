namespace GameScrubsV2.Models;

public partial class BracketScore
{
	public int Id { get; set; }

	public int Type { get; set; }

	public string? BracketPlace { get; set; }

	public int Score { get; set; }
}