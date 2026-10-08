using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using ServiceEvents.Infrastructure.EntityFramework;

#nullable disable

namespace ServiceEvents.Infrastructure.Migrations;

[DbContext(typeof(ServiceEventsDbContext))]
[Migration("20261008112000_AddEventDepartment")]
public partial class AddEventDepartment : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Department",
            table: "Events",
            type: "character varying(200)",
            maxLength: 200,
            nullable: false,
            defaultValue: "");

        migrationBuilder.Sql("""
            UPDATE "Events" AS event
            SET "Department" = organizer."Department"
            FROM "Users" AS organizer
            WHERE event."OrganizerId" = organizer."Id";
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Department",
            table: "Events");
    }
}
