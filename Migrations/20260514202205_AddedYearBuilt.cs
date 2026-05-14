using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bharathome_api.Migrations
{
    /// <inheritdoc />
    public partial class AddedYearBuilt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "built_year",
                table: "properties",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "built_year",
                table: "properties");
        }
    }
}
