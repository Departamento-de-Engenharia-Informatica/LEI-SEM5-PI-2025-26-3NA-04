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
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    IsLoadingManifest = table.Column<bool>(type: "INTEGER", nullable: false),
                    CargoVolume = table.Column<int>(type: "INTEGER", nullable: true),
                    VesselVisitNotificationId = table.Column<string>(type: "TEXT", nullable: false)
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
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    ContainerNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    CargoType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    SpecialRequirements = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Bay = table.Column<int>(type: "INTEGER", nullable: true),
                    Row = table.Column<int>(type: "INTEGER", nullable: true),
                    Tier = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Containers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Docks",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    DockName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DockLength = table.Column<int>(type: "INTEGER", nullable: false),
                    DockDraft = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Docks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MobileEquipment",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    EquipmentName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    EquipmentType = table.Column<string>(type: "TEXT", nullable: true),
                    OperationalStartDay = table.Column<int>(type: "INTEGER", nullable: true),
                    OperationalStartTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    OperationalEndDay = table.Column<int>(type: "INTEGER", nullable: true),
                    OperationalEndTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    Is24x7 = table.Column<bool>(type: "INTEGER", nullable: true),
                    ContainersPerTrip = table.Column<int>(type: "INTEGER", nullable: true),
                    AverageSpeedPerHour = table.Column<double>(type: "REAL", nullable: true),
                    ContainersPerHour = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    RequiredOperators = table.Column<int>(type: "INTEGER", nullable: false),
                    RequiredQualification = table.Column<string>(type: "TEXT", nullable: true),
                    SetupTime = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MobileEquipment", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OperatingStaff",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    MecanographicNumber = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    ShortName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Phone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    OperationalStartDay = table.Column<int>(type: "INTEGER", nullable: true),
                    OperationalStartTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    OperationalEndDay = table.Column<int>(type: "INTEGER", nullable: true),
                    OperationalEndTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    Is24x7 = table.Column<bool>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatingStaff", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShippingAgents",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    LegalName = table.Column<string>(type: "TEXT", nullable: true),
                    AlternativeName = table.Column<string>(type: "TEXT", nullable: true),
                    Address = table.Column<string>(type: "TEXT", nullable: true),
                    TaxNumber = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingAgents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StaffQualifications",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    QualificationType = table.Column<string>(type: "TEXT", nullable: true),
                    QualificationName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffQualifications", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false)
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
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Capacity = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxRows = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxBays = table.Column<int>(type: "INTEGER", nullable: true),
                    MaxTiers = table.Column<int>(type: "INTEGER", nullable: true)
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
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    VesselId = table.Column<string>(type: "TEXT", nullable: false),
                    ShippingAgentId = table.Column<string>(type: "TEXT", nullable: false),
                    ExpectedArrival = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ExpectedDeparture = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CargoType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    CargoVolume = table.Column<int>(type: "INTEGER", nullable: false),
                    SpecialHandlingRequirements = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CaptainName = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    TotalCrewCount = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    AssignedDockId = table.Column<string>(type: "TEXT", nullable: true),
                    RejectionReason = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
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
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CargoManifestId = table.Column<string>(type: "TEXT", nullable: false),
                    ContainerId = table.Column<string>(type: "TEXT", nullable: false)
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
                name: "DockMaintenanceSchedules",
                schema: "port",
                columns: table => new
                {
                    UpcomingMaintenancesDockId = table.Column<string>(type: "TEXT", nullable: false),
                    Id = table.Column<int>(type: "INTEGER", nullable: false),
                    ScheduledDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    EstimatedEndDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DockMaintenanceSchedules", x => new { x.UpcomingMaintenancesDockId, x.Id });
                    table.ForeignKey(
                        name: "FK_DockMaintenanceSchedules_Docks_UpcomingMaintenancesDockId",
                        column: x => x.UpcomingMaintenancesDockId,
                        principalSchema: "port",
                        principalTable: "Docks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StsCranes",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    CraneName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DockId = table.Column<string>(type: "TEXT", nullable: false),
                    OperationalStartDay = table.Column<int>(type: "INTEGER", nullable: true),
                    OperationalStartTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    OperationalEndDay = table.Column<int>(type: "INTEGER", nullable: true),
                    OperationalEndTime = table.Column<TimeSpan>(type: "TEXT", nullable: true),
                    Is24x7 = table.Column<bool>(type: "INTEGER", nullable: true),
                    CapacityContainersPerHour = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: true),
                    RequiredOperators = table.Column<int>(type: "INTEGER", nullable: false),
                    RequiredQualification = table.Column<string>(type: "TEXT", nullable: true),
                    SetupTime = table.Column<TimeSpan>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StsCranes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StsCranes_Docks_DockId",
                        column: x => x.DockId,
                        principalSchema: "port",
                        principalTable: "Docks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OperatingStaffQualifications",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    QualificationType = table.Column<string>(type: "TEXT", nullable: false),
                    OperatingStaffId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OperatingStaffQualifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OperatingStaffQualifications_OperatingStaff_OperatingStaffId",
                        column: x => x.OperatingStaffId,
                        principalSchema: "port",
                        principalTable: "OperatingStaff",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingAgentRepresentatives",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    CitizenId = table.Column<string>(type: "TEXT", nullable: true),
                    Nationality = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    Phone = table.Column<string>(type: "TEXT", nullable: true),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShippingAgentId = table.Column<string>(type: "TEXT", nullable: true)
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    ImoNumber = table.Column<string>(type: "TEXT", maxLength: 7, nullable: true),
                    VesselTypeId = table.Column<string>(type: "TEXT", nullable: false),
                    Operator = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true)
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
                name: "VesselVisitCargoManifests",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    VesselVisitNotificationId = table.Column<string>(type: "TEXT", nullable: false),
                    CargoManifestId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselVisitCargoManifests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VesselVisitCargoManifests_VesselVisitNotifications_VesselVisitNotificationId",
                        column: x => x.VesselVisitNotificationId,
                        principalSchema: "port",
                        principalTable: "VesselVisitNotifications",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VesselVisitSafetyOfficers",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    OfficerName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    VesselVisitNotificationId = table.Column<string>(type: "TEXT", nullable: false)
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
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Docks_DockName",
                schema: "port",
                table: "Docks",
                column: "DockName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MobileEquipment_EquipmentName",
                schema: "port",
                table: "MobileEquipment",
                column: "EquipmentName");

            migrationBuilder.CreateIndex(
                name: "IX_OperatingStaff_MecanographicNumber",
                schema: "port",
                table: "OperatingStaff",
                column: "MecanographicNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OperatingStaffQualifications_OperatingStaffId",
                schema: "port",
                table: "OperatingStaffQualifications",
                column: "OperatingStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAgentRepresentatives_ShippingAgentId",
                schema: "port",
                table: "ShippingAgentRepresentatives",
                column: "ShippingAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffQualifications_QualificationType",
                schema: "port",
                table: "StaffQualifications",
                column: "QualificationType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StsCranes_DockId",
                schema: "port",
                table: "StsCranes",
                column: "DockId");

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
                name: "IX_VesselVisitCargoManifests_VesselVisitNotificationId",
                schema: "port",
                table: "VesselVisitCargoManifests",
                column: "VesselVisitNotificationId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselVisitNotifications_AssignedDockId",
                schema: "port",
                table: "VesselVisitNotifications",
                column: "AssignedDockId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselVisitNotifications_ShippingAgentId",
                schema: "port",
                table: "VesselVisitNotifications",
                column: "ShippingAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_VesselVisitNotifications_Status",
                schema: "port",
                table: "VesselVisitNotifications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_VesselVisitNotifications_VesselId",
                schema: "port",
                table: "VesselVisitNotifications",
                column: "VesselId");

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
                name: "DockMaintenanceSchedules",
                schema: "port");

            migrationBuilder.DropTable(
                name: "MobileEquipment",
                schema: "port");

            migrationBuilder.DropTable(
                name: "OperatingStaffQualifications",
                schema: "port");

            migrationBuilder.DropTable(
                name: "ShippingAgentRepresentatives",
                schema: "port");

            migrationBuilder.DropTable(
                name: "StaffQualifications",
                schema: "port");

            migrationBuilder.DropTable(
                name: "StsCranes",
                schema: "port");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Vessels",
                schema: "port");

            migrationBuilder.DropTable(
                name: "VesselVisitCargoManifests",
                schema: "port");

            migrationBuilder.DropTable(
                name: "VesselVisitSafetyOfficers",
                schema: "port");

            migrationBuilder.DropTable(
                name: "CargoManifests",
                schema: "port");

            migrationBuilder.DropTable(
                name: "OperatingStaff",
                schema: "port");

            migrationBuilder.DropTable(
                name: "ShippingAgents",
                schema: "port");

            migrationBuilder.DropTable(
                name: "Docks",
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
