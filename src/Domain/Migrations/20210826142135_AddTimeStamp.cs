using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Movies.Domain.Migrations
{
    public partial class AddTimeStamp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "TimeStamp",
                table: "Movies",
                type: "rowversion",
                rowVersion: true,
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "Movies");
        }
    }
}
