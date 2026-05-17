namespace RankServer.Models;

public class RankHistory
{
    public long Id { get; set; }

    public long UserId { get; set; }

    public long SeasonId { get; set; }

    public int RankPosition { get; set; }

    public string RankType { get; set; }

    public long Score { get; set; }

    public DateTime CreatedAt { get; set; }
}