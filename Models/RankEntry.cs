namespace RankServer.Models;

public class RankEntry
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long SeasonId { get; set; }

    public long WarPoints { get; set; }

    public long TotalKills { get; set; }
}