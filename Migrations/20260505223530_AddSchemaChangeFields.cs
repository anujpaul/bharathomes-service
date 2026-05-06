using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bharathome_api.Migrations
{
    /// <inheritdoc />
    public partial class AddSchemaChangeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "user_profiles",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "is_paid",
                table: "user_profiles",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "kyc_document_url",
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

            migrationBuilder.AddColumn<DateTime>(
                name: "subscription_expiry",
                table: "user_profiles",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "created_at",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "is_paid",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "kyc_document_url",
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

            migrationBuilder.DropColumn(
                name: "subscription_expiry",
                table: "user_profiles");
        }
    }
}
