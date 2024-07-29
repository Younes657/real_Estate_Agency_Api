using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AgenceImmobiliareApi.Migrations
{
    /// <inheritdoc />
    public partial class fixingsomemodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RealEstates");

            migrationBuilder.DropColumn(
                name: "SubTitle",
                table: "BlogArticles");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "BlogArticles",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "BlogArticles");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RealEstates",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SubTitle",
                table: "BlogArticles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
