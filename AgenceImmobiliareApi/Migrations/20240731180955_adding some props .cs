using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgenceImmobiliareApi.Migrations
{
    /// <inheritdoc />
    public partial class addingsomeprops : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TwitterLink",
                table: "WebSiteInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "streetNumber",
                table: "WebSiteInfos",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "UserContacts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Seen",
                table: "UserContacts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NumberBlog",
                table: "BlogArticles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TwitterLink",
                table: "WebSiteInfos");

            migrationBuilder.DropColumn(
                name: "streetNumber",
                table: "WebSiteInfos");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "UserContacts");

            migrationBuilder.DropColumn(
                name: "Seen",
                table: "UserContacts");

            migrationBuilder.DropColumn(
                name: "NumberBlog",
                table: "BlogArticles");
        }
    }
}
