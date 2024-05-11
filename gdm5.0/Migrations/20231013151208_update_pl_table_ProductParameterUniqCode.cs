using Microsoft.EntityFrameworkCore.Migrations;

namespace gdm5._0.Migrations
{
    public partial class update_pl_table_ProductParameterUniqCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductParameterValueCode",
                table: "PriceListValues",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductParameterValueCode",
                table: "PriceListValues");
        }
    }
}
