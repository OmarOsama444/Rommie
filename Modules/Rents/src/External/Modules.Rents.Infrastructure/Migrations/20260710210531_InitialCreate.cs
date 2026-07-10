using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Modules.Rents.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "rnt");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "csv_seed_histories",
                schema: "rnt",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    seeded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_csv_seed_histories", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "governorates",
                schema: "rnt",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_governorates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "inbox_consumer_messages",
                schema: "rnt",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    handler_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbox_consumer_messages", x => new { x.id, x.handler_name });
                });

            migrationBuilder.CreateTable(
                name: "inbox_messages",
                schema: "rnt",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    correlation_id = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    occurred_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_consumer_messages",
                schema: "rnt",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    handler_name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_consumer_messages", x => new { x.id, x.handler_name });
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "rnt",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    correlation_id = table.Column<string>(type: "text", nullable: false),
                    type = table.Column<string>(type: "text", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    occurred_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_on_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    error = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "rnt",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "text", nullable: false),
                    first_name = table.Column<string>(type: "text", nullable: false),
                    last_name = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    rating = table.Column<double>(type: "double precision", nullable: true),
                    rating_count = table.Column<int>(type: "integer", nullable: true),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "places",
                schema: "rnt",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    rating = table.Column<double>(type: "double precision", nullable: true),
                    rate_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    governorate_id = table.Column<int>(type: "integer", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    location = table.Column<Point>(type: "geography (Point, 4326)", nullable: false),
                    governorate_id1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_places", x => x.id);
                    table.ForeignKey(
                        name: "fk_places_governorates_governorate_id",
                        column: x => x.governorate_id,
                        principalSchema: "rnt",
                        principalTable: "governorates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_places_governorates_governorate_id1",
                        column: x => x.governorate_id1,
                        principalSchema: "rnt",
                        principalTable: "governorates",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_places_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "rnt",
                        principalTable: "user",
                        principalColumn: "id");
                });

            migrationBuilder.InsertData(
                schema: "rnt",
                table: "governorates",
                columns: new[] { "id", "name" },
                values: new object[,]
                {
                    { 1, "Aswan" },
                    { 2, "Asyut" },
                    { 3, "Luxor" },
                    { 4, "Alexandria" },
                    { 5, "Ismailia" },
                    { 6, "Suez" },
                    { 7, "Dakahlia" },
                    { 8, "Faiyum" },
                    { 9, "Cairo" },
                    { 10, "Giza" },
                    { 11, "Beheira" },
                    { 12, "Sharqia" },
                    { 13, "Gharbia" },
                    { 14, "Qalyubia" },
                    { 15, "Monufia" },
                    { 16, "Minya" },
                    { 17, "New Valley" },
                    { 18, "Beni Suef" },
                    { 19, "Port Said" },
                    { 20, "South Sinai" },
                    { 21, "Damietta" },
                    { 22, "Sohag" },
                    { 23, "North Sinai" },
                    { 24, "Qena" },
                    { 25, "Kafr El Sheikh" },
                    { 26, "Matruh" },
                    { 27, "Red Sea" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_governorates_name",
                schema: "rnt",
                table: "governorates",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_places_governorate_id",
                schema: "rnt",
                table: "places",
                column: "governorate_id");

            migrationBuilder.CreateIndex(
                name: "ix_places_governorate_id1",
                schema: "rnt",
                table: "places",
                column: "governorate_id1");

            migrationBuilder.CreateIndex(
                name: "ix_places_location",
                schema: "rnt",
                table: "places",
                column: "location")
                .Annotation("Npgsql:IndexMethod", "GIST");

            migrationBuilder.CreateIndex(
                name: "ix_places_user_id",
                schema: "rnt",
                table: "places",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "csv_seed_histories",
                schema: "rnt");

            migrationBuilder.DropTable(
                name: "inbox_consumer_messages",
                schema: "rnt");

            migrationBuilder.DropTable(
                name: "inbox_messages",
                schema: "rnt");

            migrationBuilder.DropTable(
                name: "outbox_consumer_messages",
                schema: "rnt");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "rnt");

            migrationBuilder.DropTable(
                name: "places",
                schema: "rnt");

            migrationBuilder.DropTable(
                name: "governorates",
                schema: "rnt");

            migrationBuilder.DropTable(
                name: "user",
                schema: "rnt");
        }
    }
}
