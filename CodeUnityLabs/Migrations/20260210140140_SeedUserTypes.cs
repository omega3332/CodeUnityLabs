using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CodeUnityLabs.Migrations
{
    /// <inheritdoc />
    public partial class SeedUserTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserType_User_Type_Id",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_User_Type_Id",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserTypeUser_Type_Id",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "UserType",
                columns: new[] { "User_Type_Id", "Type_Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Staff" },
                    { 3, "Student" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserTypeUser_Type_Id",
                table: "Users",
                column: "UserTypeUser_Type_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserType_UserTypeUser_Type_Id",
                table: "Users",
                column: "UserTypeUser_Type_Id",
                principalTable: "UserType",
                principalColumn: "User_Type_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserType_UserTypeUser_Type_Id",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserTypeUser_Type_Id",
                table: "Users");

            migrationBuilder.DeleteData(
                table: "UserType",
                keyColumn: "User_Type_Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UserType",
                keyColumn: "User_Type_Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UserType",
                keyColumn: "User_Type_Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "UserTypeUser_Type_Id",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_User_Type_Id",
                table: "Users",
                column: "User_Type_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserType_User_Type_Id",
                table: "Users",
                column: "User_Type_Id",
                principalTable: "UserType",
                principalColumn: "User_Type_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
