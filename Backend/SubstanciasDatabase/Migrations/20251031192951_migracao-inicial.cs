using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SubstanciasDatabase.Migrations
{
    /// <inheritdoc />
    public partial class migracaoinicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "propriedades",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nome = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    ValueType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_propriedades", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "substancias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Codigo = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    Nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "text", nullable: true),
                    Notas = table.Column<string>(type: "text", nullable: true),
                    CategoriaId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_substancias", x => x.Id);
                    table.ForeignKey(
                        name: "FK_substancias_categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "substancia_propriedades",
                columns: table => new
                {
                    SubstanciaId = table.Column<int>(type: "integer", nullable: false),
                    PropriedadeId = table.Column<int>(type: "integer", nullable: false),
                    ValueType = table.Column<int>(type: "integer", nullable: false),
                    BoolValue = table.Column<bool>(type: "boolean", nullable: true),
                    DecimalValue = table.Column<decimal>(type: "numeric(18,4)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_substancia_propriedades", x => new { x.SubstanciaId, x.PropriedadeId });
                    table.ForeignKey(
                        name: "FK_substancia_propriedades_propriedades_PropriedadeId",
                        column: x => x.PropriedadeId,
                        principalTable: "propriedades",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_substancia_propriedades_substancias_SubstanciaId",
                        column: x => x.SubstanciaId,
                        principalTable: "substancias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_categorias_Nome",
                table: "categorias",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_propriedades_Nome",
                table: "propriedades",
                column: "Nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_substancia_propriedades_PropriedadeId",
                table: "substancia_propriedades",
                column: "PropriedadeId");

            migrationBuilder.CreateIndex(
                name: "IX_substancias_CategoriaId",
                table: "substancias",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_substancias_Codigo",
                table: "substancias",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "substancia_propriedades");

            migrationBuilder.DropTable(
                name: "propriedades");

            migrationBuilder.DropTable(
                name: "substancias");

            migrationBuilder.DropTable(
                name: "categorias");
        }
    }
}
