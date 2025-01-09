using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aucsystem.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedImageUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageUrl1",
                table: "Properties",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl2",
                table: "Properties",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl3",
                table: "Properties",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl1",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ImageUrl2",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "ImageUrl3",
                table: "Properties");
        }
    }
}
