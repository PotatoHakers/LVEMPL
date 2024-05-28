using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AutEmplAcc.Migrations
{
    /// <inheritdoc />
    public partial class update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SickLeaves_CandidateEmployees_CandidateEmployeeId",
                table: "SickLeaves");

            migrationBuilder.DropForeignKey(
                name: "FK_Vacations_CandidateEmployees_CandidateEmployeeId",
                table: "Vacations");

            migrationBuilder.DropTable(
                name: "CandidateEmployees");

            migrationBuilder.DropIndex(
                name: "IX_Vacations_CandidateEmployeeId",
                table: "Vacations");

            migrationBuilder.DropIndex(
                name: "IX_SickLeaves_CandidateEmployeeId",
                table: "SickLeaves");

            migrationBuilder.DropColumn(
                name: "CandidateEmployeeId",
                table: "Vacations");

            migrationBuilder.DropColumn(
                name: "CandidateEmployeeId",
                table: "SickLeaves");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CandidateEmployeeId",
                table: "Vacations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CandidateEmployeeId",
                table: "SickLeaves",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CandidateEmployees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CandidateId = table.Column<int>(type: "int", nullable: false),
                    PositionId = table.Column<int>(type: "int", nullable: false),
                    HiringDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HoursWorked = table.Column<int>(type: "int", nullable: false),
                    Rate = table.Column<double>(type: "float", nullable: false),
                    WorkExperience = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CandidateEmployees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CandidateEmployees_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CandidateEmployees_Candidates_CandidateId",
                        column: x => x.CandidateId,
                        principalTable: "Candidates",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CandidateEmployees_Positions_PositionId",
                        column: x => x.PositionId,
                        principalTable: "Positions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vacations_CandidateEmployeeId",
                table: "Vacations",
                column: "CandidateEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_SickLeaves_CandidateEmployeeId",
                table: "SickLeaves",
                column: "CandidateEmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEmployees_BranchId",
                table: "CandidateEmployees",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEmployees_CandidateId",
                table: "CandidateEmployees",
                column: "CandidateId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateEmployees_PositionId",
                table: "CandidateEmployees",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_SickLeaves_CandidateEmployees_CandidateEmployeeId",
                table: "SickLeaves",
                column: "CandidateEmployeeId",
                principalTable: "CandidateEmployees",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vacations_CandidateEmployees_CandidateEmployeeId",
                table: "Vacations",
                column: "CandidateEmployeeId",
                principalTable: "CandidateEmployees",
                principalColumn: "Id");
        }
    }
}
