using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Retail.Branch.Infrastructure.Migrations
{
    public partial class defaultbr : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: new Guid("f7c8559f-f718-4b16-aa59-41a5d6c718d2"),
                columns: new[] { "Country", "Description" },
                values: new object[] { "Nigeria", "Sterling Bank Head Quarters" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Branches",
                keyColumn: "Id",
                keyValue: new Guid("f7c8559f-f718-4b16-aa59-41a5d6c718d2"),
                columns: new[] { "Country", "Description" },
                values: new object[] { null, "Sterling Bank Head Quaters" });
        }
    }
}
