using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retail.Branch.Infrastructure.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //   name: "BranchBranchRequest");

            //migrationBuilder.DropTable(
            //    name: "BranchMembers");

            //migrationBuilder.DropTable(
            //    name: "BranchRequestLogs");

            //migrationBuilder.DropTable(
            //    name: "Branches");

            //migrationBuilder.DropTable(
            //    name: "BranchRequests");

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<char>(type: "character(1)", nullable: false),
                    LocationRef = table.Column<string>(type: "text", nullable: true),
                    IsLocked = table.Column<bool>(type: "boolean", nullable: false),
                    Created_By_BranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    Treated_By = table.Column<string>(type: "text", nullable: true),
                    Treated_By_Id = table.Column<string>(type: "text", nullable: true),
                    Treated_By_Branch_Id = table.Column<string>(type: "text", nullable: true),
                    Number = table.Column<string>(type: "text", nullable: true),
                    StreetName = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: true),
                    State = table.Column<string>(type: "text", nullable: true),
                    Lga = table.Column<string>(type: "text", nullable: true),
                    PostalCode = table.Column<string>(type: "text", nullable: true),
                    Created_By = table.Column<string>(type: "text", nullable: true),
                    Created_By_Id = table.Column<string>(type: "text", nullable: true),
                    Created_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Deleted_By_Id = table.Column<string>(type: "text", nullable: true),
                    Deleted_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BranchRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Treated_By = table.Column<string>(type: "text", nullable: true),
                    Treated_By_Id = table.Column<string>(type: "text", nullable: true),
                    Treated_By_Branch_Id = table.Column<string>(type: "text", nullable: true),
                    CreatedByBranchId = table.Column<Guid>(type: "uuid", nullable: true),
                    Request_Type = table.Column<string>(type: "text", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Meta = table.Column<string>(type: "text", nullable: true),
                    Alt_Name = table.Column<string>(type: "text", nullable: true),
                    Alt_Description = table.Column<string>(type: "text", nullable: true),
                    Created_By = table.Column<string>(type: "text", nullable: true),
                    Created_By_Id = table.Column<string>(type: "text", nullable: true),
                    Created_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Deleted_By_Id = table.Column<string>(type: "text", nullable: true),
                    Deleted_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BranchMembers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    User_Id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    User_Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created_By = table.Column<string>(type: "text", nullable: true),
                    Created_By_Id = table.Column<string>(type: "text", nullable: true),
                    Created_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Deleted_By_Id = table.Column<string>(type: "text", nullable: true),
                    Deleted_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchMembers_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchBranchRequest",
                columns: table => new
                {
                    BranchRequestsId = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchesId = table.Column<Guid>(type: "uuid", nullable: false),
                    Meta = table.Column<string>(type: "text", nullable: true),
                    Document = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchBranchRequest", x => new { x.BranchRequestsId, x.BranchesId });
                    table.ForeignKey(
                        name: "FK_BranchBranchRequest_Branches_BranchesId",
                        column: x => x.BranchesId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BranchBranchRequest_BranchRequests_BranchRequestsId",
                        column: x => x.BranchRequestsId,
                        principalTable: "BranchRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BranchRequestLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    BranchRequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    Created_By = table.Column<string>(type: "text", nullable: true),
                    Created_By_Id = table.Column<string>(type: "text", nullable: true),
                    Created_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Updated_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Deleted = table.Column<bool>(type: "boolean", nullable: false),
                    Deleted_By_Id = table.Column<string>(type: "text", nullable: true),
                    Deleted_At = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchRequestLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BranchRequestLogs_BranchRequests_BranchRequestId",
                        column: x => x.BranchRequestId,
                        principalTable: "BranchRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Branches",
                columns: new[] { "Id", "City", "Code", "Created_At", "Created_By", "Created_By_BranchId", "Created_By_Id", "Deleted", "Deleted_At", "Deleted_By_Id", "Description", "IsLocked", "Lga", "LocationRef", "Name", "Number", "PostalCode", "State", "Status", "StreetName", "Treated_By", "Treated_By_Branch_Id", "Treated_By_Id", "Updated_At" },
                values: new object[] { new Guid("f7c8559f-f718-4b16-aa59-41a5d6c718d2"), "Lagos", "HQ", new DateTime(2023, 8, 10, 7, 53, 27, 748, DateTimeKind.Utc).AddTicks(3612), null, null, "00000000-0000-0000-0000-000000000000", false, null, null, "Sterling Bank Head Quaters", true, null, null, "Sterling Bank Head Office ", "20", "P.M.B. 12735", "Lagos", 'A', "Sterling Towers, Marina", null, null, null, new DateTime(2023, 8, 10, 7, 53, 27, 748, DateTimeKind.Utc).AddTicks(3612) });

            migrationBuilder.CreateIndex(
                name: "IX_BranchBranchRequest_BranchesId",
                table: "BranchBranchRequest",
                column: "BranchesId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchMembers_BranchId",
                table: "BranchMembers",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchRequestLogs_BranchRequestId",
                table: "BranchRequestLogs",
                column: "BranchRequestId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BranchBranchRequest");

            migrationBuilder.DropTable(
                name: "BranchMembers");

            migrationBuilder.DropTable(
                name: "BranchRequestLogs");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "BranchRequests");
        }
    }
}
