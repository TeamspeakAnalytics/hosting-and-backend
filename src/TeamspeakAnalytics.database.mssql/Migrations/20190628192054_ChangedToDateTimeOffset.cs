using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TeamspeakAnalytics.database.mssql.Migrations
{
    public partial class ChangedToDateTimeOffset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastConnected",
                table: "TS3Clients",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "Created",
                table: "TS3Clients",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "ChangeDate",
                table: "TS3Clients",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "TimeStampStart",
                table: "TS3ClientConnection",
                nullable: false,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "TimeStampEnd",
                table: "TS3ClientConnection",
                nullable: false,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastConnected",
                table: "TS3Clients",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "Created",
                table: "TS3Clients",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "ChangeDate",
                table: "TS3Clients",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStampStart",
                table: "TS3ClientConnection",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "TimeStampEnd",
                table: "TS3ClientConnection",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));
        }
    }
}
