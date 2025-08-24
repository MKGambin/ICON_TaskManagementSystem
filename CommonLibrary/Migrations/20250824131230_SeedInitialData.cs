using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CommonLibrary.Migrations
{
    /// <inheritdoc />
    public partial class SeedInitialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Identifier" },
                values: new object[,]
                {
                    { 1, "alice.borderland@testmail.com", "f47ac10b-58cc-4372-a567-0e02b2c3d479" },
                    { 2, "frodo.smith@testmail.com", "c9bf9e57-1685-4c89-bafb-ff5af830be8a" },
                    { 3, "charlie.shane@testmail.com", "7c9e6679-7425-40de-944b-e07fc1f90ae7" }
                });

            migrationBuilder.InsertData(
                table: "TaskItems",
                columns: new[] { "Id", "Description", "Identifier", "Name", "TaskItemStatus", "UserId" },
                values: new object[,]
                {
                    { 1, "", "86c53f64-6f9a-40c9-acce-766c8a88ae35", "Task 1.1", 1, 1 },
                    { 2, "", "e1d82661-3c50-4ba2-9c77-5521df64a6f8", "Task 1.2", 2, 1 },
                    { 3, "", "3311ff50-45f5-43a3-8a8b-0e9b6cbaf45f", "Task 1.3", 3, 1 },
                    { 4, "Task 1.4 Cancelled..", "554fa5ed-5c60-4c01-985b-1292fcfd9cdd", "Task 1.4", 4, 1 },
                    { 5, "Completed Before Time", "7e09726a-6037-432e-941e-80cf3fa93137", "Task A (001)", 3, 2 },
                    { 6, "", "687f5642-57db-4bdf-a8cf-12c7d441c7b7", "Task B (001)", 4, 2 },
                    { 7, "", "19ae216c-fbb7-4c56-b6da-aab9bbb58830", "Task B (002)", 4, 2 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "TaskItems",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
