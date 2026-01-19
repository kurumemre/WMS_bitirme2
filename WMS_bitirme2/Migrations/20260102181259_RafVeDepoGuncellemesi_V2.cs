using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMS_bitirme2.Migrations
{
    /// <inheritdoc />
    public partial class RafVeDepoGuncellemesi_V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "PurchaseOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShelfId",
                table: "PurchaseOrderItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_WarehouseId",
                table: "PurchaseOrders",
                column: "WarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_ShelfId",
                table: "PurchaseOrderItems",
                column: "ShelfId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrderItems_Shelves_ShelfId",
                table: "PurchaseOrderItems",
                column: "ShelfId",
                principalTable: "Shelves",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseOrders_Warehouses_WarehouseId",
                table: "PurchaseOrders",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrderItems_Shelves_ShelfId",
                table: "PurchaseOrderItems");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseOrders_Warehouses_WarehouseId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrders_WarehouseId",
                table: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseOrderItems_ShelfId",
                table: "PurchaseOrderItems");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "PurchaseOrders");

            migrationBuilder.DropColumn(
                name: "ShelfId",
                table: "PurchaseOrderItems");
        }
    }
}
