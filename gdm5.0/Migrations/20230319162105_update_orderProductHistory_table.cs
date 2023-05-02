using Microsoft.EntityFrameworkCore.Migrations;

namespace gdm5._0.Migrations
{
    public partial class update_orderProductHistory_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Markup",
                table: "OrderProductHistory",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TaxNDS",
                table: "OrderProductHistory",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "OrderProductHistory",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Markup",
                table: "OrderProductHistory");

            migrationBuilder.DropColumn(
                name: "TaxNDS",
                table: "OrderProductHistory");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "OrderProductHistory");
        }
    }
}
