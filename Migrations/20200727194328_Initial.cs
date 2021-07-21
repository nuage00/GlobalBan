using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Pustalorc.GlobalBan.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Pustalorc_GlobalBan_PlayerBans",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PlayerId = table.Column<long>(type: "BIGINT UNSIGNED", nullable: false),
                    Ip = table.Column<long>(nullable: false),
                    Hwid = table.Column<string>(maxLength: 64, nullable: false),
                    AdminId = table.Column<long>(type: "BIGINT UNSIGNED", nullable: false),
                    Reason = table.Column<string>(maxLength: 512, nullable: false),
                    Duration = table.Column<long>(nullable: false),
                    ServerId = table.Column<int>(nullable: false),
                    TimeOfBan = table.Column<DateTime>(nullable: false),
                    IsUnbanned = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pustalorc_GlobalBan_PlayerBans", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Pustalorc_GlobalBan_PlayerBans");
        }
    }
}
