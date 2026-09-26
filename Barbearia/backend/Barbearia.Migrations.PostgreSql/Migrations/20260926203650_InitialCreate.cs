using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Barbearia.Migrations.PostgreSql.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_log",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<int>(type: "integer", nullable: true),
                    action = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    entity_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    old_values = table.Column<string>(type: "text", nullable: true),
                    new_values = table.Column<string>(type: "text", nullable: true),
                    correlation_id = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    ip_address = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    user_agent = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    request_path = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    request_method = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "idempotency_records",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
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
                constraints: table =>
                {
                    table.PrimaryKey("PK_idempotency_records", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    payload = table.Column<string>(type: "text", nullable: false),
                    occurred_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    processed_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    retry_count = table.Column<int>(type: "integer", nullable: false),
                    last_error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "servicos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    duracao = table.Column<int>(type: "integer", nullable: false),
                    preco = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_servicos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    numero = table.Column<string>(type: "text", nullable: false),
                    cpf = table.Column<string>(type: "text", nullable: false),
                    login = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    senha = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    ativado = table.Column<bool>(type: "boolean", nullable: false),
                    foto = table.Column<string>(type: "text", nullable: true),
                    tentativaslogin = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    bloqueioate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    codigo = table.Column<string>(type: "text", nullable: true),
                    tempocodigo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    tentativascodigo = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    codigovalido = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "barbeiro",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    especialidade = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_barbeiro", x => x.id);
                    table.ForeignKey(
                        name: "FK_barbeiro_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "refresh_token",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    expira_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revogado = table.Column<bool>(type: "boolean", nullable: false),
                    family_id = table.Column<Guid>(type: "uuid", nullable: false),
                    replaced_by_token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: true),
                    revoked_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    revocation_reason = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    created_by_ip = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_refresh_token", x => x.id);
                    table.ForeignKey(
                        name: "FK_refresh_token_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "agendamentos",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_cliente = table.Column<int>(type: "integer", nullable: false),
                    id_barbeiro = table.Column<int>(type: "integer", nullable: false),
                    id_servico = table.Column<int>(type: "integer", nullable: false),
                    duracaoMinutos = table.Column<int>(type: "integer", nullable: false),
                    horario = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    horariofim = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_agendamentos", x => x.id);
                    table.ForeignKey(
                        name: "FK_agendamentos_barbeiro_id_barbeiro",
                        column: x => x.id_barbeiro,
                        principalTable: "barbeiro",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_agendamentos_servicos_id_servico",
                        column: x => x.id_servico,
                        principalTable: "servicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_agendamentos_usuario_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "comentarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_barbeiro = table.Column<int>(type: "integer", nullable: false),
                    id_cliente = table.Column<int>(type: "integer", nullable: false),
                    id_horario = table.Column<int>(type: "integer", nullable: false),
                    nota = table.Column<int>(type: "integer", nullable: false),
                    comentario = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    horario = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    id_servico = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comentarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_comentarios_agendamentos_id_horario",
                        column: x => x.id_horario,
                        principalTable: "agendamentos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comentarios_barbeiro_id_barbeiro",
                        column: x => x.id_barbeiro,
                        principalTable: "barbeiro",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comentarios_servicos_id_servico",
                        column: x => x.id_servico,
                        principalTable: "servicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comentarios_usuario_id_cliente",
                        column: x => x.id_cliente,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_agendamentos_id_servico",
                table: "agendamentos",
                column: "id_servico");

            migrationBuilder.CreateIndex(
                name: "ix_horarios_barbeiro_horario_status",
                table: "agendamentos",
                columns: new[] { "id_barbeiro", "horario", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_horarios_cliente_status_horario",
                table: "agendamentos",
                columns: new[] { "id_cliente", "status", "horario" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_entity",
                table: "audit_log",
                columns: new[] { "entity_type", "entity_id" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_user_occurred",
                table: "audit_log",
                columns: new[] { "user_id", "occurred_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ux_barbeiro_usuario",
                table: "barbeiro",
                column: "usuario_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_comentarios_barbeiro_horario",
                table: "comentarios",
                columns: new[] { "id_barbeiro", "horario" });

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_id_cliente",
                table: "comentarios",
                column: "id_cliente");

            migrationBuilder.CreateIndex(
                name: "IX_comentarios_id_servico",
                table: "comentarios",
                column: "id_servico");

            migrationBuilder.CreateIndex(
                name: "ux_comentarios_horario",
                table: "comentarios",
                column: "id_horario",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "ix_refresh_token_family",
                table: "refresh_token",
                column: "family_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_token_usuario_revogado_expira",
                table: "refresh_token",
                columns: new[] { "usuario_id", "revogado", "expira_em" });

            migrationBuilder.CreateIndex(
                name: "ux_refresh_token_token",
                table: "refresh_token",
                column: "token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_servicos_ativo_nome",
                table: "servicos",
                columns: new[] { "ativo", "nome" });

            migrationBuilder.CreateIndex(
                name: "ix_usuario_ativado_tipo",
                table: "usuario",
                columns: new[] { "ativado", "tipo" });

            migrationBuilder.CreateIndex(
                name: "ux_usuario_cpf",
                table: "usuario",
                column: "cpf",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_usuario_email",
                table: "usuario",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_usuario_login",
                table: "usuario",
                column: "login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ux_usuario_telefone",
                table: "usuario",
                column: "numero",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_log");

            migrationBuilder.DropTable(
                name: "comentarios");

            migrationBuilder.DropTable(
                name: "idempotency_records");

            migrationBuilder.DropTable(
                name: "outbox_messages");

            migrationBuilder.DropTable(
                name: "refresh_token");

            migrationBuilder.DropTable(
                name: "agendamentos");

            migrationBuilder.DropTable(
                name: "barbeiro");

            migrationBuilder.DropTable(
                name: "servicos");

            migrationBuilder.DropTable(
                name: "usuario");
        }
    }
}
