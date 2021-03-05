using Microsoft.EntityFrameworkCore.Migrations;

namespace Pustalorc.GlobalBan.Migrations
{
    public partial class FixCast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PlayerId",
                table: "Pustalorc_GlobalBan_PlayerBans",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT UNSIGNED");

            migrationBuilder.AlterColumn<bool>(
                name: "IsUnbanned",
                table: "Pustalorc_GlobalBan_PlayerBans",
                nullable: false,
                oldClrType: typeof(ulong),
                oldType: "bit");

            migrationBuilder.AlterColumn<string>(
                name: "AdminId",
                table: "Pustalorc_GlobalBan_PlayerBans",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "BIGINT UNSIGNED");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PlayerId",
                table: "Pustalorc_GlobalBan_PlayerBans",
                type: "BIGINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<ulong>(
                name: "IsUnbanned",
                table: "Pustalorc_GlobalBan_PlayerBans",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<long>(
                name: "AdminId",
                table: "Pustalorc_GlobalBan_PlayerBans",
                type: "BIGINT UNSIGNED",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}
