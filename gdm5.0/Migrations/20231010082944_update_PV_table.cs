using Microsoft.EntityFrameworkCore.Migrations;

namespace gdm5._0.Migrations
{
    public partial class update_PV_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PriceEUR",
                table: "PriceListValues",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PriceEURNDS",
                table: "PriceListValues",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PriceUSD",
                table: "PriceListValues",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PriceUSDNDS",
                table: "PriceListValues",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PriceEUR",
                table: "PriceListValues");

            migrationBuilder.DropColumn(
                name: "PriceEURNDS",
                table: "PriceListValues");

            migrationBuilder.DropColumn(
                name: "PriceUSD",
                table: "PriceListValues");

            migrationBuilder.DropColumn(
                name: "PriceUSDNDS",
                table: "PriceListValues");
        }
    }
}
