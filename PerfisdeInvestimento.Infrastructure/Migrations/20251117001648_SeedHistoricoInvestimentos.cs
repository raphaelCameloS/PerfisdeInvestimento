using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PerfisdeInvestimento.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedHistoricoInvestimentos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "HistoricosInvestimentos",
                columns: new[] { "Id", "ClienteId", "Data", "Rentabilidade", "Tipo", "Valor" },
                values: new object[,]
                {
                    { 1, 123, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.12m, "CDB", 5000.00m },
                    { 2, 123, new DateTime(2025, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.08m, "Fundo Multimercado", 3000.00m },
                    { 3, 456, new DateTime(2025, 2, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 0.14m, "LCI", 8000.00m }
                });

            migrationBuilder.UpdateData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nome",
                value: "CDB Caixa 2025");

            migrationBuilder.UpdateData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Nome", "ValorMinimo" },
                values: new object[] { "Fundo de Investimento Caixa Assets", 10000m });

            migrationBuilder.UpdateData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nome",
                value: "LCI Imobiliário Caixa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "HistoricosInvestimentos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "HistoricosInvestimentos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "HistoricosInvestimentos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 1,
                column: "Nome",
                value: "CDB Banco ABC 2025");

            migrationBuilder.UpdateData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Nome", "ValorMinimo" },
                values: new object[] { "Fundo Ações XYZ", 5000m });

            migrationBuilder.UpdateData(
                table: "Produtos",
                keyColumn: "Id",
                keyValue: 3,
                column: "Nome",
                value: "LCI Imobiliário");
        }
    }
}
