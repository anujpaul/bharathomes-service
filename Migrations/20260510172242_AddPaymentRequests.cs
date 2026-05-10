using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bharathome_api.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "payment_requests",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    plan_code = table.Column<string>(type: "text", nullable: false),
                    plan_tier = table.Column<string>(type: "text", nullable: false),
                    plan_cycle = table.Column<string>(type: "text", nullable: false),
                    amount_inr = table.Column<int>(type: "integer", nullable: false),
                    cardholder_name = table.Column<string>(type: "text", nullable: false),
                    card_last4 = table.Column<string>(type: "text", nullable: false),
                    card_brand = table.Column<string>(type: "text", nullable: false),
                    card_expiry_month = table.Column<int>(type: "integer", nullable: false),
                    card_expiry_year = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    submitted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reviewed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    reviewer_email = table.Column<string>(type: "text", nullable: true),
                    rejection_reason = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_payment_requests_user_profiles_user_id",
                        column: x => x.user_id,
                        principalTable: "user_profiles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_payment_requests_user_id",
                table: "payment_requests",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment_requests");
        }
    }
}
