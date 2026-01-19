using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS_bitirme2.Migrations
{
    /// <inheritdoc />
    public partial class RafIdBosOlabilir : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Shelves_ShelfId",
                table: "StockMovements");

            migrationBuilder.AlterColumn<int>(
                name: "ShelfId",
                table: "StockMovements",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Shelves_ShelfId",
                table: "StockMovements",
                column: "ShelfId",
                principalTable: "Shelves",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StockMovements_Shelves_ShelfId",
                table: "StockMovements");

            migrationBuilder.AlterColumn<int>(
                name: "ShelfId",
                table: "StockMovements",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StockMovements_Shelves_ShelfId",
                table: "StockMovements",
                column: "ShelfId",
                principalTable: "Shelves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
