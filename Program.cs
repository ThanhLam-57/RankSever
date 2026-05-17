using Microsoft.EntityFrameworkCore;
using RankServer.Data;
using RankServer.Models;
using RankServer.Services;

var builder = WebApplication.CreateBuilder(args);

//---------------------------
// Services
//---------------------------

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


//---------------------------
// MySQL Railway
//---------------------------

var connectionString =
builder.Configuration
.GetConnectionString(
"DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(
options =>
options.UseMySql(
connectionString,
ServerVersion.AutoDetect(
connectionString)
));


//---------------------------
// Auto reset season
//---------------------------

builder.Services
.AddHostedService<
SeasonService>();


var app =
builder.Build();


//---------------------------
// Swagger
//---------------------------

app.UseSwagger();

app.UseSwaggerUI();

app.UseAuthorization();

app.MapControllers();


//---------------------------
// Auto migrate
//---------------------------

using (var scope =
app.Services.CreateScope())
{
    var db =
    scope.ServiceProvider
    .GetRequiredService<
    AppDbContext>();


    try
    {
        db.Database.Migrate();

        //--------------------------------
        // tạo season đầu tiên
        //--------------------------------

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
    catch (Exception ex)
    {
        Console.WriteLine(
        ex.ToString());
    }
}



//---------------------------
// Railway port
//---------------------------

var port =
Environment
.GetEnvironmentVariable(
"PORT");

if (!string.IsNullOrEmpty(
port))
{
    app.Urls.Add(
    $"http://0.0.0.0:{port}");
}

app.Run();