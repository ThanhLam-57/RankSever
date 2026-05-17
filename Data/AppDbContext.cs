using Microsoft.EntityFrameworkCore;
using RankServer.Models;

namespace RankServer.Data;

public class AppDbContext
    : DbContext
{
    public AppDbContext(
        DbContextOptions<AppDbContext>
        options)
        : base(options)
    {

    }


    //------------------------------------
    // TABLES
    //------------------------------------

    public DbSet<User>
        Users => Set<User>();


    public DbSet<RankEntry>
        RankEntries => Set<RankEntry>();


    public DbSet<RankSeason>
        RankSeasons => Set<RankSeason>();


    public DbSet<RankHistory>
        RankHistories => Set<RankHistory>();




    //------------------------------------
    // CONFIG
    //------------------------------------

    protected override void
    OnModelCreating(
        ModelBuilder b)
    {
        base.OnModelCreating(b);


        //------------------------------------
        // TikTok ID unique
        //------------------------------------

        b.Entity<User>()

        .HasIndex(
        x => x.TiktokUserId)

        .IsUnique();



        //------------------------------------
        // Top point
        //------------------------------------

        b.Entity<RankEntry>()

        .HasIndex(
        x =>
        new
        {
            x.SeasonId,
            x.WarPoints
        });



        //------------------------------------
        // Top kill
        //------------------------------------

        b.Entity<RankEntry>()

        .HasIndex(
        x =>
        new
        {
            x.SeasonId,
            x.TotalKills
        });



        //------------------------------------
        // chống trùng
        //------------------------------------

        b.Entity<RankEntry>()

        .HasIndex(
        x =>
        new
        {
            x.UserId,
            x.SeasonId
        })

        .IsUnique();



        //------------------------------------
        // History
        //------------------------------------

        b.Entity<RankHistory>()

        .HasIndex(
        x =>
        new
        {
            x.SeasonId,
            x.RankType
        });
    }
}