using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DesafioBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDeliveries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "entregas",
                columns: table => new
                {
                    identificador = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    pedido_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    entregador_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    aceita_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    concluida_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    criada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizada_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_entregas", x => x.identificador);
                });

            migrationBuilder.CreateIndex(
                name: "IX_entregas_pedido_id",
                table: "entregas",
                column: "pedido_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "entregas");
        }
    }
}
