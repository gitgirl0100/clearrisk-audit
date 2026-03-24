using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClearRiskApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ContractAddress = table.Column<string>(type: "TEXT", maxLength: 42, nullable: false),
                    FinalScore = table.Column<double>(type: "REAL", nullable: false),
                    RiskTier = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    OwnershipRisk = table.Column<double>(type: "REAL", nullable: false),
                    LiquidityRisk = table.Column<double>(type: "REAL", nullable: false),
                    DistributionRisk = table.Column<double>(type: "REAL", nullable: false),
                    CodeTransparencyRisk = table.Column<double>(type: "REAL", nullable: false),
                    ActivityRisk = table.Column<double>(type: "REAL", nullable: false),
                    ReportHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    BlockchainLogged = table.Column<bool>(type: "INTEGER", nullable: false),
                    TransactionHash = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditReports", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditReports");
        }
    }
}
