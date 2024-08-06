using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MsgApp.Migrations
{
    /// <inheritdoc />
    public partial class addingGroupMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
               name: "GroupMembers",
               columns: table => new
               {
                   Id = table.Column<int>(type: "int", nullable:false),
                   UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                   GrpId = table.Column<int>(type: "int", nullable: false),
                   JoinTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                   IncludePreviousChat = table.Column<bool>(type: "bit", nullable: false)
               },
               constraints: table =>
               {
                   table.PrimaryKey("PK_GroupMembers", x => new { x.UserId, x.GrpId });
                   table.ForeignKey(
                       name: "FK_GroupMembers_AspNetUsers_UserId",
                       column: x => x.UserId,
                       principalTable: "AspNetUsers",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
                   table.ForeignKey(
                       name: "FK_GroupMembers_Groups_GroupId",
                       column: x => x.GrpId,
                       principalTable: "Groups",
                       principalColumn: "Id",
                       onDelete: ReferentialAction.Cascade);
               });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
