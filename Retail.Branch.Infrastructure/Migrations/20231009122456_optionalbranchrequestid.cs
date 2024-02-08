using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retail.Branch.Infrastructure.Migrations
{
    public partial class optionalbranchrequestid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchRequestLogs_BranchRequests_BranchRequestId",
                table: "BranchRequestLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "BranchRequestId",
                table: "BranchRequestLogs",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_BranchRequestLogs_BranchRequests_BranchRequestId",
                table: "BranchRequestLogs",
                column: "BranchRequestId",
                principalTable: "BranchRequests",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchRequestLogs_BranchRequests_BranchRequestId",
                table: "BranchRequestLogs");

            migrationBuilder.AlterColumn<Guid>(
                name: "BranchRequestId",
                table: "BranchRequestLogs",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BranchRequestLogs_BranchRequests_BranchRequestId",
                table: "BranchRequestLogs",
                column: "BranchRequestId",
                principalTable: "BranchRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
