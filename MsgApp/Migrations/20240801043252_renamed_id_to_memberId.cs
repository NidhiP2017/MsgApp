using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsgApp.Migrations
{
    /// <inheritdoc />
    public partial class renamed_id_to_memberId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "GroupMembers",
                newName: "MemberId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "GroupMembers",
                newName: "Id");
        }
    }
}
