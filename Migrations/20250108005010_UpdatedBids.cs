using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aucsystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedBids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PropertyName",
                table: "Bids",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PropertyPrice",
                table: "Bids",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PropertyName",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "PropertyPrice",
                table: "Bids");
        }
    }
}
