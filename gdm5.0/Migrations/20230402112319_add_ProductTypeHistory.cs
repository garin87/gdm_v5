using Microsoft.EntityFrameworkCore.Migrations;

namespace gdm5._0.Migrations
{
    public partial class add_ProductTypeHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductTypeHistoryId",
                table: "ProductHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductTypeHistoryId",
                table: "ParameterHistory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductTypeHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductTypeHistory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductHistory_ProductTypeHistoryId",
                table: "ProductHistory",
                column: "ProductTypeHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ParameterHistory_ProductTypeHistoryId",
                table: "ParameterHistory",
                column: "ProductTypeHistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParameterHistory_ProductTypeHistory_ProductTypeHistoryId",
                table: "ParameterHistory",
                column: "ProductTypeHistoryId",
                principalTable: "ProductTypeHistory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductHistory_ProductTypeHistory_ProductTypeHistoryId",
                table: "ProductHistory",
                column: "ProductTypeHistoryId",
                principalTable: "ProductTypeHistory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParameterHistory_ProductTypeHistory_ProductTypeHistoryId",
                table: "ParameterHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductHistory_ProductTypeHistory_ProductTypeHistoryId",
                table: "ProductHistory");

            migrationBuilder.DropTable(
                name: "ProductTypeHistory");

            migrationBuilder.DropIndex(
                name: "IX_ProductHistory_ProductTypeHistoryId",
                table: "ProductHistory");

            migrationBuilder.DropIndex(
                name: "IX_ParameterHistory_ProductTypeHistoryId",
                table: "ParameterHistory");

            migrationBuilder.DropColumn(
                name: "ProductTypeHistoryId",
                table: "ProductHistory");

            migrationBuilder.DropColumn(
                name: "ProductTypeHistoryId",
                table: "ParameterHistory");
        }
    }
}
