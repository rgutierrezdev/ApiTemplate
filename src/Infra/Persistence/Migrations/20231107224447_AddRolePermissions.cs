using Microsoft.EntityFrameworkCore.Migrations;
using ApiTemplate.Application.Common.Constants;
using ApiTemplate.Domain.Common;

#nullable disable

namespace ApiTemplate.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddRolePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
                INSERT INTO Role (Id, Name, CreatedDate) VALUES ('{Ulid.NewGuid()}', 'Admin', GETUTCDATE());

                INSERT INTO Permission (Id, Name, DisplayName, CreatedDate) VALUES
                ('{Ulid.NewGuid()}', '{nameof(Permissions.RolesRead)}', 'Ver datos de Roles', GETUTCDATE()),
                ('{Ulid.NewGuid()}', '{nameof(Permissions.RolesWrite)}', 'Modificar datos de Roles', GETUTCDATE());

                DECLARE @adminRoleId UNIQUEIDENTIFIER;
                SELECT @adminRoleId = Id FROM Role WHERE Name = 'Admin';

                INSERT INTO RolePermission (RoleId, PermissionId, CreatedDate)
                SELECT @adminRoleId, Id, GETUTCDATE()
                FROM Permission
                WHERE Name IN ('{nameof(Permissions.RolesRead)}', '{nameof(Permissions.RolesWrite)}');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@$"
                DELETE rp
                FROM RolePermission AS rp
                JOIN Permission AS p ON p.Id = rp.PermissionId
                JOIN Role AS r ON r.Id = rp.RoleId
                WHERE r.Name = 'Admin'
                AND p.Name IN ('{nameof(Permissions.RolesRead)}', '{nameof(Permissions.RolesWrite)}');

                DELETE FROM Permission
                WHERE Name IN ('{nameof(Permissions.RolesRead)}', '{nameof(Permissions.RolesWrite)}')

                DELETE FROM Role
                WHERE Name = 'Admin';
            ");
        }
    }
}
