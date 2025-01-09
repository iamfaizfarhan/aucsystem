using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aucsystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedPropertDBModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Properties",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Bids",
                newName: "BidId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Auctions",
                newName: "AuctionId");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyId",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "SellerId",
                table: "Properties",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Properties",
                table: "Properties",
                column: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Properties",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "BidId",
                table: "Bids",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "AuctionId",
                table: "Auctions",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "PropertyId",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Properties",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Properties",
                table: "Properties",
                column: "Id");
        }
    }
}
