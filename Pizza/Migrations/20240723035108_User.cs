﻿using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pizza.Migrations
{
    /// <inheritdoc />
    public partial class User : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                            name: "Users",
                            columns: table => new
                            {
                                Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                                Name = table.Column<string>(nullable: true),
                                Username = table.Column<string>(nullable: true),
                                Password = table.Column<string>(nullable: true),
                                Role = table.Column<string>(nullable: true),
                                Phone_Number = table.Column<string>(nullable: true),
                                Address = table.Column<string>(nullable: true)
                            },
                            constraints: table =>
                            {
                                table.PrimaryKey("PK_Users", x => x.Id);
                            });
            //             migrationBuilder.InsertData(
            //                                 table: "Users",
            //                                 columns: new[] { "Id", "Name", "Username", "Password", "Role", "Phone_Number", "Address" },
            //                                 values: new object[,]
            // {
            //                                             {1,  "Admin", "admin", "admin", "Admin", "123-456-7890", "123 Main St" },
            //                                             {2,  "Customer", "baovo", "baovo", "Customer", "987-654-3210", "456 Elm St" }
            // });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

        }
    }
}
