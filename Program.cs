using Microsoft.EntityFrameworkCore;
using RankServer.Data;
using RankServer.Models;
using RankServer.Services;

var builder =
    WebApplication.CreateBuilder(args);


builder.Services
.AddControllers();


builder.Services
.AddEndpointsApiExplorer();


builder.Services
.AddSwaggerGen();


builder.Services
.AddDbContext<AppDbContext>(
x => x.UseSqlServer(
builder.Configuration
.GetConnectionString(
"DefaultConnection")
));


builder.Services
.AddHostedService<
SeasonService>();



var app =
builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var scope =
    app.Services.CreateScope())
{
    var db =
        scope.ServiceProvider
        .GetRequiredService<AppDbContext>();


    db.Database.EnsureCreated();


    if (!db.RankSeasons.Any())
    {
        db.RankSeasons.Add(
        new RankSeason
        {
            Name = "WEEK_1",

            Type = "WEEK",

            IsActive = true,

            StartDate =
                DateTime.UtcNow,

            EndDate =
                DateTime.UtcNow
                .AddDays(7)
        });


        db.RankSeasons.Add(
        new RankSeason
        {
            Name = "MONTH_1",

            Type = "MONTH",

            IsActive = true,

            StartDate =
                DateTime.UtcNow,

            EndDate =
                DateTime.UtcNow
                .AddMonths(1)
        });

        db.SaveChanges();
    }
}

app.Run();