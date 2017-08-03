using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.API.Infrastructure.CatalogMigrations
{
    public partial class initial1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PictureUri",
                table: "Catalog",
                newName: "PictureFileName");

            migrationBuilder.AddColumn<int>(
                name: "AvailableStock",
                table: "Catalog",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MaxStockThreshold",
                table: "Catalog",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "OnReorder",
                table: "Catalog",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "RestockThreshold",
                table: "Catalog",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableStock",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "MaxStockThreshold",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "OnReorder",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "RestockThreshold",
                table: "Catalog");

            migrationBuilder.RenameColumn(
                name: "PictureFileName",
                table: "Catalog",
                newName: "PictureUri");
        }
    }
}
