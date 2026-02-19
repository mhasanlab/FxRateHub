using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FxRateHub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SettingKey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false),
                    SettingValue = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRateHistory",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    TargetCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,8)", precision: 18, scale: 8, nullable: false),
                    RecordedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRateHistory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExchangeRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BaseCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false, defaultValue: "USD"),
                    TargetCurrency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(18,8)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExchangeRates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SyncLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SyncType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false, defaultValue: "FxRate"),
                    Status = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CurrenciesUpdated = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ErrorMessage = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AuthProvider = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GoogleId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsBlocked = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ApiKeys",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KeyHash = table.Column<string>(type: "varchar(128)", maxLength: 128, nullable: false),
                    KeyPrefix = table.Column<string>(type: "varchar(16)", maxLength: 16, nullable: false),
                    DailyRequestCount = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    TotalRequestCount = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    LastRequestDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiKeys_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApiUsageLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApiKeyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Endpoint = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false),
                    HttpMethod = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false),
                    ResponseStatusCode = table.Column<int>(type: "int", nullable: false),
                    RequestedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiUsageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApiUsageLogs_ApiKeys_ApiKeyId",
                        column: x => x.ApiKeyId,
                        principalTable: "ApiKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_KeyHash",
                table: "ApiKeys",
                column: "KeyHash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiKeys_UserId",
                table: "ApiKeys",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApiUsageLogs_ApiKeyId",
                table: "ApiUsageLogs",
                column: "ApiKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApiUsageLogs_ApiKeyId_RequestedAt",
                table: "ApiUsageLogs",
                columns: new[] { "ApiKeyId", "RequestedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ApiUsageLogs_RequestedAt",
                table: "ApiUsageLogs",
                column: "RequestedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AppSettings_SettingKey",
                table: "AppSettings",
                column: "SettingKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRateHistory_BaseCurrency_TargetCurrency_RecordedAt",
                table: "ExchangeRateHistory",
                columns: new[] { "BaseCurrency", "TargetCurrency", "RecordedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRateHistory_RecordedAt",
                table: "ExchangeRateHistory",
                column: "RecordedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ExchangeRates_BaseCurrency_TargetCurrency",
                table: "ExchangeRates",
                columns: new[] { "BaseCurrency", "TargetCurrency" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SyncLogs_StartedAt",
                table: "SyncLogs",
                column: "StartedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SyncLogs_Status",
                table: "SyncLogs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SyncLogs_SyncType",
                table: "SyncLogs",
                column: "SyncType");

            migrationBuilder.CreateIndex(
                name: "IX_SyncLogs_SyncType_StartedAt",
                table: "SyncLogs",
                columns: new[] { "SyncType", "StartedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_GoogleId",
                table: "Users",
                column: "GoogleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiUsageLogs");

            migrationBuilder.DropTable(
                name: "AppSettings");

            migrationBuilder.DropTable(
                name: "ExchangeRateHistory");

            migrationBuilder.DropTable(
                name: "ExchangeRates");

            migrationBuilder.DropTable(
                name: "SyncLogs");

            migrationBuilder.DropTable(
                name: "ApiKeys");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
