using Microsoft.EntityFrameworkCore;
using RankServer.Data;
using RankServer.Models;

namespace RankServer.Services;

//------------------------------------
// Season theo lịch cố định giờ VN (UTC+7)
//   WEEK : 0h thứ Hai -> 0h thứ Hai tuần sau
//   MONTH: 0h ngày 1  -> 0h ngày 1 tháng sau
// StartDate / EndDate lưu trong DB là UTC.
//------------------------------------

public static class SeasonManager
{
    // Việt Nam không có giờ mùa hè -> offset cố định
    static readonly TimeSpan VnOffset = TimeSpan.FromHours(7);

    // Chống controller và SeasonService cùng tạo season (trong 1 instance)
    static readonly SemaphoreSlim Lock = new(1, 1);


    public static (DateTime StartUtc, DateTime EndUtc, string Name)
    GetPeriod(string type, DateTime utcNow)
    {
        var vnNow = utcNow + VnOffset;

        DateTime vnStart;
        DateTime vnEnd;
        string name;

        if (type == "WEEK")
        {
            // Monday = 0 ... Sunday = 6
            int daysSinceMonday = ((int)vnNow.DayOfWeek + 6) % 7;

            vnStart = vnNow.Date.AddDays(-daysSinceMonday);
            vnEnd = vnStart.AddDays(7);
            name = $"WEEK_{vnStart:yyyyMMdd}";
        }
        else if (type == "MONTH")
        {
            vnStart = new DateTime(vnNow.Year, vnNow.Month, 1);
            vnEnd = vnStart.AddMonths(1);
            name = $"MONTH_{vnStart:yyyyMM}";
        }
        else
        {
            throw new ArgumentException($"Unknown season type: {type}");
        }

        return (
            DateTime.SpecifyKind(vnStart - VnOffset, DateTimeKind.Utc),
            DateTime.SpecifyKind(vnEnd - VnOffset, DateTimeKind.Utc),
            name);
    }


    //------------------------------------
    // Lấy season hiện tại, tự đóng season hết hạn và mở season mới
    //------------------------------------

    public static async Task<RankSeason>
    GetCurrentAsync(AppDbContext db, string type)
    {
        await Lock.WaitAsync();

        try
        {
            var now = DateTime.UtcNow;
            var period = GetPeriod(type, now);

            var actives = await db.RankSeasons
                .Where(x => x.Type == type && x.IsActive)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            var current = actives.FirstOrDefault();

            // dọn season active bị trùng (nếu có)
            foreach (var dup in actives.Skip(1))
                dup.IsActive = false;

            if (current != null && now < current.EndDate)
            {
                // season cũ kiểu "cuộn" (now + 7 ngày / + 1 tháng):
                // giữ điểm, chỉ chỉnh EndDate về mốc cố định của kỳ này
                if (current.EndDate != period.EndUtc)
                    current.EndDate = period.EndUtc;
            }
            else
            {
                // chưa có hoặc đã hết hạn -> đóng và mở kỳ mới
                if (current != null)
                    current.IsActive = false;

                current = new RankSeason
                {
                    Name = period.Name,
                    Type = type,
                    StartDate = period.StartUtc,
                    EndDate = period.EndUtc,
                    IsActive = true
                };

                db.RankSeasons.Add(current);
            }

            // đóng season cũ + tạo season mới trong cùng 1 lần lưu
            await db.SaveChangesAsync();

            return current;
        }
        finally
        {
            Lock.Release();
        }
    }
}
