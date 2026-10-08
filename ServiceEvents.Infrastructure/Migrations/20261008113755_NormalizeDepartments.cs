using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceEvents.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeDepartments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.Sql("""
                INSERT INTO "Departments" ("Id", "Name")
                SELECT gen_random_uuid(), MIN(department_name)
                FROM (
                    SELECT NULLIF(BTRIM("Department"), '') AS department_name FROM "Users"
                    UNION ALL
                    SELECT NULLIF(BTRIM("Department"), '') AS department_name FROM "Events"
                ) AS legacy_departments
                WHERE department_name IS NOT NULL
                GROUP BY LOWER(department_name);

                INSERT INTO "Departments" ("Id", "Name")
                SELECT gen_random_uuid(), 'Без департамента'
                WHERE NOT EXISTS (
                    SELECT 1 FROM "Departments" WHERE "Name" = 'Без департамента'
                );
                """);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DepartmentId",
                table: "Events",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE "Users" AS users
                SET "DepartmentId" = departments."Id"
                FROM "Departments" AS departments
                WHERE LOWER(NULLIF(BTRIM(users."Department"), '')) = LOWER(departments."Name")
                   OR (NULLIF(BTRIM(users."Department"), '') IS NULL AND departments."Name" = 'Без департамента');

                UPDATE "Events" AS events
                SET "DepartmentId" = departments."Id"
                FROM "Departments" AS departments
                WHERE LOWER(NULLIF(BTRIM(events."Department"), '')) = LOWER(departments."Name")
                   OR (NULLIF(BTRIM(events."Department"), '') IS NULL AND departments."Name" = 'Без департамента');
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "Users",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "DepartmentId",
                table: "Events",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "Department",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Department",
                table: "Events");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_DepartmentId",
                table: "Events",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_Name",
                table: "Departments",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Departments_DepartmentId",
                table: "Events",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Departments_DepartmentId",
                table: "Users",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Departments_DepartmentId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Departments_DepartmentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DepartmentId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Events_DepartmentId",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Users",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "Events",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE "Users" AS users
                SET "Department" = departments."Name"
                FROM "Departments" AS departments
                WHERE users."DepartmentId" = departments."Id";

                UPDATE "Events" AS events
                SET "Department" = departments."Name"
                FROM "Departments" AS departments
                WHERE events."DepartmentId" = departments."Id";
                """);

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Events");

            migrationBuilder.DropTable(name: "Departments");
        }
    }
}
