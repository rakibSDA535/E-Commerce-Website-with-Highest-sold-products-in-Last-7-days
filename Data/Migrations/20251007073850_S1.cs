using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CardCore.Data.Migrations
{
    /// <inheritdoc />
    public partial class S1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_OrderStatus_OrderStatusID",
                table: "Order");

            migrationBuilder.DropIndex(
                name: "IX_Order_OrderStatusID",
                table: "Order");

            migrationBuilder.RenameColumn(
                name: "OrderStatusID",
                table: "Order",
                newName: "OrderStatus");

            migrationBuilder.AddColumn<int>(
                name: "StatusId",
                table: "OrderStatus",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "OrderStatus");

            migrationBuilder.RenameColumn(
                name: "OrderStatus",
                table: "Order",
                newName: "OrderStatusID");

            migrationBuilder.CreateIndex(
                name: "IX_Order_OrderStatusID",
                table: "Order",
                column: "OrderStatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_OrderStatus_OrderStatusID",
                table: "Order",
                column: "OrderStatusID",
                principalTable: "OrderStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
