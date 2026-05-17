namespace RankServer.Models;

public class RankSeason
{
    public long Id { get; set; }

    public string Name { get; set; }

    public string Type { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsActive { get; set; }
}