using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bharathome_api.Migrations
{
    /// <inheritdoc />
    public partial class AddedUserStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "user_status",
                table: "user_profiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "user_status",
                table: "user_profiles");
        }
    }
}
