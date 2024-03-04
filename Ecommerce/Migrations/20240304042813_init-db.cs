using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class initdb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Images_Id",
                table: "ProductImages");

            migrationBuilder.AlterColumn<string>(
                name: "ImageId",
                table: "ProductImages",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductImages_ImageId",
                table: "ProductImages",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Images_ImageId",
                table: "ProductImages",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "ImageId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductImages_Images_ImageId",
                table: "ProductImages");

            migrationBuilder.DropIndex(
                name: "IX_ProductImages_ImageId",
                table: "ProductImages");

            migrationBuilder.AlterColumn<string>(
                name: "ImageId",
                table: "ProductImages",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductImages_Images_Id",
                table: "ProductImages",
                column: "Id",
                principalTable: "Images",
                principalColumn: "ImageId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
