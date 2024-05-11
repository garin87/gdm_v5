using Microsoft.EntityFrameworkCore.Migrations;

namespace gdm5._0.Migrations
{
    public partial class update_VP_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PercentOfMarkup",
                table: "PriceListValues",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PercentOfMarkup",
                table: "PriceListValues");
        }
    }
}
