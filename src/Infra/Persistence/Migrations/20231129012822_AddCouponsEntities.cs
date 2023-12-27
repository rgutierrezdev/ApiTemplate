using System;
using Microsoft.EntityFrameworkCore.Migrations;
using ApiTemplate.Application.Common.Constants;
using ApiTemplate.Domain.Common;

#nullable disable

namespace ApiTemplate.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCouponsEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Coupon",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    ExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SingleUse = table.Column<bool>(type: "bit", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coupon", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coupon_User_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CouponCompanyUser",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CouponId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CouponCompanyUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CouponCompanyUser_CompanyUser_CompanyUserId",
                        column: x => x.CompanyUserId,
                        principalTable: "CompanyUser",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CouponCompanyUser_Coupon_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupon",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_Code",
                table: "Coupon",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coupon_CreatedByUserId",
                table: "Coupon",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponCompanyUser_CompanyUserId",
                table: "CouponCompanyUser",
                column: "CompanyUserId");

            migrationBuilder.CreateIndex(
                name: "IX_CouponCompanyUser_CouponId",
                table: "CouponCompanyUser",
                column: "CouponId");

            migrationBuilder.Sql(@$"
                INSERT INTO Permission (Id, Name, DisplayName, IsAdmin, CreatedDate) VALUES
                ('{Ulid.NewGuid()}', '{Permissions.CouponsRead}', 'Ver datos de Cupones', 1, GETUTCDATE()),
                ('{Ulid.NewGuid()}', '{Permissions.CouponsWrite}', 'Modificar datos de Cupones', 1, GETUTCDATE());

                DECLARE @adminRoleId UNIQUEIDENTIFIER;
                SELECT @adminRoleId = Id FROM Role WHERE Name = 'Admin';

                INSERT INTO RolePermission (RoleId, PermissionId, CreatedDate)
                SELECT @adminRoleId, Id, GETUTCDATE()
                FROM Permission
                WHERE Name IN ('{Permissions.CouponsRead}', '{Permissions.CouponsWrite}');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CouponCompanyUser");

            migrationBuilder.DropTable(
                name: "Coupon");

            migrationBuilder.Sql(@$"
                DELETE rp
                FROM RolePermission AS rp
                JOIN Permission AS p ON p.Id = rp.PermissionId
                JOIN Role AS r ON r.Id = rp.RoleId
                WHERE r.Name = 'Admin'
                AND p.Name IN ('{nameof(Permissions.CouponsRead)}', '{nameof(Permissions.CouponsWrite)}');

                DELETE FROM Permission
                WHERE Name IN ('{nameof(Permissions.CouponsRead)}', '{nameof(Permissions.CouponsWrite)}')
            ");
        }
    }
}
