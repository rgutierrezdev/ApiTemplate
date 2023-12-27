using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTemplate.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserLastName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lastname",
                table: "User",
                newName: "LastName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "User",
                newName: "Lastname");
        }
    }
}
