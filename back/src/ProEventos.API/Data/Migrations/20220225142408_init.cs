using Microsoft.EntityFrameworkCore.Migrations;

namespace ProEventos.API.Data.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Eventos", //Essa migration foi criada com base no arquivo DataContext.cs, la criamos um dbSet chamado "Eventos" que eh justamente o nome que esta aqui nessa linha.
                columns: table => new
                { //Nossa tabela tera esses campos e os tipos descritos.
                    EventoId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Local = table.Column<string>(type: "TEXT", nullable: true),
                    DataEvento = table.Column<string>(type: "TEXT", nullable: true),
                    Tema = table.Column<string>(type: "TEXT", nullable: true),
                    QtdPessoas = table.Column<int>(type: "INTEGER", nullable: false),
                    Lote = table.Column<string>(type: "TEXT", nullable: true),
                    imageURL = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Eventos", x => x.EventoId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder) //aqui esta o metodo para dropar a tabela Eventos.
        {
            migrationBuilder.DropTable(
                name: "Eventos");
        }
    }
}
