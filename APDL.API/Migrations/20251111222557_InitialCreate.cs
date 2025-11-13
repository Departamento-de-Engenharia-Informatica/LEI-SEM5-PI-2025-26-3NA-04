using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDDNetCore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "port");

            migrationBuilder.CreateTable(
                name: "CargoManifests",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsLoadingManifest = table.Column<bool>(type: "bit", nullable: false),
                    CargoVolume = table.Column<int>(type: "int", nullable: true),
                    VesselVisitNotificationId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoManifests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Containers",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContainerNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    CargoType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SpecialRequirements = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Bay = table.Column<int>(type: "int", nullable: true),
                    Row = table.Column<int>(type: "int", nullable: true),
                    Tier = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Containers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Facilities",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxCapacityTEU = table.Column<int>(type: "int", nullable: true),
                    CurrentOccupancyTEU = table.Column<int>(type: "int", nullable: false),
                    FacilityType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Facilities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingAgents",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LegalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AlternativeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingAgents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VesselTypes",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: true),
                    MaxRows = table.Column<int>(type: "int", nullable: true),
                    MaxBays = table.Column<int>(type: "int", nullable: true),
                    MaxTiers = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VesselVisitNotifications",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ExpectedArrival = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ExpectedDeparture = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselVisitNotifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CargoManifestContainers",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CargoManifestId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContainerId = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CargoManifestContainers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CargoManifestContainers_CargoManifests_CargoManifestId",
                        column: x => x.CargoManifestId,
                        principalSchema: "port",
                        principalTable: "CargoManifests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FacilityDockAssignments",
                schema: "port",
                columns: table => new
                {
                    FacilityId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DebugColumn1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DistanceToDock = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacilityDockAssignments", x => new { x.FacilityId, x.Id });
                    table.ForeignKey(
                        name: "FK_FacilityDockAssignments_Facilities_FacilityId",
                        column: x => x.FacilityId,
                        principalSchema: "port",
                        principalTable: "Facilities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingAgentRepresentatives",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CitizenId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Nationality = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ShippingAgentId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingAgentRepresentatives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShippingAgentRepresentatives_ShippingAgents_ShippingAgentId",
                        column: x => x.ShippingAgentId,
                        principalSchema: "port",
                        principalTable: "ShippingAgents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Vessels",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ImoNumber = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: true),
                    VesselTypeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Operator = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vessels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Vessels_VesselTypes_VesselTypeId",
                        column: x => x.VesselTypeId,
                        principalSchema: "port",
                        principalTable: "VesselTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VesselVisitSafetyOfficers",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OfficerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VesselVisitNotificationId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselVisitSafetyOfficers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselVisitSafetyOfficers_VesselVisitNotifications_VesselVisitNotificationId",
                        column: x => x.VesselVisitNotificationId,
                        principalSchema: "port",
                        principalTable: "VesselVisitNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CargoManifestContainers_CargoManifestId",
                schema: "port",
                table: "CargoManifestContainers",
                column: "CargoManifestId");

            migrationBuilder.CreateIndex(
                name: "IX_Containers_ContainerNumber",
                schema: "port",
                table: "Containers",
                column: "ContainerNumber",
                unique: true,
                filter: "[ContainerNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAgentRepresentatives_ShippingAgentId",
                schema: "port",
                table: "ShippingAgentRepresentatives",
                column: "ShippingAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vessels_VesselTypeId",
                schema: "port",
                table: "Vessels",
                column: "VesselTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselVisitSafetyOfficers_VesselVisitNotificationId",
                schema: "port",
                table: "VesselVisitSafetyOfficers",
                column: "VesselVisitNotificationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CargoManifestContainers",
                schema: "port");

            migrationBuilder.DropTable(
                name: "Containers",
                schema: "port");

            migrationBuilder.DropTable(
                name: "FacilityDockAssignments",
                schema: "port");

            migrationBuilder.DropTable(
                name: "ShippingAgentRepresentatives",
                schema: "port");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Vessels",
                schema: "port");

            migrationBuilder.DropTable(
                name: "VesselVisitSafetyOfficers",
                schema: "port");

            migrationBuilder.DropTable(
                name: "CargoManifests",
                schema: "port");

            migrationBuilder.DropTable(
                name: "Facilities",
                schema: "port");

            migrationBuilder.DropTable(
                name: "ShippingAgents",
                schema: "port");

            migrationBuilder.DropTable(
                name: "VesselTypes",
                schema: "port");

            migrationBuilder.DropTable(
                name: "VesselVisitNotifications",
                schema: "port");
        }
    }
}
