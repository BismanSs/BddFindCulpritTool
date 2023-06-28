using Microsoft.EntityFrameworkCore.Migrations;

namespace BddFindCulpritTool.Migrations
{
    public partial class initalsetup : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BinarySearchPoint",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    lastBadCommit = table.Column<string>(nullable: true),
                    lastGoodCommit = table.Column<string>(nullable: true),
                    bisectedCommit = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinarySearchPoint", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BinarySearchPrRun",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    GitHash = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    RunStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BinarySearchPrRun", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BackendPath = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrRun",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    GitHash = table.Column<string>(nullable: true),
                    Link = table.Column<string>(nullable: true),
                    RunStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrRun", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BinarySearchPoint");

            migrationBuilder.DropTable(
                name: "BinarySearchPrRun");

            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "PrRun");
        }
    }
}
