using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retail.Branch.Infrastructure.Migrations
{
    public partial class branchtrail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BranchId",
                table: "BranchRequestLogs",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchId",
                table: "BranchRequestLogs");
        }
    }
}
