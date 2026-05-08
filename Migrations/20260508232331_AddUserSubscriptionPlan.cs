using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bharathome_api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserSubscriptionPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "current_plan_code",
                table: "user_profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "current_plan_tier",
                table: "user_profiles",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "subscription_started_at",
                table: "user_profiles",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "current_plan_code",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "current_plan_tier",
                table: "user_profiles");

            migrationBuilder.DropColumn(
                name: "subscription_started_at",
                table: "user_profiles");
        }
    }
}
