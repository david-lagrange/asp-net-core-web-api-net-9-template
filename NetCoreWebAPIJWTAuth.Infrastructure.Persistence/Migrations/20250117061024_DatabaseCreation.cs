using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreWebAPIJWTAuth.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class DatabaseCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    BaseEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Address = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Country = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.BaseEntityId);
                });

            migrationBuilder.CreateTable(
                name: "DependantEntitys",
                columns: table => new
                {
                    DependantEntityId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    Age = table.Column<int>(type: "integer", nullable: false),
                    Position = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BaseEntityId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DependantEntitys", x => x.DependantEntityId);
                    table.ForeignKey(
                        name: "FK_DependantEntitys_Companies_BaseEntityId",
                        column: x => x.BaseEntityId,
                        principalTable: "Companies",
                        principalColumn: "BaseEntityId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DependantEntitys_BaseEntityId",
                table: "DependantEntitys",
                column: "BaseEntityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DependantEntitys");

            migrationBuilder.DropTable(
                name: "Companies");
        }
    }
}
