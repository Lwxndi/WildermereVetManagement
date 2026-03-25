using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IntergratedProject.Migrations
{
    /// <inheritdoc />
    public partial class MedicalMade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecordFile_MedicalRecord_MedicalRecordId",
                table: "MedicalRecordFile");

            migrationBuilder.AddColumn<int>(
                name: "CurrentHospitalId",
                table: "Pet",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Pet",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrescription",
                table: "MedicalRecordFile",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSigned",
                table: "MedicalRecord",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "SignedAt",
                table: "MedicalRecord",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SignedByRole",
                table: "MedicalRecord",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecordFile_MedicalRecord_MedicalRecordId",
                table: "MedicalRecordFile",
                column: "MedicalRecordId",
                principalTable: "MedicalRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecordFile_MedicalRecord_MedicalRecordId",
                table: "MedicalRecordFile");

            migrationBuilder.DropColumn(
                name: "CurrentHospitalId",
                table: "Pet");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Pet");

            migrationBuilder.DropColumn(
                name: "IsPrescription",
                table: "MedicalRecordFile");

            migrationBuilder.DropColumn(
                name: "IsSigned",
                table: "MedicalRecord");

            migrationBuilder.DropColumn(
                name: "SignedAt",
                table: "MedicalRecord");

            migrationBuilder.DropColumn(
                name: "SignedByRole",
                table: "MedicalRecord");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecordFile_MedicalRecord_MedicalRecordId",
                table: "MedicalRecordFile",
                column: "MedicalRecordId",
                principalTable: "MedicalRecord",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
