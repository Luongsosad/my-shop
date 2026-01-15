using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace my_shop.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddVoucherSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "old_price",
                table: "orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "voucher_id",
                table: "orders",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "created_at",
                table: "customers",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.CreateTable(
                name: "vouchers",
                columns: table => new
                {
                    voucher_id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    discount_percent = table.Column<int>(type: "integer", nullable: false),
                    max_discount_amount = table.Column<int>(type: "integer", nullable: false),
                    min_order_amount = table.Column<int>(type: "integer", nullable: false),
                    total_quantity = table.Column<int>(type: "integer", nullable: false),
                    remaining_quantity = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vouchers", x => x.voucher_id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_orders_voucher_id",
                table: "orders",
                column: "voucher_id");

            migrationBuilder.CreateIndex(
                name: "IX_vouchers_code",
                table: "vouchers",
                column: "code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_orders_vouchers_voucher_id",
                table: "orders",
                column: "voucher_id",
                principalTable: "vouchers",
                principalColumn: "voucher_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orders_vouchers_voucher_id",
                table: "orders");

            migrationBuilder.DropTable(
                name: "vouchers");

            migrationBuilder.DropIndex(
                name: "IX_orders_voucher_id",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "old_price",
                table: "orders");

            migrationBuilder.DropColumn(
                name: "voucher_id",
                table: "orders");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "customers",
                type: "timestamp without time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTimeOffset),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");
        }
    }
}
