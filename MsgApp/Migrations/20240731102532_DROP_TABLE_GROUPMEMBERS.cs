using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsgApp.Migrations
{
    /// <inheritdoc />
    public partial class DROP_TABLE_GROUPMEMBERS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
            name: "GroupMembers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
        }
    }
}
