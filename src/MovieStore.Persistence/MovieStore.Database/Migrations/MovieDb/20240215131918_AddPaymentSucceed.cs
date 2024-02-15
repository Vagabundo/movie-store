using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieStore.Database.Migrations.MovieDb
{
    /// <inheritdoc />
    public partial class AddPaymentSucceed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Branch",
                table: "Orders",
                newName: "BranchId");

            migrationBuilder.AddColumn<bool>(
                name: "PaymentSucceed",
                table: "Orders",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_BranchId",
                table: "Orders",
                column: "BranchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Branches_BranchId",
                table: "Orders",
                column: "BranchId",
                principalTable: "Branches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Branches_BranchId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_BranchId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentSucceed",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "BranchId",
                table: "Orders",
                newName: "Branch");
        }
    }
}
