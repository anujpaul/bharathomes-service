using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bharathome_api.Migrations
{
    /// <inheritdoc />
    public partial class AddKeycDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "kyc_document_url",
                table: "user_profiles",
                newName: "rera_number");

            migrationBuilder.AddColumn<string>(
                name: "kyc_document_number",
                table: "user_profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kyc_document_type",
                table: "user_profiles",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "kyc_document_number",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "kyc_document_type",
                table: "user_profiles");

            migrationBuilder.RenameColumn(
                name: "rera_number",
                table: "user_profiles",
                newName: "kyc_document_url");
        }
    }
}
