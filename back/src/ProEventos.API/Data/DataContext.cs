using Microsoft.EntityFrameworkCore;
using ProEventos.API.Models;

namespace ProEventos.API.Data
{
    public class DataContext : DbContext //Esse DbContext vem do Microsoft.EntityFrameworkCore, e necesario para comecar a criar o contexto para o DB.
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)  { } //Isso eh um construtor necessario para que o contexto funcione. E esse DbContextOptions deve ser passado para o pai, entao por isos usamos o base (pai) e passamos para ele o options
        public DbSet<Evento> Eventos { get; set; }
    }
}