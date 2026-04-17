using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voltei.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialWithWalletEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppleDeviceRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    DeviceLibraryIdentifier = table.Column<string>(type: "TEXT", nullable: false),
                    PushToken = table.Column<string>(type: "TEXT", nullable: false),
                    PassTypeIdentifier = table.Column<string>(type: "TEXT", nullable: false),
                    SerialNumber = table.Column<string>(type: "TEXT", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppleDeviceRegistrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    LogoUrl = table.Column<string>(type: "TEXT", nullable: true),
                    CoresPrimaria = table.Column<string>(type: "TEXT", nullable: false),
                    CoresSecundaria = table.Column<string>(type: "TEXT", nullable: false),
                    Plano = table.Column<int>(type: "INTEGER", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    NegocioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    CheckinsNecessarios = table.Column<int>(type: "INTEGER", nullable: false),
                    Ativa = table.Column<bool>(type: "INTEGER", nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "TEXT", nullable: false),
                    WalletClassId = table.Column<string>(type: "TEXT", nullable: true),
                    ApplePassTypeId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Campaigns_Businesses_NegocioId",
                        column: x => x.NegocioId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    SenhaHash = table.Column<string>(type: "TEXT", nullable: false),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    NegocioId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Businesses_NegocioId",
                        column: x => x.NegocioId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Enrollments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    CampanhaId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<Guid>(type: "TEXT", nullable: false),
                    CheckinsAtuais = table.Column<int>(type: "INTEGER", nullable: false),
                    Resgatou = table.Column<bool>(type: "INTEGER", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    WalletObjectId = table.Column<string>(type: "TEXT", nullable: true),
                    ApplePassSerial = table.Column<string>(type: "TEXT", nullable: true),
                    ApplePassAuthToken = table.Column<string>(type: "TEXT", nullable: true),
                    ApplePushToken = table.Column<string>(type: "TEXT", nullable: true),
                    CriadaEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enrollments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Enrollments_Campaigns_CampanhaId",
                        column: x => x.CampanhaId,
                        principalTable: "Campaigns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Enrollments_Customers_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CheckinLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParticipacaoId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RegistradoPor = table.Column<Guid>(type: "TEXT", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CheckinLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CheckinLogs_Enrollments_ParticipacaoId",
                        column: x => x.ParticipacaoId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CheckinLogs_Users_RegistradoPor",
                        column: x => x.RegistradoPor,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppleDeviceRegistrations_DeviceLibraryIdentifier_PassTypeIdentifier_SerialNumber",
                table: "AppleDeviceRegistrations",
                columns: new[] { "DeviceLibraryIdentifier", "PassTypeIdentifier", "SerialNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_NegocioId",
                table: "Campaigns",
                column: "NegocioId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckinLogs_ParticipacaoId",
                table: "CheckinLogs",
                column: "ParticipacaoId");

            migrationBuilder.CreateIndex(
                name: "IX_CheckinLogs_RegistradoPor",
                table: "CheckinLogs",
                column: "RegistradoPor");

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Telefone",
                table: "Customers",
                column: "Telefone",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CampanhaId_ClienteId",
                table: "Enrollments",
                columns: new[] { "CampanhaId", "ClienteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_ClienteId",
                table: "Enrollments",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_Token",
                table: "Enrollments",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_NegocioId",
                table: "Users",
                column: "NegocioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppleDeviceRegistrations");

            migrationBuilder.DropTable(
                name: "CheckinLogs");

            migrationBuilder.DropTable(
                name: "Enrollments");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Businesses");
        }
    }
}
