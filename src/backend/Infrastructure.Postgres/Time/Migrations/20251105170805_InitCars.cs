using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Postgres.Time.Migrations
{
    /// <inheritdoc />
    public partial class InitCars : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.RenameTable(
                name: "testers",
                newName: "testers",
                newSchema: "public");

            migrationBuilder.CreateTable(
                name: "cars",
                schema: "public",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Model = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Owner = table.Column<Guid>(type: "uuid", nullable: false),
                    RegNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cars", x => x.Id);
                    table.ForeignKey(
                        name: "FK_cars_testers_Owner",
                        column: x => x.Owner,
                        principalSchema: "public",
                        principalTable: "testers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_cars_Owner",
                schema: "public",
                table: "cars",
                column: "Owner");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cars",
                schema: "public");

            migrationBuilder.RenameTable(
                name: "testers",
                schema: "public",
                newName: "testers");
        }
    }
}
