using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LoginBackend.Migrations
{
    /// <inheritdoc />
    public partial class cambios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Favoritos_Monster_MonsterId",
                table: "Favoritos");

            migrationBuilder.DropTable(
                name: "Monster");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Favoritos",
                table: "Favoritos");

            migrationBuilder.DropIndex(
                name: "IX_Favoritos_Correo_MonsterId",
                table: "Favoritos");

            migrationBuilder.DropIndex(
                name: "IX_Favoritos_MonsterId",
                table: "Favoritos");

            migrationBuilder.DropColumn(
                name: "Correo",
                table: "Favoritos");

            migrationBuilder.DropColumn(
                name: "MonsterId",
                table: "Favoritos");

            migrationBuilder.RenameTable(
                name: "Favoritos",
                newName: "MonstersFavoritos");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "MonstersFavoritos",
                newName: "UserId");

            migrationBuilder.AddColumn<string>(
                name: "MonsterName",
                table: "MonstersFavoritos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MonstersFavoritos",
                table: "MonstersFavoritos",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MonstersFavoritos",
                table: "MonstersFavoritos");

            migrationBuilder.DropColumn(
                name: "MonsterName",
                table: "MonstersFavoritos");

            migrationBuilder.RenameTable(
                name: "MonstersFavoritos",
                newName: "Favoritos");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Favoritos",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Correo",
                table: "Favoritos",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "MonsterId",
                table: "Favoritos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Favoritos",
                table: "Favoritos",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Monster",
                columns: table => new
                {
                    MonsterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monster", x => x.MonsterId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_Correo_MonsterId",
                table: "Favoritos",
                columns: new[] { "Correo", "MonsterId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favoritos_MonsterId",
                table: "Favoritos",
                column: "MonsterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Favoritos_Monster_MonsterId",
                table: "Favoritos",
                column: "MonsterId",
                principalTable: "Monster",
                principalColumn: "MonsterId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
