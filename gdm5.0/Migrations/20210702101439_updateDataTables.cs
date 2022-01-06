using Microsoft.EntityFrameworkCore.Migrations;

namespace gdm5._0.Migrations
{
    public partial class updateDataTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductStandartCost",
                table: "Products",
                newName: "StandartCost");

            migrationBuilder.RenameColumn(
                name: "PriceWithTaxNDS",
                table: "Orders",
                newName: "TaxNDS");

            migrationBuilder.AddColumn<int>(
                name: "WareHouseId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Priority",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PriorityColor",
                table: "Customer",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "WareHouse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationDetails = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WareHouse", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_WareHouseId",
                table: "Products",
                column: "WareHouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_WareHouse_WareHouseId",
                table: "Products",
                column: "WareHouseId",
                principalTable: "WareHouse",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_WareHouse_WareHouseId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "WareHouse");

            migrationBuilder.DropIndex(
                name: "IX_Products_WareHouseId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "WareHouseId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Customer");

            migrationBuilder.DropColumn(
                name: "PriorityColor",
                table: "Customer");

            migrationBuilder.RenameColumn(
                name: "StandartCost",
                table: "Products",
                newName: "ProductStandartCost");

            migrationBuilder.RenameColumn(
                name: "TaxNDS",
                table: "Orders",
                newName: "PriceWithTaxNDS");
        }
    }
}
