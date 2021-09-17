using Microsoft.EntityFrameworkCore.Migrations;

namespace Movies.Domain.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            #pragma warning disable CA1062
            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Genre = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Title",
                table: "Movies",
                column: "Title",
                unique: true,
                filter: "[Title] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            #pragma warning disable CA1062
            migrationBuilder.DropTable(name: "Movies");
        }
    }
}
