using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BarbeariaInfrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    bloqueioate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    codigo = table.Column<string>(type: "text", nullable: true),
                    tempocodigo = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                    expira_em = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    revogado = table.Column<bool>(type: "boolean", nullable: false)
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
                name: "horarios",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    id_cliente = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    id_barbeiro = table.Column<int>(type: "integer", nullable: false),
                    id_servico = table.Column<int>(type: "integer", nullable: false),
                    horario = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_horarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_horarios_barbeiro_id_barbeiro",
                        column: x => x.id_barbeiro,
                        principalTable: "barbeiro",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_horarios_servicos_id_servico",
                        column: x => x.id_servico,
                        principalTable: "servicos",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_horarios_usuario_id_cliente",
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
                    horario = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    id_servico = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_comentarios", x => x.id);
                    table.ForeignKey(
                        name: "FK_comentarios_barbeiro_id_barbeiro",
                        column: x => x.id_barbeiro,
                        principalTable: "barbeiro",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_comentarios_horarios_id_horario",
                        column: x => x.id_horario,
                        principalTable: "horarios",
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
                name: "ix_horarios_barbeiro_horario_status",
                table: "horarios",
                columns: new[] { "id_barbeiro", "horario", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_horarios_cliente_status_horario",
                table: "horarios",
                columns: new[] { "id_cliente", "status", "horario" });

            migrationBuilder.CreateIndex(
                name: "IX_horarios_id_servico",
                table: "horarios",
                column: "id_servico");

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
                name: "comentarios");

            migrationBuilder.DropTable(
                name: "refresh_token");

            migrationBuilder.DropTable(
                name: "horarios");

            migrationBuilder.DropTable(
                name: "barbeiro");

            migrationBuilder.DropTable(
                name: "servicos");

            migrationBuilder.DropTable(
                name: "usuario");
        }
    }
}
