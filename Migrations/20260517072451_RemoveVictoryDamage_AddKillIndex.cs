using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RankServer.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVictoryDamage_AddKillIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RankEntries_SeasonId_Victories",
                table: "RankEntries");

            migrationBuilder.DropColumn(
                name: "TotalDamage",
                table: "RankEntries");

            migrationBuilder.DropColumn(
                name: "Victories",
                table: "RankEntries");

            migrationBuilder.AlterColumn<string>(
                name: "RankType",
                table: "RankHistories",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_RankHistories_SeasonId_RankType",
                table: "RankHistories",
                columns: new[] { "SeasonId", "RankType" });

            migrationBuilder.CreateIndex(
                name: "IX_RankEntries_SeasonId_TotalKills",
                table: "RankEntries",
                columns: new[] { "SeasonId", "TotalKills" });

            migrationBuilder.CreateIndex(
                name: "IX_RankEntries_UserId_SeasonId",
                table: "RankEntries",
                columns: new[] { "UserId", "SeasonId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RankHistories_SeasonId_RankType",
                table: "RankHistories");

            migrationBuilder.DropIndex(
                name: "IX_RankEntries_SeasonId_TotalKills",
                table: "RankEntries");

            migrationBuilder.DropIndex(
                name: "IX_RankEntries_UserId_SeasonId",
                table: "RankEntries");

            migrationBuilder.AlterColumn<string>(
                name: "RankType",
                table: "RankHistories",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<long>(
                name: "TotalDamage",
                table: "RankEntries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Victories",
                table: "RankEntries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_RankEntries_SeasonId_Victories",
                table: "RankEntries",
                columns: new[] { "SeasonId", "Victories" });
        }
    }
}
