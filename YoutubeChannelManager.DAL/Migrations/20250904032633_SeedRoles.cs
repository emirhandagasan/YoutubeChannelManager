using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace YoutubeChannelManager.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "16804a67-085f-4a5a-afd7-ce75e2f91699", null, "User", "USER" },
                    { "76c38b65-2ea6-40dc-b015-656ecbfcfbd0", null, "SuperAdmin", "SUPERADMIN" },
                    { "790721b2-0c00-478a-aa23-d7de179aa7d5", null, "Admin", "ADMIN" },
                    { "eefd8221-d606-4616-bb14-cae96d26640b", null, "Editor", "EDITOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "16804a67-085f-4a5a-afd7-ce75e2f91699");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "76c38b65-2ea6-40dc-b015-656ecbfcfbd0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "790721b2-0c00-478a-aa23-d7de179aa7d5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "eefd8221-d606-4616-bb14-cae96d26640b");
        }
    }
}
