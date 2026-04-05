using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmurfDAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Forests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Creatures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Health = table.Column<int>(type: "int", nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    ForestId = table.Column<int>(type: "int", nullable: false),
                    CreatureType = table.Column<string>(type: "nvarchar(8)", maxLength: 8, nullable: false),
                    Speed = table.Column<int>(type: "int", nullable: true),
                    Damage = table.Column<int>(type: "int", nullable: true),
                    MagicLevel = table.Column<int>(type: "int", nullable: true),
                    WebStrength = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Creatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Creatures_Forests_ForestId",
                        column: x => x.ForestId,
                        principalTable: "Forests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    X = table.Column<int>(type: "int", nullable: false),
                    Y = table.Column<int>(type: "int", nullable: false),
                    ForestId = table.Column<int>(type: "int", nullable: false),
                    ItemType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    HealthBoost = table.Column<int>(type: "int", nullable: true),
                    GlobalHealAmount = table.Column<int>(type: "int", nullable: true),
                    BoostAmount = table.Column<int>(type: "int", nullable: true),
                    IsGolden = table.Column<bool>(type: "bit", nullable: true),
                    IsCollected = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Items_Forests_ForestId",
                        column: x => x.ForestId,
                        principalTable: "Forests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Creatures_ForestId",
                table: "Creatures",
                column: "ForestId");

            migrationBuilder.CreateIndex(
                name: "IX_Creatures_X_Y",
                table: "Creatures",
                columns: new[] { "X", "Y" });

            migrationBuilder.CreateIndex(
                name: "IX_Items_ForestId",
                table: "Items",
                column: "ForestId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_X_Y",
                table: "Items",
                columns: new[] { "X", "Y" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Creatures");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "Forests");
        }
    }
}
