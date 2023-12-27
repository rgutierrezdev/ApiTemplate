using System;
using Microsoft.EntityFrameworkCore.Migrations;
using ApiTemplate.Domain.Common;

#nullable disable

namespace ApiTemplate.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyProfileChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_SignedFile_RegistrationSignedFileId",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyDocumentFile_File_FileId",
                table: "CompanyDocumentFile");

            migrationBuilder.DropForeignKey(
                name: "FK_SignedFile_File_FileId",
                table: "SignedFile");

            migrationBuilder.DropIndex(
                name: "IX_SignedFile_FileId",
                table: "SignedFile");

            migrationBuilder.DropIndex(
                name: "IX_CompanyDocumentFile_FileId",
                table: "CompanyDocumentFile");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "SignedFile");

            migrationBuilder.DropColumn(
                name: "FileId",
                table: "CompanyDocumentFile");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "CompanyDocument");

            migrationBuilder.RenameColumn(
                name: "RegistrationSignedFileId",
                table: "Company",
                newName: "LogoFileId");

            migrationBuilder.RenameColumn(
                name: "PaymentType",
                table: "Company",
                newName: "YearlyIncome");

            migrationBuilder.Sql($@"
                UPDATE Company SET LogoFileId=NULL;
                UPDATE Company SET YearlyIncome=NULL;
            ");

            migrationBuilder.RenameIndex(
                name: "IX_Company_RegistrationSignedFileId",
                table: "Company",
                newName: "IX_Company_LogoFileId");

            migrationBuilder.AddColumn<bool>(
                name: "MarkForDeletion",
                table: "File",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Public",
                table: "File",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Owner",
                table: "CompanyUser",
                type: "bit",
                nullable: true);

            migrationBuilder.Sql(@$"
                UPDATE [File] SET [MarkForDeletion]=0 WHERE [MarkForDeletion] IS NULL;
                UPDATE [File] SET [Public]=0 WHERE [Public] IS NULL;
                UPDATE [CompanyUser] SET [Owner]=1 WHERE [Owner] IS NULL;
            ");

            migrationBuilder.AlterColumn<bool>(
                name: "MarkForDeletion",
                table: "File",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Public",
                table: "File",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Owner",
                table: "CompanyUser",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ChangeCompanyDocumentFileId",
                table: "CompanyDocumentFile",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CreditEnabled",
                table: "CompanyDocument",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsChange",
                table: "CompanyAssociate",
                type: "bit",
                nullable: true);

            migrationBuilder.Sql(@$"
                UPDATE [CompanyAssociate] SET [IsChange]=0 WHERE [IsChange] IS NULL;
            ");

            migrationBuilder.AlterColumn<bool>(
                name: "IsChange",
                table: "CompanyAssociate",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AboutUs",
                table: "Company",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssociatesReviewMessage",
                table: "Company",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssociatesReviewStatus",
                table: "Company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillingTaxesReviewMessage",
                table: "Company",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "BillingTaxesReviewStatus",
                table: "Company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyInfoReviewMessage",
                table: "Company",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CompanyInfoReviewStatus",
                table: "Company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ConstitutionDate",
                table: "Company",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactEmail",
                table: "Company",
                type: "nvarchar(320)",
                maxLength: 320,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CreditEnabled",
                table: "Company",
                type: "bit",
                nullable: true);

            migrationBuilder.Sql(@$"
                UPDATE [Company] SET [CreditEnabled]=0 WHERE [CreditEnabled] IS NULL;
            ");

            migrationBuilder.AlterColumn<bool>(
                name: "CreditEnabled",
                table: "Company",
                type: "bit",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CreditLimit",
                table: "Company",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreditReviewMessage",
                table: "Company",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreditReviewStatus",
                table: "Company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DocumentsReviewStatus",
                table: "Company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EmployeesNumber",
                table: "Company",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebsiteUrl",
                table: "Company",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyAddress",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdditionalInfo = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Enabled = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyAddress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyAddress_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompanyAddress_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanyChange",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LegalType = table.Column<int>(type: "int", nullable: true),
                    LegalName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    CiiuCode = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    PersonType = table.Column<int>(type: "int", nullable: true),
                    BusinessStructureId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DocumentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Document = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    VerificationDigit = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LegalRepresentativeFirstName = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    LegalRepresentativeLastName = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: true),
                    LegalRepresentativeEmail = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    LegalRepresentativeDocumentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LegalRepresentativeDocument = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DocumentsReviewStatus = table.Column<int>(type: "int", nullable: true),
                    CompanyInfoReviewStatus = table.Column<int>(type: "int", nullable: true),
                    CompanyInfoReviewMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RetentionSubject = table.Column<bool>(type: "bit", nullable: true),
                    RequiredToDeclareIncome = table.Column<bool>(type: "bit", nullable: true),
                    VatResponsible = table.Column<bool>(type: "bit", nullable: true),
                    DianGreatContributor = table.Column<bool>(type: "bit", nullable: true),
                    DianGreatContributorRes = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    SalesRetentionAgent = table.Column<bool>(type: "bit", nullable: true),
                    SalesRetentionAgentRes = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    IncomeSelfRetainer = table.Column<bool>(type: "bit", nullable: true),
                    IncomeSelfRetainerRes = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    Regime = table.Column<int>(type: "int", nullable: true),
                    IcaActivity = table.Column<string>(type: "nvarchar(4)", maxLength: 4, nullable: true),
                    IcaAutoRetainer = table.Column<bool>(type: "bit", nullable: true),
                    IcaAutoRetainerRes = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    BillingTaxesReviewStatus = table.Column<int>(type: "int", nullable: true),
                    BillingTaxesReviewMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreditEnabled = table.Column<bool>(type: "bit", nullable: true),
                    AuthorizesFinancialInformation = table.Column<bool>(type: "bit", nullable: true),
                    CreditReviewStatus = table.Column<int>(type: "int", nullable: true),
                    CreditReviewMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    HasPepRelative = table.Column<bool>(type: "bit", nullable: true),
                    UnderOath = table.Column<bool>(type: "bit", nullable: true),
                    AssociatesReviewStatus = table.Column<int>(type: "int", nullable: true),
                    AssociatesReviewMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyChange", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyChange_BusinessStructure_BusinessStructureId",
                        column: x => x.BusinessStructureId,
                        principalTable: "BusinessStructure",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompanyChange_City_CityId",
                        column: x => x.CityId,
                        principalTable: "City",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompanyChange_Company_Id",
                        column: x => x.Id,
                        principalTable: "Company",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompanyChange_DocumentType_DocumentTypeId",
                        column: x => x.DocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompanyChange_DocumentType_LegalRepresentativeDocumentTypeId",
                        column: x => x.LegalRepresentativeDocumentTypeId,
                        principalTable: "DocumentType",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanySignedFile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanySignedFile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanySignedFile_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompanySignedFile_SignedFile_Id",
                        column: x => x.Id,
                        principalTable: "SignedFile",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EconomicSector",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EconomicSector", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CompanyCategory",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyCategory", x => new { x.CompanyId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_CompanyCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Category",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompanyCategory_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CompanyEconomicSector",
                columns: table => new
                {
                    CompanyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EconomicSectorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEconomicSector", x => new { x.CompanyId, x.EconomicSectorId });
                    table.ForeignKey(
                        name: "FK_CompanyEconomicSector_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompanyEconomicSector_EconomicSector_EconomicSectorId",
                        column: x => x.EconomicSectorId,
                        principalTable: "EconomicSector",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDocumentFile_ChangeCompanyDocumentFileId",
                table: "CompanyDocumentFile",
                column: "ChangeCompanyDocumentFileId",
                unique: true,
                filter: "[ChangeCompanyDocumentFileId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAddress_CityId",
                table: "CompanyAddress",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyAddress_CompanyId",
                table: "CompanyAddress",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyCategory_CategoryId",
                table: "CompanyCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyChange_BusinessStructureId",
                table: "CompanyChange",
                column: "BusinessStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyChange_CityId",
                table: "CompanyChange",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyChange_DocumentTypeId",
                table: "CompanyChange",
                column: "DocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyChange_LegalRepresentativeDocumentTypeId",
                table: "CompanyChange",
                column: "LegalRepresentativeDocumentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEconomicSector_EconomicSectorId",
                table: "CompanyEconomicSector",
                column: "EconomicSectorId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanySignedFile_CompanyId",
                table: "CompanySignedFile",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_File_LogoFileId",
                table: "Company",
                column: "LogoFileId",
                principalTable: "File",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyDocumentFile_CompanyDocumentFile_ChangeCompanyDocumentFileId",
                table: "CompanyDocumentFile",
                column: "ChangeCompanyDocumentFileId",
                principalTable: "CompanyDocumentFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyDocumentFile_File_Id",
                table: "CompanyDocumentFile",
                column: "Id",
                principalTable: "File",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SignedFile_File_Id",
                table: "SignedFile",
                column: "Id",
                principalTable: "File",
                principalColumn: "Id");

            migrationBuilder.InsertData(
                 table: "EconomicSector",
                 columns: new[] { "Id", "Name", "CreatedDate" },
                 values: new object[,]
                 {
                     { Ulid.NewGuid(), "Agropecuario", DateTime.UtcNow },
                     { Ulid.NewGuid(), "Industrial", DateTime.UtcNow },
                     { Ulid.NewGuid(), "Minero", DateTime.UtcNow },
                     { Ulid.NewGuid(), "Construcción", DateTime.UtcNow },
                     { Ulid.NewGuid(), "Transporte", DateTime.UtcNow },
                     { Ulid.NewGuid(), "Comercial", DateTime.UtcNow },
                     { Ulid.NewGuid(), "Comunicaciones", DateTime.UtcNow },
                     { Ulid.NewGuid(), "Turismo", DateTime.UtcNow },
                     { Ulid.NewGuid(), "Educación", DateTime.UtcNow },
                     { Ulid.NewGuid(), "Sanitario", DateTime.UtcNow },
                     { Ulid.NewGuid(), "Financiero", DateTime.UtcNow },
                 });

            migrationBuilder.InsertData(
                table: "Category",
                columns: new[] { "Id", "Name", "CreatedDate" },
                values: new object[,]
                {
                    { Ulid.NewGuid(), "Agropecuario", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Refrigeración", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Ventilación", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Compresores", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Herramientas", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Equipos de Medición", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Repuestos Industriales", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Rodamientos", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Sensores", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Equipos Industriales", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Aceites", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Combustible", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Lubricantes", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Grasas", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Automotríz", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Llantas y Neumáticos", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Agropecuario", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Jardinería", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Eléctricos", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Baterías", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Audio", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Cableado Eléctrico", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Electricidad", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Electrodomésticos", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Computación", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Comunicación", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Impresión", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Instrumentación", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Redes", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Tecnología", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Teléfonos", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Alarmas", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Cámaras de Seguridad", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Vigilancia", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Ferretería", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Soldadura", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Tornillería", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Materiales de Construcción", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Alambres", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Acero", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Carpintería", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Cementos", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Hierro", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Articulos Deportivos", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Cuidado Personal", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Juguetería", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Musical", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Aseo", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Cafetería", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Comida", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Utensilios", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Embalaje", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Mobiliario", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Oficina", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Papelería", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Señalización", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Dotación", DateTime.UtcNow },
                    { Ulid.NewGuid(), "EPPs", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Uniformes", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Equipos de Laboratorio", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Farmacia", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Insumos Médicos", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Medicamentos", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Filtración", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Plomería", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Tubería", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Iluminación", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Textiles", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Químicos", DateTime.UtcNow },
                    { Ulid.NewGuid(), "Pintura", DateTime.UtcNow },
                });

            migrationBuilder.Sql(@$"
                UPDATE CompanyDocument SET CreditEnabled=1
                WHERE Name IN (
                    'Estados financieros de los últimos dos cierres fiscales',
                    'Copia de las dos ultimas declaraciónes de renta presentadas y pagadas',
                    'Referencias Comerciales'
                );
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Company_File_LogoFileId",
                table: "Company");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyDocumentFile_CompanyDocumentFile_ChangeCompanyDocumentFileId",
                table: "CompanyDocumentFile");

            migrationBuilder.DropForeignKey(
                name: "FK_CompanyDocumentFile_File_Id",
                table: "CompanyDocumentFile");

            migrationBuilder.DropForeignKey(
                name: "FK_SignedFile_File_Id",
                table: "SignedFile");

            migrationBuilder.DropTable(
                name: "CompanyAddress");

            migrationBuilder.DropTable(
                name: "CompanyCategory");

            migrationBuilder.DropTable(
                name: "CompanyChange");

            migrationBuilder.DropTable(
                name: "CompanyEconomicSector");

            migrationBuilder.DropTable(
                name: "CompanySignedFile");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "EconomicSector");

            migrationBuilder.DropIndex(
                name: "IX_CompanyDocumentFile_ChangeCompanyDocumentFileId",
                table: "CompanyDocumentFile");

            migrationBuilder.DropColumn(
                name: "MarkForDeletion",
                table: "File");

            migrationBuilder.DropColumn(
                name: "Public",
                table: "File");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "CompanyUser");

            migrationBuilder.DropColumn(
                name: "ChangeCompanyDocumentFileId",
                table: "CompanyDocumentFile");

            migrationBuilder.DropColumn(
                name: "CreditEnabled",
                table: "CompanyDocument");

            migrationBuilder.DropColumn(
                name: "IsChange",
                table: "CompanyAssociate");

            migrationBuilder.DropColumn(
                name: "AboutUs",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "AssociatesReviewMessage",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "AssociatesReviewStatus",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "BillingTaxesReviewMessage",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "BillingTaxesReviewStatus",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "CompanyInfoReviewMessage",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "CompanyInfoReviewStatus",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "ConstitutionDate",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "ContactEmail",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "CreditEnabled",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "CreditLimit",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "CreditReviewMessage",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "CreditReviewStatus",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "DocumentsReviewStatus",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "EmployeesNumber",
                table: "Company");

            migrationBuilder.DropColumn(
                name: "WebsiteUrl",
                table: "Company");

            migrationBuilder.RenameColumn(
                name: "YearlyIncome",
                table: "Company",
                newName: "PaymentType");

            migrationBuilder.RenameColumn(
                name: "LogoFileId",
                table: "Company",
                newName: "RegistrationSignedFileId");

            migrationBuilder.RenameIndex(
                name: "IX_Company_LogoFileId",
                table: "Company",
                newName: "IX_Company_RegistrationSignedFileId");

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "SignedFile",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "FileId",
                table: "CompanyDocumentFile",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "CompanyDocument",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SignedFile_FileId",
                table: "SignedFile",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDocumentFile_FileId",
                table: "CompanyDocumentFile",
                column: "FileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Company_SignedFile_RegistrationSignedFileId",
                table: "Company",
                column: "RegistrationSignedFileId",
                principalTable: "SignedFile",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyDocumentFile_File_FileId",
                table: "CompanyDocumentFile",
                column: "FileId",
                principalTable: "File",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SignedFile_File_FileId",
                table: "SignedFile",
                column: "FileId",
                principalTable: "File",
                principalColumn: "Id");

            migrationBuilder.Sql(@$"
                UPDATE CompanyDocument SET PaymentType=2 -- PaymentType.Credit
                WHERE Name IN (
                    'Estados financieros de los últimos dos cierres fiscales',
                    'Copia de las dos ultimas declaraciónes de renta presentadas y pagadas',
                    'Referencias Comerciales'
                )
            ");
        }
    }
}
