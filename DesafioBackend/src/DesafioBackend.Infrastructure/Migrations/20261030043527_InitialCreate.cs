using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DesafioBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "entregadores",
                columns: table => new
                {
                    identificador = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    cnpj = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    data_nascimento = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    numero_cnh = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    tipo_cnh = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    imagem_cnh = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entregadores", x => x.identificador);
                });

            migrationBuilder.CreateTable(
                name: "locacoes",
                columns: table => new
                {
                    identificador = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entregador_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    moto_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    data_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_termino = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_previsao_termino = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_devolucao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    plano = table.Column<int>(type: "integer", nullable: false),
                    valor_diaria = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    valor_total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_locacoes", x => x.identificador);
                });

            migrationBuilder.CreateTable(
                name: "motos",
                columns: table => new
                {
                    identificador = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ano = table.Column<int>(type: "integer", nullable: false),
                    modelo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    placa = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_motos", x => x.identificador);
                });

            migrationBuilder.CreateTable(
                name: "notificacoes_motos",
                columns: table => new
                {
                    identificador = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    moto_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ano = table.Column<int>(type: "integer", nullable: false),
                    modelo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    placa = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    mensagem = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    data_notificacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notificacoes_motos", x => x.identificador);
                });

            migrationBuilder.CreateIndex(
                name: "IX_entregadores_cnpj",
                table: "entregadores",
                column: "cnpj",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_entregadores_numero_cnh",
                table: "entregadores",
                column: "numero_cnh",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_motos_placa",
                table: "motos",
                column: "placa",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entregadores");

            migrationBuilder.DropTable(
                name: "locacoes");

            migrationBuilder.DropTable(
                name: "motos");

            migrationBuilder.DropTable(
                name: "notificacoes_motos");
        }
    }
}
