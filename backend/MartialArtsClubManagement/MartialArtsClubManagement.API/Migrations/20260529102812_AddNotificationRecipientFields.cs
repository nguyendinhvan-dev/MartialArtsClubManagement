using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MartialArtsClubManagement.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationRecipientFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NguoiNhan",
                table: "ThongBao",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "TatCa");

            migrationBuilder.AddColumn<int>(
                name: "MaLop",
                table: "ThongBao",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaLop",
                table: "ThongBao");

            migrationBuilder.DropColumn(
                name: "NguoiNhan",
                table: "ThongBao");
        }
    }
}
