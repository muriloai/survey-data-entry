using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyDataEntry.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionnaire : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "QuestionsJson",
                table: "Surveys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "StructureLocked",
                table: "Surveys",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "Version",
                table: "Surveys",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuestionsJson",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "StructureLocked",
                table: "Surveys");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Surveys");
        }
    }
}
