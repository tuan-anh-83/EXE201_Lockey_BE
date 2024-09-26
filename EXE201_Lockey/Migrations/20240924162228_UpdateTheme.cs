﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EXE201_Lockey.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTheme : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "Theme",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Theme");
        }
    }
}