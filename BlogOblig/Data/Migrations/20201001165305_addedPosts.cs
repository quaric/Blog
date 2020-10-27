using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogOblig.Data.Migrations
{
    public partial class addedPosts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Posts",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Posts_OwnerId",
                table: "Posts",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_OwnerId",
                table: "Posts",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_OwnerId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_OwnerId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Posts");
        }
    }
}
