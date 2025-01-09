using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aucsystem.Migrations
{
    /// <inheritdoc />
    public partial class AddBidFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Properties_PropertyId",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "PropertId",
                table: "Bids");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyId",
                table: "Bids",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Properties_PropertyId",
                table: "Bids",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "PropertyId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Properties_PropertyId",
                table: "Bids");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyId",
                table: "Bids",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "PropertId",
                table: "Bids",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Properties_PropertyId",
                table: "Bids",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "PropertyId");
        }
    }
}
