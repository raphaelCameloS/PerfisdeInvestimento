using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PerfisdeInvestimento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoricosInvestimentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false),
                    Valor = table.Column<decimal>(type: "TEXT", nullable: false),
                    Rentabilidade = table.Column<decimal>(type: "TEXT", nullable: false),
                    Data = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricosInvestimentos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PerfisRisco",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    TipoPerfil = table.Column<string>(type: "TEXT", nullable: false),
                    Pontuacao = table.Column<int>(type: "INTEGER", nullable: false),
                    Descricao = table.Column<string>(type: "TEXT", nullable: false),
                    DataCalculo = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerfisRisco", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", nullable: false),
                    Rentabilidade = table.Column<decimal>(type: "TEXT", nullable: false),
                    Risco = table.Column<string>(type: "TEXT", nullable: false),
                    ValorMinimo = table.Column<decimal>(type: "TEXT", nullable: false),
                    PrazoMinimoMeses = table.Column<int>(type: "INTEGER", nullable: false),
                    PerfilRecomendado = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Simulacoes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    Produto = table.Column<string>(type: "TEXT", nullable: false),
                    ValorInvestido = table.Column<decimal>(type: "TEXT", nullable: false),
                    ValorFinal = table.Column<decimal>(type: "TEXT", nullable: false),
                    PrazoMeses = table.Column<int>(type: "INTEGER", nullable: false),
                    DataSimulacao = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Simulacoes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Telemetrias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Endpoint = table.Column<string>(type: "TEXT", nullable: false),
                    RequestTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ResponseTimeMs = table.Column<long>(type: "INTEGER", nullable: false),
                    StatusCode = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Telemetrias", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Produtos",
                columns: new[] { "Id", "Nome", "PerfilRecomendado", "PrazoMinimoMeses", "Rentabilidade", "Risco", "Tipo", "ValorMinimo" },
                values: new object[,]
                {
                    { 1, "CDB Banco ABC 2025", "Conservador", 6, 0.12m, "Baixo", "CDB", 1000m },
                    { 2, "Fundo Ações XYZ", "Agressivo", 12, 0.18m, "Alto", "Fundo", 5000m },
                    { 3, "LCI Imobiliário", "Moderado", 24, 0.14m, "Médio", "LCI", 2000m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricosInvestimentos");

            migrationBuilder.DropTable(
                name: "PerfisRisco");

            migrationBuilder.DropTable(
                name: "Produtos");

            migrationBuilder.DropTable(
                name: "Simulacoes");

            migrationBuilder.DropTable(
                name: "Telemetrias");
        }
    }
}
