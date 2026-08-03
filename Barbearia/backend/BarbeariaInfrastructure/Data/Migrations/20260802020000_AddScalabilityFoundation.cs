using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Barbearia.Core.Infrastructure.Data;

#nullable disable

namespace BarbeariaInfrastructure.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260802020000_AddScalabilityFoundation")]
public partial class AddScalabilityFoundation : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "idempotency_records",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                user_id = table.Column<int>(type: "integer", nullable: false),
                operation = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                request_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                response_body = table.Column<string>(type: "text", nullable: true),
                created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                expires_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                completed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_idempotency_records", x => x.id));

        migrationBuilder.CreateTable(
            name: "outbox_messages",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                type = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                payload = table.Column<string>(type: "jsonb", nullable: false),
                occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                retry_count = table.Column<int>(type: "integer", nullable: false),
                last_error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_outbox_messages", x => x.id));

        migrationBuilder.CreateIndex(
            name: "ix_idempotency_expires_at",
            table: "idempotency_records",
            column: "expires_at_utc");

        migrationBuilder.CreateIndex(
            name: "ux_idempotency_key_user_operation",
            table: "idempotency_records",
            columns: new[] { "key", "user_id", "operation" },
            unique: true);

        migrationBuilder.CreateIndex(
            name: "ix_outbox_pending",
            table: "outbox_messages",
            columns: new[] { "processed_at_utc", "occurred_at_utc" });

        migrationBuilder.Sql(
            "CREATE UNIQUE INDEX ux_horarios_barbeiro_horario_ativo " +
            "ON horarios (id_barbeiro, horario) WHERE status = 0;");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("DROP INDEX IF EXISTS ux_horarios_barbeiro_horario_ativo;");
        migrationBuilder.DropTable(name: "idempotency_records");
        migrationBuilder.DropTable(name: "outbox_messages");
    }
}
