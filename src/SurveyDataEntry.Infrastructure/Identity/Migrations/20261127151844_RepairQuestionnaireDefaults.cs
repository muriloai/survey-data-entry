using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyDataEntry.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class RepairQuestionnaireDefaults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Surveys SET QuestionsJson = N'[]' WHERE QuestionsJson = N''");
            migrationBuilder.AlterColumn<string>(
                name: "QuestionsJson",
                table: "Surveys",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Surveys SET QuestionsJson = N'[]' WHERE QuestionsJson = N''");
            migrationBuilder.AlterColumn<string>(
                name: "QuestionsJson",
                table: "Surveys",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "[]");
        }
    }
}

