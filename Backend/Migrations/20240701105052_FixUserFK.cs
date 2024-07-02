using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class FixUserFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blog_AspNetUsers_UserId1",
                table: "Blog");

            migrationBuilder.DropIndex(
                name: "IX_Blog_UserId1",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Blog");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Blog",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_UserId",
                table: "Blog",
                column: "UserId");

            migrationBuilder.Sql("update Blog set UserId = (select top 1 Id from AspNetUsers);"); //not null huney condition ma yo halnu paryo if aagadi ko row ma null xa avney

            migrationBuilder.AddForeignKey(
                name: "FK_Blog_AspNetUsers_UserId",
                table: "Blog",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blog_AspNetUsers_UserId",
                table: "Blog");

            migrationBuilder.DropIndex(
                name: "IX_Blog_UserId",
                table: "Blog");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Blog",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Blog",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blog_UserId1",
                table: "Blog",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Blog_AspNetUsers_UserId1",
                table: "Blog",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
