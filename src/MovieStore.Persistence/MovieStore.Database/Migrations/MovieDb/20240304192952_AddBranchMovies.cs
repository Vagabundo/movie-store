using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MovieStore.Database.Migrations.MovieDb
{
    /// <inheritdoc />
    public partial class AddBranchMovies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Branches",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "BranchMovies",
                columns: table => new
                {
                    BranchId = table.Column<Guid>(type: "TEXT", nullable: false),
                    MovieId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "TEXT", nullable: false),
                    ModifiedBy = table.Column<Guid>(type: "TEXT", nullable: false),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BranchMovies", x => new { x.MovieId, x.BranchId });
                    table.ForeignKey(
                        name: "FK_BranchMovies_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BranchMovies_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchMovies_BranchId",
                table: "BranchMovies",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_BranchMovies_MovieId_BranchId",
                table: "BranchMovies",
                columns: new[] { "MovieId", "BranchId" },
                unique: true,
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BranchMovies");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Branches",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldDefaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
