namespace RankServer.DTO;

public class MatchResultRequest
{
    public string TikTokUserId { get; set; }

    public string Username { get; set; }

    public string Avatar { get; set; }

    public int MatchPoint { get; set; }

    public int KillCount { get; set; }
}