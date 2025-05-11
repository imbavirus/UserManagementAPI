using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UserManagementAPI.Application.Migrations
{
    /// <inheritdoc />
    public partial class CreatedAndUpdatedOn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1ul,
                columns: new[] { "CreatedOn", "UpdatedOn" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2ul,
                columns: new[] { "CreatedOn", "UpdatedOn" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3ul,
                columns: new[] { "CreatedOn", "UpdatedOn" },
                values: new object[] { new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1ul,
                columns: new[] { "CreatedOn", "UpdatedOn" },
                values: new object[] { new DateTime(2025, 5, 10, 13, 44, 53, 618, DateTimeKind.Utc).AddTicks(4335), new DateTime(2025, 5, 10, 13, 44, 53, 618, DateTimeKind.Utc).AddTicks(4541) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2ul,
                columns: new[] { "CreatedOn", "UpdatedOn" },
                values: new object[] { new DateTime(2025, 5, 10, 13, 44, 53, 618, DateTimeKind.Utc).AddTicks(4969), new DateTime(2025, 5, 10, 13, 44, 53, 618, DateTimeKind.Utc).AddTicks(4970) });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3ul,
                columns: new[] { "CreatedOn", "UpdatedOn" },
                values: new object[] { new DateTime(2025, 5, 10, 13, 44, 53, 618, DateTimeKind.Utc).AddTicks(4971), new DateTime(2025, 5, 10, 13, 44, 53, 618, DateTimeKind.Utc).AddTicks(4972) });
        }
    }
}
