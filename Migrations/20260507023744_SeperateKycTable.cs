using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bharathome_api.Migrations
{
    /// <inheritdoc />
    public partial class SeperateKycTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "kyc_document_number",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "kyc_document_type",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "kyc_document_urls",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "kyc_rejection_reason",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "kyc_status",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "kyc_submitted_at",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "kyc_verified_at",
                table: "user_profiles");

            migrationBuilder.CreateTable(
                name: "user_kycs",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    kyc_status = table.Column<int>(type: "integer", nullable: false),
                    kyc_document_type = table.Column<string>(type: "text", nullable: true),
                    kyc_document_number = table.Column<string>(type: "text", nullable: true),
                    kyc_submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    kyc_verified_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    kyc_rejection_reason = table.Column<string>(type: "text", nullable: true),
                    kyc_document_urls = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_kycs", x => x.user_id);
                    table.ForeignKey(
                        name: "fk_user_kycs_user_profiles_user_id",
                        column: x => x.user_id,
                        principalTable: "user_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_kycs");

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

            migrationBuilder.AddColumn<string>(
                name: "kyc_document_urls",
                table: "user_profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "kyc_rejection_reason",
                table: "user_profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "kyc_status",
                table: "user_profiles",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "kyc_submitted_at",
                table: "user_profiles",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "kyc_verified_at",
                table: "user_profiles",
                type: "timestamp with time zone",
                nullable: true);
        }
    }
}
