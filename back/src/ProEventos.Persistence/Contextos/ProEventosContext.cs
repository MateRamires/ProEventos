using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProEventos.Domain;
using ProEventos.Domain.Identity;

namespace ProEventos.Persistance.Contextos
{
    public class ProEventosContext : IdentityDbContext<User, Role, int, 
                                                       IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, 
                                                       IdentityRoleClaim<int>, IdentityUserToken<int>> //Esse DbContext vem do Microsoft.EntityFrameworkCore, e necesario para comecar a criar o contexto para o DB.
    {
        public ProEventosContext(DbContextOptions<ProEventosContext> options) : base(options)  { } //Isso eh um construtor necessario para que o contexto funcione. E esse DbContextOptions deve ser passado para o pai, entao por isos usamos o base (pai) e passamos para ele o options
        public DbSet<Evento> Eventos { get; set; }
        public DbSet<Lote> Lotes { get; set; }
        public DbSet<Palestrante> Palestrantes { get; set; }
        public DbSet<PalestranteEvento> PalestrantesEventos { get; set; }
        public DbSet<RedeSocial> RedesSociais { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder){

            base.OnModelCreating(modelBuilder);

            //Primeira forma de fazer o onModelCreating das tabelas.

            modelBuilder.Entity<UserRole>(userRole=>{
                userRole.HasKey(ur => new { ur.UserId, ur.RoleId});

                userRole.HasOne(ur => ur.Role)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.RoleId)
                    .IsRequired();

                userRole.HasOne(ur => ur.User)
                    .WithMany(r => r.UserRoles)
                    .HasForeignKey(ur => ur.UserId)
                    .IsRequired();
            });

            //Segunda forma de fazer o onModelCreating das tabelas. (Ambas estao corretas)

            modelBuilder.Entity<PalestranteEvento>()
                .HasKey(PE => new {PE.EventoId, PE.PalestranteId});

            modelBuilder.Entity<Evento>()
                .HasMany(e => e.RedesSociais)
                .WithOne(rs => rs.Evento)
                .OnDelete(DeleteBehavior.Cascade); //Aqui estamos basicamente falando ao c# que ao deletar um evento, deve ser deletado as redes sociais associadas a esse evento (devido ao Cascade).

            modelBuilder.Entity<Palestrante>()
                .HasMany(e => e.RedesSociais)
                .WithOne(rs => rs.Palestrante)
               .OnDelete(DeleteBehavior.Cascade); //A mesma coisa do de cima, so que aqui estamos falando que quando deletar um palestrante, delete as redes sociais dele tambem.

            //No problema acima, nos tambem poderiamos ter criado uma classe de rede social para palestrante e uma outra classe de rede social para evento, ai não precisaria fazer essa especificação acima.
        }

    }
}







