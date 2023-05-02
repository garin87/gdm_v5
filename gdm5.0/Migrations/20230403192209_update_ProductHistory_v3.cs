using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace gdm5._0.Migrations
{
    public partial class update_ProductHistory_v3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeletedProductTypeId",
                table: "ProductTypeHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PrimeCostEUR",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PrimeCostUSD",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOfReceipt",
                table: "ProductHistory",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeletedProductTypeId",
                table: "ProductHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PrimeCostEUR",
                table: "ProductHistory",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PrimeCostUSD",
                table: "ProductHistory",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Supplier",
                table: "ProductHistory",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedParameterId",
                table: "ParameterHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeletedProductTypeId",
                table: "ParameterHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedProductTypeId",
                table: "ProductTypeHistory");

            migrationBuilder.DropColumn(
                name: "PrimeCostEUR",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "PrimeCostUSD",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "DateOfReceipt",
                table: "ProductHistory");

            migrationBuilder.DropColumn(
                name: "DeletedProductTypeId",
                table: "ProductHistory");

            migrationBuilder.DropColumn(
                name: "PrimeCostEUR",
                table: "ProductHistory");

            migrationBuilder.DropColumn(
                name: "PrimeCostUSD",
                table: "ProductHistory");

            migrationBuilder.DropColumn(
                name: "Supplier",
                table: "ProductHistory");

            migrationBuilder.DropColumn(
                name: "DeletedParameterId",
                table: "ParameterHistory");

            migrationBuilder.DropColumn(
                name: "DeletedProductTypeId",
                table: "ParameterHistory");
        }
    }
}
