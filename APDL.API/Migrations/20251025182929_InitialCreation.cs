using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DDDNetCore.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "port");

            migrationBuilder.CreateTable(
                name: "Facilities",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MaxCapacityTEU = table.Column<int>(type: "int", nullable: false),
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
                name: "VesselTypes",
                schema: "port",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    MaxRows = table.Column<int>(type: "int", nullable: false),
                    MaxBays = table.Column<int>(type: "int", nullable: false),
                    MaxTiers = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VesselTypes", x => x.Id);
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

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAgentRepresentatives_ShippingAgentId",
                schema: "port",
                table: "ShippingAgentRepresentatives",
                column: "ShippingAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Vessels_VesselTypeId",
                schema: "port",
                table: "Vessels",
                column: "VesselTypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Facilities",
                schema: "port");

            migrationBuilder.DropTable(
                name: "ShippingAgentRepresentatives",
                schema: "port");

            migrationBuilder.DropTable(
                name: "Vessels",
                schema: "port");

            migrationBuilder.DropTable(
                name: "ShippingAgents",
                schema: "port");

            migrationBuilder.DropTable(
                name: "VesselTypes",
                schema: "port");
        }
    }
}
