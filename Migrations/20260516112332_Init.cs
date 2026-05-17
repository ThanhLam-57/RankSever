using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RankServer.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RankEntries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SeasonId = table.Column<long>(type: "bigint", nullable: false),
                    WarPoints = table.Column<long>(type: "bigint", nullable: false),
                    Victories = table.Column<int>(type: "int", nullable: false),
                    TotalKills = table.Column<long>(type: "bigint", nullable: false),
                    TotalDamage = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankEntries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RankHistories",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    SeasonId = table.Column<long>(type: "bigint", nullable: false),
                    RankPosition = table.Column<int>(type: "int", nullable: false),
                    RankType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Score = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankHistories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RankSeasons",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RankSeasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TiktokUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Nickname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    Exp = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RankEntries_SeasonId_Victories",
                table: "RankEntries",
                columns: new[] { "SeasonId", "Victories" });

            migrationBuilder.CreateIndex(
                name: "IX_RankEntries_SeasonId_WarPoints",
                table: "RankEntries",
                columns: new[] { "SeasonId", "WarPoints" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_TiktokUserId",
                table: "Users",
                column: "TiktokUserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RankEntries");

            migrationBuilder.DropTable(
                name: "RankHistories");

            migrationBuilder.DropTable(
                name: "RankSeasons");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
