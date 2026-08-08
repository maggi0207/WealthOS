using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WealthOS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSettingsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkspaceName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    AvatarUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Timezone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Theme = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    LayoutDensity = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    SidebarCollapsed = table.Column<bool>(type: "boolean", nullable: false),
                    CurrencyCode = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: false),
                    Locale = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    DateFormat = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    NumberFormat = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    EmailNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    PushNotifications = table.Column<bool>(type: "boolean", nullable: false),
                    GoalReminders = table.Column<bool>(type: "boolean", nullable: false),
                    LoanEmiReminders = table.Column<bool>(type: "boolean", nullable: false),
                    InvestmentAlerts = table.Column<bool>(type: "boolean", nullable: false),
                    AiAdvisorInsights = table.Column<bool>(type: "boolean", nullable: false),
                    WeeklySummary = table.Column<bool>(type: "boolean", nullable: false),
                    MonthlyReport = table.Column<bool>(type: "boolean", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "boolean", nullable: false),
                    AngelOneConnected = table.Column<bool>(type: "boolean", nullable: false),
                    IndiaBondsConnected = table.Column<bool>(type: "boolean", nullable: false),
                    BankSyncConnected = table.Column<bool>(type: "boolean", nullable: false),
                    UpiConnected = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSettings",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSettings");
        }
    }
}
