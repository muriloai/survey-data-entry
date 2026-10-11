using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SurveyDataEntry.Infrastructure.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddOperatorAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SurveyAssignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SurveyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OperatorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Blocked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SurveyAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SurveyAssignments_AspNetUsers_OperatorId",
                        column: x => x.OperatorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SurveyAssignments_Surveys_SurveyId",
                        column: x => x.SurveyId,
                        principalTable: "Surveys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAssignments_OperatorId",
                table: "SurveyAssignments",
                column: "OperatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SurveyAssignments_SurveyId_OperatorId",
                table: "SurveyAssignments",
                columns: new[] { "SurveyId", "OperatorId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SurveyAssignments");
        }
    }
}
