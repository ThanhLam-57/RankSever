using Microsoft.EntityFrameworkCore;
using RankServer.Data;
using RankServer.Models;

namespace RankServer.Services;

public class SeasonService
    : BackgroundService
{
    private readonly
    IServiceScopeFactory
    _scope;

    public SeasonService(
        IServiceScopeFactory scope)
    {
        _scope = scope;
    }


    protected override async Task
    ExecuteAsync(
    CancellationToken stop)
    {
        while (
        !stop.IsCancellationRequested)
        {
            using var s =
            _scope.CreateScope();

            var db =
            s.ServiceProvider
            .GetRequiredService<
            AppDbContext>();


            await Check(
                db,
                "WEEK");


            await Check(
                db,
                "MONTH");


            await Task.Delay(
                TimeSpan.FromMinutes(1),
                stop);
        }
    }



    async Task Check(
        AppDbContext db,
        string type)
    {
        var season =
        await db.RankSeasons
        .FirstOrDefaultAsync(
        x =>
        x.Type == type
        &&
        x.IsActive);


        //--------------------------------
        // chưa có season
        //--------------------------------

        if (season == null)
        {
            await CreateSeason(
                db,
                type);

            return;
        }


        //--------------------------------
        // hết hạn
        //--------------------------------

        if (DateTime.UtcNow
            >=
            season.EndDate)
        {
            season.IsActive = false;

            await db
            .SaveChangesAsync();


            await CreateSeason(
                db,
                type);
        }
    }



    async Task CreateSeason(
        AppDbContext db,
        string type)
    {
        var now =
        DateTime.UtcNow;


        var season =
        new RankSeason
        {
            Name =
            type == "WEEK"
            ?
            $"WEEK_{now:yyyyMMdd}"
            :
            $"MONTH_{now:yyyyMM}",


            Type = type,


            StartDate =
            now,


            EndDate =
            type == "WEEK"
            ?
            now.AddDays(7)
            :
            now.AddMonths(1),


            IsActive = true
        };


        db.RankSeasons
        .Add(season);

        await db
        .SaveChangesAsync();
    }
}