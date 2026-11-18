using RankServer.Data;

namespace RankServer.Services;

//------------------------------------
// Mỗi phút kiểm tra và reset season
// theo lịch cố định giờ VN (xem SeasonManager)
//------------------------------------

public class SeasonService
    : BackgroundService
{
    private readonly
    IServiceScopeFactory
    _scope;

    private readonly
    ILogger<SeasonService>
    _log;

    public SeasonService(
        IServiceScopeFactory scope,
        ILogger<SeasonService> log)
    {
        _scope = scope;
        _log = log;
    }


    protected override async Task
    ExecuteAsync(
    CancellationToken stop)
    {
        while (
        !stop.IsCancellationRequested)
        {
            try
            {
                using var s =
                _scope.CreateScope();

                var db =
                s.ServiceProvider
                .GetRequiredService<
                AppDbContext>();

                await SeasonManager
                .GetCurrentAsync(db, "WEEK");

                await SeasonManager
                .GetCurrentAsync(db, "MONTH");
            }
            catch (Exception ex)
            {
                // lỗi DB tạm thời không được làm chết service
                _log.LogError(ex, "Season check failed");
            }

            await Task.Delay(
                TimeSpan.FromMinutes(1),
                stop);
        }
    }
}
