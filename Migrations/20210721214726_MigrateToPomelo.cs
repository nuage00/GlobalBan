using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Pustalorc.GlobalBan.Migrations
{
    public partial class MigrateToPomelo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeOfBan",
                table: "Pustalorc_GlobalBan_PlayerBans",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime");

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Pustalorc_GlobalBan_PlayerBans",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(512)",
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<ulong>(
                name: "PlayerId",
                table: "Pustalorc_GlobalBan_PlayerBans",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<uint>(
                name: "Ip",
                table: "Pustalorc_GlobalBan_PlayerBans",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "Hwid",
                table: "Pustalorc_GlobalBan_PlayerBans",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(64)",
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<uint>(
                name: "Duration",
                table: "Pustalorc_GlobalBan_PlayerBans",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<ulong>(
                name: "AdminId",
                table: "Pustalorc_GlobalBan_PlayerBans",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeOfBan",
                table: "Pustalorc_GlobalBan_PlayerBans",
                type: "datetime",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<string>(
                name: "Reason",
                table: "Pustalorc_GlobalBan_PlayerBans",
                type: "varchar(512)",
                maxLength: 512,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 512);

            migrationBuilder.AlterColumn<string>(
                name: "PlayerId",
                table: "Pustalorc_GlobalBan_PlayerBans",
                type: "text",
                nullable: false,
                oldClrType: typeof(ulong));

            migrationBuilder.AlterColumn<long>(
                name: "Ip",
                table: "Pustalorc_GlobalBan_PlayerBans",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<string>(
                name: "Hwid",
                table: "Pustalorc_GlobalBan_PlayerBans",
                type: "varchar(64)",
                maxLength: 64,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 64);

            migrationBuilder.AlterColumn<long>(
                name: "Duration",
                table: "Pustalorc_GlobalBan_PlayerBans",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(uint));

            migrationBuilder.AlterColumn<string>(
                name: "AdminId",
                table: "Pustalorc_GlobalBan_PlayerBans",
                type: "text",
                nullable: false,
                oldClrType: typeof(ulong));
        }
    }
}
