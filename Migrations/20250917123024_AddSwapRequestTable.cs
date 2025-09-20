using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SkillSwapApp.Migrations
{
    public partial class AddSwapRequestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SwapRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FromUserId = table.Column<string>(nullable: false),
                    ToUserId = table.Column<string>(nullable: false),
                    OfferedSkill = table.Column<string>(nullable: false),
                    NeededSkill = table.Column<string>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SwapRequests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SwapRequests");
        }
    }
}
