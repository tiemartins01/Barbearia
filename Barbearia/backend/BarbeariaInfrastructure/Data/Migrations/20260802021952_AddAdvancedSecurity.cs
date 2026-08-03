using System;
using Barbearia.Core.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BarbeariaInfrastructure.Data.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260802021952_AddAdvancedSecurity")]
public partial class AddAdvancedSecurity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Refresh tokens são datas absolutas. PostgreSQL/Npgsql exigem UTC para timestamptz.
        migrationBuilder.Sql("""
            ALTER TABLE refresh_token
                ALTER COLUMN expira_em TYPE timestamp with time zone
                    USING expira_em AT TIME ZONE 'UTC',
                ALTER COLUMN criado_em TYPE timestamp with time zone
                    USING criado_em AT TIME ZONE 'UTC';
            """);

        migrationBuilder.AddColumn<string>(
            name: "created_by_ip",
            table: "refresh_token",
            type: "character varying(64)",
            maxLength: 64,
            nullable: true);

        migrationBuilder.AddColumn<Guid>(
            name: "family_id",
            table: "refresh_token",
            type: "uuid",
            nullable: false,
            defaultValueSql: "gen_random_uuid()");

        migrationBuilder.AddColumn<string>(
            name: "replaced_by_token",
            table: "refresh_token",
            type: "character varying(512)",
            maxLength: 512,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "revocation_reason",
            table: "refresh_token",
            type: "character varying(128)",
            maxLength: 128,
            nullable: true);

        migrationBuilder.AddColumn<DateTime>(
            name: "revoked_at_utc",
            table: "refresh_token",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.CreateTable(
            name: "audit_log",
            columns: table => new
            {
                id = table.Column<long>(type: "bigint", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", Npgsql.EntityFrameworkCore.PostgreSQL.Metadata.NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                user_id = table.Column<int>(type: "integer", nullable: true),
                action = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                entity_type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                entity_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                old_values = table.Column<string>(type: "jsonb", nullable: true),
                new_values = table.Column<string>(type: "jsonb", nullable: true),
                correlation_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                user_agent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                request_path = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                request_method = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_audit_log", x => x.id));

        migrationBuilder.CreateIndex(
            name: "ix_audit_log_entity",
            table: "audit_log",
            columns: new[] { "entity_type", "entity_id" });

        migrationBuilder.CreateIndex(
            name: "ix_audit_log_user_occurred",
            table: "audit_log",
            columns: new[] { "user_id", "occurred_at_utc" });

        migrationBuilder.CreateIndex(
            name: "ix_refresh_token_family",
            table: "refresh_token",
            column: "family_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "audit_log");
        migrationBuilder.DropIndex(name: "ix_refresh_token_family", table: "refresh_token");

        migrationBuilder.DropColumn(name: "created_by_ip", table: "refresh_token");
        migrationBuilder.DropColumn(name: "family_id", table: "refresh_token");
        migrationBuilder.DropColumn(name: "replaced_by_token", table: "refresh_token");
        migrationBuilder.DropColumn(name: "revocation_reason", table: "refresh_token");
        migrationBuilder.DropColumn(name: "revoked_at_utc", table: "refresh_token");

        migrationBuilder.Sql("""
            ALTER TABLE refresh_token
                ALTER COLUMN expira_em TYPE timestamp without time zone
                    USING expira_em AT TIME ZONE 'UTC',
                ALTER COLUMN criado_em TYPE timestamp without time zone
                    USING criado_em AT TIME ZONE 'UTC';
            """);
    }
}
