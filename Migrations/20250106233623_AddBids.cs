using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aucsystem.Migrations
{
    /// <inheritdoc />
    public partial class AddBids : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Auctions_AuctionId",
                table: "Bids");

            migrationBuilder.DropIndex(
                name: "IX_Bids_AuctionId",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "AreaSize",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Bathrooms",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Bedrooms",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "IsAvailableForBuyNow",
                table: "Properties");

            migrationBuilder.RenameColumn(
                name: "TimePlaced",
                table: "Bids",
                newName: "BidDate");

            migrationBuilder.RenameColumn(
                name: "AuctionId",
                table: "Bids",
                newName: "PropertId");

            migrationBuilder.AddColumn<int>(
                name: "PropertyId",
                table: "Bids",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bids_PropertyId",
                table: "Bids",
                column: "PropertyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Properties_PropertyId",
                table: "Bids",
                column: "PropertyId",
                principalTable: "Properties",
                principalColumn: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bids_Properties_PropertyId",
                table: "Bids");

            migrationBuilder.DropIndex(
                name: "IX_Bids_PropertyId",
                table: "Bids");

            migrationBuilder.DropColumn(
                name: "PropertyId",
                table: "Bids");

            migrationBuilder.RenameColumn(
                name: "PropertId",
                table: "Bids",
                newName: "AuctionId");

            migrationBuilder.RenameColumn(
                name: "BidDate",
                table: "Bids",
                newName: "TimePlaced");

            migrationBuilder.AddColumn<int>(
                name: "AreaSize",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Bathrooms",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Bedrooms",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableForBuyNow",
                table: "Properties",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Bids_AuctionId",
                table: "Bids",
                column: "AuctionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bids_Auctions_AuctionId",
                table: "Bids",
                column: "AuctionId",
                principalTable: "Auctions",
                principalColumn: "AuctionId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
