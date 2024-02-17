using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retail.Branch.Infrastructure.Migrations
{
    public partial class Auditlog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    User_Id = table.Column<string>(type: "text", nullable: false),
                    User_Name = table.Column<string>(type: "text", nullable: false),
                    Action_Type = table.Column<string>(type: "text", nullable: false),
                    Clicks = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    SourceIPs = table.Column<string>(type: "text", nullable: false),
                    BranchCode = table.Column<string>(type: "text", nullable: false),
                    LoginTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Created_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.User_Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");
        }
    }
}
