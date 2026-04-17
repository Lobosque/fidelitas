using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Voltei.Api.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppleDeviceRegistrations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DeviceLibraryIdentifier = table.Column<string>(type: "text", nullable: false),
                    PushToken = table.Column<string>(type: "text", nullable: false),
                    PassTypeIdentifier = table.Column<string>(type: "text", nullable: false),
                    SerialNumber = table.Column<string>(type: "text", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppleDeviceRegistrations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    LogoUrl = table.Column<string>(type: "text", nullable: true),
                    CoresPrimaria = table.Column<string>(type: "text", nullable: false),
                    CoresSecundaria = table.Column<string>(type: "text", nullable: false),
                    Plano = table.Column<int>(type: "integer", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Telefone = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    NegocioId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: false),
                    CheckinsNecessarios = table.Column<int>(type: "integer", nullable: false),
                    Ativa = table.Column<bool>(type: "boolean", nullable: false),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    WalletClassId = table.Column<string>(type: "text", nullable: true),
                    ApplePassTypeId = table.Column<string>(type: "text", nullable: true)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    SenhaHash = table.Column<string>(type: "text", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    NegocioId = table.Column<Guid>(type: "uuid", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CampanhaId = table.Column<Guid>(type: "uuid", nullable: false),
                    ClienteId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckinsAtuais = table.Column<int>(type: "integer", nullable: false),
                    Resgatou = table.Column<bool>(type: "boolean", nullable: false),
                    Token = table.Column<string>(type: "text", nullable: false),
                    WalletObjectId = table.Column<string>(type: "text", nullable: true),
                    ApplePassSerial = table.Column<string>(type: "text", nullable: true),
                    ApplePassAuthToken = table.Column<string>(type: "text", nullable: true),
                    ApplePushToken = table.Column<string>(type: "text", nullable: true),
                    CriadaEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ParticipacaoId = table.Column<Guid>(type: "uuid", nullable: false),
                    RegistradoPor = table.Column<Guid>(type: "uuid", nullable: false),
                    CriadoEm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
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
                name: "IX_AppleDeviceRegistrations_DeviceLibraryIdentifier_PassTypeId~",
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
