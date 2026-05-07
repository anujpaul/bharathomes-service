using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bharathome_api.Migrations
{
    /// <inheritdoc />
    public partial class UsingUserProfileDto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "kyc_verified_at",
                table: "user_kycs",
                newName: "verified_at");

            migrationBuilder.RenameColumn(
                name: "kyc_submitted_at",
                table: "user_kycs",
                newName: "submitted_at");

            migrationBuilder.RenameColumn(
                name: "kyc_status",
                table: "user_kycs",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "kyc_rejection_reason",
                table: "user_kycs",
                newName: "rejection_reason");

            migrationBuilder.RenameColumn(
                name: "kyc_document_urls",
                table: "user_kycs",
                newName: "document_urls");

            migrationBuilder.RenameColumn(
                name: "kyc_document_type",
                table: "user_kycs",
                newName: "document_type");

            migrationBuilder.RenameColumn(
                name: "kyc_document_number",
                table: "user_kycs",
                newName: "document_number");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "verified_at",
                table: "user_kycs",
                newName: "kyc_verified_at");

            migrationBuilder.RenameColumn(
                name: "submitted_at",
                table: "user_kycs",
                newName: "kyc_submitted_at");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "user_kycs",
                newName: "kyc_status");

            migrationBuilder.RenameColumn(
                name: "rejection_reason",
                table: "user_kycs",
                newName: "kyc_rejection_reason");

            migrationBuilder.RenameColumn(
                name: "document_urls",
                table: "user_kycs",
                newName: "kyc_document_urls");

            migrationBuilder.RenameColumn(
                name: "document_type",
                table: "user_kycs",
                newName: "kyc_document_type");

            migrationBuilder.RenameColumn(
                name: "document_number",
                table: "user_kycs",
                newName: "kyc_document_number");
        }
    }
}
