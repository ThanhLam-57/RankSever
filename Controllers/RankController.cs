using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RankServer.Data;
using RankServer.DTO;
using RankServer.Models;

namespace RankServer.Controllers;

[ApiController]
[Route("api/rank")]
public class RankController : ControllerBase
{
    private readonly AppDbContext _db;

    public RankController(AppDbContext db)
    {
        _db = db;
    }

    //------------------------------------
    // TỰ TẠO SEASON
    //------------------------------------

    private async Task<RankSeason>
    GetOrCreateSeason(string type)
    {
        var season =
        await _db.RankSeasons
        .FirstOrDefaultAsync(
        x =>
        x.Type == type
        &&
        x.IsActive);

        if (season != null)
            return season;


        var now = DateTime.UtcNow;

        season =
        new RankSeason
        {
            Name =
            type == "WEEK"
            ?
            $"WEEK_{now:yyyyMMdd}"
            :
            $"MONTH_{now:yyyyMM}",

            Type = type,

            StartDate = now,

            EndDate =
            type == "WEEK"
            ?
            now.AddDays(7)
            :
            now.AddMonths(1),

            IsActive = true
        };

        _db.RankSeasons.Add(season);

        await _db.SaveChangesAsync();

        return season;
    }


    //------------------------------------
    // POST UPDATE
    //------------------------------------

    [HttpPost("update")]
    public async Task<IActionResult>
    Update(
    MatchResultListRequest req)
    {
        try
        {
            var week =
            await GetOrCreateSeason(
            "WEEK");

            var month =
            await GetOrCreateSeason(
            "MONTH");


            foreach (
            var p
            in req.Players)
            {
                var user =
                await _db.Users
                .FirstOrDefaultAsync(
                x =>
                x.TiktokUserId
                ==
                p.TikTokUserId);


                if (user == null)
                {
                    user =
                    new User
                    {
                        TiktokUserId =
                        p.TikTokUserId,

                        Nickname =
                        p.Username,

                        AvatarUrl =
                        p.Avatar
                    };

                    _db.Users.Add(user);

                    await _db.SaveChangesAsync();
                }
                else
                {
                    user.Nickname =
                    p.Username;

                    user.AvatarUrl =
                    p.Avatar;
                }


                await AddPoint(
                    user.Id,
                    week.Id,
                    p);


                await AddPoint(
                    user.Id,
                    month.Id,
                    p);
            }


            return Ok(
            new
            {
                success = true,

                total =
                req.Players.Count
            });
        }

        catch (Exception ex)
        {
            return StatusCode(
                500,
                ex.Message);
        }
    }



    //------------------------------------
    // CỘNG ĐIỂM
    //------------------------------------

    private async Task AddPoint(
    long userId,
    long seasonId,
    MatchResultRequest req)
    {
        var rank =
        await _db.RankEntries
        .FirstOrDefaultAsync(
        x =>
        x.UserId == userId
        &&
        x.SeasonId == seasonId);


        if (rank == null)
        {
            rank =
            new RankEntry
            {
                UserId = userId,

                SeasonId = seasonId,

                WarPoints = 0,

                TotalKills = 0
            };

            _db.RankEntries
            .Add(rank);
        }


        rank.WarPoints +=
        req.MatchPoint;


        rank.TotalKills +=
        req.KillCount;


        await _db.SaveChangesAsync();
    }



    //------------------------------------
    // API
    //------------------------------------

    [HttpGet("weekly/point")]
    public async Task<IActionResult>
    WeeklyPoint()
    {
        return Ok(
        await GetRank(
        "WEEK",
        "POINT"));
    }



    [HttpGet("weekly/kill")]
    public async Task<IActionResult>
    WeeklyKill()
    {
        return Ok(
        await GetRank(
        "WEEK",
        "KILL"));
    }



    [HttpGet("monthly/point")]
    public async Task<IActionResult>
    MonthlyPoint()
    {
        return Ok(
        await GetRank(
        "MONTH",
        "POINT"));
    }



    [HttpGet("monthly/kill")]
    public async Task<IActionResult>
    MonthlyKill()
    {
        return Ok(
        await GetRank(
        "MONTH",
        "KILL"));
    }



    //------------------------------------
    // GET RANK
    //------------------------------------

    private async Task<List<object>>
    GetRank(
    string seasonType,
    string rankType)
    {
        var season =
        await _db.RankSeasons
        .FirstAsync(
        x =>
        x.Type ==
        seasonType
        &&
        x.IsActive);



        if (rankType == "KILL")
        {
            var data =
            await
            (
                from r
                in _db.RankEntries

                join u
                in _db.Users

                on r.UserId
                equals u.Id

                where
                r.SeasonId
                ==
                season.Id

                orderby
                r.TotalKills
                descending

                select new
                {
                    u.TiktokUserId,

                    u.Nickname,

                    u.AvatarUrl,

                    Score =
                    r.TotalKills,

                    r.WarPoints
                }

            )
            .Take(100)
            .ToListAsync();


            return data
            .Select(
            (x, index) =>
            (object)new
            {
                Rank = index + 1,

                x.TiktokUserId,

                x.Nickname,

                x.AvatarUrl,

                x.Score,

                x.WarPoints
            })
            .ToList();
        }

        else
        {
            var data =
            await
            (
                from r
                in _db.RankEntries

                join u
                in _db.Users

                on r.UserId
                equals u.Id

                where
                r.SeasonId
                ==
                season.Id

                orderby
                r.WarPoints
                descending

                select new
                {
                    u.TiktokUserId,

                    u.Nickname,

                    u.AvatarUrl,

                    Score =
                    r.WarPoints,

                    r.TotalKills
                }

            )
            .Take(100)
            .ToListAsync();


            return data
            .Select(
            (x, index) =>
            (object)new
            {
                Rank = index + 1,

                x.TiktokUserId,

                x.Nickname,

                x.AvatarUrl,

                x.Score,

                x.TotalKills
            })
            .ToList();
        }
    }
    //------------------------------------
    // USER RANK API
    //------------------------------------

    [HttpGet(
    "weekly/point/{tikTokUserId}")]
    public async Task<IActionResult>
    WeeklyPointUser(
    string tikTokUserId)
    {
        return Ok(
        await GetUserRank(
        tikTokUserId,
        "WEEK",
        "POINT"));
    }


    [HttpGet(
    "weekly/kill/{tikTokUserId}")]
    public async Task<IActionResult>
    WeeklyKillUser(
    string tikTokUserId)
    {
        return Ok(
        await GetUserRank(
        tikTokUserId,
        "WEEK",
        "KILL"));
    }


    [HttpGet(
    "monthly/point/{tikTokUserId}")]
    public async Task<IActionResult>
    MonthlyPointUser(
    string tikTokUserId)
    {
        return Ok(
        await GetUserRank(
        tikTokUserId,
        "MONTH",
        "POINT"));
    }


    [HttpGet(
    "monthly/kill/{tikTokUserId}")]
    public async Task<IActionResult>
    MonthlyKillUser(
    string tikTokUserId)
    {
        return Ok(
        await GetUserRank(
        tikTokUserId,
        "MONTH",
        "KILL"));
    }



    //------------------------------------
    // GET USER RANK
    //------------------------------------
    private async Task<object>
GetUserRank(
string tikTokUserId,
string seasonType,
string rankType)
    {
        var season =
        await GetOrCreateSeason(
        seasonType);


        var data =
        await
        (
            from r
            in _db.RankEntries

            join u
            in _db.Users

            on r.UserId
            equals u.Id

            where
            r.SeasonId
            ==
            season.Id

            orderby
            rankType == "POINT"
            ?
            r.WarPoints
            :
            r.TotalKills
            descending

            select new
            {
                u.TiktokUserId,

                u.Nickname,

                u.AvatarUrl,

                r.WarPoints,

                r.TotalKills
            }

        )
        .ToListAsync();


        var index =
        data.FindIndex(
        x =>
        x.TiktokUserId
        ==
        tikTokUserId);


        if (index < 0)
        {
            return new
            {
                message =
                "No rank"
            };
        }


        var me =
        data[index];


        return new
        {
            Rank =
            index + 1,

            me.TiktokUserId,

            me.Nickname,

            me.AvatarUrl,

            me.WarPoints,

            me.TotalKills
        };
    }
    //private async Task<object>
    //GetUserRank(
    //string tikTokUserId,
    //string seasonType,
    //string rankType)
    //{
    //    var season =
    //    await GetOrCreateSeason(
    //    seasonType);



    //    var data =
    //    await _db.RankEntries

    //    .Include(
    //    x => x.User)

    //    .Where(
    //    x =>
    //    x.SeasonId ==
    //    season.Id)

    //    .OrderByDescending(
    //    x =>
    //    rankType == "POINT"
    //    ?
    //    x.WarPoints
    //    :
    //    x.TotalKills)

    //    .ToListAsync();



    //    var index =
    //    data.FindIndex(
    //    x =>
    //    x.User
    //    .TiktokUserId
    //    ==
    //    tikTokUserId);



    //    if (index < 0)
    //    {
    //        return new
    //        {
    //            message =
    //            "No rank"
    //        };
    //    }


    //    var me =
    //    data[index];


    //    return new
    //    {
    //        Rank =
    //        index + 1,

    //        TikTokUserId =
    //        me.User.TiktokUserId,

    //        Nickname =
    //        me.User.Nickname,

    //        AvatarUrl =
    //        me.User.AvatarUrl,

    //        WarPoints =
    //        me.WarPoints,

    //        TotalKills =
    //        me.TotalKills
    //    };
    //}


    //------------------------------------
    // THÔNG TIN USER
    //------------------------------------

    [HttpGet(
    "me/{tikTokUserId}")]
    public async Task<IActionResult>
    GetMyRank(
    string tikTokUserId)
    {
        var user =
        await _db.Users
        .FirstOrDefaultAsync(
        x =>
        x.TiktokUserId
        ==
        tikTokUserId);


        if (user == null)
        {
            return NotFound(
            "User not found");
        }


        var week =
        await GetOrCreateSeason(
        "WEEK");


        var month =
        await GetOrCreateSeason(
        "MONTH");



        var weekly =
        await _db.RankEntries
        .FirstOrDefaultAsync(
        x =>
        x.UserId ==
        user.Id
        &&
        x.SeasonId ==
        week.Id);



        var monthly =
        await _db.RankEntries
        .FirstOrDefaultAsync(
        x =>
        x.UserId ==
        user.Id
        &&
        x.SeasonId ==
        month.Id);



        return Ok(
        new
        {
            user.Id,

            user.TiktokUserId,

            user.Nickname,

            user.AvatarUrl,

            WeeklyPoint =
            weekly?.WarPoints ?? 0,

            WeeklyKill =
            weekly?.TotalKills ?? 0,

            MonthlyPoint =
            monthly?.WarPoints ?? 0,

            MonthlyKill =
            monthly?.TotalKills ?? 0
        });
    }
}