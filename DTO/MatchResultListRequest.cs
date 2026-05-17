namespace RankServer.DTO;

public class MatchResultListRequest
{
    public List<MatchResultRequest>
        Players
    { get; set; }
        = new();
}