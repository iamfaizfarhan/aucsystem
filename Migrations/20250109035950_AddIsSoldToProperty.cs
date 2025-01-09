﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aucsystem.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSoldToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSold",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSold",
                table: "Properties");
        }
    }
}
