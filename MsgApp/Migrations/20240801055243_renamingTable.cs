using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsgApp.Migrations
{
    /// <inheritdoc />
    public partial class renamingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
            name: "GroupMember",
            newName: "GroupMembers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
            name: "GroupMembers",
            newName: "GroupMember");
        }
    }
}
