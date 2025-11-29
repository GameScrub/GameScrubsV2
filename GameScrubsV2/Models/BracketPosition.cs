namespace GameScrubsV2.Models;

public partial class BracketPosition
{
	public int Id { get; set; }

	public int Type { get; set; }

	public string? Player1 { get; set; }

	public string? Player2 { get; set; }

	public string? WinLocation { get; set; }

	public string? Loselocation { get; set; }
}