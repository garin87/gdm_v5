using Microsoft.EntityFrameworkCore.Migrations;

namespace gdm5._0.Migrations
{
    public partial class add_ParameterHistory_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductParameterHistory_Parameters_ParameterId",
                table: "ProductParameterHistory");

            migrationBuilder.RenameColumn(
                name: "ParameterId",
                table: "ProductParameterHistory",
                newName: "ParameterHistoryId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductParameterHistory_ParameterId",
                table: "ProductParameterHistory",
                newName: "IX_ProductParameterHistory_ParameterHistoryId");

            migrationBuilder.CreateTable(
                name: "ParameterHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NameType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParameterHistory", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_ProductParameterHistory_ParameterHistory_ParameterHistoryId",
                table: "ProductParameterHistory",
                column: "ParameterHistoryId",
                principalTable: "ParameterHistory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductParameterHistory_ParameterHistory_ParameterHistoryId",
                table: "ProductParameterHistory");

            migrationBuilder.DropTable(
                name: "ParameterHistory");

            migrationBuilder.RenameColumn(
                name: "ParameterHistoryId",
                table: "ProductParameterHistory",
                newName: "ParameterId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductParameterHistory_ParameterHistoryId",
                table: "ProductParameterHistory",
                newName: "IX_ProductParameterHistory_ParameterId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductParameterHistory_Parameters_ParameterId",
                table: "ProductParameterHistory",
                column: "ParameterId",
                principalTable: "Parameters",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
