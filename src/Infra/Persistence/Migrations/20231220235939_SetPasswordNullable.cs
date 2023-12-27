using Microsoft.EntityFrameworkCore.Migrations;
using ApiTemplate.Application.Common.Constants;
using ApiTemplate.Domain.Common;

#nullable disable

namespace ApiTemplate.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SetPasswordNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "User",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.Sql(@$"
                INSERT INTO Permission (Id, Name, DisplayName, IsAdmin, CreatedDate) VALUES
                ('{Ulid.NewGuid()}', '{Permissions.UsersRead}', 'Ver datos de Usuarios administrativos', 1, GETUTCDATE()),
                ('{Ulid.NewGuid()}', '{Permissions.UsersWrite}', 'Modificar datos de Usuarios administrativos', 1, GETUTCDATE());

                DECLARE @adminRoleId UNIQUEIDENTIFIER;
                SELECT @adminRoleId = Id FROM Role WHERE Name = 'Admin';

                INSERT INTO RolePermission (RoleId, PermissionId, CreatedDate)
                SELECT @adminRoleId, Id, GETUTCDATE()
                FROM Permission
                WHERE Name IN ('{Permissions.UsersRead}', '{Permissions.UsersWrite}');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Password",
                table: "User",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.Sql(@$"
                DELETE rp
                FROM RolePermission AS rp
                JOIN Permission AS p ON p.Id = rp.PermissionId
                JOIN Role AS r ON r.Id = rp.RoleId
                WHERE r.Name = 'Admin'
                AND p.Name IN ('{nameof(Permissions.UsersRead)}', '{nameof(Permissions.UsersWrite)}');

                DELETE FROM Permission
                WHERE Name IN ('{nameof(Permissions.UsersRead)}', '{nameof(Permissions.UsersWrite)}')
            ");
        }
    }
}
