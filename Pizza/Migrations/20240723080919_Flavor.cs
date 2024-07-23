using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza.Models
{
    /// <inheritdoc />
    public partial class Flavor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Flavors",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Taste = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flavors", x => x.Id);
                });

            // migrationBuilder.InsertData(
            //     table: "Flavors",
            //     columns: new[] { "Id", "Taste", "Price" },
            //     values: new object[,]
            //     {
            //         { 1, "Tomato", 12f },
            //         { 2, "Mango", 14f },
            //         { 3, "Chilly", 16f },
            //         { 4, "Popcorn", 18f }
            //     });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Flavors");
        }
    }
}
